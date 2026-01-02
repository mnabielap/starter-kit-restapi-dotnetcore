# 🚀 Starter Kit REST API .NET Core 8

A production-ready, scalable **REST API Starter Kit** built with **.NET Core 8**. 
This project follows the **Clean Architecture** (Onion Architecture) principles, separating Core Logic from Infrastructure and Presentation.

It supports **SQLite** for local development and **PostgreSQL** for production (Docker), complete with **JWT Authentication**, **Role-Based Access Control (RBAC)**, and **Automated Documentation**.

## ✨ Key Features

-   **Clean Architecture:** Strict separation of concerns (Domain, Application, Infrastructure, API).
-   **Multi-Database Support:** Seamlessly switch between **SQLite** (Dev) and **PostgreSQL** (Docker/Prod).
-   **Validation:** Robust request validation using **FluentValidation**.
-   **Object Mapping:** simplified entity-to-DTO mapping using **AutoMapper**.
-   **Authentication & Authorization:** Secure **JWT** implementation with Refresh Tokens and RBAC (Admin/User).
-   **Docker Ready:** Custom `Dockerfile` that automatically handles database migrations for PostgreSQL inside the container.
-   **Swagger UI:** Integrated OpenAPI/Swagger documentation.
-   **API Testing Suite:** Pre-configured Python scripts for instant API testing (no Postman required).

---

## 🏗️ Architecture Overview

The solution is divided into 4 layers:

1.  **Domain:** Core business entities, enums, and constants. (No dependencies).
2.  **Application:** Business logic, DTOs, Interfaces, and Validators. (Depends on Domain).
3.  **Infrastructure:** Database implementation (EF Core), JWT generation, Repositories. (Depends on Application).
4.  **Api:** Controllers, Middleware, and Entry Point. (Depends on Application & Infrastructure).

---

## 🛠️ Prerequisites

Before you begin, ensure you have the following installed:
-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (If running via Docker)
-   [Python 3.x](https://www.python.org/downloads/) (For running API tests)

---

## 🚀 Getting Started

We recommend running the project **Locally (Method 1)** first to understand the structure, then moving to **Docker (Method 2)**.

### Method 1: Local Development (SQLite)

This uses **SQLite** for a zero-configuration setup.

**1. Install Entity Framework Tool**
```bash
dotnet tool install --global dotnet-ef
```

**2. Restore Dependencies**
```bash
dotnet restore
```

**3. Build the Project**
```bash
dotnet build
```

**4. Create Database Migrations & Update DB**
This creates the `starterkit.db` file locally.
```bash
dotnet ef migrations add InitialCreate --project src/StarterKit.Infrastructure --startup-project src/StarterKit.Api --output-dir Data/Migrations
dotnet ef database update --project src/StarterKit.Infrastructure --startup-project src/StarterKit.Api
```

**5. Run the Application**
```bash
dotnet run --project src/StarterKit.Api
```
The server will start at `http://localhost:5000` (or the port defined in `launchSettings.json`).

---

### Method 2: Running with Docker (PostgreSQL)

This setup uses **PostgreSQL** and creates a persistent environment. The `Dockerfile` is smart enough to **automatically generate PostgreSQL-specific migrations** during the build process, so you don't need to manually handle migration files.

**1. Prepare Environment File**
Create a file named `.env.docker` in the root directory:
```ini
# .env.docker
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5005

# Database Configuration
DatabaseProvider=Postgres
ConnectionStrings__DefaultConnection=Host=restapi-postgres-container;Port=5432;Database=restapi_db;Username=postgres;Password=mysecretpassword

# JWT Secrets
Jwt__Secret=this_is_a_very_secure_and_very_long_jwt_secret_for_docker_env
Jwt__AccessExpirationMinutes=30
Jwt__RefreshExpirationDays=30
Jwt__ResetPasswordExpirationMinutes=10
Jwt__VerifyEmailExpirationMinutes=10
```

**2. Create Network & Volumes**
Run these commands once to set up networking and persistence.
```bash
# Create Network
docker network create restapi_dotnetcore_network

# Create Volume for DB Data
docker volume create restapi_dotnetcore_db_volume

# Create Volume for Media/Uploads
docker volume create restapi_dotnetcore_media_volume
```

**3. Run PostgreSQL Container**
```bash
docker run -d \
  --name restapi-postgres-container \
  --network restapi_dotnetcore_network \
  -v restapi_dotnetcore_db_volume:/var/lib/postgresql/data \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=mysecretpassword \
  -e POSTGRES_DB=restapi_db \
  postgres:15-alpine
```

**4. Build the API Image**
*Note: Use `--no-cache` if you changed database logic to ensure fresh migrations are generated.*
```bash
docker build --no-cache -t restapi-dotnetcore-app .
```

**5. Run the API Container**
```bash
docker run -d -p 5005:5005 \
  --env-file .env.docker \
  --network restapi_dotnetcore_network \
  -v restapi_dotnetcore_media_volume:/app/media \
  --name restapi-dotnetcore-container \
  restapi-dotnetcore-app
```
The API is now running at `http://localhost:5005`.

---

## 🐳 Docker Management Commands

Here are some useful commands to manage your containers:

#### View logs from the running container
```bash
docker logs -f restapi-dotnetcore-container
```

#### Stop the container
```bash
docker stop restapi-dotnetcore-container
```

#### Restart the container
```bash
docker start restapi-dotnetcore-container
```

#### Remove the container (after stopping)
```bash
docker rm restapi-dotnetcore-container
```

#### List existing volumes
```bash
docker volume ls
```

#### Remove a volume
> **WARNING:** This will permanently delete your database data!
```bash
docker volume rm restapi_dotnetcore_db_volume
```

---

## 📖 API Documentation (Swagger)

Once the server is running, you can access the interactive documentation:

*   **Local:** `http://localhost:5000/swagger` (Check your terminal for exact port)
*   **Docker:** `http://localhost:5005/swagger`

---

## 🧪 API Testing (Python Scripts)

We provide a suite of Python scripts in the `api_tests/` folder to test the API endpoints. These scripts automatically handle **Token Management** (saving/loading tokens to `secrets.json`).

**Prerequisite:** Open `api_tests/utils.py` and ensure `BASE_URL` matches your running server (e.g., port `5000` for Local or `5005` for Docker).

### How to Run Tests
Simply run the python files directly. No arguments are needed.

**1. Register Admin**
Creates a user and saves tokens to `secrets.json`.
```bash
python api_tests/A1.auth_register.py
```

**2. Login**
Logs in and refreshes the tokens in `secrets.json`.
```bash
python api_tests/A2.auth_login.py
```

**3. Get All Users (Admin Only)**
Uses the saved token to fetch the user list.
```bash
python api_tests/B2.user_get_all.py
```

---

## 📂 Project Structure

```text
/
├── src/
│   ├── StarterKit.Domain/          # [Layer 1] Core Entities & Enums
│   ├── StarterKit.Application/     # [Layer 2] Business Logic, DTOs, Validators
│   ├── StarterKit.Infrastructure/  # [Layer 3] Database (EF Core), Repositories, JWT
│   └── StarterKit.Api/             # [Layer 4] Controllers, Middleware, Entry Point
├── api_tests/                      # Python scripts for API testing
├── .env.docker                     # Environment variables for Docker
├── .dockerignore                   # Files ignored by Docker build
├── Dockerfile                      # Multi-stage build instruction
├── entrypoint.sh                   # Script executed when container starts
├── starter-kit-restapi-dotnetcore.sln
└── README.md
```