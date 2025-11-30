#!/bin/bash
set -e

echo "--- Starter Kit .NET Core Container Starting ---"

# Note: Database migration is already handled in Program.cs
# (context.Database.Migrate()) which runs automatically when the app starts.

echo "--- Starting Kestrel Server on Port 5005 ---"
# 'exec' ensures the application process replaces the shell process (PID 1)
exec dotnet StarterKit.Api.dll