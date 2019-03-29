using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor.Interfaces.Container
{
    public class ServiceContainer
    {
        private static readonly IList<RegisteredObject> _registeredObjects = new List<RegisteredObject>();

        public static void Register<TTypeToResolve, TConcrete>()
        {
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), typeof(TConcrete)));
        }

        public static void Register<TConcrete>()
        {
            _registeredObjects.Add(new RegisteredObject(typeof(TConcrete), typeof(TConcrete)));
        }

        public static TTypeToResolve Resolve<TTypeToResolve>()
        {
            return (TTypeToResolve) ResolveObject(typeof(TTypeToResolve));
        }

        private static object ResolveObject(Type typeToResolve)
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