﻿using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace GraphEditor.Nodes
{
    public abstract class NodeTypeDataBase : INodeTypeData
    {
        public NodeTypeDataBase()
        {
            Name = "<not set>";
            Description = "<not set>";

            Image = LoadGraphic(NodeType);
            Icon = LoadGraphic(NodeType, "ico");
        }

        private int _newIndex = 1;

        public string NextNewName
        {
            get { return $"{Name} {_newIndex++}"; }
        }

        private static byte[] LoadGraphic(Type nodeType, string suffix = "")
        {
            suffix = string.IsNullOrEmpty(suffix) ? "" : $"_{suffix}";
            var resPath = $"/Ui/{nodeType.Name}{suffix}.png";

            var src = new BitmapImage();
            try
            {
                src.BeginInit();
                src.UriSource = new Uri($"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName()};component{resPath}");
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
            }
            catch (IOException)
            {
                Console.WriteLine($"Resource '{resPath}' not found");
                return null;
            }

            using (var ms = new MemoryStream())
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(src));
                pngEncoder.Save(ms);
                return ms.GetBuffer();
            }
        }

        public string Type => NodeType.Name;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public byte[] Icon { get; protected set; }

        public byte[] Image { get; protected set; }

        public virtual bool CanConnectToIn(INodeTypeData otherNode, int otherOutIndex, int myInIndex) => myInIndex % 2 == 0;  // TODO: CanConnectToIn zum Testen

        public virtual bool CanConnectToOut(INodeTypeData otherNode, int otherInIndex, int myOutIndex) => myOutIndex % 2 == 1;  // TODO: CanConnectToOut zum Testen

        protected abstract Type NodeType { get; }

        public INodeData CreateNode(Action<IConnectorData, bool> onActiveChanged, Func<IConnectorData, bool> canBeDeactivated)
        {
            return Activator.CreateInstance(NodeType, this, onActiveChanged, canBeDeactivated) as INodeData;
        }
    }
}
