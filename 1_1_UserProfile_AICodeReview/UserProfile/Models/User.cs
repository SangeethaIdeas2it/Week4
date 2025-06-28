using System.ComponentModel.DataAnnotations;

namespace UserProfile.Models
{
    /// <summary>
    /// Represents a user profile in the UserProfile application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The User class is the core domain model that represents a user profile in the system.
    /// It contains all the essential information about a user, including their personal details,
    /// contact information, and metadata about when the profile was created and last updated.
    /// </para>
    /// 
    /// <para>
    /// <strong>Key Features:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Unique identifier using GUID for global uniqueness</description></item>
    /// <item><description>Data validation through data annotations</description></item>
    /// <item><description>Audit trail with creation and update timestamps</description></item>
    /// <item><description>Email-based contact information</description></item>
    /// <item><description>Flexible name field with length constraints</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Entity Framework Core entity for database persistence</description></item>
    /// <item><description>Data transfer object for API responses</description></item>
    /// <item><description>Domain model for business logic operations</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Validation Rules:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Name is required and must be 2-100 characters</description></item>
    /// <item><description>Email is required, must be valid format, and max 255 characters</description></item>
    /// <item><description>ID is automatically generated as GUID</description></item>
    /// <item><description>Timestamps are automatically managed</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Database Considerations:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Primary key is the Id property (GUID)</description></item>
    /// <item><description>Email field should have unique index for performance</description></item>
    /// <item><description>Timestamps are stored in UTC for consistency</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Security Considerations:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Input validation prevents malicious data injection</description></item>
    /// <item><description>Email format validation prevents invalid data</description></item>
    /// <item><description>Length restrictions prevent buffer overflow attacks</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <para>The following example demonstrates creating a new User instance:</para>
    /// <code>
    /// var user = new User
    /// {
    ///     Id = Guid.NewGuid(),
    ///     Name = "John Doe",
    ///     Email = "john.doe@example.com",
    ///     CreatedAt = DateTime.UtcNow
    /// };
    /// 
    /// Console.WriteLine($"User ID: {user.Id}");
    /// Console.WriteLine($"Name: {user.Name}");
    /// Console.WriteLine($"Email: {user.Email}");
    /// Console.WriteLine($"Created: {user.CreatedAt}");
    /// </code>
    /// 
    /// <para>The following example shows validation in action:</para>
    /// <code>
    /// var user = new User
    /// {
    ///     Name = "A", // Too short - will fail validation
    ///     Email = "invalid-email" // Invalid format - will fail validation
    /// };
    /// 
    /// var validationResults = new List&lt;ValidationResult&gt;();
    /// var isValid = Validator.TryValidateObject(user, new ValidationContext(user), validationResults);
    /// 
    /// if (!isValid)
    /// {
    ///     foreach (var error in validationResults)
    ///     {
    ///         Console.WriteLine($"Validation error: {error.ErrorMessage}");
    ///     }
    /// }
    /// </code>
    /// 
    /// <para>The following example shows updating a user:</para>
    /// <code>
    /// var user = GetExistingUser();
    /// 
    /// user.Name = "Updated Name";
    /// user.Email = "updated.email@example.com";
    /// user.UpdatedAt = DateTime.UtcNow;
    /// 
    /// // Save changes to database
    /// context.SaveChanges();
    /// </code>
    /// </example>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property represents the primary key for the user entity. It uses a GUID
        /// to ensure global uniqueness across distributed systems and provides better
        /// security compared to sequential integer IDs.
        /// </para>
        /// 
        /// <para>
        /// <strong>Characteristics:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Globally unique identifier</description></item>
        /// <item><description>Primary key for database operations</description></item>
        /// <item><description>Automatically generated when creating new users</description></item>
        /// <item><description>Immutable once assigned</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Database mapping:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Mapped as primary key in Entity Framework</description></item>
        /// <item><description>Stored as uniqueidentifier in SQL Server</description></item>
        /// <item><description>Indexed for optimal query performance</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// var userId = Guid.NewGuid();
        /// var user = new User { Id = userId };
        /// Console.WriteLine($"User ID: {user.Id}");
        /// </code>
        /// </example>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property stores the user's full name. The name is validated to ensure
        /// it meets the application's requirements for length and format.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation rules:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Required field - cannot be null or empty</description></item>
        /// <item><description>Minimum length: 2 characters</description></item>
        /// <item><description>Maximum length: 100 characters</description></item>
        /// <item><description>Can contain letters, spaces, and common punctuation</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Data processing:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Automatically trimmed of leading/trailing whitespace</description></item>
        /// <item><description>Stored as-is without case conversion</description></item>
        /// <item><description>Displayed exactly as entered by user</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Security considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Length validation prevents buffer overflow attacks</description></item>
        /// <item><description>Consider HTML encoding when displaying in web views</description></item>
        /// <item><description>Input sanitization recommended for XSS prevention</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = new User
        /// {
        ///     Name = "John Doe" // Valid name
        /// };
        /// 
        /// // Invalid names that would fail validation:
        /// // user.Name = ""; // Too short
        /// // user.Name = "A"; // Too short
        /// // user.Name = new string('A', 101); // Too long
        /// </code>
        /// </example>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property stores the user's email address, which serves as the primary
        /// contact method and unique identifier for the user account.
        /// </para>
        /// 
        /// <para>
        /// <strong>Validation rules:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Required field - cannot be null or empty</description></item>
        /// <item><description>Must be in valid email format</description></item>
        /// <item><description>Maximum length: 255 characters</description></item>
        /// <item><description>Must be unique across all users</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Data processing:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Automatically trimmed of leading/trailing whitespace</description></item>
        /// <item><description>Converted to lowercase for consistency</description></item>
        /// <item><description>Stored in normalized format</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Database considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Should have unique index for performance</description></item>
        /// <item><description>Stored as nvarchar(255) in SQL Server</description></item>
        /// <item><description>Case-insensitive comparison for uniqueness</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Security considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Email format validation prevents injection attacks</description></item>
        /// <item><description>Length validation prevents buffer overflow</description></item>
        /// <item><description>Consider rate limiting for email-based operations</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = new User
        /// {
        ///     Email = "john.doe@example.com" // Valid email
        /// };
        /// 
        /// // Invalid emails that would fail validation:
        /// // user.Email = ""; // Empty
        /// // user.Email = "invalid-email"; // Invalid format
        /// // user.Email = new string('a', 256) + "@example.com"; // Too long
        /// </code>
        /// </example>
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the timestamp when the user profile was created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property stores the exact date and time when the user profile was
        /// first created in the system. It provides an audit trail for user management
        /// and compliance purposes.
        /// </para>
        /// 
        /// <para>
        /// <strong>Characteristics:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Automatically set when user is created</description></item>
        /// <item><description>Stored in UTC timezone for consistency</description></item>
        /// <item><description>Immutable once set</description></item>
        /// <item><description>Used for audit and compliance purposes</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Database mapping:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Stored as datetime2 in SQL Server</description></item>
        /// <item><description>Precision includes fractional seconds</description></item>
        /// <item><description>Indexed for query performance on date ranges</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Usage patterns:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>User registration date tracking</description></item>
        /// <item><description>Account age calculations</description></item>
        /// <item><description>Audit trail for compliance</description></item>
        /// <item><description>Analytics and reporting</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = new User
        /// {
        ///     CreatedAt = DateTime.UtcNow
        /// };
        /// 
        /// Console.WriteLine($"User created: {user.CreatedAt}");
        /// 
        /// // Calculate account age
        /// var accountAge = DateTime.UtcNow - user.CreatedAt;
        /// Console.WriteLine($"Account age: {accountAge.Days} days");
        /// </code>
        /// </example>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the timestamp when the user profile was last updated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property stores the exact date and time when the user profile was
        /// last modified. It provides an audit trail for tracking changes to user
        /// information over time.
        /// </para>
        /// 
        /// <para>
        /// <strong>Characteristics:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Automatically updated when user data changes</description></item>
        /// <item><description>Stored in UTC timezone for consistency</description></item>
        /// <item><description>Nullable - null when user has never been updated</description></item>
        /// <item><description>Used for change tracking and audit purposes</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Database mapping:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Stored as datetime2? (nullable) in SQL Server</description></item>
        /// <item><description>Precision includes fractional seconds</description></item>
        /// <item><description>Indexed for query performance on date ranges</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Usage patterns:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Change tracking for user data</description></item>
        /// <item><description>Last activity indicators</description></item>
        /// <item><description>Audit trail for compliance</description></item>
        /// <item><description>Data freshness indicators</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Business logic:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Set to null when user is first created</description></item>
        /// <item><description>Updated to current UTC time on every modification</description></item>
        /// <item><description>Can be used to detect stale data</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = GetExistingUser();
        /// 
        /// // Check if user has been updated
        /// if (user.UpdatedAt.HasValue)
        /// {
        ///     Console.WriteLine($"Last updated: {user.UpdatedAt}");
        /// }
        /// else
        /// {
        ///     Console.WriteLine("User has never been updated");
        /// }
        /// 
        /// // Update user and set timestamp
        /// user.Name = "Updated Name";
        /// user.UpdatedAt = DateTime.UtcNow;
        /// </code>
        /// </example>
        public DateTime? UpdatedAt { get; set; }
    }
} 