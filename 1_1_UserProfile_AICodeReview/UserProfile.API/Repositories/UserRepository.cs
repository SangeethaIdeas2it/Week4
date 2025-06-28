using System;
using System.Collections.Generic;
using System.Linq;
using UserProfile.API.Data;
using UserProfile.Models;
using UserProfile.Repositories;

namespace UserProfile.API.Repositories
{
    /// <summary>
    /// Data access layer implementation for user profile operations using Entity Framework Core.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The UserRepository provides concrete implementation of the IUserRepository interface,
    /// handling all database operations for user profile management. This repository uses
    /// Entity Framework Core for data access and implements the Repository pattern to
    /// abstract database operations from the business logic layer.
    /// </para>
    /// 
    /// <para>
    /// <strong>Key Features:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>CRUD operations for user profiles</description></item>
    /// <item><description>Entity Framework Core integration</description></item>
    /// <item><description>Automatic change tracking and persistence</description></item>
    /// <item><description>LINQ-based query capabilities</description></item>
    /// <item><description>Transaction management through DbContext</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Usage Patterns:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Dependency injection for DbContext access</description></item>
    /// <item><description>Unit of Work pattern through DbContext</description></item>
    /// <item><description>Repository pattern for data access abstraction</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Dependencies:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>UserDbContext - Entity Framework Core context</description></item>
    /// <item><description>User - Domain model for database operations</description></item>
    /// <item><description>IUserRepository - Interface contract</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Performance Considerations:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Entity Framework Core change tracking</description></item>
    /// <item><description>Lazy loading capabilities</description></item>
    /// <item><description>Query optimization through LINQ</description></item>
    /// <item><description>Connection pooling through DbContext</description></item>
    /// </list>
    /// 
    /// <para>
    /// <strong>Security Features:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item><description>Parameterized queries prevent SQL injection</description></item>
    /// <item><description>Entity Framework Core security best practices</description></item>
    /// <item><description>Input validation through domain models</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <para>The following example demonstrates basic usage of the UserRepository:</para>
    /// <code>
    /// // Dependency injection setup
    /// services.AddScoped&lt;IUserRepository, UserRepository&gt;();
    /// 
    /// // Usage in service
    /// public class UserService
    /// {
    ///     private readonly IUserRepository _userRepository;
    ///     
    ///     public UserService(IUserRepository userRepository)
    ///     {
    ///         _userRepository = userRepository;
    ///     }
    ///     
    ///     public async Task&lt;User&gt; CreateUserAsync(User user)
    ///     {
    ///         await _userRepository.AddUserAsync(user);
    ///         return user;
    ///     }
    /// }
    /// </code>
    /// 
    /// <para>The following example shows CRUD operations:</para>
    /// <code>
    /// var repository = new UserRepository(dbContext);
    /// 
    /// // Create
    /// var user = new User { Name = "John Doe", Email = "john@example.com" };
    /// repository.AddUser(user);
    /// 
    /// // Read
    /// var retrievedUser = repository.GetUserById(user.Id);
    /// var userByEmail = repository.GetUserByEmail("john@example.com");
    /// var allUsers = repository.GetAllUsers();
    /// 
    /// // Update
    /// user.Name = "John Smith";
    /// repository.UpdateUser(user);
    /// 
    /// // Delete
    /// repository.DeleteUser(user.Id);
    /// </code>
    /// </example>
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UserRepository with the required database context.
        /// </summary>
        /// <param name="context">
        /// The Entity Framework Core database context for user operations.
        /// Must not be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="context"/> is null.
        /// </exception>
        /// <example>
        /// <code>
        /// var dbContext = new UserDbContext(options);
        /// var repository = new UserRepository(dbContext);
        /// </code>
        /// </example>
        public UserRepository(UserDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method adds a new user entity to the database context and immediately
        /// saves the changes to persist the data. The method uses Entity Framework Core's
        /// change tracking to manage the entity lifecycle.
        /// </para>
        /// 
        /// <para>
        /// <strong>Operation details:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Adds user to DbContext change tracker</description></item>
        /// <item><description>Immediately saves changes to database</description></item>
        /// <item><description>Updates entity with database-generated values</description></item>
        /// <item><description>Throws exception on validation or constraint violations</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Immediate database write operation</description></item>
        /// <item><description>Consider batching for multiple operations</description></item>
        /// <item><description>Transaction scope is method-level</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Error handling:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>DbUpdateException for constraint violations</description></item>
        /// <item><description>ValidationException for invalid data</description></item>
        /// <item><description>Connection exceptions for network issues</description></item>
        /// </list>
        /// </remarks>
        /// <param name="user">
        /// The user entity to add to the database.
        /// Must not be null and must have valid data.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="user"/> is null.
        /// </exception>
        /// <exception cref="DbUpdateException">
        /// Thrown when database constraints are violated (e.g., duplicate email).
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when entity validation fails.
        /// </exception>
        /// <example>
        /// <code>
        /// var user = new User
        /// {
        ///     Id = Guid.NewGuid(),
        ///     Name = "Jane Doe",
        ///     Email = "jane.doe@example.com",
        ///     CreatedAt = DateTime.UtcNow
        /// };
        /// 
        /// try
        /// {
        ///     repository.AddUser(user);
        ///     Console.WriteLine($"User added with ID: {user.Id}");
        /// }
        /// catch (DbUpdateException ex)
        /// {
        ///     Console.WriteLine($"Database error: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes a user from the database by their unique identifier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method removes a user entity from the database based on the provided
        /// GUID identifier. The method first checks if the user exists before attempting
        /// deletion to provide better error handling.
        /// </para>
        /// 
        /// <para>
        /// <strong>Operation details:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Finds user by primary key</description></item>
        /// <item><description>Removes entity from change tracker if found</description></item>
        /// <item><description>Saves changes to persist deletion</description></item>
        /// <item><description>Silently ignores if user doesn't exist</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Database query to find user before deletion</description></item>
        /// <item><description>Immediate database write operation</description></item>
        /// <item><description>Consider soft delete for audit purposes</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Error handling:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>DbUpdateException for constraint violations</description></item>
        /// <item><description>Connection exceptions for network issues</description></item>
        /// <item><description>Silent failure if user doesn't exist</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userId">
        /// The unique identifier (GUID) of the user to delete.
        /// </param>
        /// <exception cref="DbUpdateException">
        /// Thrown when database constraints prevent deletion.
        /// </exception>
        /// <example>
        /// <code>
        /// var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        /// 
        /// try
        /// {
        ///     repository.DeleteUser(userId);
        ///     Console.WriteLine("User deleted successfully");
        /// }
        /// catch (DbUpdateException ex)
        /// {
        ///     Console.WriteLine($"Deletion failed: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public void DeleteUser(Guid userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method retrieves all user entities from the database and returns them
        /// as an enumerable collection. The method uses Entity Framework Core's LINQ
        /// provider to execute the query efficiently.
        /// </para>
        /// 
        /// <para>
        /// <strong>Operation details:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Executes SELECT query on Users table</description></item>
        /// <item><description>Returns all records without filtering</description></item>
        /// <item><description>Uses Entity Framework Core change tracking</description></item>
        /// <item><description>Materializes results in memory</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Loads all users into memory</description></item>
        /// <item><description>Consider pagination for large datasets</description></item>
        /// <item><description>Consider projection for specific fields only</description></item>
        /// <item><description>Database query executed immediately</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Memory considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>All users loaded into memory</description></item>
        /// <item><description>Consider streaming for large datasets</description></item>
        /// <item><description>Monitor memory usage in production</description></item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// An enumerable collection of all User entities in the database.
        /// Returns empty collection if no users exist.
        /// </returns>
        /// <example>
        /// <code>
        /// var allUsers = repository.GetAllUsers();
        /// 
        /// foreach (var user in allUsers)
        /// {
        ///     Console.WriteLine($"User: {user.Name} ({user.Email})");
        /// }
        /// 
        /// Console.WriteLine($"Total users: {allUsers.Count()}");
        /// </code>
        /// </example>
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        /// <summary>
        /// Retrieves a user from the database by their email address.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method finds a user entity in the database based on the provided email
        /// address. The search is case-sensitive and uses Entity Framework Core's LINQ
        /// provider for efficient querying.
        /// </para>
        /// 
        /// <para>
        /// <strong>Operation details:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Executes SELECT query with email filter</description></item>
        /// <item><description>Returns first matching user or null</description></item>
        /// <item><description>Uses Entity Framework Core change tracking</description></item>
        /// <item><description>Case-sensitive email comparison</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Requires email field to be indexed for optimal performance</description></item>
        /// <item><description>Single database query execution</description></item>
        /// <item><description>Consider caching for frequently accessed emails</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Database considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Email field should have unique index</description></item>
        /// <item><description>Case sensitivity depends on database collation</description></item>
        /// <item><description>Consider case-insensitive search if needed</description></item>
        /// </list>
        /// </remarks>
        /// <param name="email">
        /// The email address to search for.
        /// Should be normalized (lowercase) for consistent results.
        /// </param>
        /// <returns>
        /// The User entity if found, or null if no user exists with the specified email.
        /// </returns>
        /// <example>
        /// <code>
        /// var user = repository.GetUserByEmail("john.doe@example.com");
        /// 
        /// if (user != null)
        /// {
        ///     Console.WriteLine($"Found user: {user.Name}");
        /// }
        /// else
        /// {
        ///     Console.WriteLine("User not found");
        /// }
        /// </code>
        /// </example>
        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// Retrieves a user from the database by their unique identifier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method finds a user entity in the database based on the provided GUID
        /// identifier. The search uses the primary key for optimal performance and
        /// leverages Entity Framework Core's change tracking.
        /// </para>
        /// 
        /// <para>
        /// <strong>Operation details:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Executes SELECT query by primary key</description></item>
        /// <item><description>Returns user entity or null if not found</description></item>
        /// <item><description>Uses Entity Framework Core change tracking</description></item>
        /// <item><description>Most efficient lookup method</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Primary key lookup - fastest possible query</description></item>
        /// <item><description>Single database query execution</description></item>
        /// <item><description>Consider caching for frequently accessed users</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Database considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Primary key is automatically indexed</description></item>
        /// <item><description>GUID-based primary key for global uniqueness</description></item>
        /// <item><description>Optimal for distributed systems</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userId">
        /// The unique identifier (GUID) of the user to retrieve.
        /// </param>
        /// <returns>
        /// The User entity if found, or null if no user exists with the specified ID.
        /// </returns>
        /// <example>
        /// <code>
        /// var userId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        /// var user = repository.GetUserById(userId);
        /// 
        /// if (user != null)
        /// {
        ///     Console.WriteLine($"Found user: {user.Name} ({user.Email})");
        /// }
        /// else
        /// {
        ///     Console.WriteLine("User not found");
        /// }
        /// </code>
        /// </example>
        public User? GetUserById(Guid userId)
        {
            return _context.Users.Find(userId);
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method updates an existing user entity in the database. The method
        /// uses Entity Framework Core's change tracking to detect modifications and
        /// persists only the changed properties to the database.
        /// </para>
        /// 
        /// <para>
        /// <strong>Operation details:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Marks entity as modified in change tracker</description></item>
        /// <item><description>Detects and persists only changed properties</description></item>
        /// <item><description>Immediately saves changes to database</description></item>
        /// <item><description>Updates entity with database-generated values</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Performance considerations:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>Only changed properties are updated</description></item>
        /// <item><description>Immediate database write operation</description></item>
        /// <item><description>Consider optimistic concurrency control</description></item>
        /// <item><description>Transaction scope is method-level</description></item>
        /// </list>
        /// 
        /// <para>
        /// <strong>Error handling:</strong>
        /// </para>
        /// <list type="bullet">
        /// <item><description>DbUpdateException for constraint violations</description></item>
        /// <item><description>DbUpdateConcurrencyException for concurrency conflicts</description></item>
        /// <item><description>ValidationException for invalid data</description></item>
        /// </list>
        /// </remarks>
        /// <param name="user">
        /// The user entity to update in the database.
        /// Must not be null and must have a valid ID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="user"/> is null.
        /// </exception>
        /// <exception cref="DbUpdateException">
        /// Thrown when database constraints are violated.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        /// Thrown when optimistic concurrency conflicts occur.
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when entity validation fails.
        /// </exception>
        /// <example>
        /// <code>
        /// var user = repository.GetUserById(userId);
        /// if (user != null)
        /// {
        ///     user.Name = "Updated Name";
        ///     user.Email = "updated.email@example.com";
        ///     user.UpdatedAt = DateTime.UtcNow;
        ///     
        ///     try
        ///     {
        ///         repository.UpdateUser(user);
        ///         Console.WriteLine("User updated successfully");
        ///     }
        ///     catch (DbUpdateException ex)
        ///     {
        ///         Console.WriteLine($"Update failed: {ex.Message}");
        ///     }
        /// }
        /// </code>
        /// </example>
        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
} 