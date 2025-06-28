# UserProfile Application

## 📋 Overview

The UserProfile application is a comprehensive RESTful API built with ASP.NET Core 9.0 that provides user profile management capabilities. This application demonstrates modern software development practices including clean architecture, dependency injection, comprehensive validation, and security best practices.

### 🎯 Purpose and Responsibility

The UserProfile application serves as a robust backend service for managing user profiles in web applications. It provides a complete set of CRUD (Create, Read, Update, Delete) operations for user data with comprehensive validation, error handling, and security measures.

### ✨ Key Features and Capabilities

- **RESTful API Design**: Clean, RESTful endpoints following HTTP standards
- **Comprehensive CRUD Operations**: Full user profile management capabilities
- **Data Validation**: Multi-layer validation using data annotations and business logic
- **Error Handling**: Structured error responses with appropriate HTTP status codes
- **Logging**: Comprehensive structured logging for monitoring and debugging
- **Security**: Input validation, sanitization, and security headers
- **Database Integration**: Entity Framework Core with in-memory database support
- **Testing**: Unit tests and integration tests for reliability
- **Documentation**: Comprehensive XML documentation and API documentation

### 🏗️ Architecture

The application follows clean architecture principles with clear separation of concerns:

```
UserProfile/
├── UserProfile.API/          # Web API layer (Controllers, Program.cs)
├── UserProfile/              # Core business logic (Services, Models)
├── UserProfile.API.Tests/    # Integration tests
└── UserProfile.Tests/        # Unit tests
```

### 🔧 Dependencies and Requirements

#### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code
- Git for version control

#### NuGet Packages
- `Microsoft.AspNetCore.OpenApi` (9.0.4)
- `Microsoft.EntityFrameworkCore.InMemory` (9.0.6)
- `Swashbuckle.AspNetCore` (9.0.1)

## 🚀 Getting Started

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd UserProfileApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the application**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   cd UserProfile.API
   dotnet run
   ```

The API will be available at:
- **HTTP**: http://localhost:5165
- **HTTPS**: https://localhost:7215
- **Swagger UI**: https://localhost:7215/swagger

### Configuration

The application uses the following configuration files:

- `appsettings.json` - General configuration
- `appsettings.Development.json` - Development-specific settings
- `launchSettings.json` - Launch configuration

## 📚 API Reference

### Base URL
```
https://localhost:7215/api
```

### Endpoints

#### Create User
```http
POST /users
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

**Response (201 Created)**
```json
{
  "id": "12345678-1234-1234-1234-123456789012",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

#### Get User by ID
```http
GET /users/{userId}
```

**Response (200 OK)**
```json
{
  "id": "12345678-1234-1234-1234-123456789012",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

#### Update User
```http
PUT /users/{userId}
Content-Type: application/json

{
  "id": "12345678-1234-1234-1234-123456789012",
  "name": "John Smith",
  "email": "john.smith@example.com"
}
```

**Response (204 No Content)**

#### Delete User
```http
DELETE /users/{userId}
```

**Response (204 No Content)**

### Error Responses

#### Validation Error (400 Bad Request)
```json
{
  "errors": [
    "Name must be between 2 and 100 characters",
    "Invalid email format"
  ]
}
```

#### User Not Found (404 Not Found)
```json
{
  "error": "User not found"
}
```

#### Email Conflict (409 Conflict)
```json
{
  "error": "User with this email already exists"
}
```

#### Server Error (500 Internal Server Error)
```json
{
  "error": "An internal server error occurred"
}
```

## 🏛️ Architecture Components

### Controllers

#### UsersController
- **Purpose**: Handles HTTP requests for user operations
- **Responsibilities**: Request validation, response formatting, error handling
- **Key Methods**: CreateUser, GetUserById, UpdateUser, DeleteUser

### Services

#### UserService
- **Purpose**: Business logic layer for user operations
- **Responsibilities**: Data validation, business rules, data processing
- **Key Methods**: CreateUser, GetUserById, UpdateUser, DeleteUser, ValidateUserDto

### Models

#### User
- **Purpose**: Domain model representing a user profile
- **Properties**: Id (Guid), Name (string), Email (string), CreatedAt (DateTime), UpdatedAt (DateTime?)
- **Validation**: Data annotations for input validation

#### UserDto
- **Purpose**: Data transfer object for API operations
- **Properties**: Id (Guid), Name (string), Email (string)
- **Validation**: Data annotations for input validation

### Repositories

#### UserRepository
- **Purpose**: Data access layer implementation
- **Responsibilities**: Database operations using Entity Framework Core
- **Key Methods**: AddUser, GetUserById, GetUserByEmail, UpdateUser, DeleteUser

#### IUserRepository
- **Purpose**: Repository interface for dependency injection
- **Responsibilities**: Defines contract for data access operations

### Data Access

#### UserDbContext
- **Purpose**: Entity Framework Core database context
- **Responsibilities**: Database configuration, entity mapping
- **Features**: In-memory database for development

## 🔒 Security Features

### Input Validation
- Data annotations on models
- Business logic validation in services
- ModelState validation in controllers

### Security Headers
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY
- X-XSS-Protection: 1; mode=block

### Error Handling
- Structured error responses
- No sensitive information disclosure
- Comprehensive logging

## 🧪 Testing

### Unit Tests
```bash
cd UserProfile.Tests
dotnet test
```

### Integration Tests
```bash
cd UserProfile.API.Tests
dotnet test
```

### Test Coverage
- Controller action testing
- Service method testing
- Repository operation testing
- Error handling scenarios

## 📖 Usage Examples

### Basic Usage

#### Creating a User
```csharp
var userDto = new UserDto
{
    Name = "Jane Doe",
    Email = "jane.doe@example.com"
};

var result = await controller.CreateUser(userDto);
```

#### Retrieving a User
```csharp
var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
var user = await controller.GetUserById(userId);
```

#### Updating a User
```csharp
var userDto = new UserDto
{
    Id = userId,
    Name = "Jane Smith",
    Email = "jane.smith@example.com"
};

await controller.UpdateUser(userId, userDto);
```

#### Deleting a User
```csharp
await controller.DeleteUser(userId);
```

### Error Handling

#### Validation Errors
```csharp
try
{
    var invalidUserDto = new UserDto
    {
        Name = "A", // Too short
        Email = "invalid-email" // Invalid format
    };
    
    var result = await controller.CreateUser(invalidUserDto);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
```

#### Business Rule Violations
```csharp
try
{
    var duplicateUserDto = new UserDto
    {
        Name = "John Doe",
        Email = "existing.email@example.com" // Already exists
    };
    
    var result = await controller.CreateUser(duplicateUserDto);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Business rule violation: {ex.Message}");
}
```

## 🔧 Configuration Options

### Database Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UserProfileDB;Trusted_Connection=true;"
  }
}
```

### Logging Configuration
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### CORS Configuration
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("https://yourdomain.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
```

## 🚀 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
```bash
dotnet publish -c Release
dotnet run --environment Production
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["UserProfile.API/UserProfile.API.csproj", "UserProfile.API/"]
COPY ["UserProfile/UserProfile.csproj", "UserProfile/"]
RUN dotnet restore "UserProfile.API/UserProfile.API.csproj"
COPY . .
WORKDIR "/src/UserProfile.API"
RUN dotnet build "UserProfile.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserProfile.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserProfile.API.dll"]
```

## 📊 Performance Considerations

### Database Optimization
- Primary key indexing (automatic with Entity Framework)
- Email field indexing for uniqueness checks
- Consider pagination for large datasets

### Caching Strategies
- Consider implementing caching for frequently accessed users
- Cache email uniqueness checks in high-traffic scenarios

### Connection Management
- Entity Framework Core connection pooling
- Proper disposal of DbContext instances

## 🔍 Monitoring and Logging

### Structured Logging
```csharp
_logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
_logger.LogWarning("Invalid argument provided for user creation: {Message}", ex.Message);
_logger.LogError(ex, "Unexpected error occurred while creating user");
```

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UserDbContext>();
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the documentation
- Review the test examples

## 🔄 Version History

- **v1.0.0**: Initial release with basic CRUD operations
- **v1.1.0**: Added comprehensive validation and error handling
- **v1.2.0**: Enhanced security features and documentation

---

*This documentation provides comprehensive information about the UserProfile application. For specific implementation details, refer to the XML documentation in the source code.* 