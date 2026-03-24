# Conduit — RealWorld Backend API · .NET 10

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-10.0-388E3C?style=for-the-badge&logo=dotnet)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=JSON%20web%20tokens)

> A fully featured backend REST API for the [RealWorld](https://github.com/gothinkster/realworld) "Conduit" application (Medium.com clone), built with **ASP.NET Core 10**.

This codebase was created to demonstrate a real-world backend API built with .NET 10 including CRUD operations, JWT authentication, routing, Entity Framework Core relational mapping, and more. It adheres to the [RealWorld API spec](https://realworld-docs.netlify.app/docs/specs/backend-specs/introduction), meaning it can be paired with any compliant frontend — including the [Angular v20 frontend](https://github.com/TomislavVinkovic/realworld-app-angular-v20) in this project.

*Note: This API goes above and beyond the base RealWorld specification by including support for `multipart/form-data` physical image uploads for user profiles and utilizing a secure Access/Refresh token rotation system!*

---

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| C# | 13.0 | Language |
| ASP.NET Core | 10.0 | Framework |
| Entity Framework Core | 10.0 | ORM / Database abstraction |
| SQL Server / SQLite / Postgres | — | Database |
| JWT Bearer Authentication | — | API token authentication |
| BCrypt.Net | — | Password hashing |

---

## API Endpoints

The API is mounted at `/api` and implements the RealWorld spec, along with several custom endpoints for advanced authentication and frontend integration:

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/user/login` | No | Login |
| `POST` | `/api/user` | No | Register |
| `GET` | `/api/user` | Yes | Get current user |
| `PUT` | `/api/user` | Yes | Update current user |
| `GET` | `/api/user/edit` | Yes | Get current user data for edit form *(Custom)* |
| `POST` | `/api/user/refresh` | No | Refresh expired access token *(Custom)* |
| `GET` | `/api/user/logout` | Yes | Invalidate refresh token *(Custom)* |
| `GET` | `/api/profiles/{username}` | Optional | Get profile |
| `POST` | `/api/profiles/{username}/follow` | Yes | Follow user |
| `DELETE` | `/api/profiles/{username}/unfollow` | Yes | Unfollow user |
| `GET` | `/api/articles` | Optional | List articles (filterable) |
| `GET` | `/api/articles/feed` | Yes | Get personalised feed |
| `GET` | `/api/articles/{slug}` | Optional | Get article |
| `POST` | `/api/articles` | Yes | Create article |
| `PUT` | `/api/articles/{slug}` | Yes | Update article |
| `DELETE` | `/api/articles/{slug}` | Yes | Delete article |
| `POST` | `/api/articles/{slug}/favorite` | Yes | Favourite article |
| `DELETE` | `/api/articles/{slug}/unfavorite` | Yes | Unfavourite article |
| `GET` | `/api/articles/{slug}/comments` | Optional | Get comments |
| `POST` | `/api/articles/{slug}/comments` | Yes | Add comment |
| `DELETE` | `/api/articles/{slug}/comments/{id}` | Yes | Delete comment |
| `GET` | `/api/tags` | No | Get tags |

---

## Project Structure

```text
├── Controllers/               # API endpoints, route definitions, and model binding
├── DTOs/                      # Strongly-typed Records and Classes for data transfer
├── Models/                    # Entity Framework Core POCOs (User, Article, Comment, Tag)
├── Services/                  # Core business logic and database interactions
├── wwwroot/uploads/           # Local storage directory for user-uploaded profile images
├── appsettings.json           # Environment configuration (DB connections, JWT secrets)
├── Program.cs                 # Application entry point, DI container, and middleware pipeline
└── RealWorld.csproj # C# project dependencies and build configuration
```

---

## Prerequisites

- **.NET 10 SDK** or later
- **SQL Server** (or SQLite/PostgreSQL depending on your EF Core provider setup)
- A web server or .NET's built-in Kestrel dev server

---

## Getting Started

### 1. Clone the repository

```bash
git clone [https://github.com/TomislavVinkovic/realworld-api-dotnet-10.git](https://github.com/TomislavVinkovic/realworld-api-dotnet-10.git)
cd realworld-api-dotnet-10
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Set up your configuration file

Open `appsettings.json` (or `appsettings.Development.json`) and configure your database connection and JWT secrets:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RealWorldDb;User Id=sa;Password=your_password;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Key": "YOUR_SUPER_SECRET_KEY_MAKE_SURE_IT_IS_LONG_ENOUGH",
    "Issuer": "RealWorldApp",
    "Audience": "RealWorldUsers"
  }
}
```

### 4. Run database migrations

Generate the database schema and apply the unique constraints using Entity Framework Core tools:

```bash
dotnet ef database update
```

### 5. Start the development server

```bash
dotnet watch
```
*(Using `dotnet watch` enables Hot Reload for faster development!)*

The API will typically be available at **[https://localhost:7274/api](https://localhost:7274/api)** (port may vary based on your `launchSettings.json`).

---

## CORS Configuration

The API has CORS enabled to allow requests from frontend applications. By default, requests from `http://localhost:4200` (Angular dev server) are permitted. You can adjust the allowed origins directly in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

---

## Authentication

Authentication is handled via **JWT (JSON Web Tokens)**. To access protected endpoints, include the token returned on login or registration in the `Authorization` header. 

*Note: Depending on how you configured your JWT Bearer defaults, you may need to use `Bearer` or `Token` as the prefix.*

```text
Authorization: Bearer <your_jwt_here>
```

*(This application also implements Refresh Tokens stored securely in the database for extended session management. Send the refresh token to `/api/user/refresh` to get a new access token).*

---

## Pairing with the Angular v20 Frontend

This API is designed to work directly with the [realworld-app-angular-v20](https://github.com/TomislavVinkovic/realworld-app-angular-v20) frontend. Once the API is running locally, update the Angular environment file to point at your local .NET port:

```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7274/api'
};
```

---

## Further Reading

- [RealWorld API Spec](https://realworld-docs.netlify.app/docs/specs/backend-specs/introduction)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

## License

MIT