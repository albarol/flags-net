using System;

public class FlagSourceException : Exception
{
    public FlagSourceException() { }
    public FlagSourceException(string message) : base(message) { }
    public FlagSourceException(string message, System.Exception inner) : base(message, inner) { }
}