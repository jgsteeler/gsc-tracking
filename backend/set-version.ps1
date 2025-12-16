#!/usr/bin/env pwsh
# Script to set version information including build metadata
# This script is intended to be run during CI/CD builds to inject
# version information with commit hash and build number

param(
    [string]$ProjectPath = "GscTracking.Api/GscTracking.Api.csproj",
    [string]$BuildNumber = ""
)

# Get the git commit hash (short version)
$commitHash = ""
try {
    $commitHash = git rev-parse --short HEAD 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Could not get git commit hash. Using 'unknown'."
        $commitHash = "unknown"
    }
} catch {
    Write-Warning "Git not available. Using 'unknown' for commit hash."
    $commitHash = "unknown"
}

# Read the current version from the .csproj file
$csprojContent = Get-Content $ProjectPath -Raw
$versionMatch = [regex]::Match($csprojContent, '<Version>([\d\.]+)</Version>')

if ($versionMatch.Success) {
    $version = $versionMatch.Groups[1].Value
    Write-Host "Found version: $version"
    
    # Build the informational version string
    # Format: {version}+{buildnumber}.{commithash}
    $informationalVersion = $version
    
    if ($BuildNumber) {
        $informationalVersion += "+$BuildNumber.$commitHash"
    } else {
        $informationalVersion += "+$commitHash"
    }
    
    Write-Host "Setting InformationalVersion to: $informationalVersion"
    
    # Check if InformationalVersion already exists in the file
    if ($csprojContent -match '<InformationalVersion>') {
        # Replace existing InformationalVersion
        $csprojContent = $csprojContent -replace '<InformationalVersion>.*?</InformationalVersion>', "<InformationalVersion>$informationalVersion</InformationalVersion>"
    } else {
        # Add InformationalVersion after Version tag
        $csprojContent = $csprojContent -replace '(<Version>[\d\.]+</Version>)', "`$1`n    <InformationalVersion>$informationalVersion</InformationalVersion>"
    }
    
    # Write the updated content back to the file
    Set-Content -Path $ProjectPath -Value $csprojContent -NoNewline
    
    Write-Host "Successfully updated $ProjectPath"
    Write-Host "Version: $version"
    Write-Host "InformationalVersion: $informationalVersion"
} else {
    Write-Error "Could not find Version tag in $ProjectPath"
    exit 1
}
