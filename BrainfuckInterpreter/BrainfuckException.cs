using System;

namespace BrainfuckInterpreter
{
    public class BrainfuckException : Exception
    {
        public BrainfuckException(string message) : base(message)
        {
        }
    }
}
