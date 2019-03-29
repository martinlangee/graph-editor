namespace GraphEditor.Interfaces.Persistence
{
    public interface IGraphPersistence
    {
        void LoadFromFile(string fileName);

        void SaveToFile(string fileName);
    }
}
