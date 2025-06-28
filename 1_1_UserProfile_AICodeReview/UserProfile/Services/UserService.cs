using System;
using System.ComponentModel.DataAnnotations;
using UserProfile.Models;
using UserProfile.Repositories;

namespace UserProfile.Services
{
    /// <summary>
    /// Core business logic service for user profile management operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The UserService provides comprehensive business logic for user profile management,
    /// including creation, retrieval, updates, and deletion of user profiles. This service
    /// acts as the primary business layer between the API controllers and the data access layer.
    /// </para>
    /// 
    /// <para>
    /// <strong>Key Features:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Comprehensive user data validation and sanitization</description></item>
    /// <item><description>Email uniqueness enforcement across all users</description></item>
    /// <item><description>Data normalization (trimming, case conversion)</description></item>
    /// <item><description>Business rule enforcement and validation</description></item>
    /// <item><description>Exception handling with meaningful error messages</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Dependency injection for repository access</description></item>
    /// <item><description>Exception-based error handling</description></item>
    /// <item><description>Data validation before persistence</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Dependencies:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>IUserRepository - Data access operations</description></item>
    /// <item><description>UserDto - Data transfer object for input</description></item>
    /// <item><description>User - Domain model for output</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Security Features:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Input validation and sanitization</description></item>
    /// <item><description>Email format validation</description></item>
    /// <item><description>Data length restrictions</description></item>
    /// <item><description>Business rule enforcement</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Performance Considerations:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Database queries for email uniqueness checks</description></item>
    /// <item><description>Potential race conditions in high-concurrency scenarios</description></item>
    /// <item><description>Consider implementing optimistic concurrency control</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <para>The following example demonstrates basic usage of the UserService:</para>
    /// <code>
    /// // Create a new user
    /// var userDto = new UserDto
    /// {
    ///     Name = "John Doe",
    ///     Email = "john.doe@example.com"
    /// };
    /// 
    /// var userService = new UserService(userRepository);
    /// var newUser = userService.CreateUser(userDto);
    /// Console.WriteLine($"User created with ID: {newUser.Id}");
    /// 
    /// // Retrieve user by ID
    /// var user = userService.GetUserById(newUser.Id);
    /// if (user != null)
    /// {
    ///     Console.WriteLine($"User: {user.Name} ({user.Email})");
    /// }
    /// 
    /// // Update user
    /// userDto.Id = newUser.Id;
    /// userDto.Name = "John Smith";
    /// userService.UpdateUser(userDto);
    /// 
    /// // Delete user
    /// userService.DeleteUser(newUser.Id);
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
    ///     var user = userService.CreateUser(invalidUserDto);
    /// }
    /// catch (ArgumentException ex)
    /// {
    ///     Console.WriteLine($"Validation error: {ex.Message}");
    /// }
    /// catch (InvalidOperationException ex)
    /// {
    ///     Console.WriteLine($"Business rule violation: {ex.Message}");
    /// }
    /// </code>
    /// </example>
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the UserService with the required repository dependency.
        /// </summary>
        /// <param name="userRepository">
        /// The user repository that handles data access operations.
        /// Must not be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="userRepository"/> is null.
        /// </exception>
        /// <example>
        /// <code>
        /// var userRepository = new UserRepository(dbContext);
        /// var userService = new UserService(userRepository);
        /// </code>
        /// </example>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Creates a new user in the system with the provided user information.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs comprehensive validation of the user data, checks for email uniqueness,
        /// and creates a new user record with a unique GUID identifier. The method ensures data integrity
        /// by trimming whitespace from name and email fields and normalizing email to lowercase.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Name must be between 2-100 characters and not null/empty</description></item>
        /// <item><description>Email must be valid format and not exceed 255 characters</description></item>
        /// <item><description>Email must be unique across all users</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Data processing:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Name and email are trimmed of whitespace</description></item>
        /// <item><description>Email is converted to lowercase for consistency</description></item>
        /// <item><description>Unique GUID is generated for the user ID</description></item>
        /// <item><description>Creation timestamp is set to UTC time</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Database query to check email uniqueness (potential race condition in high-concurrency scenarios)</description></item>
        /// <item><description>Consider implementing optimistic concurrency control for production use</description></item>
        /// <item><description>Consider implementing caching for email uniqueness checks in high-traffic scenarios</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Security considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Input validation prevents malicious data injection</description></item>
        /// <item><description>Email normalization prevents duplicate accounts with different cases</description></item>
        /// <item><description>Consider implementing rate limiting for user creation in production</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userDto">
        /// The user data transfer object containing the user information to create.
        /// Must not be null and must contain valid name and email properties.
        /// </param>
        /// <returns>
        /// A new <see cref="User"/> object representing the created user with:
        /// <list type="bullet">
        /// <item><description>Unique GUID identifier</description></item>
        /// <item><description>Normalized name and email (trimmed and lowercase)</description></item>
        /// <item><description>Creation timestamp in UTC</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="userDto"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when validation fails for any of the following reasons:
        /// <list type="bullet">
        /// <item><description>Name is null, empty, or whitespace</description></item>
        /// <item><description>Name length is less than 2 or greater than 100 characters</description></item>
        /// <item><description>Email is null, empty, or whitespace</description></item>
        /// <item><description>Email format is invalid</description></item>
        /// <item><description>Email length exceeds 255 characters</description></item>
        /// </list>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a user with the same email address already exists in the system.
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates how to create a new user:</para>
        /// <code>
        /// var userService = new UserService(userRepository);
        /// 
        /// var userDto = new UserDto
        /// {
        ///     Name = "John Doe",
        ///     Email = "john.doe@example.com"
        /// };
        /// 
        /// try
        /// {
        ///     var newUser = userService.CreateUser(userDto);
        ///     Console.WriteLine($"User created with ID: {newUser.Id}");
        ///     Console.WriteLine($"Created at: {newUser.CreatedAt}");
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     Console.WriteLine($"User creation failed: {ex.Message}");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows error handling for validation failures:</para>
        /// <code>
        /// try
        /// {
        ///     var invalidUserDto = new UserDto
        ///     {
        ///         Name = "A", // Too short
        ///         Email = "invalid-email" // Invalid format
        ///     };
        ///     
        ///     var user = userService.CreateUser(invalidUserDto);
        /// }
        /// catch (ArgumentException ex)
        /// {
        ///     Console.WriteLine($"Validation error: {ex.Message}");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows handling email conflicts:</para>
        /// <code>
        /// try
        /// {
        ///     var duplicateUserDto = new UserDto
        ///     {
        ///         Name = "Jane Doe",
        ///         Email = "john.doe@example.com" // Same email as existing user
        ///     };
        ///     
        ///     var user = userService.CreateUser(duplicateUserDto);
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     Console.WriteLine($"Email conflict: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public User CreateUser(UserDto userDto)
        {
            ValidateUserDto(userDto);

            // Check for existing user with same email (potential race condition in production)
            var existingUser = _userRepository.GetUserByEmail(userDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name.Trim(),
                Email = userDto.Email.Trim().ToLowerInvariant(),
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.AddUser(user);
            return user;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method retrieves a user profile using the provided GUID identifier.
        /// The method performs validation on the input parameter and returns the user
        /// if found, or null if the user doesn't exist.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>GUID format validation (non-empty)</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Direct database lookup by primary key (efficient)</description></item>
        /// <item><description>Consider implementing caching for frequently accessed users</description></item>
        /// <item><description>Consider implementing pagination for bulk user retrieval</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Security considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Input validation prevents GUID format attacks</description></item>
        /// <item><description>Consider implementing authorization checks in production</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userId">
        /// The unique identifier (GUID) of the user to retrieve.
        /// Must not be empty.
        /// </param>
        /// <returns>
        /// A <see cref="User"/> object if found, or null if the user doesn't exist.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="userId"/> is empty (Guid.Empty).
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates retrieving a user:</para>
        /// <code>
        /// var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        /// var user = userService.GetUserById(userId);
        /// 
        /// if (user != null)
        /// {
        ///     Console.WriteLine($"User: {user.Name} ({user.Email})");
        ///     Console.WriteLine($"Created: {user.CreatedAt}");
        /// }
        /// else
        /// {
        ///     Console.WriteLine("User not found");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows error handling:</para>
        /// <code>
        /// try
        /// {
        ///     var user = userService.GetUserById(Guid.Empty);
        /// }
        /// catch (ArgumentException ex)
        /// {
        ///     Console.WriteLine($"Invalid user ID: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public User? GetUserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            return _userRepository.GetUserById(userId);
        }

        /// <summary>
        /// Updates an existing user profile with new information.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method updates an existing user profile with the provided information.
        /// The method validates both the input data and ensures the user exists before
        /// performing the update operation. It also checks for email conflicts if the
        /// email is being changed.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>UserDto validation (same as CreateUser)</description></item>
        /// <item><description>User existence verification</description></item>
        /// <item><description>Email uniqueness validation (if email is changed)</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Data processing:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Name and email are trimmed of whitespace</description></item>
        /// <item><description>Email is converted to lowercase for consistency</description></item>
        /// <item><description>Updated timestamp is set to UTC time</description></item>
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
        /// 
        /// <para>
        /// <strong>Security considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Input validation prevents malicious data injection</description></item>
        /// <item><description>Email normalization prevents duplicate accounts with different cases</description></item>
        /// <item><description>Consider implementing authorization checks in production</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userDto">
        /// The user data transfer object containing the updated user information.
        /// Must not be null and must contain valid name and email properties.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="userDto"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when validation fails for user data.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the user is not found or when the email is already in use by another user.
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates updating a user:</para>
        /// <code>
        /// var userDto = new UserDto
        /// {
        ///     Id = existingUserId,
        ///     Name = "Updated Name",
        ///     Email = "updated.email@example.com"
        /// };
        /// 
        /// try
        /// {
        ///     userService.UpdateUser(userDto);
        ///     Console.WriteLine("User updated successfully");
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     Console.WriteLine($"Update failed: {ex.Message}");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows handling email conflicts:</para>
        /// <code>
        /// try
        /// {
        ///     var userDto = new UserDto
        ///     {
        ///         Id = userId,
        ///         Name = "John Doe",
        ///         Email = "existing.email@example.com" // Email of another user
        ///     };
        ///     
        ///     userService.UpdateUser(userDto);
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     Console.WriteLine($"Email conflict: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public void UpdateUser(UserDto userDto)
        {
            ValidateUserDto(userDto);

            var existingUser = _userRepository.GetUserById(userDto.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Check if email is being changed and if it conflicts with another user
            if (!string.Equals(existingUser.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var userWithEmail = _userRepository.GetUserByEmail(userDto.Email);
                if (userWithEmail != null && userWithEmail.Id != userDto.Id)
                {
                    throw new InvalidOperationException("Email is already in use by another user.");
                }
            }

            existingUser.Name = userDto.Name.Trim();
            existingUser.Email = userDto.Email.Trim().ToLowerInvariant();
            existingUser.UpdatedAt = DateTime.UtcNow;

            _userRepository.UpdateUser(existingUser);
        }

        /// <summary>
        /// Deletes a user profile from the system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method permanently removes a user profile from the system using the
        /// provided unique identifier. The method validates the user exists before
        /// performing the deletion operation.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation performed:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>GUID format validation (non-empty)</description></item>
        /// <item><description>User existence verification</description></item>
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
        /// <item><description>Input validation prevents GUID format attacks</description></item>
        /// <item><description>Permanent deletion - ensure proper authorization in production</description></item>
        /// <item><description>Consider implementing audit logging for compliance</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Data integrity considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Permanent deletion - data cannot be recovered</description></item>
        /// <item><description>Consider implementing soft delete for data retention policies</description></item>
        /// <item><description>Consider implementing cascade delete for related data</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userId">
        /// The unique identifier (GUID) of the user to delete.
        /// Must not be empty.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="userId"/> is empty (Guid.Empty).
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the user is not found.
        /// </exception>
        /// <example>
        /// <para>The following example demonstrates deleting a user:</para>
        /// <code>
        /// var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        /// 
        /// try
        /// {
        ///     userService.DeleteUser(userId);
        ///     Console.WriteLine("User deleted successfully");
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     Console.WriteLine($"User not found: {ex.Message}");
        /// }
        /// </code>
        /// 
        /// <para>The following example shows error handling:</para>
        /// <code>
        /// try
        /// {
        ///     userService.DeleteUser(Guid.Empty);
        /// }
        /// catch (ArgumentException ex)
        /// {
        ///     Console.WriteLine($"Invalid user ID: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public void DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            var existingUser = _userRepository.GetUserById(userId);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            _userRepository.DeleteUser(userId);
        }

        /// <summary>
        /// Validates a UserDto object for data integrity and business rules.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This private method performs comprehensive validation of user data before
        /// any database operations. It ensures data quality and prevents invalid data
        /// from being persisted to the database.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation rules:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>UserDto must not be null</description></item>
        /// <item><description>Name must not be null, empty, or whitespace</description></item>
        /// <item><description>Name must be between 2 and 100 characters</description></item>
        /// <item><description>Email must not be null, empty, or whitespace</description></item>
        /// <item><description>Email must be in valid format</description></item>
        /// <item><description>Email must not exceed 255 characters</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Local validation only - no database queries</description></item>
        /// <item><description>Fast execution for input validation</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Security considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Prevents null reference exceptions</description></item>
        /// <item><description>Validates email format to prevent injection attacks</description></item>
        /// <item><description>Enforces length limits to prevent buffer overflow attacks</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userDto">
        /// The user data transfer object to validate.
        /// Must not be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="userDto"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when validation fails for any of the following reasons:
        /// <list type="bullet">
        /// <item><description>Name is null, empty, or whitespace</description></item>
        /// <item><description>Name length is less than 2 or greater than 100 characters</description></item>
        /// <item><description>Email is null, empty, or whitespace</description></item>
        /// <item><description>Email format is invalid</description></item>
        /// <item><description>Email length exceeds 255 characters</description></item>
        /// </list>
        /// </exception>
        /// <example>
        /// <para>This method is called internally by other public methods:</para>
        /// <code>
        /// // Called by CreateUser and UpdateUser methods
        /// ValidateUserDto(userDto);
        /// </code>
        /// </example>
        private static void ValidateUserDto(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                throw new ArgumentException("Name is required.", nameof(userDto.Name));
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                throw new ArgumentException("Email is required.", nameof(userDto.Email));
            }

            // Validate email format
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(userDto.Email))
            {
                throw new ArgumentException("Invalid email format.", nameof(userDto.Email));
            }

            // Validate name length
            if (userDto.Name.Length < 2 || userDto.Name.Length > 100)
            {
                throw new ArgumentException("Name must be between 2 and 100 characters.", nameof(userDto.Name));
            }

            // Validate email length
            if (userDto.Email.Length > 255)
            {
                throw new ArgumentException("Email cannot exceed 255 characters.", nameof(userDto.Email));
            }
        }
    }
} 