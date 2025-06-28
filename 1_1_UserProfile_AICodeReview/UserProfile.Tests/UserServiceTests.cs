using System;
using Moq;
using UserProfile.Models;
using UserProfile.Repositories;
using UserProfile.Services;
using Xunit;

namespace UserProfile.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public void CreateUser_Should_Create_And_Return_User()
        {
            // Arrange
            var userDto = new UserDto { Name = "Test User", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(userDto.Email)).Returns((User)null);

            // Act
            var createdUser = _userService.CreateUser(userDto);

            // Assert
            Assert.NotNull(createdUser);
            Assert.Equal(userDto.Name, createdUser.Name);
            Assert.Equal(userDto.Email, createdUser.Email);
            _userRepositoryMock.Verify(repo => repo.AddUser(It.Is<User>(u => u.Name == userDto.Name && u.Email == userDto.Email)), Times.Once);
        }

        [Fact]
        public void CreateUser_Should_Throw_InvalidOperationException_When_Email_Already_Exists()
        {
            // Arrange
            var userDto = new UserDto { Name = "Test User", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmail(userDto.Email)).Returns(new User());

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _userService.CreateUser(userDto));
        }

        [Fact]
        public void CreateUser_Should_Throw_ArgumentNullException_When_UserDto_Is_Null()
        {
            // Arrange
            UserDto userDto = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _userService.CreateUser(userDto));
        }

        [Fact]
        public void GetUserById_Should_Return_Null_When_User_Does_Not_Exist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).Returns((User)null);

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserById_Should_Return_User_When_User_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).Returns(user);

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public void UpdateUser_Should_Update_User()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDto = new UserDto { Id = userId, Name = "Updated Name", Email = "updated@example.com" };
            var existingUser = new User { Id = userId, Name = "Original Name", Email = "original@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserById(userId)).Returns(existingUser);

            // Act
            _userService.UpdateUser(userDto);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateUser(It.Is<User>(u => u.Id == userId && u.Name == userDto.Name && u.Email == userDto.Email)), Times.Once);
        }

        [Fact]
        public void UpdateUser_Should_Throw_InvalidOperationException_When_User_Not_Found()
        {
            // Arrange
            var userDto = new UserDto { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserById(userDto.Id)).Returns((User)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _userService.UpdateUser(userDto));
        }

        [Fact]
        public void UpdateUser_Should_Throw_ArgumentNullException_When_UserDto_Is_Null()
        {
            // Arrange
            UserDto userDto = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _userService.UpdateUser(userDto));
        }

        [Fact]
        public void DeleteUser_Should_Delete_User()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            _userService.DeleteUser(userId);

            // Assert
            _userRepositoryMock.Verify(repo => repo.DeleteUser(userId), Times.Once);
        }
    }
} 