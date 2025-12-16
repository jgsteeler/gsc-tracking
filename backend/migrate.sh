#!/bin/bash
# Database Migration Script for GSC Tracking API
# This script applies Entity Framework Core migrations to the database

set -e  # Exit on error

echo "🔄 Starting database migration..."

# Check if DATABASE_URL is set
if [ -z "$DATABASE_URL" ]; then
    echo "❌ Error: DATABASE_URL environment variable is not set"
    exit 1
fi

echo "✅ Database connection configured"

# Navigate to the API project directory
cd /app || cd "$(dirname "$0")/GscTracking.Api" || exit 1

# Install dotnet-ef tool if not already installed
if ! command -v dotnet-ef &> /dev/null; then
    echo "📦 Installing EF Core tools..."
    dotnet tool install --global dotnet-ef
    export PATH="$PATH:/root/.dotnet/tools"
fi

echo "🔧 Applying database migrations..."

# Apply migrations
dotnet ef database update --no-build || {
    echo "❌ Migration failed!"
    exit 1
}

echo "✅ Database migration completed successfully!"
