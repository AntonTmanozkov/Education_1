using System;

namespace SOVa
{
    public class OldValueException : Exception
    {
        public OldValueException() { }
        public OldValueException(string message) : base(message) { }
    }
}
