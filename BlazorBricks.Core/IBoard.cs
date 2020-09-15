using System;
using System.Collections.Generic;
//
using System.Text;
using BlazorBricks.Core.Shapes;


namespace BlazorBricks.Core
{
    public interface IBoard
    {
        bool TestPieceOnPosition(IShape shape, int x, int y);
        void RemovePieceFromCurPosition(IShape shape);
        void PutPieceOnPosition(IShape shape, int x, int y);
        void ProcessNextMove();
        ShapeKind BackColor { get; set; }
        int Width { get; }
        int Height { get; }
        int Level { get; }
    }
}
