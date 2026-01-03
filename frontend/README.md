# Task Manager Frontend

A React application for managing user tasks with full CRUD operations.

## Features

- ✅ View all tasks in a responsive grid layout
- ✅ Create new tasks with validation
- ✅ Edit existing tasks
- ✅ Delete tasks with confirmation
- ✅ Filter by all tasks or overdue tasks
- ✅ Form validation for all fields
- ✅ Redux state management
- ✅ Loading states and error handling
- ✅ Responsive design

## Tech Stack

- **React 18** - UI library
- **TypeScript** - Type safety
- **Redux Toolkit** - State management
- **Axios** - HTTP client

## Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── TaskCard/       # Individual task display
│   ├── TaskForm/       # Create/Edit task form
│   └── TaskList/       # Task grid container
├── services/           # API service layer
│   └── taskService.ts  # Backend API calls
├── store/              # Redux state management
│   ├── hooks.ts        # Typed Redux hooks
│   ├── store.ts        # Store configuration
│   └── taskSlice.ts    # Task state slice
├── types/              # TypeScript type definitions
│   └── task.types.ts   # Task-related types
├── utils/              # Utility functions
│   ├── formatters.ts   # Date/priority formatters
│   └── validation.ts   # Form validation
├── App.tsx             # Main application component
├── App.css             # Global styles
└── index.tsx           # Entry point
```

## Setup Instructions

### Prerequisites

- Node.js 16+ installed
- Backend API running (default: https://localhost:7001)

### Installation

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Configure the API URL:
   - Edit `.env` file and update `REACT_APP_API_URL` to match your backend URL
   - Default: `https://localhost:7001/api`

4. Start the development server:
   ```bash
   npm start
   ```

5. Open [http://localhost:3000](http://localhost:3000) in your browser

## API Integration

The frontend connects to the following backend endpoints:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/tasks | Get all tasks |
| GET | /api/tasks/:id | Get task by ID |
| GET | /api/tasks/overdue | Get overdue tasks |
| POST | /api/tasks | Create new task |
| PUT | /api/tasks/:id | Update existing task |
| DELETE | /api/tasks/:id | Delete task |

## State Management

Uses Redux Toolkit for state management with the following features:

- **Async Thunks**: For API calls with loading states
- **Error Handling**: Centralized error state
- **Selectors**: Typed selectors for accessing state

## Validation Rules

All fields are validated according to backend requirements:

| Field | Rules |
|-------|-------|
| Title | Required, 1-100 characters |
| Description | Required, max 500 characters |
| Due Date | Required |
| Priority | Required (Low, Medium, High, Critical) |
| Full Name | Required, 2-100 characters |
| Telephone | Required, valid phone format, max 20 characters |
| Email | Required, valid email format, max 100 characters |

## Available Scripts

- `npm start` - Run development server
- `npm build` - Build for production
- `npm test` - Run tests

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| REACT_APP_API_URL | Backend API base URL | https://localhost:7001/api |
