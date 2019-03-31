using System;

namespace GraphEditor.Interfaces.Container
{
    public class RegisteredObject
    {
        private object[] _args;
        private object _instance;

        public RegisteredObject(Type concreteType, Type typeToResolve, params object[] args)
        {
            ConcreteType = concreteType;
            TypeToResolve = typeToResolve;
            _args = args;
        }

        public Type ConcreteType { get; private set; }

        public Type TypeToResolve { get; private set; }

        public object Instance => _instance = _instance ?? Activator.CreateInstance(ConcreteType, _args);
    }
}
