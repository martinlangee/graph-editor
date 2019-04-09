using System;

namespace GraphEditor.Interface.Nodes
{
    public interface IConnectorData
    {
        string Name { get; }

        int Index { get; }

        bool IsOutBound { get; }

        bool IsActive { get; set; }

        byte[] Icon { get; }

        object Type { get; }  // TDOD: später semantischen Typ einführen für Prüfung, wer kann an wen
    }
}
