namespace BlazorBricks.Core
{
    public interface IBrick
    {
        double X { get; set; }
        double Y { get; set; }
        ShapeKind Kind { get; set; }
    }
}
