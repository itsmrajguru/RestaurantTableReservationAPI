# Restaurant Table Reservation API - Code Walkthrough & Viva Guide

This document is your **study guide** to understand the entire codebase from scratch. You can read this to prepare for your viva/presentation. It explains how all the folders and files connect with each other.

---

## 1. The Architecture (Layered Approach)

Our project uses the **N-Tier (Layered) Architecture**. When a user (from Postman or Web App) makes a request, the data flows like this:

`Request` ➔ `Controller` ➔ `Service Layer` ➔ `Repository Layer` ➔ `Database (SQL Server)`

- **Controllers:** (The Waiter) They take the order (HTTP Request) from the user.
- **Services:** (The Chef) They contain the actual Business Logic (like checking if a table is free).
- **Repositories:** (The Kitchen Staff) They only do one thing: talk to the Database using Entity Framework Core.
- **Models/DTOs:** (The Ingredients) The data structures that are passed around.

---

## 2. Models & Enums (The Database Tables)
*Path: `/Models`*

Models represent our actual Database Tables using Entity Framework Core (EF Core).
- **User.cs:** Stores Admin, Staff, and Customer details along with their hashed passwords.
- **RestaurantTable.cs:** Details about tables (TableNumber, Capacity, IsActive).
- **TimeSlot.cs:** Predefined slots (e.g., 10 AM to 12 PM) for reservations.
- **Reservation.cs:** The main entity tying everything together (UserId + TableId + TimeSlotId + Date).
- **OperatingHours.cs & RestaurantConfiguration.cs:** Master data for restaurant rules.
- **Enums (`UserRoles`, `ReservationStatus`):** Keep our statuses strict (e.g., Pending, Confirmed, WalkIn, Cancelled).

---

## 3. DTOs (Data Transfer Objects)
*Path: `/DTOs`*

Why do we need DTOs? We never expose our raw Database Models to the outside world for security reasons. 
- Example: When registering a user, the user sends a password. But when returning the user data, we DO NOT return the `PasswordHash`. 
- So, we use **Request DTOs** (what we receive from the user) and **Response DTOs** (what we send back to the user).
- **AutoMapper** (configured in `/Mappings/MappingProfile.cs`) automatically converts our Database Models to DTOs and vice-versa so we don't have to write manual mapping code everywhere.

---

## 4. The Data Layer (DbContext & Repositories)
*Path: `/Data` and `/Repositories`*

- **AppDbContext.cs:** This is the bridge between our C# code and the SQL Server database. It inherits from EF Core's `DbContext`. It maps our C# Models to SQL Tables using `DbSet`.
- **DataSeeder.cs:** When the app starts, this file checks if the database is empty. If yes, it automatically inserts demo tables, time slots, and users (Pratik, Vikrant, etc.) so we don't have to test with a blank database.
- **Repositories (e.g., `ReservationRepository.cs`):** 
  - The Repository Pattern isolates the database logic.
  - If we want to find a reservation, the Service layer asks the Repository `GetByIdAsync()`. The repository writes the LINQ query to fetch it.

---

## 5. The Services Layer (The Brain / Business Logic)
*Path: `/Services`*

This is the most important folder. All the "thinking" happens here.
- **AuthService.cs:** 
  - Handles Login and Registration. 
  - Verifies passwords using `BCrypt`.
  - Generates the JWT (JSON Web Token) which contains the user's ID and Role.
- **ReservationService.cs:** 
  - Creates reservations. But before saving to DB, it calls the `ReservationValidationEngine`.
  - It checks if the restaurant is closed on that day.
  - It checks if the user is trying to book beyond the max capacity.
  - It checks if someone else has already booked that Table on that Date & TimeSlot. (Overlap prevention).
  - Handles Status changes (Confirm, Cancel, Check-In, No-Show).
- **AvailabilityService.cs:** Calculates which tables and slots are currently free for a given date and party size.

---

## 6. The Controllers (The Entry Points)
*Path: `/Controllers`*

Controllers define the API Endpoints (URLs like `/api/reservations`).
- **AuthController:** Exposes `/login` and `/register`.
- **ReservationsController:** 
  - Contains endpoints like `POST /`, `GET /my/upcoming`, `PUT /{id}/confirm`.
  - Uses the `[Authorize(Roles = "...")]` attribute. For example, `[Authorize(Roles = "Customer")]` ensures that if a Staff tries to access a customer-only endpoint, they get a `403 Forbidden` error.
- Controllers **never** write database queries. They just receive the DTO, pass it to the Service, and return an HTTP Status Code (`200 OK`, `201 Created`, `400 Bad Request`).

---

## 7. Security & Middlewares
*Path: `/Middlewares` and `/Filters`*

- **GlobalExceptionMiddleware.cs:** 
  - Imagine if a database error happens; we don't want the user to see a massive red error trace (which is a security risk). 
  - This middleware acts like a safety net. It catches ANY crash in the application and returns a clean JSON message with a `500 Internal Server Error` or `400 Bad Request`.
- **JWT (JSON Web Token):** Configured in `Program.cs`. When a user logs in, they get a Token. They must send this Token in the `Authorization` header for every future request. 

---

## 8. Program.cs (The Startup File)

This is the very first file that runs when you type `dotnet run`.
What it does:
1. **Dependency Injection (DI):** You will see many lines like `builder.Services.AddScoped<IReservationService, ReservationService>();`. This tells ASP.NET Core how to inject dependencies automatically.
2. **Database Connection:** Reads the connection string from `appsettings.json` and connects to SQL Server.
3. **Authentication Setup:** Configures the JWT Bearer token validation rules.
4. **Swagger Setup:** Generates the beautiful API documentation UI we see in the browser.
5. **App Pipeline:** Runs the database migrations, seeds the data, and starts listening for HTTP requests on port 5021.

---

### 🎯 How to explain a specific flow in an interview?
**Example Question:** *"How does creating a reservation work in your API?"*
**Your Answer:** 
1. The customer sends a JSON payload to `ReservationsController` via POST.
2. The Controller checks if the JWT token is valid and extracts the `UserId`.
3. The Controller passes the DTO to `ReservationService.CreateReservationAsync()`.
4. The Service uses `ReservationValidationEngine` to check if the table is available and the restaurant is open.
5. If valid, the Service maps the DTO to a `Reservation` model.
6. The Service passes this model to `ReservationRepository`.
7. The Repository uses `AppDbContext` to `Save()` it to SQL Server.
8. The Service returns the success response to the Controller, which returns a `201 Created` to the customer.
