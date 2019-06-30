using System;

namespace Socketize.Exceptions
{
  public class SocketizeException : Exception
  {
    public SocketizeException(string message) : base(message)
    {
    }

    public SocketizeException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}