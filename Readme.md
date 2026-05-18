# 🛡️ API Rate Limiter & Analytics Engine

A high-performance, custom-built API Rate Limiting and Analytics system developed in C# and ASP.NET Core Minimal APIs. This project avoids heavy ORMs and demonstrates core backend engineering principles, including thread-safe memory operations, HTTP pipeline middleware, and raw database interactions.

## 🚀 Key Features

*   **Custom Rate Limiting Middleware:** Intercepts incoming requests and limits traffic based on client IP addresses (e.g., Max 5 requests per minute).
*   **Thread-Safe Memory Operations:** Uses `ConcurrentDictionary` to handle high-concurrency request counting without blocking threads.
*   **Fire-and-Forget Database Logging:** Asynchronously logs request data (IP, Time, Block Status) to SQL Server without slowing down the main API response time.
*   **Raw SQL & ADO.NET:** Bypasses Entity Framework to use raw SQL queries for maximum control and performance.
*   **Analytics Dashboard API:** Provides a statistical overview of API traffic, including total requests, blocked requests, and top spammer IPs using SQL aggregations.

## 🛠️ Tech Stack

*   **Language:** C# and .NET 8.0
*   **Framework:** ASP.NET Core Minimal APIs
*   **Database:** MS SQL Server
*   **Data Access:** ADO.NET (`Microsoft.Data.SqlClient`)
*   **Concepts:** Middlewares, `ConcurrentDictionary`, Async/Await I/O, Raw SQL.

## ⚙️ How to Run Locally

**1. Clone the repository**
```bash
git clone https://github.com/your-username/rate-limiter-api.git
cd rate-limiter-api
```

**2. Setup the Database (SQL Server)**
Run the following SQL script in SSMS or Azure Data Studio:
```sql
CREATE DATABASE ApiAnalyticsDB;
GO
USE ApiAnalyticsDB;
GO
CREATE TABLE RequestLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IPAddress VARCHAR(50) NOT NULL,
    RequestTime DATETIME DEFAULT GETDATE(),
    IsBlocked BIT NOT NULL
);
```

**3. Configure Connection String**
Update the `_connectionString` in both `Program.cs` and `RateLimitingMiddleware.cs` with your local SQL Server instance name.

**4. Run the API**
```bash
dotnet run
```

## 🔌 API Endpoints

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/students` | Fetches a mock list of students. Triggers rate limit tracking. |
| `POST` | `/api/students` | Adds a new student via JSON payload. Triggers rate limit tracking. |
| `GET` | `/api/analytics` | Returns real-time database stats (Total Requests, Blocked Count, Top Spammer). |