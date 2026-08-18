# Database Schema (ER Diagram)

This project uses Entity Framework Core (Code-First approach) with SQL Server. Below is the Entity-Relationship (ER) diagram representing our database schema.

```mermaid
erDiagram
    Users ||--o{ Reservations : "places"
    RestaurantTables ||--o{ Reservations : "booked for"
    TimeSlots ||--o{ Reservations : "scheduled during"

    Users {
        int Id PK
        string Name
        string Email
        string PasswordHash
        string Role
        datetime CreatedAt
    }

    RestaurantTables {
        int Id PK
        string TableNumber
        int Capacity
        bool IsActive
        string Description
        string Status
    }

    TimeSlots {
        int Id PK
        time StartTime
        time EndTime
        bool IsActive
    }

    Reservations {
        int Id PK
        date ReservationDate
        int PartySize
        string Notes
        string Status
        int UserId FK
        int TableId FK
        int TimeSlotId FK
    }

    OperatingHours {
        int Id PK
        int DayOfWeek
        time OpeningTime
        time ClosingTime
        bool IsClosed
    }

    RestaurantConfigurations {
        int Id PK
        int MaxPartySize
        int CancellationWindowHours
        int AdvanceBookingDays
    }
```

### Table Relationships:
1. **Users ↔ Reservations (1:N):** One user (Customer) can place multiple reservations, but each reservation belongs to exactly one user.
2. **Tables ↔ Reservations (1:N):** One table can be booked multiple times (on different dates/times), but each reservation is tied to a specific table.
3. **TimeSlots ↔ Reservations (1:N):** A specific time slot (e.g., 10:00 AM - 12:00 PM) can have multiple reservations (across different tables), but each reservation is locked to exactly one time slot.

*(Note: EF Core Migrations are already generated and can be found inside the `Migrations/` folder in the source code).*
