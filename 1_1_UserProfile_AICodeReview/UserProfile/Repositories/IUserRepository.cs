using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserProfile.Models;

namespace UserProfile.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task<int> GetTotalUsersCountAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid userId);
        Task<bool> UserExistsAsync(Guid userId);
        Task<bool> EmailExistsAsync(string email);
        
        // Synchronous methods for backward compatibility
        User? GetUserById(Guid userId);
        User? GetUserByEmail(string email);
        IEnumerable<User> GetAllUsers();
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(Guid userId);
    }
} 