#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/.
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied.
// See the License for the specific language governing rights and limitations under the License.
#endregion

using System;

namespace GraphEditor.Interface.Container
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
