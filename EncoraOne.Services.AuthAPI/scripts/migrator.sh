#!/usr/bin/env bash
set -e

echo "Waiting for SQL Server..."
sleep 20

# Ensure dotnet-ef is available
if ! command -v dotnet-ef >/dev/null 2>&1; then
	echo "dotnet-ef not found — installing global tool..."
	dotnet tool install --global dotnet-ef || dotnet tool update --global dotnet-ef
	export PATH="$PATH:/root/.dotnet/tools"
fi

echo "Running migrations..."

# Retry migrations a few times in case SQL Server is still starting
max_attempts=10
attempt=1
until dotnet ef database update --project /app/Grievance.API.csproj --startup-project /app; do
	if [ "$attempt" -ge "$max_attempts" ]; then
		echo "Migrations failed after $attempt attempts." >&2
		exit 1
	fi
	echo "Migration attempt $attempt failed — retrying in 5s..."
	attempt=$((attempt+1))
	sleep 5
done

echo "Migrations completed."
