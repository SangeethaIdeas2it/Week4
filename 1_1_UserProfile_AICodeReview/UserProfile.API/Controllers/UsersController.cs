using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserProfile.Models;
using UserProfile.Services;

namespace UserProfile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
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

        [HttpGet("{userId}")]
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

        [HttpPut("{userId}")]
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

        [HttpDelete("{userId}")]
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