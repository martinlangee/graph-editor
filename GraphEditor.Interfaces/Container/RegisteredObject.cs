using System;

namespace GraphEditor.Interfaces.Container
{
    public class RegisteredObject
    {
        private object[] _args;
        private object _instance;

        public RegisteredObject(Type typeToResolve, Type concreteType, params object[] args)
        {
            TypeToResolve = typeToResolve;
            ConcreteType = concreteType;
            _args = args;
        }

        public Type TypeToResolve { get; private set; }

        public Type ConcreteType { get; private set; }

        public object Instance => _instance = _instance ?? Activator.CreateInstance(ConcreteType, _args);
    }
}
