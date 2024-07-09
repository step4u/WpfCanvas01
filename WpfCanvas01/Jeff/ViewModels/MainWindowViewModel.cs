using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Jeff.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        int _shapeidx;
        int _linekindidx;
        int _thicknessidx;
        int _coloridx;
        int _completemethodidx;

        public MainWindowViewModel()
        {
            Shapes = new Dictionary<int, string>() { { 0, "None" }, { 1, "Line" }, { 2, "Retangle" }, { 3, "Polyline" }, { 4, "Eclipse" }, { 5, "Polygon" }, { 6, "Curve" } };
            LineKinds = new Dictionary<DoubleCollection, string>() { { new DoubleCollection { 1, 0 }, "실선" }, { new DoubleCollection { 1, 1, 3, 3 }, "점선" } };
            Thicknesses = new Dictionary<int, int>() { { 0, 1 }, { 1, 2 }, { 3, 4 }, { 5, 5 } };
            Colors = new Dictionary<int, string>() { { 0, "Black" }, { 1, "Red" }, { 2, "Green" }, { 3, "Blue" }, { 4, "Yellow" } };
            PointShape = new Dictionary<int, string>() { { 0, "Arrow" }, { 1, "Cross" } };
            CompleteMethods = new Dictionary<int, string>() { { 0, "DoubleClick" }, { 1, "RightClick" } };
        }

        public Dictionary<int, string> Shapes { get; set; }
        public Dictionary<DoubleCollection, string> LineKinds { get; set; }
        public Dictionary<int, int> Thicknesses { get; set; }
        public Dictionary<int, string> Colors { get; set; }
        public Dictionary<int, string> PointShape { get; set; }
        public Dictionary<int, string> CompleteMethods { get; set; }

        public int ShapeIdx
        {
            get { return _shapeidx; }
            set
            {
                _shapeidx = value;
                OnPropertyChanged("ShapeIdx");
            }
        }

        public int LineKindIdx
        {
            get { return _linekindidx; }
            set
            {
                _linekindidx = value;
                OnPropertyChanged("LineKindIdx");
            }
        }

        public int ThicknessIdx
        {
            get { return _thicknessidx; }
            set
            {
                _thicknessidx = value;
                OnPropertyChanged("ThicknessIdx");
            }
        }

        public int ColorIdx
        {
            get { return _coloridx; }
            set
            {
                _coloridx = value;
                OnPropertyChanged("ColorIdx");
            }
        }

        public int CompleteMethodIdx
        {
            get { return _completemethodidx; }
            set
            {
                _completemethodidx = value;
                OnPropertyChanged("CompleteMethodIdx");
            }
        }
        


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
