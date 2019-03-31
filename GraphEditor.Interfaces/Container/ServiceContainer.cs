using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor.Interfaces.Container
{
    public class ServiceContainer
    {
        private static readonly IList<RegisteredObject> _registeredObjects = new List<RegisteredObject>();

        public static void Register<TConcrete, TTypeToResolve>(params object[] args)
        {
            _registeredObjects.Add(new RegisteredObject(typeof(TConcrete), typeof(TTypeToResolve), args));
        }

        public static void Register<TConcrete>(params object[] args)
        {
            _registeredObjects.Add(new RegisteredObject(typeof(TConcrete), typeof(TConcrete), args));
        }

        public static TTypeToResolve Get<TTypeToResolve>()
        {
            return (TTypeToResolve) GetObject(typeof(TTypeToResolve));
        }

        private static object GetObject(Type typeToResolve)
        {
            var registeredObject = _registeredObjects.FirstOrDefault(o => o.TypeToResolve == typeToResolve);
            if (registeredObject == null)
            {
                throw new TypeNotRegisteredException($"The type {typeToResolve.Name} has not been registered");
            }
            return registeredObject.Instance;
        }
    }
}