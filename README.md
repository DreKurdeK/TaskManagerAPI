# Task Manager API

## Description
Task Manager API is a simple RESTful web application designed to manage ToDo tasks.
The API provides endpoints for creating, retrieving, updating, and deleting tasks, as well as additional features such as searching and filtering tasks by specific criteria.

The project is built using .NET 9 Minimal API, with PostgreSQL as the database, and uses FluentValidation for data validation and EntityFramework Core. It also includes xUnit tests to ensure the application is working.

---

## Features
### Task Management:
- Create a new ToDo task.
- Retrieve all tasks or specific tasks by ID.
- Update existing tasks.
- Mark tasks as complete.
- Delete tasks.

### Filtering:
- Get tasks for today, tomorrow, or the current week.
- Filter tasks by a specific title.
- Retrieve tasks within a specific number of days.

### Validation:
- Input data is validated using FluentValidation

---

## Endpoints
| Method   | Endpoint                   | Description                                     |
|----------|----------------------------|-------------------------------------------------|
| `GET`    | `/todos`                   | Retrieve all tasks.                            |
| `GET`    | `/todos/{id}`              | Retrieve a task by its ID.                     |
| `GET`    | `/todos/search/{title}`    | Search tasks by title.                         |
| `GET`    | `/todos/upcoming/{days}`   | Get tasks due within a specified number of days. |
| `GET`    | `/todos/today`             | Get tasks scheduled for today.                 |
| `GET`    | `/todos/next-day`          | Get tasks scheduled for tomorrow.              |
| `GET`    | `/todos/current-week`      | Get tasks scheduled for the current week.      |
| `POST`   | `/todos`                   | Create a new task.                             |
| `PUT`    | `/todos/{id}`              | Update an existing task.                       |
| `PATCH`  | `/todos/{id}/done`         | Mark a task as done.                           |
| `DELETE` | `/todos/{id}`              | Delete a task by its ID.                       |

---

## Technologies Used
- **Framework:** .NET 9 Minimal API
- **Database:** PostgreSQL (Entity Framework Core)
- **Validation:** FluentValidation
- **Testing:** xUnit (Integration Tests)
- **Containerization (Optional):** Docker

---

## Setup and Installation

### Prerequisites
- .NET 9 SDK
- PostgreSQL
- Docker (optional)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/task-manager-api.git
   cd task-manager-api
2. Configure the database:
    Open appsettings.json.
    Update the connection string to point to your PostgreSQL instance. Example:
    ```bash
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=TaskManagerDB;Username=your_username;Password=your_password"
      }

3. Apply migrations:
4. ```bash
    dotnet ef database update
5. Optionally, build and run with Docker:
    ```bash
    docker-compose up
6. Access the API at:
http://localhost:5000

