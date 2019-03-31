﻿using GraphEditor.Bl.Persistence;
using GraphEditor.Interfaces.Container;
using GraphEditor.Interfaces.Nodes;
using GraphEditor.Interfaces.Persistence;
using GraphEditor.Nodes.Bl;
using GraphEditor.ViewModel;

namespace GraphEditor.Bl
{
    public class BootStrapper
    {
        public static void InitServices()
        {
            ServiceContainer.Register<NodeTypeRepository, INodeTypeRepository>();
            ServiceContainer.Register<AreaViewModel>();
            ServiceContainer.Register<GraphPersistence, IGraphPersistence>();
        }
    }
}
