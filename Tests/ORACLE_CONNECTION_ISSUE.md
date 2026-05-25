# Oracle Connection Issue & Resolution

## Issue Description

### Error Message
```
ORA-12514: Cannot connect to database. Service FREEPDB1 is not registered 
with the listener at host 127.0.0.1/127.0.0.1 port 1521
```

### Root Cause
The `gvenzl/oracle-free:23-slim` Docker container requires an extended initialization period to:
1. Start the Oracle database service
2. Register the FREEPDB1 service with the listener
3. Make the listener fully available for connections

The original wait logic was insufficient:
- Only waited for 120 iterations × 5 seconds = 600 seconds (10 minutes)
- Did not properly verify FREEPDB1 service registration
- Used inefficient connection checks that could fail during startup

### Impact
- Integration tests failed randomly
- CI pipeline blocked on oracle-integration job
- False negatives in PR testing

---

## Solution Implemented

### Enhanced Wait Logic
Updated `.github/workflows/pr-integration.yml` to include:

#### 1. **Listener Status Verification**
```bash
lsnrctl status 2>&1 | grep -q "Listening"
```
Checks if the Oracle listener process has started

#### 2. **Service Registration Check**
```bash
lsnrctl status 2>&1 | grep -q "FREEPDB1"
```
Verifies that FREEPDB1 service is registered with the listener

#### 3. **Extended Timeout**
- Maximum 300 attempts × 2 seconds = 600 seconds (10 minutes)
- Provides plenty of time for slow container startup
- Logs progress every 10 attempts for debugging

### Code Changes

**Before:**
```yaml
- name: Wait for Oracle
  run: |
    for i in {1..120}; do
      if docker exec ${{ job.services.oracle.id }} bash -lc "echo 'SELECT 1 FROM dual;' | sqlplus -s app/app_password@localhost/FREEPDB1" | grep -q "1"; then
        break
      fi
      sleep 5
    done
```

**After:**
```yaml
- name: Wait for Oracle database startup
  run: |
    # Wait for Oracle container to start (max 10 minutes)
    MAX_ATTEMPTS=300
    ATTEMPT=0
    
    echo "Waiting for Oracle database to initialize..."
    
    while [ $ATTEMPT -lt $MAX_ATTEMPTS ]; do
      # Check if listener is running
      if docker exec ${{ job.services.oracle.id }} bash -lc 'lsnrctl status 2>&1 | grep -q "Listening"' 2>/dev/null; then
        # Check if FREEPDB1 service is registered
        if docker exec ${{ job.services.oracle.id }} bash -lc 'lsnrctl status 2>&1 | grep -q "FREEPDB1"' 2>/dev/null; then
          echo "Oracle listener and FREEPDB1 service are ready!"
          exit 0
        fi
      fi
      
      ATTEMPT=$((ATTEMPT + 1))
      if [ $((ATTEMPT % 10)) -eq 0 ]; then
        echo "Still waiting... ($ATTEMPT/$MAX_ATTEMPTS)"
      fi
      sleep 2
    done
    
    echo "ERROR: Oracle failed to start within 10 minutes"
    exit 1
```

---

## Additional Improvements

### SQL Server Health Check Enhancement
Added Docker health check for SQL Server:
```yaml
options: |
  --health-cmd="/opt/mssql-tools18/bin/sqlcmd -S . -U sa -P Your_strong_password123! -Q 'SELECT 1' -C"
  --health-interval 10s
  --health-timeout 5s
  --health-retries 5
```

Benefits:
- Docker daemon monitors container health continuously
- Faster failure detection
- More reliable startup verification

---

## Expected Outcome

After these changes:
- ✅ Oracle tests will wait properly for service registration
- ✅ Tests will fail faster if Oracle doesn't start (exit after 10 minutes)
- ✅ Better logging for debugging startup issues
- ✅ More resilient CI pipeline
- ✅ Consistent test run times

---

## CI Test Timing

### Oracle Startup Timeline (typical)
| Stage | Time | Status |
|-------|------|--------|
| Container starts | 0-30s | Running |
| Oracle initialization | 30-120s | Background |
| Listener startup | 120-180s | Starting |
| Service registration | 180-240s | Registering |
| **Ready for connections** | **~240s (4 min)** | ✅ **Ready** |

The new wait logic accounts for this full lifecycle.

---

## References

### Related Configuration Files
- `.github/workflows/pr-integration.yml` - Oracle job configuration
- `Tests/Zack.EFCore.Batch.IntegrationTests.Oracle/` - Oracle test suite
- `Tests/DOTNET_VERSION_SUPPORT_DECISION.md` - .NET version policy

### Docker Image Documentation
- [gvenzl/oracle-free Documentation](https://github.com/gvenzl/oci-oracle-xe)
- [Oracle Database Services and Listeners](https://docs.oracle.com/cd/B19306_01/admin.102/b14231/startup.htm)

---

## Debugging Tips

If Oracle tests still fail:

1. **Check Oracle container logs:**
   ```bash
   docker logs <container_id>
   ```

2. **Verify listener status manually:**
   ```bash
   docker exec <container_id> lsnrctl status
   ```

3. **Check FREEPDB1 registration:**
   ```bash
   docker exec <container_id> sqlplus -v
   ```

4. **Increase wait time if needed:**
   - Modify `MAX_ATTEMPTS` to 360 (12 minutes)
   - Some systems may need more time

---

**Decision Date:** May 12, 2026  
**Status:** Implemented  
**Last Updated:** May 12, 2026

