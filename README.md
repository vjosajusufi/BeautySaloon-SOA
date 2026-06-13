# Beauty Salon Appointment Booking System

A web-based appointment booking system for a beauty salon, built using **ASP.NET Core Web API (.NET 10)** with a **React + Vite** frontend. Developed as part of the Service Oriented Architecture course at South East European University.

---

## Live Demo

- **Frontend:** https://beautysaloon-client.onrender.com
- **Backend API:** https://beautysaloon-api.onrender.com
- **Swagger UI:** https://beautysaloon-api.onrender.com/swagger

---

## Table of Contents

- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Features](#features)
- [Getting Started](#getting-started)
- [Running the Backend](#running-the-backend)
- [Running the Frontend](#running-the-frontend)
- [Running Tests](#running-tests)
- [Environment Variables](#environment-variables)
- [API Endpoints](#api-endpoints)
- [Deployment](#deployment)
- [CI/CD Pipeline](#cicd-pipeline)

---

## Project Structure

```
BeautySaloon-SOA/
│
├── .github/
│   └── workflows/
│       └── ci-cd.yml               # GitHub Actions CI/CD pipeline
│
├── BeautySaloon-API/
│   └── BeautySaloon-API/
│       ├── Controllers/             # API controllers (Auth, Appointments, Services, Users, WorkingHours)
│       ├── Data/
│       │   ├── AppDbContext.cs      # Entity Framework Core database context
│       │   └── DatabaseSeeder.cs   # Seeds initial data (admin user, services, working hours)
│       ├── DTOs/                   # Data Transfer Objects for request/response models
│       ├── Helpers/
│       │   └── MappingProfile.cs   # AutoMapper profile for entity-DTO mappings
│       ├── Migrations/             # Entity Framework Core database migrations
│       ├── Models/                 # Entity models (User, Appointment, Service, WorkingHours)
│       ├── Repositories/           # Repository implementations and interfaces
│       ├── Services/               # Business logic services and interfaces
│       ├── appsettings.json        # Application configuration
│       ├── Dockerfile              # Docker configuration for containerized deployment
│       └── Program.cs              # Application entry point and service registration
│
├── BeautySaloon.Tests/
│   ├── Controllers/                # Controller unit tests
│   ├── Repositories/               # Repository unit tests (EF Core InMemory)
│   ├── Services/                   # Service unit tests (NSubstitute mocks)
│   ├── coverage-report/            # HTML coverage report
│   └── coverage.runsettings        # Coverage configuration (excludes Migrations, Program.cs)
│
├── beauty-salon-client/
│   ├── public/                     # Static assets
│   ├── src/
│   │   ├── api/
│   │   │   └── axios.js            # Axios instance with base URL configuration
│   │   ├── components/             # Reusable React components
│   │   ├── pages/                  # Page components (Home, Login, Register, Appointments, etc.)
│   │   └── main.jsx                # React application entry point
│   ├── index.html                  # HTML entry point
│   ├── package.json                # Node.js dependencies
│   └── vite.config.js              # Vite build configuration
│
└── README.md                       # This file
```

---

## Technologies Used

### Backend
- **ASP.NET Core Web API** (.NET 10)
- **Entity Framework Core** — ORM for database access
- **PostgreSQL** — Relational database
- **JWT Authentication** — Secure token-based auth
- **AutoMapper** — Object-to-object mapping
- **Swagger / OpenAPI** — API documentation

### Frontend
- **React 18** — UI library
- **Vite** — Build tool
- **Axios** — HTTP client

### Testing
- **xUnit** — Test framework
- **NSubstitute** — Mocking library
- **EF Core InMemory** — In-memory database for repository tests
- **coverlet** — Code coverage collection
- **ReportGenerator** — HTML coverage reports

### DevOps
- **GitHub Actions** — CI/CD pipeline
- **Render** — Cloud deployment (backend + frontend + database)
- **Docker** — Backend containerization

---

## Features

- Customer registration and login with JWT authentication
- Browse available salon services
- Book and cancel appointments
- Double-booking prevention
- Admin management of users, services, appointments, and working hours
- Role-based authorization (Client / Admin)
- Response caching and output caching
- Automatic database migrations on startup

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

### Clone the Repository

```bash
git clone https://github.com/vjosajusufi/BeautySaloon-SOA.git
cd BeautySaloon-SOA
```

---

## Running the Backend

### 1. Configure the database

Open `BeautySaloon-API/BeautySaloon-API/appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=BeautySalonDB;Username=postgres;Password=yourpassword"
}
```

### 2. Restore and run

```bash
cd BeautySaloon-API/BeautySaloon-API
dotnet restore
dotnet run
```

The API will start at `http://localhost:5262`. Migrations and database seeding run automatically on startup.

### 3. Access Swagger UI

Open your browser and go to:
```
http://localhost:5262/swagger
```

---

## Running the Frontend

```bash
cd beauty-salon-client
npm install
npm run dev
```

The frontend will start at `http://localhost:5173`.

---

## Running Tests

### Run all tests

```bash
cd BeautySaloon.Tests
dotnet test
```

### Run tests with coverage

```bash
dotnet test --settings coverage.runsettings
reportgenerator -reports:TestResults\**\coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
start coverage-report\index.html
```

### Test summary

| Category | Tests |
|---|---|
| Services/AuthServiceTests | 4 |
| Services/SalonServiceServiceTests | 6 + 2 |
| Services/AppointmentServiceTests | 3 + 8 |
| Services/UserServiceTests | 11 |
| Services/WorkingHoursServiceTests | 9 |
| Controllers/AppointmentsControllerTests | 5 + 7 |
| Controllers/AuthControllerTests | 4 |
| Controllers/ServicesControllerTests | 5 + 3 |
| Controllers/UsersControllerTests | 9 |
| Controllers/WorkingHoursControllerTests | 6 |
| Repositories/AppointmentRepositoryTests | 4 + 4 |
| Repositories/ServiceRepositoryTests | 3 + 3 |
| Repositories/UserRepositoryTests | 8 |
| Repositories/WorkingHoursRepositoryTests | 7 |
| **Total** | **118** |

**Line coverage: 99.8% | Branch coverage: 94.8%**

---

## Environment Variables

### Backend (Render / Production)

| Variable | Description |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Jwt__Key` | JWT signing key (min 32 characters) |
| `Jwt__Issuer` | JWT issuer |
| `Jwt__Audience` | JWT audience |
| `Jwt__ExpiryDays` | Token expiry in days |

### Frontend (Render / Production)

| Variable | Description |
|---|---|
| `VITE_API_URL` | Backend API base URL |

---

## API Endpoints

### Auth
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and get JWT token |

### Services
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/services` | Get all active services | Public |
| GET | `/api/services/{id}` | Get service by ID | Public |
| POST | `/api/services` | Create a service | Admin |
| PUT | `/api/services/{id}` | Update a service | Admin |
| DELETE | `/api/services/{id}` | Delete a service | Admin |

### Appointments
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/appointments` | Get all appointments | Admin |
| GET | `/api/appointments/{id}` | Get appointment by ID | Auth |
| GET | `/api/appointments/my` | Get my appointments | Auth |
| POST | `/api/appointments` | Book an appointment | Auth |
| PUT | `/api/appointments/{id}` | Update appointment | Auth |
| DELETE | `/api/appointments/{id}` | Cancel appointment | Auth |

### Users
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/users` | Get all users | Admin |
| GET | `/api/users/{id}` | Get user by ID | Admin |
| GET | `/api/users/me` | Get current user | Auth |
| PUT | `/api/users/{id}` | Update user | Auth |
| DELETE | `/api/users/{id}` | Delete user | Admin |

### Working Hours
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/workinghours` | Get all working hours | Public |
| GET | `/api/workinghours/{id}` | Get by ID | Public |
| POST | `/api/workinghours` | Create working hours | Admin |
| PUT | `/api/workinghours/{id}` | Update working hours | Admin |

---

## Deployment

The application is deployed on **Render**:

- **Database:** Render PostgreSQL (free tier)
- **Backend:** Render Web Service using Docker
- **Frontend:** Render Static Site

### Backend Dockerfile

The backend uses a multi-stage Docker build:
1. **Build stage** — restores and builds the .NET project
2. **Publish stage** — publishes the release build
3. **Final stage** — runs the published app on ASP.NET runtime

---

## CI/CD Pipeline

The GitHub Actions pipeline (`.github/workflows/ci-cd.yml`) runs on every push to `main`:

1. **Test job** — runs all 118 xUnit tests on Ubuntu
2. **Deploy backend** — triggers Render backend deploy (only if tests pass)
3. **Deploy frontend** — triggers Render frontend deploy (only if tests pass)

This ensures broken code is never deployed to production.

---

## Author

**Vjosa Jusufi**  
South East European University  
Service Oriented Architecture — 2026
