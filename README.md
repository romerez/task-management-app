# Task Management Application

A full-stack task management application built with .NET Core, React, SQL Server, and RabbitMQ.

## 📋 Features

- ✅ Create, Read, Update, Delete tasks
- ✅ Task priority levels (Low, Medium, High, Critical)
- ✅ User details (Full Name, Email, Phone)
- ✅ Overdue task tracking
- ✅ Background reminder service with RabbitMQ

---

## 🛠️ Prerequisites

Make sure you have installed:

| Tool | Version | Download |
|------|---------|----------|
| Docker Desktop | Latest | [docker.com](https://www.docker.com/products/docker-desktop/) |
| .NET SDK | 9.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| Node.js | 18+ | [nodejs.org](https://nodejs.org/) |

---

## 🚀 Quick Start

### Step 1: Clone & Setup Environment

```bash
# Clone the repository
git clone <repository-url>
cd Supercom

# Create environment file from example
copy .env.example .env
```

Edit `.env` file with your passwords:
```env
SA_PASSWORD=YourStrongPassword123!
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=YourRabbitMQPassword!
```

---

### Step 2: Start Docker Containers

```bash
# Start SQL Server and RabbitMQ
docker-compose up -d

# Verify containers are running
docker ps
```

You should see:
- `taskmanagement-db` (SQL Server on port 1433)
- `taskmanagement-rabbitmq` (RabbitMQ on ports 5672, 15672)

> 💡 RabbitMQ Management UI: http://localhost:15672 (guest/guest)

---

### Step 3: Configure .NET User Secrets

#### Backend API:
```bash
cd backend/TasksManagerAPI

# Initialize secrets (if not already done)
dotnet user-secrets init

# Set the connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=TasksManagerDb;User ID=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

#### Reminder Service:
```bash
cd ReminderService/TaskManagement.ReminderService

# Initialize secrets
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=TasksManagerDb;User ID=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True;MultipleActiveResultSets=true"
dotnet user-secrets set "RabbitMQ:Password" "YOUR_RABBITMQ_PASSWORD"
```

---

### Step 4: Run the Backend API

```bash
cd backend/TasksManagerAPI

# Restore packages
dotnet restore

# Run the API
dotnet run
```

The API will start at:
- **HTTPS:** https://localhost:7001
- **HTTP:** http://localhost:5000
- **Swagger UI:** https://localhost:7001/swagger

> 📝 The database will be created automatically on first run (EF Core migrations).

---

### Step 5: Run the Frontend

```bash
cd frontend

# Install dependencies
npm install

# Start the development server
npm start
```

The React app will open at: **http://localhost:3000**

---

### Step 6: (Optional) Run the Reminder Service

```bash
cd ReminderService/TaskManagement.ReminderService

dotnet restore
dotnet run
```

This background service:
- Checks for overdue tasks every 5 minutes
- Publishes reminders to RabbitMQ
- Logs: `"Hi your Task is due {Task xxxxx}"`

---

## 📁 Project Structure

```
Supercom/
├── backend/
│   └── TasksManagerAPI/       # .NET Core REST API
│       ├── Controllers/       # API endpoints
│       ├── Services/          # Business logic
│       ├── Repositories/      # Data access
│       ├── Models/            # Domain entities
│       └── DTOs/              # Data transfer objects
│
├── frontend/                  # React Application
│   ├── src/
│   │   ├── components/        # UI components
│   │   ├── store/             # Redux state management
│   │   ├── services/          # API client
│   │   └── types/             # TypeScript types
│
├── ReminderService/           # Background Worker Service
│   └── TaskManagement.ReminderService/
│
└── docker-compose.yml         # SQL Server & RabbitMQ
```

---

## 🔌 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks` | Get all tasks |
| GET | `/api/tasks/{id}` | Get task by ID |
| GET | `/api/tasks/overdue` | Get overdue tasks |
| POST | `/api/tasks` | Create a new task |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Delete a task |

---

## 🧪 Running Tests

### Backend Tests:
```bash
cd backend/TasksManagerAPI.Tests
dotnet test
```

### Reminder Service Tests:
```bash
cd ReminderService/TaskManagement.ReminderService.Tests
dotnet test
```

### Frontend Tests:
```bash
cd frontend
npm test
```

---

## 🛑 Stopping the Application

```bash
# Stop Docker containers
docker-compose down

# To also remove data volumes:
docker-compose down -v
```

---

## 🔧 Troubleshooting

### Docker containers won't start
```bash
# Check logs
docker logs taskmanagement-db
docker logs taskmanagement-rabbitmq
```

### Database connection issues
- Ensure SQL Server container is healthy: `docker ps`
- Verify password in user-secrets matches `.env` file
- Wait 30 seconds after container start for SQL Server to initialize

### Port already in use
```bash
# Find process using port
netstat -ano | findstr :1433
netstat -ano | findstr :3000

# Kill process by PID
taskkill /PID <pid> /F
```

### Reset everything
```bash
docker-compose down -v
docker-compose up -d
```

---

## 📚 Tech Stack

| Layer | Technology |
|-------|------------|
| Backend API | .NET 9, Entity Framework Core |
| Frontend | React 18, TypeScript, Redux Toolkit, Material-UI |
| Database | SQL Server 2022 |
| Message Queue | RabbitMQ |
| Containerization | Docker |

---

## 📄 License

This project is for demonstration purposes.
