# Test Documentation

This document describes all automated tests in the Task Management application.

---

## 📊 Test Summary

| Project | Framework | Tests | Coverage |
|---------|-----------|-------|----------|
| Backend API | xUnit + Moq | 20 tests | Controller + Service layers |
| Reminder Service | xUnit + Moq | 6+ tests | Background service logic |
| Frontend | Jest | 25+ tests | Form validation |

---

## 🔵 Backend API Tests

**Location:** `backend/TasksManagerAPI.Tests/`

### Running Tests
```bash
cd backend/TasksManagerAPI.Tests
dotnet test
```

---

### Controller Tests (`Controllers/TasksControllerTests.cs`)

Tests the REST API endpoints and HTTP responses.

| Test Name | What It Tests |
|-----------|---------------|
| `GetAll_ReturnsOkWithTasks` | GET `/api/tasks` returns 200 OK with list of tasks |
| `GetById_WhenTaskExists_ReturnsOkWithTask` | GET `/api/tasks/{id}` returns 200 OK when task found |
| `GetById_WhenTaskNotExists_ReturnsNotFound` | GET `/api/tasks/{id}` returns 404 when task not found |
| `GetOverdue_ReturnsOkWithOverdueTasks` | GET `/api/tasks/overdue` returns overdue tasks |
| `Create_WithValidDto_ReturnsCreatedAtAction` | POST `/api/tasks` returns 201 Created with location header |
| `Update_WhenTaskExists_ReturnsOkWithUpdatedTask` | PUT `/api/tasks/{id}` returns 200 OK when task updated |
| `Update_WhenTaskNotExists_ReturnsNotFound` | PUT `/api/tasks/{id}` returns 404 when task not found |
| `Delete_WhenTaskExists_ReturnsNoContent` | DELETE `/api/tasks/{id}` returns 204 No Content on success |
| `Delete_WhenTaskNotExists_ReturnsNotFound` | DELETE `/api/tasks/{id}` returns 404 when task not found |

---

### Service Tests (`Services/TaskServiceTests.cs`)

Tests the business logic layer.

| Test Name | What It Tests |
|-----------|---------------|
| `GetAllTasksAsync_ReturnsAllTasks` | Retrieves all tasks from repository |
| `GetTaskByIdAsync_WhenTaskExists_ReturnsTask` | Finds and returns existing task |
| `GetTaskByIdAsync_WhenTaskNotExists_ReturnsNull` | Returns null for non-existent task |
| `GetOverdueTasksAsync_ReturnsOverdueTasks` | Filters tasks past due date |
| `CreateTaskAsync_CreatesAndReturnsTask` | Creates new task and returns DTO |
| `CreateTaskAsync_TrimsInputFields` | Removes whitespace from user input |
| `UpdateTaskAsync_WhenTaskExists_ReturnsUpdatedTask` | Updates existing task |
| `UpdateTaskAsync_WhenTaskNotExists_ReturnsNull` | Returns null when updating non-existent task |
| `DeleteTaskAsync_WhenTaskExists_ReturnsTrue` | Returns true on successful deletion |
| `DeleteTaskAsync_WhenTaskNotExists_ReturnsFalse` | Returns false when task not found |

---

## 🟠 Reminder Service Tests

**Location:** `ReminderService/TaskManagement.ReminderService.Tests/`

### Running Tests
```bash
cd ReminderService/TaskManagement.ReminderService.Tests
dotnet test
```

---

### TaskReminderService Tests (`Services/TaskReminderServiceTests.cs`)

Tests the background worker that checks for overdue tasks and publishes reminders.

| Test Name | What It Tests |
|-----------|---------------|
| `CheckAndPublishOverdueTasksAsync_WithNoOverdueTasks_ReturnsZero` | No reminders sent when all tasks are in the future |
| `CheckAndPublishOverdueTasksAsync_WithOverdueTasks_PublishesReminders` | Publishes reminder to RabbitMQ when task is overdue |
| `CheckAndPublishOverdueTasksAsync_SkipsTasksWithRecentReminder` | Avoids duplicate reminders (once per day) |
| `CheckAndPublishOverdueTasksAsync_ProcessesMultipleOverdueTasks` | Handles batch processing of multiple overdue tasks |
| `CheckAndPublishOverdueTasksAsync_UpdatesLastReminderSentAt` | Updates timestamp after sending reminder |
| `CheckAndPublishOverdueTasksAsync_StopsOnCancellation` | Gracefully stops when cancellation requested |

**Key Testing Techniques:**
- Uses **In-Memory Database** for isolated testing
- **Mocks** RabbitMQ service to verify message publishing
- Tests **idempotency** (no duplicate reminders)

---

## 🟢 Frontend Tests

**Location:** `frontend/src/utils/validation.test.ts`

### Running Tests
```bash
cd frontend
npm test
```

---

### Form Validation Tests

Tests client-side validation for the task form.

#### Title Validation
| Test | What It Tests |
|------|---------------|
| `should return error when title is empty` | Title is required |
| `should return error when title exceeds 100 characters` | Max length enforcement |
| `should not return error for valid title` | Valid input passes |

#### Description Validation
| Test | What It Tests |
|------|---------------|
| `should return error when description is empty` | Description is required |
| `should return error when description exceeds 500 characters` | Max length enforcement |
| `should not return error for valid description` | Valid input passes |

#### Due Date Validation
| Test | What It Tests |
|------|---------------|
| `should return error when dueDate is empty` | Due date is required |
| `should not return error for valid dueDate` | Valid date passes |

#### Priority Validation
| Test | What It Tests |
|------|---------------|
| `should return error when priority is undefined` | Priority is required |
| `should accept all priority levels` | Low, Medium, High, Critical all valid |

#### Full Name Validation
| Test | What It Tests |
|------|---------------|
| `should return error when fullName is empty` | Name is required |
| `should return error when fullName is less than 2 characters` | Min length = 2 |
| `should return error when fullName exceeds 100 characters` | Max length = 100 |
| `should not return error for valid fullName` | Valid name passes |

#### Telephone Validation
| Test | What It Tests |
|------|---------------|
| `should return error when telephone is empty` | Phone is required |
| `should return error for invalid phone format` | Rejects non-phone strings |
| `should accept valid phone formats` | Accepts: `123-456-7890`, `+1 234 567 8901`, `(123) 456-7890` |

#### Email Validation
| Test | What It Tests |
|------|---------------|
| `should return error when email is empty` | Email is required |
| `should return error for invalid email format` | Rejects: `invalid`, `@test.com` |
| `should return error when email exceeds 100 characters` | Max length = 100 |
| `should not return error for valid email` | Valid email passes |

#### Complete Form Validation
| Test | What It Tests |
|------|---------------|
| `should return no errors for valid form data` | All valid = no errors |
| `should return multiple errors for invalid form data` | Returns all 7 field errors |

#### Utility Function Tests
| Test | What It Tests |
|------|---------------|
| `hasValidationErrors - empty object` | Returns false when no errors |
| `hasValidationErrors - with errors` | Returns true when errors exist |

---

## 🧪 Running All Tests

```bash
# From project root

# Backend API tests
cd backend/TasksManagerAPI.Tests && dotnet test

# Reminder Service tests  
cd ReminderService/TaskManagement.ReminderService.Tests && dotnet test

# Frontend tests
cd frontend && npm test

# Run all with coverage (backend)
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📝 Test Patterns Used

| Pattern | Where Used | Purpose |
|---------|------------|---------|
| **Arrange-Act-Assert** | All tests | Clear test structure |
| **Mocking (Moq)** | Service/Controller tests | Isolate dependencies |
| **In-Memory Database** | ReminderService tests | Fast DB testing |
| **Test Data Builders** | Backend tests | Consistent test data |
| **Parameterized Tests** | Frontend validation | Test multiple inputs |
