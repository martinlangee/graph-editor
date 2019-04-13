using System;

namespace GraphEditor.Ui.Tools
{
    public static class UiStates
    {
        private static bool showLabels;

        public static bool ShowLabels
        {
            get => showLabels;
            set
            {
                if (showLabels != value)
                {
                    showLabels = value;
                    OnShowLabelsChanged?.Invoke(showLabels);
                }
            }
        }

        public static void ShowLabelsChanged(bool visible)
        {
            ShowLabels = visible;
        }

        public static event Action<bool> OnShowLabelsChanged;
    }
}
