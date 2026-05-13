# NuGet Release (Choose One Trigger)

This guide describes release flows for `.github/workflows/publish-nuget.yml`.
For each release, choose exactly one trigger:

- Option A: push a git tag
- Option B: run GitHub Actions UI (`workflow_dispatch`)

## Checklist

- [ ] Repository secret `NUGET_API_KEY` is configured.
- [ ] You have push permission for tags.
- [ ] Target version has not been published to NuGet.org.
- [ ] Local branch contains the commit to release.

## Version and Tag Rules

- Workflow trigger: `push.tags: v*`
- Tag format: `v<version>`
- Example tags:
  - `v1.0.1`
  - `v1.0.1-beta.1`
- Published package version is resolved from tag name by removing the leading `v`.
- Manual trigger input: `version` (for example `1.0.1` or `1.0.1-beta.1`).

## Release Steps (Choose One)

### Option A: Git Tag Trigger

1. Sync latest repository state.
2. Create an annotated release tag.
3. Push the tag to origin.
4. Monitor GitHub Actions workflow run.
5. Verify packages on NuGet.org.

### Commands

```bash
git fetch --tags
git checkout main
git pull

git tag -a v1.0.1 -m "Release v1.0.1"
git push origin v1.0.1
```

### Option B: GitHub Actions UI Trigger

1. Open GitHub repository -> `Actions` -> `publish-nuget`.
2. Click `Run workflow`.
3. Enter `version` (for example `1.0.1` or `1.0.1-beta.1`).
4. Start the workflow and monitor progress.
5. Verify packages on NuGet.org.

## GitHub Actions Verification

Open `Actions` -> `publish-nuget` and confirm:

- `Resolve package version` prints expected version.
- `Pack` step produces `.nupkg` files under `artifacts/nuget`.
- `Publish to NuGet.org` completes successfully.

For Option B, also confirm `Resolve package version` matches the `version` input you entered.

## Published Packages

The workflow publishes these packages:

- `Zack.EFCore.BatchInsert`
- `Zack.EFCore.BatchInsert.MSSQL`
- `Zack.EFCore.BatchInsert.MySQL.Pomelo`
- `Zack.EFCore.BatchInsert.Npgsql`
- `Zack.EFCore.BatchInsert.Oracle`
- `Zack.EFCore.BatchInsert.Dm`

## Retry and Rollback

- If push failed before publication, fix issue and re-run the workflow.
- Do not run both trigger paths for the same version.
- If some packages were published, do not reuse the same stable version; publish a new version tag.
- If you pushed a wrong tag before successful publish, delete and recreate the tag:

```bash
git tag -d v1.0.1
git push origin :refs/tags/v1.0.1

git tag -a v1.0.2 -m "Release v1.0.2"
git push origin v1.0.2
```

## Common Failures

- `Missing required secret: NUGET_API_KEY`
  - Add repository secret `NUGET_API_KEY` and rerun.
- `No .nupkg files found to publish`
  - Check pack step output and project paths.
- NuGet push rejected due to duplicate version
  - Use a new version tag for changed artifacts.

