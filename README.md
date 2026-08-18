# Restaurant Table Reservation API

An enterprise-grade, RESTful Web API built with **ASP.NET Core 10** and **Entity Framework Core**. This API manages the complete lifecycle of a restaurant's table reservations, bridging the digital booking system with the physical reality of the restaurant floor.

It features a rigorous state-machine, robust concurrency controls to prevent double-booking, and Role-Based Access Control (RBAC) using JWT Authentication.

---

## 🌟 Key Features

1. **Role-Based Authentication (JWT)**
   - **Admin**: Full access to manage tables, operating hours, configurations, and oversee all reservations.
   - **Staff**: Operational access to check-in customers, handle walk-ins, mark no-shows, and complete dining sessions.
   - **Customer**: Can search for availability, book reservations, and view/cancel their own bookings securely.

2. **Advanced Concurrency & Double-Booking Prevention**
   - Utilizes `Serializable` transaction isolation levels to guarantee that two customers booking the exact same table at the exact same millisecond will not result in a double-booking.

3. **Smart Availability Engine**
   - Dynamically calculates available time slots based on the restaurant's configurable operating hours, table capacities, and existing reservations.

4. **Physical Table State Synchronization**
   - The API prevents arbitrary manual updates to table statuses. Instead, the physical `TableStatus` (Available, Reserved, Occupied) reacts dynamically and automatically to the `ReservationStatus` (Confirmed, Checked-In, Completed, Cancelled).

5. **Walk-In Handling**
   - Bypasses standard advance-notice rules. Instantly locks an available table and transitions it to `Occupied` in a single transaction.

6. **Configurable Business Rules**
   - Admins can dynamically adjust the `CancellationWindowHours` and `AdvanceNoticeHours` without changing the code.

---

## 🛠️ Technology Stack

- **Framework**: .NET 10.0 (ASP.NET Core Web API)
- **Language**: C#
- **Database**: SQL Server
- **ORM**: Entity Framework Core (Code-First)
- **Authentication**: JWT (JSON Web Tokens)
- **Documentation**: Swagger / OpenAPI

---

## 🚀 Setup & Installation

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer Edition)
- SQL Server Management Studio (SSMS) or Azure Data Studio

### 1. Clone the Repository
```bash
git clone <your-repository-url>
cd RestaurantTableReservationAPI
```

### 2. Configure the Database Connection
Open `appsettings.json` and ensure the `DefaultConnection` string points to your local SQL Server instance.
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=RestaurantDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Apply Migrations & Seed Database
The project uses EF Core Code-First. Open your terminal in the project directory and run:
```bash
dotnet ef database update
```
*Note: The application is configured to automatically seed the default Roles, Admin user, and Restaurant Configurations on startup.*

### 4. Run the API
```bash
dotnet run
```
The API will launch and be available on `http://localhost:5021`. 
Navigate to `http://localhost:5021/swagger` to interact with the endpoints.

---

## 🔐 Role Credentials for Testing

When you run the API for the first time, it automatically creates a default Admin account. You can use this account to create Staff and Customer accounts.

**Default Admin Account**
- **Email**: `admin@restaurant.com`
- **Password**: `Admin@123`

### How to Authenticate via Swagger:
1. Go to the `POST /api/auth/login` endpoint.
2. Enter the Admin credentials above.
3. Copy the `token` string from the response.
4. Scroll to the top of Swagger, click the **Authorize** button.
5. Type `Bearer <your-token>` (make sure to include the word 'Bearer' and a space) and click Authorize.

---

## 📌 Core API Endpoints

### Authentication (`/api/auth`)
- `POST /register`: Register a new Customer.
- `POST /login`: Authenticate and receive a JWT.
- `POST /register-staff`: (Admin Only) Register a new Staff member.

### Availability (`/api/availability`)
- `GET /`: Search for available time slots by Date and Party Size.

### Reservations (`/api/reservations`)
- `POST /`: (Customer) Book an available table.
- `GET /my/upcoming`: (Customer) View upcoming bookings securely.
- `PUT /{id}/cancel`: (Customer/Staff/Admin) Cancel a booking. Enforces a 24-hour window for customers, bypassable by Staff.
- `PUT /{id}/check-in`: (Staff) Check in a customer. Instantly marks the physical table as `Occupied`.
- `POST /walk-in`: (Staff) Instantly assigns and locks an available table for a physical walk-in.
- `PUT /{id}/complete`: (Staff) Completes the dining experience and clears the table back to `Available`.

### Administration (`/api/tables`, `/api/operatinghours`, `/api/config`)
- Full CRUD endpoints strictly locked to the `Admin` role to manage the physical architecture and rules of the restaurant.

---

## 🏗️ Architecture & Patterns
- **Repository Pattern**: Abstracts database operations and promotes testability.
- **Service Layer**: Encapsulates core business logic, validation, and strict state-machine rules.
- **DTOs (Data Transfer Objects)**: Strictly controls the data shape entering and leaving the API.
- **Global Error Handling**: Standardized error responses to prevent sensitive stack traces from leaking.
