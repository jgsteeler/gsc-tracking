#!/bin/bash
# Docker Setup Test Script
# This script validates the Docker and Docker Compose setup

set -e

echo "======================================"
echo "GSC Tracking Docker Setup Test"
echo "======================================"
echo ""

# Check Docker installation
echo "✓ Checking Docker installation..."
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi
docker --version
echo ""

# Check Docker Compose installation
echo "✓ Checking Docker Compose installation..."
if ! docker compose version &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi
docker compose version
echo ""

# Check if Docker daemon is running
echo "✓ Checking if Docker daemon is running..."
if ! docker info &> /dev/null; then
    echo "❌ Docker daemon is not running. Please start Docker."
    exit 1
fi
echo "Docker daemon is running"
echo ""

# Check if .env file exists
echo "✓ Checking for .env file..."
if [ ! -f .env ]; then
    echo "⚠️  .env file not found. Creating from .env.example..."
    if [ -f .env.example ]; then
        cp .env.example .env
        echo "✓ Created .env file from .env.example"
    else
        echo "❌ .env.example not found. Please create a .env file."
        exit 1
    fi
else
    echo ".env file exists"
fi
echo ""

# Validate docker-compose.yml syntax
echo "✓ Validating docker-compose.yml..."
if docker compose config --quiet; then
    echo "docker-compose.yml is valid"
else
    echo "❌ docker-compose.yml has errors"
    exit 1
fi
echo ""

# Check if ports are available
echo "✓ Checking if required ports are available..."
PORTS=(5173 8080 5432)
for port in "${PORTS[@]}"; do
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1 ; then
        echo "⚠️  Port $port is already in use. You may need to change it in .env"
    else
        echo "Port $port is available"
    fi
done
echo ""

# Test building images (optional, commented out by default)
# echo "✓ Testing Docker image builds (this may take several minutes)..."
# docker compose build --no-cache
# echo "✓ Images built successfully"
# echo ""

echo "======================================"
echo "✓ All checks passed!"
echo "======================================"
echo ""
echo "You can now start the application with:"
echo "  docker compose up -d"
echo ""
echo "To view logs:"
echo "  docker compose logs -f"
echo ""
echo "To stop the application:"
echo "  docker compose down"
echo ""
echo "For more information, see DOCKER.md"
