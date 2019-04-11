using System;

namespace GraphEditor.Interface.Nodes
{
    public interface IBaseConnectorData
    {
        string Name { get; }

        int Index { get; }

        bool IsOutBound { get; }

        bool IsActive { get; set; }

        byte[] Icon { get; }
    }

    public interface IConnectorData<T> : IBaseConnectorData
    {
        T Type { get; }  // TDOD: später semantischen Typ einführen für Prüfung, wer kann an wen
    }
}
