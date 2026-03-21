# Conduit — RealWorld Backend API · .NET 10

> A fully featured backend REST API for the [RealWorld](https://github.com/gothinkster/realworld) "Conduit" application (Medium.com clone), built with **ASP.NET Core 10**.

This codebase was created to demonstrate a real-world backend API built with .NET 10 including CRUD operations, JWT authentication, routing, Entity Framework Core relational mapping, and more. It fully adheres to the [RealWorld API spec](https://realworld-docs.netlify.app/docs/specs/backend-specs/introduction), meaning it can be paired with any compliant frontend — including the [Angular v20 frontend](https://github.com/TomislavVinkovic/realworld-app-angular-v20) in this project.

*Note: This API goes above and beyond the base RealWorld specification by including support for `multipart/form-data` physical image uploads for user profiles!*

---

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| C# | 13.0 | Language |
| ASP.NET Core | 10.0 | Framework |
| Entity Framework Core | 10.0 | ORM  |
| MariaDB | — | Database |
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
└── your_project_name.csproj   # C# project dependencies and build configuration