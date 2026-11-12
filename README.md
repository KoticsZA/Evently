# Evently 

**Discover. Explore. Experience.**

Evently is an event discovery and booking platform designed to make finding and attending events easier. The platform allows users to explore upcoming events across different cities and provinces, discover experiences that interest them, and manage their event bookings in one place.

The project aims to provide a centralised platform for event discovery and ticket booking, connecting event organisers with audiences while simplifying the process of finding and attending events.

## Core Features

- **Event Discovery:** Browse upcoming events and explore available experiences.
- **Location-Based Exploration:** Discover events across different South African cities and provinces.
- **Event Details:** Access relevant information about events, including descriptions, dates, venues and ticket availability.
- **Ticket Booking:** Facilitate ticket reservations and purchases.
- **User Authentication:** Secure user registration, login and account management.
- **Booking Management:** Keep track of event bookings and upcoming experiences.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Primary programming language |
| ASP.NET Core MVC | Web application framework |
| Entity Framework Core | Object-relational mapping (ORM) |
| SQL Server | Relational database |
| ASP.NET Core Identity | Authentication and user management |
| HTML & CSS | Frontend structure and styling |
| JavaScript | Client-side functionality |

## Architecture

Evently is developed using the Model-View-Controller (MVC) architectural pattern.

The application uses Entity Framework Core for database interactions and ASP.NET Core Identity for authentication and user management.

The project is structured to maintain separation of concerns between application logic, data access and presentation.

## Getting Started

### Prerequisites

- .NET SDK (compatible with the project)
- Visual Studio 2022 or later
- SQL Server
- Git

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/YOUR_USERNAME/Evently.git
   ```

2. Navigate to the project directory:

   ```bash
   cd Evently
   ```

3. Restore NuGet packages:

   ```bash
   dotnet restore
   ```

4. Configure your database connection string using User Secrets or another secure configuration provider.

5. Apply Entity Framework Core migrations:

   ```bash
   dotnet ef database update
   ```

6. Run the application:

   ```bash
   dotnet run
   ```

7. Open the local URL displayed in your terminal.

## Configuration

Database credentials and other sensitive configuration values should not be committed to source control.

Use ASP.NET Core User Secrets during local development to manage sensitive application settings.

## Project Status

Evently is an ongoing development project. Features and functionality are being implemented and refined as development progresses.

## Author

Developed as an independent portfolio project.

---

*Evently — Bringing events closer to you.*
