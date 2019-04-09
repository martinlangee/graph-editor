using GraphEditor.Interface.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor.Interface.Container
{
    public class ServiceContainer
    {
        private class TransactionHelper : IDisposable
        {
            Action _onDispose;

            public TransactionHelper(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                _onDispose?.Invoke();
            }
        }

        const string OnBuiltUpMethod = "OnBuiltUp";
        const string TearDownMethod = "TearDown";

        private static readonly IList<RegisteredObject> _registeredObjects = new List<RegisteredObject>();

        public static IDisposable RegisterTransaction()
        {
            return new TransactionHelper(() => OnFinishedRegisterTransaction());
        }

        public static IDisposable UnRegisterTransaction()
        {
            return new TransactionHelper(() => OnFinishedRegisterTransaction());
        }

        private static void OnFinishedRegisterTransaction()
        {
            _registeredObjects.ForEach(ro =>
            {
                ro.ConcreteType.GetMethod(OnBuiltUpMethod)?.Invoke(ro.Instance, null);
            });
        }

        public static void FinalizeServices()
        {
            _registeredObjects.ForEach(ro =>
            {
                ro.ConcreteType.GetMethod(TearDownMethod)?.Invoke(ro.Instance, null);
            });
        }

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