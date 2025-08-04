# SQL Server 2025 (Based on SQL Server 2022) with Full-Text Search
# SQL Server 2022 includes full-text search capabilities out of the box
# Reference: https://hub.docker.com/_/microsoft-mssql-server

FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=YourStrong!Passw0rd
ENV MSSQL_PID=Developer
ENV MSSQL_TCP_PORT=1433

# Switch to root to create directories and copy files
USER root

# Create directory for initialization scripts
RUN mkdir -p /docker-entrypoint-initdb.d

# Copy initialization SQL script
COPY init-scripts/setup-fulltext.sql /docker-entrypoint-initdb.d/

# Copy startup script
COPY init-scripts/init-db.sh /usr/local/bin/

# Make scripts executable
RUN chmod +x /usr/local/bin/init-db.sh

# Switch back to mssql user
USER mssql

# Expose SQL Server port
EXPOSE 1433

# Health check to verify SQL Server is running and responsive
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=5 \
    CMD /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -C -Q "SELECT 1" || exit 1

# Use a startup script that initializes the database after SQL Server starts
CMD ["/bin/bash", "/usr/local/bin/init-db.sh"]