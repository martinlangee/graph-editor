using System;

namespace GraphEditor.Interfaces.Container
{
    public class TypeNotRegisteredException : Exception
    {
        public TypeNotRegisteredException(string message)
            : base(message)
        {
        }
    }
}
