#!/bin/bash

# Start SQL Server in background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
sleep 30s

echo "Checking full-text search status..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT SERVERPROPERTY('IsFullTextInstalled') as IsFullTextInstalled;"

echo "Trying to enable full-text service..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "EXEC sp_fulltext_service 'load_os_resources', 1; EXEC sp_fulltext_service 'verify_signature', 0;"

# Run the initialization script
echo "Running initialization script..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /usr/src/app/01-setup-fulltext.sql

# Keep the container running
echo "SQL Server initialized with full-text search. Ready for connections."
wait
