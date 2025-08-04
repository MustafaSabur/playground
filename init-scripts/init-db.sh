#!/bin/bash

# Start SQL Server in the background
echo "Starting SQL Server..."
/opt/mssql/bin/sqlservr &

# Store the PID of SQL Server
SQLSERVER_PID=$!

# Function to cleanup on exit
cleanup() {
    echo "Shutting down SQL Server..."
    kill -TERM $SQLSERVER_PID 2>/dev/null
    wait $SQLSERVER_PID 2>/dev/null
}

# Set trap to cleanup on script exit
trap cleanup EXIT TERM INT

# Wait for SQL Server to start and be ready for connections
echo "Waiting for SQL Server to be ready..."
for i in {1..60}; do
    if /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -C -Q "SELECT 1" > /dev/null 2>&1; then
        echo "SQL Server is ready!"
        break
    fi
    if [ $i -eq 60 ]; then
        echo "Timeout waiting for SQL Server to start"
        exit 1
    fi
    echo "Waiting... ($i/60)"
    sleep 2
done

# Run initialization script
echo "Running Full-Text Search initialization..."
if [ -f "/docker-entrypoint-initdb.d/setup-fulltext.sql" ]; then
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -C -i /docker-entrypoint-initdb.d/setup-fulltext.sql
    if [ $? -eq 0 ]; then
        echo "Full-Text Search setup completed successfully!"
    else
        echo "Error running Full-Text Search setup script"
    fi
else
    echo "Setup script not found"
fi

# Keep SQL Server running
echo "SQL Server with Full-Text Search is ready for connections!"
echo "Container will keep running. Use Ctrl+C to stop."

# Wait for SQL Server process to complete
wait $SQLSERVER_PID