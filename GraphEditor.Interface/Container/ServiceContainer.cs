#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
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