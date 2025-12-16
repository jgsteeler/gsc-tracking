#!/bin/bash
# Script to set version information including build metadata
# This script is intended to be run during CI/CD builds to inject
# version information with commit hash and build number

PROJECT_PATH="${1:-GscTracking.Api/GscTracking.Api.csproj}"
BUILD_NUMBER="${2:-}"

# Get the git commit hash (short version)
COMMIT_HASH=$(git rev-parse --short HEAD 2>/dev/null || echo "unknown")

if [ "$COMMIT_HASH" = "unknown" ]; then
    echo "Warning: Could not get git commit hash. Using 'unknown'."
fi

# Read the current version from the .csproj file
if [ ! -f "$PROJECT_PATH" ]; then
    echo "Error: Project file not found at $PROJECT_PATH"
    exit 1
fi

VERSION=$(grep -oP '<Version>\K[^<]+' "$PROJECT_PATH")

if [ -z "$VERSION" ]; then
    echo "Error: Could not find Version tag in $PROJECT_PATH"
    exit 1
fi

echo "Found version: $VERSION"

# Build the informational version string
# Format: {version}+{buildnumber}.{commithash}
if [ -n "$BUILD_NUMBER" ]; then
    INFORMATIONAL_VERSION="$VERSION+$BUILD_NUMBER.$COMMIT_HASH"
else
    INFORMATIONAL_VERSION="$VERSION+$COMMIT_HASH"
fi

echo "Setting InformationalVersion to: $INFORMATIONAL_VERSION"

# Check if InformationalVersion already exists in the file
if grep -q '<InformationalVersion>' "$PROJECT_PATH"; then
    # Replace existing InformationalVersion
    sed -i "s|<InformationalVersion>.*</InformationalVersion>|<InformationalVersion>$INFORMATIONAL_VERSION</InformationalVersion>|" "$PROJECT_PATH"
else
    # Add InformationalVersion after Version tag
    sed -i "s|<Version>$VERSION</Version>|<Version>$VERSION</Version>\n    <InformationalVersion>$INFORMATIONAL_VERSION</InformationalVersion>|" "$PROJECT_PATH"
fi

echo "Successfully updated $PROJECT_PATH"
echo "Version: $VERSION"
echo "InformationalVersion: $INFORMATIONAL_VERSION"
