using System;

namespace UserProfile.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId) 
            : base($"User with ID {userId} was not found.")
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }

    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string email) 
            : base($"User with email {email} already exists.")
        {
            Email = email;
        }

        public string Email { get; }
    }

    public class InvalidUserDataException : Exception
    {
        public InvalidUserDataException(string message) : base(message)
        {
        }

        public InvalidUserDataException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    public class EmailAlreadyInUseException : Exception
    {
        public EmailAlreadyInUseException(string email) 
            : base($"Email {email} is already in use by another user.")
        {
            Email = email;
        }

        public string Email { get; }
    }
} 