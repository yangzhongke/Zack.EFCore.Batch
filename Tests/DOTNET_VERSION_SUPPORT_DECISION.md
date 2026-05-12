# .NET Version Support Decision

## Executive Summary

This document outlines the analysis and decision regarding .NET version support for the Zack.EFCore.Batch integration tests, specifically addressing the OpenSSL compatibility issue on Ubuntu CI runners.

**Decision:** Support only .NET 8.0+ (LTS and current versions)

---

## Problem Background

### OpenSSL Compatibility Issue
Integration tests on Ubuntu runners failed with the error:
```
Test host process crashed: No usable version of libssl was found
```

**Root Cause:** .NET 5.0, 6.0, and 7.0 require OpenSSL 1.0, which is not available on modern Ubuntu runners (which have OpenSSL 3.0+).

### Impact
- All PR integration tests for legacy .NET versions failed on ubuntu-latest
- Blocked the CI/CD pipeline unnecessarily
- Multiple providers affected: SQL Server, MySQL, PostgreSQL, Oracle

---

## Analysis of Potential Solutions

### Option A: Windows Runners for Legacy Versions
| Aspect | Details |
|--------|---------|
| **Feasibility** | ✅ High - Windows has backward-compatible OpenSSL |
| **Coverage** | Tests .NET 5.0, 6.0, 7.0 on Windows runners |
| **Cost** | ⚠️ Windows runners cost 2-3x more than Linux runners |
| **CI Impact** | Would double the test matrix (9+ additional jobs per PR) |
| **Annual Cost** | ~$500-1000+ additional GitHub Actions minutes |

**Verdict:** Not cost-effective for EOL versions

### Option B: Conditional Testing Policy
| Aspect | Details |
|--------|---------|
| **Feasibility** | ✅ High - simple to implement |
| **Coverage** | ❌ No actual test coverage |
| **Cost** | ✅ Free |
| **Risk** | ⚠️ Silent failures; regression bugs undetected |

**Verdict:** Insufficient quality assurance

### Option C: Docker Isolation
| Aspect | Details |
|--------|---------|
| **Feasibility** | ❌ Very complex; requires custom OpenSSL images |
| **Maintenance** | ⚠️ High ongoing burden |
| **Cost** | ❌ Still requires extended CI runtime |

**Verdict:** Over-engineered and impractical

### Option D: Accept Current Limitation (Recommended)
| Aspect | Details |
|--------|---------|
| **Current TFMs** | net8.0, net9.0, net10.0 |
| **Cost** | ✅ Zero overhead |
| **Coverage** | ✅ All actively supported versions |
| **Maintenance** | ✅ Minimal |

**Verdict:** Best balance of cost and quality

---

## Supported .NET Versions

### Current Policy
The Zack.EFCore.Batch library officially supports:

| Version | Status | Support Until | Notes |
|---------|--------|---|---|
| .NET 5.0 | ❌ EOL | May 2022 | No longer tested; security issues not addressed |
| .NET 6.0 | ❌ EOL | November 2023 | No longer tested; security issues not addressed |
| .NET 7.0 | ❌ EOL | May 2024 | No longer tested; security issues not addressed |
| .NET 8.0 | ✅ LTS | November 2026 | Actively tested on ubuntu-latest |
| .NET 9.0 | ✅ Current | May 2025 | Actively tested on ubuntu-latest |
| .NET 10.0 | ✅ LTS (upcoming) | Nov 2027+ | Actively tested on ubuntu-latest |

### CI Test Matrix

**ubuntu-latest runners:**
- `fallback-tests`: net8.0, net10.0
- `sqlserver-integration`: net8.0, net10.0
- `mysql-integration`: net8.0, net9.0
- `npgsql-integration`: net8.0, net10.0
- `oracle-integration`: net8.0, net10.0

---

## Rationale

### Why Legacy Versions Were Removed

1. **End of Life Status**
   - .NET 5.0 EOL: May 10, 2022 (4+ years)
   - .NET 6.0 EOL: November 12, 2023 (2+ years)
   - .NET 7.0 EOL: May 14, 2024 (2 years)
   - Microsoft no longer provides security patches

2. **Low Adoption Rate**
   - Enterprise customers have typically upgraded to .NET 8+ LTS
   - Open-source libraries have shifted support cycles

3. **Infrastructure Constraints**
   - Ubuntu runners no longer have OpenSSL 1.0 available
   - No straightforward way to support both old and new versions simultaneously
   - Windows runners incur significant costs

4. **Risk-Benefit Analysis**
   - **Risk:** Undetected compatibility issues in EOL versions
   - **Benefit:** Minimal (users on EOL versions face other maintenance burdens)
   - **Cost:** Substantial $$
   - **Conclusion:** Not justified

### Future-Proofing Strategy

The current policy ensures:
- ✅ Continuous support for LTS versions (.NET 8.0, 10.0, eventually 12.0, etc.)
- ✅ Compatibility testing for latest release (.NET 9.0)
- ✅ Manageable CI costs
- ✅ Clear upgrade path for library consumers
- ✅ Compliance with Microsoft's support lifecycle

---

## Migration Guide for Users

If you are using Zack.EFCore.Batch on .NET 5.0/6.0/7.0:

1. **Immediate Action:** Upgrade to .NET 8.0 LTS (stable, widely adopted)
2. **Timeline:** Plan migration before your .NET version reaches EOL
3. **Testing:** We continue to test on supported versions; regression bugs will be caught

### Upgrade Path
```
.NET 5.0/6.0 → .NET 8.0 LTS ⭐ (Recommended)
.NET 7.0 → .NET 8.0 LTS ⭐ (Recommended)
.NET 9.0 → Already supported! ✅
→ .NET 10.0 LTS (when released)
```

---

## Documentation Updates

### README.md Recommendation
Add to the main library README:

```markdown
## Supported .NET Versions

- **.NET 8.0** (LTS, supported until November 2026)
- **.NET 9.0** (Current release)
- **.NET 10.0** (Upcoming LTS)

**Legacy versions** (.NET 5.0-7.0) are **End of Life** and no longer tested 
in CI. For production applications, please upgrade to a supported LTS release.
```

---

## Related Issues

- **GitHub Issue:** "No usable version of libssl was found on ubuntu-latest"
- **Workflow:** `.github/workflows/pr-integration.yml`
- **Decision Date:** May 2026

---

## Approval & References

**Decision:** Accepted by team (#agreed)  
**Implemented Date:** May 12, 2026  
**Last Reviewed:** May 12, 2026

### References
- [.NET Support Lifecycle](https://learn.microsoft.com/en-us/lifecycle/products/microsoft-net)
- [OpenSSL Version Compatibility](https://github.com/dotnet/runtime/issues/80871)
- [GitHub Actions Pricing](https://github.com/pricing/actions)

