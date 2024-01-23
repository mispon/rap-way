using System;

namespace Core
{
    /// <summary>
    /// Специализированное исключение
    /// </summary>
    public class RapWayException : Exception
    {
        public RapWayException(string message) : base(message) {}
    }
}