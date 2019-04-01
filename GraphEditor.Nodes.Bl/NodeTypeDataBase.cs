using GraphEditor.Interfaces.Nodes;
using System;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace GraphEditor.Nodes.Bl
{
    public abstract class NodeTypeDataBase : INodeTypeData
    {
        public NodeTypeDataBase()
        {
            Name = "<not set>";
            Description = "<not set>";

            Image = LoadResource();
            Icon = LoadResource("Icon");
        }

        private byte[] LoadResource(string suffix = "")
        {
            suffix = string.IsNullOrEmpty(suffix) ? "" : $"_{suffix}";

            BitmapImage src = new BitmapImage();
            try
            {
                src.BeginInit();
                src.UriSource = new Uri($"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName()};component/Nodes/{NodeType.Name}{suffix}.png");
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
            }
            catch (IOException)
            {
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

        private void LoadIcon()
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri($"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName()};component/Nodes/{NodeType.Name}_Icon.png");
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            using (var ms = new MemoryStream())
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(src));
                pngEncoder.Save(ms);
                Image = ms.GetBuffer();
            }
        }

        public string Type => GetType().Name;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public byte[] Icon { get; protected set; }

        public byte[] Image { get; protected set; }

        public virtual UserControl ConfigUi => null;

        public virtual bool CanConnectToIn(INodeTypeData otherNode, int otherOutIndex, int myInIndex) => myInIndex % 2 == 0;

        public virtual bool CanConnectToOut(INodeTypeData otherNode, int otherInIndex, int myOutIndex) => myOutIndex % 2 == 1;

        protected abstract Type NodeType { get; }

        public INodeData CreateNode()
        {
            return Activator.CreateInstance(NodeType, this) as INodeData;
        }
    }
}
