using System;

namespace Utils
{
    /// <summary>
    /// Специализированное исключение
    /// </summary>
    public class RapWayException : Exception
    {
        public RapWayException() {}

        public RapWayException(string message)
            : base(message) {}

        public RapWayException(string message, Exception inner)
            : base(message, inner) {}
    }
}