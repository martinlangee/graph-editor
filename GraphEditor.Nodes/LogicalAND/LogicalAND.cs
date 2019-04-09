﻿using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;

namespace GraphEditor.Nodes.LogicalAND
{
    public class LogicalAND : NodeDataBase
    {
        public LogicalAND(INodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            Ins.Add(new ConnectorData("IN 1", 0, false, true, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("IN 2", 1, false, false, onActiveChanged, canBeDeactivated));
            Ins.Add(new ConnectorData("IN 3", 2, false, true, onActiveChanged, canBeDeactivated));

            Outs.Add(new ConnectorData("AND", 0, true, true, onActiveChanged, canBeDeactivated));
        }

        protected override Type ConfigControlType => typeof(LogicalANDControl);

        protected override void LoadTypeSpecificData(XElement parentXml)
        {

        }

        protected override void SaveTypeSpecificData(XElement parent)
        {

        }
    }
}