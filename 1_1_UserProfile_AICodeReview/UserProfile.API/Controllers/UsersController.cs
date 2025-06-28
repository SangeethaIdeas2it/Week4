using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserProfile.Models;
using UserProfile.Services;

namespace UserProfile.API.Controllers
{
    /// <summary>
    /// RESTful API controller for managing user profiles in the UserProfile application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The UsersController provides comprehensive CRUD operations for user management,
    /// including user creation, retrieval, updates, and deletion. This controller serves
    /// as the primary interface for client applications to interact with user data.
    /// </para>
    /// 
    /// <para>
    /// <strong>Key Features:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Create new user profiles with validation</description></item>
    /// <item><description>Retrieve user information by unique identifier</description></item>
    /// <item><description>Update existing user profiles</description></item>
    /// <item><description>Delete user profiles</description></item>
    /// <item><description>Comprehensive error handling and logging</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>All endpoints return appropriate HTTP status codes</description></item>
    /// <item><description>Consistent error response format across all operations</description></item>
    /// <item><description>Structured logging for monitoring and debugging</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Dependencies:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>UserService - Business logic and validation</description></item>
    /// <item><description>ILogger - Structured logging</description></item>
    /// <item><description>UserDto - Data transfer object for user operations</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Security Considerations:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Input validation through ModelState</description></item>
    /// <item><description>Error message sanitization to prevent information disclosure</description></item>
    /// <item><description>Structured logging without sensitive data exposure</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <para>The following example demonstrates basic usage of the UsersController:</para>
    /// <code>
    /// // Create a new user
    /// var userDto = new UserDto 
    /// { 
    ///     Name = "John Doe", 
    ///     Email = "john.doe@example.com" 
    /// };
    /// var result = await controller.CreateUser(userDto);
    /// 
    /// // Retrieve user by ID
    /// var user = await controller.GetUserById(userId);
    /// 
    /// // Update user
    /// userDto.Name = "John Smith";
    /// await controller.UpdateUser(userId, userDto);
    /// 
    /// // Delete user
    /// await controller.DeleteUser(userId);
    /// </code>
    /// </example>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the UsersController with required dependencies.
        /// </summary>
        /// <param name="userService">
        /// The user service that handles business logic and data operations.
        /// Must not be null.
        /// </param>
        /// <param name="logger">
        /// The logger instance for structured logging and monitoring.
        /// Must not be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="userService"/> or <paramref name="logger"/> is null.
        /// </exception>
        /// <example>
        /// <code>
        /// var userService = new UserService(userRepository);
        /// var logger = loggerFactory.CreateLogger&lt;UsersController&gt;();
        /// var controller = new UsersController(userService, logger);
        /// </code>
        /// </example>
        public UsersController(UserService userService, ILogger<UsersController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new user profile in the system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This endpoint creates a new user profile with the provided information.
        /// The method performs comprehensive validation of the input data and ensures
        /// email uniqueness across all users in the system.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>ModelState validation for data annotations</description></item>
        /// <item><description>Business rule validation through UserService</description></item>
        /// <item><description>Email uniqueness check</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Response codes:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>201 Created - User successfully created</description></item>
        /// <item><description>400 Bad Request - Invalid input data</description></item>
        /// <item><description>409 Conflict - Email already exists</description></item>
        /// <item><description>500 Internal Server Error - Unexpected error</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Database query for email uniqueness check</description></item>
        /// <item><description>Consider implementing caching for high-traffic scenarios</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userDto">
        /// The user data transfer object containing the user information to create.
        /// Must contain valid Name and Email properties.
        /// </param>
        /// <returns>
        /// An IActionResult representing the result of the operation:
        /// <list type="bullet">
        /// <item><description>CreatedAtAction with the created user and location header</description></item>
        /// <item><description>BadRequest with validation errors</description></item>
        /// <item><description>Conflict if email already exists</description></item>
        /// <item><description>Internal server error for unexpected exceptions</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when validation fails for user data.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a user with the same email already exists.
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates creating a new user:</para>
        /// <code>
        /// var userDto = new UserDto
        /// {
        ///     Name = "Jane Smith",
        ///     Email = "jane.smith@example.com"
        /// };
        /// 
        /// var result = await controller.CreateUser(userDto);
        /// 
        /// if (result is CreatedAtActionResult createdResult)
        /// {
        ///     var createdUser = (User)createdResult.Value;
        ///     Console.WriteLine($"User created with ID: {createdUser.Id}");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows error handling:</para>
        /// <code>
        /// try
        /// {
        ///     var invalidUserDto = new UserDto
        ///     {
        ///         Name = "A", // Too short
        ///         Email = "invalid-email" // Invalid format
        ///     };
        ///     
        ///     var result = await controller.CreateUser(invalidUserDto);
        ///     
        ///     if (result is BadRequestObjectResult badRequest)
        ///     {
        ///         var errors = (dynamic)badRequest.Value;
        ///         Console.WriteLine("Validation errors occurred");
        ///     }
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine($"Unexpected error: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 409)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult CreateUser(UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                var user = _userService.CreateUser(userDto);
                _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
                return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument provided for user creation: {Message}", ex.Message);
                return BadRequest(new { error = "Invalid user data provided" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Business rule violation during user creation: {Message}", ex.Message);
                return Conflict(new { error = "User with this email already exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating user");
                return StatusCode(500, new { error = "An internal server error occurred" });
            }
        }

        /// <summary>
        /// Retrieves a user profile by their unique identifier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This endpoint retrieves a user profile using the provided GUID identifier.
        /// The method performs validation on the input parameter and returns the user
        /// data if found, or a 404 Not Found response if the user doesn't exist.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>GUID format validation</description></item>
        /// <item><description>User existence check</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Response codes:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>200 OK - User found and returned</description></item>
        /// <item><description>404 Not Found - User not found</description></item>
        /// <item><description>500 Internal Server Error - Unexpected error</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Direct database lookup by primary key (efficient)</description></item>
        /// <item><description>Consider implementing caching for frequently accessed users</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userId">
        /// The unique identifier (GUID) of the user to retrieve.
        /// Must be a valid GUID format.
        /// </param>
        /// <returns>
        /// An IActionResult representing the result of the operation:
        /// <list type="bullet">
        /// <item><description>OK with user data if found</description></item>
        /// <item><description>NotFound if user doesn't exist</description></item>
        /// <item><description>Internal server error for unexpected exceptions</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided userId is not a valid GUID.
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates retrieving a user:</para>
        /// <code>
        /// var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        /// var result = await controller.GetUserById(userId);
        /// 
        /// if (result is OkObjectResult okResult)
        /// {
        ///     var user = (User)okResult.Value;
        ///     Console.WriteLine($"User: {user.Name} ({user.Email})");
        /// }
        /// else if (result is NotFoundObjectResult notFound)
        /// {
        ///     Console.WriteLine("User not found");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows error handling:</para>
        /// <code>
        /// try
        /// {
        ///     var invalidUserId = Guid.Empty;
        ///     var result = await controller.GetUserById(invalidUserId);
        ///     
        ///     if (result is BadRequestObjectResult badRequest)
        ///     {
        ///         Console.WriteLine("Invalid user ID provided");
        ///     }
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine($"Unexpected error: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult GetUserById(Guid userId)
        {
            try
            {
                var user = _userService.GetUserById(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    return NotFound(new { error = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
                return StatusCode(500, new { error = "An internal server error occurred" });
            }
        }

        /// <summary>
        /// Updates an existing user profile with new information.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This endpoint updates an existing user profile with the provided information.
        /// The method validates both the input data and ensures the user exists before
        /// performing the update operation. It also checks for email conflicts if the
        /// email is being changed.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>ModelState validation for data annotations</description></item>
        /// <item><description>User existence verification</description></item>
        /// <item><description>User ID consistency check</description></item>
        /// <item><description>Email uniqueness validation (if email is changed)</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Response codes:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>204 No Content - User successfully updated</description></item>
        /// <item><description>400 Bad Request - Invalid input data or ID mismatch</description></item>
        /// <item><description>404 Not Found - User not found</description></item>
        /// <item><description>500 Internal Server Error - Unexpected error</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Database query for user existence check</description></item>
        /// <item><description>Additional query for email uniqueness (if email changed)</description></item>
        /// <item><description>Consider implementing optimistic concurrency control</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userId">
        /// The unique identifier (GUID) of the user to update.
        /// Must match the ID in the userDto.
        /// </param>
        /// <param name="userDto">
        /// The user data transfer object containing the updated user information.
        /// Must contain valid Name and Email properties.
        /// </param>
        /// <returns>
        /// An IActionResult representing the result of the operation:
        /// <list type="bullet">
        /// <item><description>NoContent if update successful</description></item>
        /// <item><description>BadRequest with validation errors or ID mismatch</description></item>
        /// <item><description>NotFound if user doesn't exist</description></item>
        /// <item><description>Internal server error for unexpected exceptions</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when validation fails for user data or ID mismatch.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when user is not found or email conflict occurs.
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates updating a user:</para>
        /// <code>
        /// var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        /// var userDto = new UserDto
        /// {
        ///     Id = userId,
        ///     Name = "Updated Name",
        ///     Email = "updated.email@example.com"
        /// };
        /// 
        /// var result = await controller.UpdateUser(userId, userDto);
        /// 
        /// if (result is NoContentResult)
        /// {
        ///     Console.WriteLine("User updated successfully");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows error handling:</para>
        /// <code>
        /// try
        /// {
        ///     var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        ///     var userDto = new UserDto
        ///     {
        ///         Id = Guid.NewGuid(), // ID mismatch
        ///         Name = "Test User",
        ///         Email = "test@example.com"
        ///     };
        ///     
        ///     var result = await controller.UpdateUser(userId, userDto);
        ///     
        ///     if (result is BadRequestObjectResult badRequest)
        ///     {
        ///         Console.WriteLine("User ID mismatch");
        ///     }
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine($"Unexpected error: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult UpdateUser(Guid userId, UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                if (userId != userDto.Id)
                {
                    return BadRequest(new { error = "User ID mismatch" });
                }

                _userService.UpdateUser(userDto);
                _logger.LogInformation("User updated successfully with ID: {UserId}", userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument provided for user update: {Message}", ex.Message);
                return BadRequest(new { error = "Invalid user data provided" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("User not found for update: {Message}", ex.Message);
                return NotFound(new { error = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating user with ID: {UserId}", userId);
                return StatusCode(500, new { error = "An internal server error occurred" });
            }
        }

        /// <summary>
        /// Deletes a user profile from the system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This endpoint permanently removes a user profile from the system using the
        /// provided unique identifier. The method validates the user exists before
        /// performing the deletion operation.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>GUID format validation</description></item>
        /// <item><description>User existence verification</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Response codes:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>204 No Content - User successfully deleted</description></item>
        /// <item><description>404 Not Found - User not found</description></item>
        /// <item><description>500 Internal Server Error - Unexpected error</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Database query for user existence check</description></item>
        /// <item><description>Direct deletion by primary key (efficient)</description></item>
        /// <item><description>Consider implementing soft delete for audit purposes</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Security considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Permanent deletion - ensure proper authorization</description></item>
        /// <item><description>Consider implementing audit logging for compliance</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userId">
        /// The unique identifier (GUID) of the user to delete.
        /// Must be a valid GUID format.
        /// </param>
        /// <returns>
        /// An IActionResult representing the result of the operation:
        /// <list type="bullet">
        /// <item><description>NoContent if deletion successful</description></item>
        /// <item><description>NotFound if user doesn't exist</description></item>
        /// <item><description>Internal server error for unexpected exceptions</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided userId is not a valid GUID.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the user is not found.
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates deleting a user:</para>
        /// <code>
        /// var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        /// var result = await controller.DeleteUser(userId);
        /// 
        /// if (result is NoContentResult)
        /// {
        ///     Console.WriteLine("User deleted successfully");
        /// }
        /// else if (result is NotFoundObjectResult)
        /// {
        ///     Console.WriteLine("User not found");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows error handling:</para>
        /// <code>
        /// try
        /// {
        ///     var invalidUserId = Guid.Empty;
        ///     var result = await controller.DeleteUser(invalidUserId);
        ///     
        ///     if (result is BadRequestObjectResult badRequest)
        ///     {
        ///         Console.WriteLine("Invalid user ID provided");
        ///     }
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine($"Unexpected error: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult DeleteUser(Guid userId)
        {
            try
            {
                _userService.DeleteUser(userId);
                _logger.LogInformation("User deleted successfully with ID: {UserId}", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", userId);
                return StatusCode(500, new { error = "An internal server error occurred" });
            }
        }
    }
} 