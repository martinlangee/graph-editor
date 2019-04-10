﻿using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;

namespace GraphEditor.MyNodes.LogicalAND
{
    public class LogicalAND : NodeDataBase
    {
        public LogicalAND(INodeTypeData nodeTypeData, Action<IConnectorData> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("IN 1", 0, false);
            CreateConnector("IN 2", 1, false);
            CreateConnector("IN 3", 2, false);
            CreateConnector("IN 4", 3, false);
            CreateConnector("IN 5", 4, false);

            CreateConnector("AND", 0, true);
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