using System.ComponentModel;

namespace BlazorBricks.Core
{
    public interface IBrick : INotifyPropertyChanged
    {
        double X { get; set; }
        double Y { get; set; }
        ShapeKind Kind { get; set; }
    }
}
