using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GraphEditor.Nodes.ViewModel
{
    public abstract class NodeDataBase : BaseNotification, INodeData
    {
        Func<IConnectorData, bool> _checkIsConnected;
        bool _changing;
        private string _name;

        public NodeDataBase(INodeTypeData nodeTypeData, Func<IConnectorData, bool> getIsConnected)
        {
            TypeData = nodeTypeData;
            _checkIsConnected = getIsConnected;

            Id = Guid.NewGuid().ToString();
            Name = Type;

            InConnectors = new ObservableCollection<IConnectorData>();
            OutConnectors = new ObservableCollection<IConnectorData>();
        }

        public INodeTypeData TypeData { get; }

        public string Id { get; }

        public string Type => GetType().Name;

        public IList<IConnectorData> InConnectors { get; private set; }

        public IList<IConnectorData> OutConnectors { get; private set; }

        public string Name { get => _name; set => SetProperty<NodeDataBase, string>(ref _name, value, nameof(Name)); }

        protected abstract Type ConfigControlType { get; }

        public INodeConfigUi CreateConfigUi()
        {
            return Activator.CreateInstance(ConfigControlType, this) as INodeConfigUi;
        }
    }
}
