using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModel
{
    public class ConnectionViewModel: BaseNotification
    {
        bool _isSelected;

        public ConnectionViewModel()
        {
            Path = new ObservableCollection<Point>();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value, nameof(IsSelected)); }
        }

        public ObservableCollection<Point> Path { get; }
    }
}
