namespace GraphEditor.Interfaces.Nodes
{
    public interface IConnectorData
    {
        string Name { get; }

        int Index { get; }

        bool IsOutBound { get; }

        bool IsActive { get; set; }

        object Type { get; }  // TDOD: später semantischen Typ einführen für Prüfung, wer kann an wen
    }
}
