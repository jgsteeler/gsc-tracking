# Backend Versioning

This directory contains scripts to manage version information for the GSC Tracking API.

## Version Information

The API uses two types of version information:

1. **Version**: Standard semantic version (e.g., `0.1.11`) stored in `GscTracking.Api.csproj`
2. **InformationalVersion**: Extended version with build metadata (e.g., `0.1.11+ce4c206` or `0.1.11+123.ce4c206`)

The InformationalVersion follows the [Semantic Versioning 2.0](https://semver.org/) specification and includes:
- Base version number
- Build metadata (commit hash, optionally with build number)

## Version Scripts

Two scripts are provided to set version information during builds:

### Bash Script (Linux/macOS)
```bash
./set-version.sh [PROJECT_PATH] [BUILD_NUMBER]
```

### PowerShell Script (Windows/Cross-platform)
```powershell
./set-version.ps1 -ProjectPath [PATH] -BuildNumber [NUMBER]
```

### Parameters

- `PROJECT_PATH` (optional): Path to the .csproj file. Default: `GscTracking.Api/GscTracking.Api.csproj`
- `BUILD_NUMBER` (optional): CI/CD build number. If provided, format is `{version}+{buildnumber}.{commithash}`. If omitted, format is `{version}+{commithash}`

### Examples

```bash
# Set version with commit hash only
./set-version.sh

# Set version with build number and commit hash
./set-version.sh GscTracking.Api/GscTracking.Api.csproj 123

# PowerShell version
./set-version.ps1 -BuildNumber 456
```

## CI/CD Integration

### GitHub Actions Example

Add this step before building the API:

```yaml
- name: Set API version
  working-directory: backend
  run: ./set-version.sh GscTracking.Api/GscTracking.Api.csproj ${{ github.run_number }}
  shell: bash
```

### Environment Variables

The scripts automatically extract the git commit hash using:
```bash
git rev-parse --short HEAD
```

Ensure your CI/CD environment has git available and the repository is checked out with history.

## Version Endpoint

The API exposes version information at the `/api/hello` endpoint:

```bash
curl http://localhost:5091/api/hello
```

Response:
```json
{
  "message": "Hello from GSC Tracking API!",
  "version": "0.1.11+ce4c206",
  "assemblyVersion": "0.1.11.0",
  "timestamp": "2025-12-16T01:37:06.5499973Z"
}
```

- `version`: InformationalVersion with build metadata
- `assemblyVersion`: Standard assembly version
- `timestamp`: Current UTC time

## Release Please Integration

Release Please automatically updates the `<Version>` tag in `GscTracking.Api.csproj` when creating releases. The configuration in `.github/release-please-config.json` includes:

```json
{
  "backend": {
    "release-type": "simple",
    "extra-files": [
      "GscTracking.Api/GscTracking.Api.csproj"
    ]
  }
}
```

This ensures the version number in the .csproj file stays synchronized with the version managed by Release Please.

## Local Development

For local development, you can run the version script to inject the current commit hash:

```bash
cd backend
./set-version.sh
dotnet build
dotnet run
```

The InformationalVersion will include your current commit hash, making it easy to identify which code version is running.

## Notes

- The `<InformationalVersion>` property in the .csproj file is updated by the scripts and should not be manually modified
- The property `IncludeSourceRevisionInInformationalVersion` is set to `false` to prevent MSBuild from automatically appending source control information
- The scripts are idempotent - running them multiple times will update the InformationalVersion each time
- Version information is embedded in the compiled assembly and accessible via reflection
