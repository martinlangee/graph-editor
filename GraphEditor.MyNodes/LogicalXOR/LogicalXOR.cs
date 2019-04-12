﻿using System;
using System.Reflection;
using System.Xml.Linq;
using GraphEditor.Interface.Nodes;

namespace GraphEditor.MyNodes.LogicalXOR
{
    public class LogicalXOR : NodeDataBase
    {
        public LogicalXOR(IBaseNodeTypeData nodeTypeData, Action<IBaseConnectorData> onActiveChanged, Func<IBaseConnectorData, bool> canBeDeactivated)
            : base(nodeTypeData, onActiveChanged, canBeDeactivated, Assembly.GetExecutingAssembly())
        {
            CreateConnector("IN 1", 0, false, SignalType.Red);
            CreateConnector("IN 2", 1, false, SignalType.Red);
            CreateConnector("IN 3", 2, false, SignalType.Red);
            CreateConnector("IN 4", 3, false, SignalType.Red);
            CreateConnector("IN 5", 4, false, SignalType.Red);

            CreateConnector("XOR", 0, true, SignalType.Red);
        }

        protected override Type ConfigControlType => typeof(LogicalXORControl);

        protected override void LoadTypeSpecificData(XElement parentXml)
        {

        }

        protected override void SaveTypeSpecificData(XElement parent)
        {

        }
    }
}