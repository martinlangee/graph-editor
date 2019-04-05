using GraphEditor.Interfaces.ConfigUi;
using GraphEditor.Interfaces.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GraphEditor.Nodes
{
    public abstract class NodeDataBase : BaseNotification, INodeData
    {
        bool _changing;
        private string _name;

        public NodeDataBase(INodeTypeData nodeTypeData)
        {
            TypeData = nodeTypeData;

            Id = Guid.NewGuid().ToString();
            Name = Type;

            var inConn = new ObservableCollection<string>();
            inConn.CollectionChanged += InConnectorsChanged;
            InConnectors = inConn;

            var outConn = new ObservableCollection<string>();
            outConn.CollectionChanged += OutConnectorsChanged;
            OutConnectors = outConn;
        }

        private void InConnectorsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_changing) return;
            _changing = true;
            try
            {
                if (!OnInConnectorsChanged?.Invoke(e) == false)
                {
                    InConnectors = new ObservableCollection<string>((List<string>)e.OldItems);
                }
            }
            finally
            {
                _changing = false;
            }
        }

        private void OutConnectorsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_changing) return;
            _changing = true;
            try
            {
                if (!OnOutConnectorsChanged?.Invoke(e) == false)
                {
                    OutConnectors = new ObservableCollection<string>((List<string>)e.OldItems);
                }
            }
            finally
            {
                _changing = false;
            }
        }

        public INodeTypeData TypeData { get; }

        public string Id { get; }

        public string Type => GetType().Name;

        public IList<string> InConnectors { get; private set; }

        public IList<string> OutConnectors { get; private set; }

        public string Name { get => _name; set => SetProperty<NodeDataBase, string>(ref _name, value, nameof(Name)); }

        protected abstract Type ConfigControlType { get; }

        public Func<NotifyCollectionChangedEventArgs, bool> OnInConnectorsChanged { private get; set; }

        public Func<NotifyCollectionChangedEventArgs, bool> OnOutConnectorsChanged { private get; set; }

        public INodeConfigUi CreateConfigUi()
        {
            return Activator.CreateInstance(ConfigControlType, this) as INodeConfigUi;
        }
    }
}
