using System;
using System.ComponentModel.DataAnnotations;
using UserProfile.Models;
using UserProfile.Repositories;

namespace UserProfile.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Creates a new user in the system with the provided user information.
        /// </summary>
        /// <remarks>
        /// This method performs comprehensive validation of the user data, checks for email uniqueness,
        /// and creates a new user record with a unique GUID identifier. The method ensures data integrity
        /// by trimming whitespace from name and email fields and normalizing email to lowercase.
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
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Database query to check email uniqueness (potential race condition in high-concurrency scenarios)</description></item>
        /// <item><description>Consider implementing optimistic concurrency control for production use</description></item>
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

        public User? GetUserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            return _userRepository.GetUserById(userId);
        }

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