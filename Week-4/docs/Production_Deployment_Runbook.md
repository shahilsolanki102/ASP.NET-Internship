# Production Deployment & Operations Runbook

This operational runbook provides instructions for deploying, containerizing, monitoring, and rolling back the ASP.NET Core applications across staging and production environments.

---

## 🚀 1. Automated Deployment via CI/CD

The repository includes a GitHub Actions pipeline (`.github/workflows/ci-cd.yml`):
- **Triggers**: Every commit pushed to the `main` branch.
- **Workflow Pipeline Stages**:
  1. `build-and-test`: Restores, compiles, and executes all 26 unit and integration tests.
  2. `docker-build`: Builds multi-stage Docker image and tags with commit SHA.
  3. `publish-and-package`: Publishes optimized Release binaries and uploads deployment zip artifacts.

---

## 🐳 2. Docker & Container Orchestration

### Prerequisites
- Docker Engine 24.0+
- Docker Compose v2.20+

### Step-by-Step Container Deployment
```bash
# 1. Navigate to deployment directory
cd "d:\ASP.NET Intern\Week-4\deployment"

# 2. Build and launch all multi-container services (App + SQL Server + Nginx)
docker-compose up -d --build

# 3. Verify healthy container status
docker-compose ps

# 4. View real-time application logs
docker-compose logs -f app
```

---

## 💻 3. One-Click PowerShell Deployment Script
To execute automated testing, publishing, and zip packaging locally:
```powershell
# Run the automated deployment script
powershell -ExecutionPolicy Bypass -File "d:\ASP.NET Intern\Week-4\deployment\deploy.ps1"
```

---

## 🔄 4. Zero-Downtime Rollback Strategy

In the event of a production fault:
1. **Container Rollback**:
   ```bash
   # Revert to previous tagged container image
   docker tag orderflow-app:<PREVIOUS_SHA> orderflow-app:latest
   docker-compose up -d --no-deps app
   ```
2. **Database Rollback**:
   Execute the corresponding rollback migration script or restore from the automated SQL backup volume (`sqldata`).
