using System;

public class PathFilterException : Exception
{
    public PathFilterException() { }
    public PathFilterException(string message) : base(message) { }
    public PathFilterException(string message, System.Exception inner) : base(message, inner) { }
}