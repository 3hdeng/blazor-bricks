using System;
namespace BlazorBricks.Core
{
    public interface IBricksArray
    {
        //string GetShapeString();
        //void Init();
        
        IBrick[,] BrickArr { get; }
        string ShapeString { get; }
        int Width { get; }
        int Height { get; }
    }
}
