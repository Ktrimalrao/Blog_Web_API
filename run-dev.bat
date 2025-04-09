@echo off
echo Starting Blog Web API and Frontend...

:: Start the backend API
start cmd /k "cd BlogWebApi && dotnet run"

:: Start the frontend
start cmd /k "cd blog-frontend && npm run dev"

echo Both servers are starting...
echo Backend will be available at: https://localhost:7001
echo Frontend will be available at: http://localhost:5173 