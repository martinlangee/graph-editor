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
        private string _name;

        public NodeDataBase(INodeTypeData nodeTypeData, Action<IConnectorData, bool> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
        {
            TypeData = nodeTypeData;

            Id = Guid.NewGuid().ToString();
            Name = TypeData.NextNewName;

            Ins = new ObservableCollection<IConnectorData>();
            Outs = new ObservableCollection<IConnectorData>();
        }

        public INodeTypeData TypeData { get; }

        public string Id { get; }

        public string Type => GetType().Name;

        public IList<IConnectorData> Ins { get; private set; }

        public IList<IConnectorData> Outs { get; private set; }

        public string Name { get => _name; set => SetProperty<NodeDataBase, string>(ref _name, value, nameof(Name)); }

        protected abstract Type ConfigControlType { get; }

        public INodeConfigUi CreateConfigUi()
        {
            return Activator.CreateInstance(ConfigControlType, this) as INodeConfigUi;
        }
    }
}
