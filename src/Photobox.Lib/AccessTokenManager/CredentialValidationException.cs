﻿namespace Photobox.Lib.AccessTokenManager;

/// <summary>
/// Exception that is thrown when user credentials are invalid or fail to be validated.
/// </summary>
public class CredentialValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialValidationException"/> class.
    /// </summary>
    public CredentialValidationException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialValidationException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CredentialValidationException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialValidationException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public CredentialValidationException(string message, Exception inner)
        : base(message, inner) { }
}
