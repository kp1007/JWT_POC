# Deployment Guide

Production deployment guide for JWT POC application.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Environment Configuration](#environment-configuration)
- [Database Setup](#database-setup)
- [Application Deployment](#application-deployment)
- [Docker Deployment](#docker-deployment)
- [Cloud Deployment](#cloud-deployment)
- [Security Hardening](#security-hardening)
- [Monitoring Setup](#monitoring-setup)
- [Backup and Recovery](#backup-and-recovery)

## Prerequisites

### Required Software

- .NET 8 Runtime
- SQL Server 2019+ or Azure SQL Database
- IIS 10+ or Nginx (Linux)
- SSL/TLS Certificate
- (Optional) Docker & Docker Compose
- (Optional) Kubernetes cluster

### Minimum System Requirements

**Single Server**:
- CPU: 2 cores
- RAM: 4 GB
- Storage: 20 GB SSD
- Network: 100 Mbps

**Production (Recommended)**:
- CPU: 4+ cores
- RAM: 8+ GB
- Storage: 50+ GB SSD
- Network: 1 Gbps
- Load Balancer
- Database Server (separate)

## Environment Configuration

### 1. Create Production appsettings

Create `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-sql.example.com;Database=JwtPocDb;User Id=jwtpocapp;Password=***;Encrypt=true;TrustServerCertificate=false;"
  },
  "Jwt": {
    "SecretKey": "*** USE ENVIRONMENT VARIABLE ***",
    "Issuer": "JwtPocApi",
    "Audience": "JwtPocClient",
    "AccessTokenExpirationMinutes": 15
  },
  "Security": {
    "MaxLoginAttempts": 5,
    "LockoutDurationMinutes": 30,
    "RequireEmailConfirmation": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 2. Environment Variables

**Set secure values via environment variables**:

```bash
# Linux/Mac
export JWT__SECRETKEY="your-super-secure-secret-key-from-vault"
export CONNECTIONSTRINGS__DEFAULTCONNECTION="your-database-connection"

# Windows PowerShell
$env:JWT__SECRETKEY="your-super-secure-secret-key-from-vault"
$env:CONNECTIONSTRINGS__DEFAULTCONNECTION="your-database-connection"

# Docker
docker run -e JWT__SECRETKEY="..." -e CONNECTIONSTRINGS__DEFAULTCONNECTION="..." ...
```

### 3. Secrets Management

**Azure Key Vault**:
```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

**AWS Secrets Manager**:
```csharp
builder.Configuration.AddSecretsManager(configurator: opts =>
{
    opts.SecretFilter = entry => entry.Name.StartsWith("JwtPoc");
});
```

## Database Setup

### 1. Create Production Database

```sql
CREATE DATABASE JwtPocDb;
GO

CREATE LOGIN jwtpocapp WITH PASSWORD = 'SecurePassword123!';
GO

USE JwtPocDb;
CREATE USER jwtpocapp FOR LOGIN jwtpocapp;
GO

ALTER ROLE db_datareader ADD MEMBER jwtpocapp;
ALTER ROLE db_datawriter ADD MEMBER jwtpocapp;
GO
```

### 2. Run Migrations

```bash
# From project directory
cd src/JwtPoc.Api

# Update database
dotnet ef database update --project ../JwtPoc.Infrastructure --connection "YourConnectionString"
```

### 3. Seed Production Data

```bash
# Modify DbSeeder to skip test users in production
# Or manually create admin user via SQL
```

## Application Deployment

### Option 1: Windows Server (IIS)

#### Publish Application

```bash
dotnet publish src/JwtPoc.Api/JwtPoc.Api.csproj \
  -c Release \
  -o ./publish \
  /p:EnvironmentName=Production
```

#### IIS Configuration

1. **Install .NET 8 Hosting Bundle**
   - Download from Microsoft
   - Install on server
   - Restart IIS

2. **Create Application Pool**
   - Name: JwtPocAppPool
   - .NET CLR Version: No Managed Code
   - Managed Pipeline Mode: Integrated
   - Identity: ApplicationPoolIdentity

3. **Create Website**
   - Site name: JwtPocApi
   - Physical path: C:\inetpub\wwwroot\jwtpocapi
   - Binding: https, port 443
   - SSL Certificate: Your certificate

4. **Configure web.config**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet"
                  arguments=".\JwtPoc.Api.dll"
                  stdoutLogEnabled="false"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

### Option 2: Linux (Nginx + Systemd)

#### Publish Application

```bash
dotnet publish -c Release -o /var/www/jwtpocapi
```

#### Create Systemd Service

`/etc/systemd/system/jwtpocapi.service`:

```ini
[Unit]
Description=JWT POC API
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/jwtpocapi
ExecStart=/usr/bin/dotnet /var/www/jwtpocapi/JwtPoc.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=jwtpocapi
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

#### Configure Nginx

`/etc/nginx/sites-available/jwtpocapi`:

```nginx
server {
    listen 80;
    server_name api.yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name api.yourdomain.com;

    ssl_certificate /etc/ssl/certs/yourdomain.crt;
    ssl_certificate_key /etc/ssl/private/yourdomain.key;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

#### Start Service

```bash
sudo systemctl enable jwtpocapi
sudo systemctl start jwtpocapi
sudo systemctl status jwtpocapi

sudo nginx -t
sudo systemctl reload nginx
```

## Docker Deployment

### Build Image

```bash
docker build -t jwtpocapi:1.0.0 .
```

### Run Container

```bash
docker run -d \
  --name jwtpocapi \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e JWT__SECRETKEY="your-secret" \
  -e CONNECTIONSTRINGS__DEFAULTCONNECTION="your-connection" \
  --restart unless-stopped \
  jwtpocapi:1.0.0
```

### Docker Compose Production

`docker-compose.prod.yml`:

```yaml
version: '3.8'

services:
  api:
    image: jwtpocapi:1.0.0
    container_name: jwtpocapi-prod
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - JWT__SECRETKEY=${JWT_SECRET}
      - CONNECTIONSTRINGS__DEFAULTCONNECTION=${DB_CONNECTION}
    ports:
      - "8080:8080"
    restart: unless-stopped
    networks:
      - jwtpoc-network
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: jwtpocapi-db
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=${DB_SA_PASSWORD}
      - MSSQL_PID=Standard
    volumes:
      - sqldata:/var/opt/mssql
    restart: unless-stopped
    networks:
      - jwtpoc-network

volumes:
  sqldata:

networks:
  jwtpoc-network:
    driver: bridge
```

Run:
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## Cloud Deployment

### Azure App Service

#### Using Azure CLI

```bash
# Login
az login

# Create resource group
az group create --name JwtPocRG --location eastus

# Create App Service plan
az appservice plan create \
  --name JwtPocPlan \
  --resource-group JwtPocRG \
  --sku B1 \
  --is-linux

# Create web app
az webapp create \
  --resource-group JwtPocRG \
  --plan JwtPocPlan \
  --name jwtpocapi \
  --runtime "DOTNETCORE:8.0"

# Configure app settings
az webapp config appsettings set \
  --resource-group JwtPocRG \
  --name jwtpocapi \
  --settings ASPNETCORE_ENVIRONMENT=Production

# Deploy
az webapp deployment source config-zip \
  --resource-group JwtPocRG \
  --name jwtpocapi \
  --src publish.zip
```

### AWS Elastic Beanstalk

```bash
# Install EB CLI
pip install awsebcli

# Initialize
eb init -p "64bit Amazon Linux 2 v2.5.0 running .NET Core" jwtpocapi

# Create environment
eb create jwtpocapi-prod

# Deploy
eb deploy
```

### Kubernetes

`deployment.yaml`:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: jwtpocapi
spec:
  replicas: 3
  selector:
    matchLabels:
      app: jwtpocapi
  template:
    metadata:
      labels:
        app: jwtpocapi
    spec:
      containers:
      - name: api
        image: your-registry/jwtpocapi:1.0.0
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: JWT__SECRETKEY
          valueFrom:
            secretKeyRef:
              name: jwtpoc-secrets
              key: jwt-secret
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
---
apiVersion: v1
kind: Service
metadata:
  name: jwtpocapi-service
spec:
  selector:
    app: jwtpocapi
  ports:
  - port: 80
    targetPort: 8080
  type: LoadBalancer
```

Deploy:
```bash
kubectl apply -f deployment.yaml
```

## Security Hardening

### 1. SSL/TLS Configuration

**Generate SSL Certificate**:
- Use Let's Encrypt for free certificates
- Or purchase from trusted CA

**Strong Cipher Suites** (Nginx):
```nginx
ssl_protocols TLSv1.2 TLSv1.3;
ssl_ciphers 'ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256';
ssl_prefer_server_ciphers on;
```

### 2. Firewall Rules

**Allow only necessary ports**:
- 443 (HTTPS)
- 22 (SSH - restricted IPs)
- Database port (restricted to app server)

**Linux (ufw)**:
```bash
sudo ufw allow 443/tcp
sudo ufw allow 22/tcp from YOUR_ADMIN_IP
sudo ufw enable
```

### 3. Database Security

- Use strong passwords
- Enable encryption at rest
- Enable encryption in transit
- Restrict network access
- Regular backups
- Audit logging

### 4. Application Security

- Disable Swagger in production
- Enable HSTS
- Configure CSP headers
- Remove development endpoints
- Enable rate limiting
- Configure proper CORS

## Monitoring Setup

### Application Insights (Azure)

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

app.MapHealthChecks("/health");
```

### Logging

**Serilog to File**:
```json
"Serilog": {
  "WriteTo": [
    {
      "Name": "File",
      "Args": {
        "path": "/var/log/jwtpocapi/log-.txt",
        "rollingInterval": "Day",
        "retainedFileCountLimit": 30
      }
    }
  ]
}
```

**Log Aggregation**:
- Seq
- ELK Stack
- Azure Application Insights
- AWS CloudWatch

## Backup and Recovery

### Database Backup

**Automated Backups**:
```sql
BACKUP DATABASE JwtPocDb
TO DISK = '/backups/JwtPocDb_FULL.bak'
WITH INIT, COMPRESSION;
```

**Backup Schedule**:
- Full backup: Daily
- Differential: Every 6 hours
- Transaction log: Every 15 minutes

### Application Backup

**Docker Volumes**:
```bash
docker run --rm \
  -v sqldata:/data \
  -v /backup:/backup \
  alpine tar czf /backup/sqldata-backup.tar.gz /data
```

### Disaster Recovery Plan

1. **RPO (Recovery Point Objective)**: < 15 minutes
2. **RTO (Recovery Time Objective)**: < 1 hour
3. **Backup Retention**: 30 days
4. **Test Restores**: Monthly

## Performance Tuning

### Database Optimization

```sql
-- Add indexes
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);

-- Update statistics
UPDATE STATISTICS Users;
UPDATE STATISTICS RefreshTokens;
```

### Application Pool Settings (IIS)

- Idle Timeout: 20 minutes
- Recycling: Daily at 2 AM
- Max Worker Processes: CPU count

### Response Caching

```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

## Rollback Strategy

### Prepare for Rollback

1. Tag stable version in Git
2. Keep previous deployment artifacts
3. Document current configuration
4. Have database backup ready

### Rollback Steps

```bash
# Stop current version
sudo systemctl stop jwtpocapi

# Restore previous version
cp -r /var/www/jwtpocapi-backup/* /var/www/jwtpocapi/

# Restore database (if needed)
sqlcmd -S server -Q "RESTORE DATABASE JwtPocDb FROM DISK='/backups/before-deploy.bak'"

# Start service
sudo systemctl start jwtpocapi
```

## Post-Deployment Checklist

- [ ] All services running
- [ ] Database connectivity verified
- [ ] HTTPS working correctly
- [ ] Health check returning 200
- [ ] Swagger disabled (production)
- [ ] Logs being written correctly
- [ ] Authentication working
- [ ] Authorization policies working
- [ ] 2FA functioning
- [ ] Rate limiting active
- [ ] Monitoring alerts configured
- [ ] Backup job scheduled
- [ ] Load testing completed
- [ ] Security scan completed
- [ ] Documentation updated

---

For support during deployment, contact: devops@example.com
