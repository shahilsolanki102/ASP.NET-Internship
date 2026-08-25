# Week 4: Project Documentation, Deployment, and DevOps Integration

Enterprise DevOps automation, multi-stage Docker containerization, and GitHub Actions CI/CD workflows for the ASP.NET Core internship solutions.

---

## 🌟 Key Deliverables & Highlights

- **CI/CD Pipeline (`.github/workflows/ci-cd.yml`)**: Automated Build, 26-Test Quality Gate, Docker Buildx containerization, and Release packaging.
- **Production Containerization**: Multi-stage `Dockerfile` (Alpine runtime < 120MB, non-root user).
- **Multi-Container Orchestration**: `docker-compose.yml` (ASP.NET Core + Microsoft SQL Server 2022 + Nginx Reverse Proxy).
- **One-Click Automated Deployment**: `deploy.ps1` PowerShell pipeline script.
- **Enterprise Runbooks & Documentation**: Architectural blueprint, operations runbook, and Word report.

---

## 🚀 How to Deploy & Run

### 1. Run Automated CI/CD Deployment Script
```powershell
powershell -ExecutionPolicy Bypass -File "Week-4\deployment\deploy.ps1"
```

### 2. Run with Docker Compose
```bash
cd "Week-4/deployment"
docker-compose up -d --build
```
Navigate to: `http://localhost` (via Nginx proxy) or `http://localhost:8080` (direct Kestrel).

---

## 📂 Deliverables & Reports
- **Word Report**: `docs/DevOps_and_Deployment_Report_Week4.docx`
- **Portal Submission Description**: `docs/Portal_Submission_Text.md`
- **Architecture Guide**: `docs/Comprehensive_System_Architecture_Guide.md`
- **Deployment Runbook**: `docs/Production_Deployment_Runbook.md`
