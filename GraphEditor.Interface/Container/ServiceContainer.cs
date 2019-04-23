#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
// License for the specific language governing rights and limitations under the License.
#endregion

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
            _registeredObjects.ForEach(obj =>
            {
                obj.ConcreteType.GetMethod(OnBuiltUpMethod)?.Invoke(obj.Instance, null);
            });
        }

        public static void FinalizeServices()
        {
            _registeredObjects.ForEach(obj =>
            {
                obj.ConcreteType.GetMethod(TearDownMethod)?.Invoke(obj.Instance, null);
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