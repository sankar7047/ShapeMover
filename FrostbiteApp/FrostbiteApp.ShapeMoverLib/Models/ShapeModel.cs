using FrostbiteApp.ShapeMoverLib.Base;
using System.Windows.Media;

namespace FrostbiteApp.ShapeMoverLib.Models
{
    public class ShapeModel : BaseNotify
    {
        private double height;
        private double width;
        private double x;
        private double y;
        private SolidColorBrush fillColor;

        public double Height
        {
            get => height;
            set => SetPropertyChanged(ref height, value);
        }
        public double Width
        {
            get => width;
            set => SetPropertyChanged(ref width, value);
        }
        public double X
        {
            get => x;
            set => SetPropertyChanged(ref x, value);
        }
        public double Y
        {
            get => y;
            set => SetPropertyChanged(ref y, value);
        }
        public SolidColorBrush FillColor
        {
            get => fillColor;
            set => SetPropertyChanged(ref fillColor, value);
        }
    }
}
