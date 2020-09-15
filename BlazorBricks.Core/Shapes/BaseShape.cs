using System;
//
using System.Diagnostics;
using BlazorBricks.Core;
using BlazorBricks.Core.Exceptions;


namespace BlazorBricks.Core.Shapes
{
    /// <summary>
    /// Represents the base shape for Bricks game.
    /// </summary>
    public abstract class BaseShape : BaseBricksArray, IShape
    {
        protected int x = 0;
        protected int y = 0;

        protected bool anchored = false;
        protected IBoard containerBoard = null;

        public BaseShape(int x, int y, int width, int height, string shapeString)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException("x");

            if (y < 0)
                throw new ArgumentOutOfRangeException("y");

            this.x = x;
            this.y = y;
            LoadData(width, height, shapeString);
        }

        public BaseShape(int width, int height, string shapeString, ShapeKind shapeKind)
        {
            this.ShapeKind = shapeKind;
            LoadData(width, height, shapeString);
        }

        private void LoadData(int width, int height, string shapeString)
        {
            if (shapeString.Length != width * height)
            {
                throw new InvalidShapeSizeException();
            }
            else if (HasInvalidShapeCharacter(shapeString))
            {
                throw new InvalidShapeStringCharacterException();
            }
            else
            {
                this._w = width;
                this._h = height;
                this._shapeStr = shapeString;
                this._brickArr = new IBrick[width, height];
                int i = 0;
                for (int row = 0; row < height; row++)
                {
                    for(int column = 0; column < width; column++)
                    {
                        int nColor = Convert.ToInt32(shapeString.Substring(i, 1));
                        IBrick brick = null;
                        if (nColor > 0)
                        {
                            brick = new Brick(column, row, this.ShapeKind);
                        }
                        _brickArr[column, row] = brick;
                        i++;
                    }
                }
            }
        }

        private static bool HasInvalidShapeCharacter(string shapeString)
        {
            bool ret = false;
            foreach(Char c in shapeString.ToCharArray())
            {
                if (c.ToString() != "0" && c.ToString() != "1")
                {
                    ret = true;
                }
            }
            return ret;
        }

        public void Anchor()
        {
            anchored = true;
        }

        public bool MoveLeft()
        {
            bool test = false;
            if (!anchored)
            {
                if (containerBoard == null)
                    throw new NullContainerBoardException();

                containerBoard.RemovePieceFromCurPosition(this);

                test = containerBoard.TestPieceOnPosition(this, this.X - 1, this.Y);
                if (test)
                {
                    containerBoard.RemovePieceFromCurPosition(this);
                    containerBoard.PutPieceOnPosition(this, this.X - 1, this.Y);
                }
            }
            return test;
        }

        public bool MoveRight()
        {
            bool test = false;
            if (!anchored)
            {
                if (containerBoard == null)
                    throw new NullContainerBoardException();

                containerBoard.RemovePieceFromCurPosition(this);

                test = containerBoard.TestPieceOnPosition(this, this.X + 1, this.Y);
                if (test)
                {
                    containerBoard.PutPieceOnPosition(this, this.X + 1, this.Y);
                }
            }
            return test;
        }

        public bool MoveDown()
        {
            bool test = false;

            if (!anchored)
            {
                containerBoard.RemovePieceFromCurPosition(this);

                //should anchor if shape can't move down from current position
                if (!containerBoard.TestPieceOnPosition(this, this.X, this.Y + 1))
                {
                    containerBoard.PutPieceOnPosition(this, this.X, this.Y);
                    this.Anchor();
                }
                else
                {
                    if (containerBoard == null)
                        throw new NullContainerBoardException();

                    test = containerBoard.TestPieceOnPosition(this, this.X, this.Y + 1);
                    if (test)
                    {
                        containerBoard.PutPieceOnPosition(this, this.X, this.Y + 1);
                    }
                }
            }

            return test;
        }

        public bool Rotate90()
        {
            bool test = false;
            if (!anchored)
            {
                if (containerBoard == null)
                    throw new NullContainerBoardException();

                IBrick[,] newBrickArr = new IBrick[_h, _w];
                IBrick[,] oldBrickArr = new IBrick[_w, _h];
                //store the original _brickArr in case rotate operation fails
                for (int row = 0; row < _h; row++)
                {
                    for (int column = 0; column < _w; column++)
                    {
                        newBrickArr[_h - row - 1, column] = _brickArr[column, row];
                        oldBrickArr[column, row] = _brickArr[column, row];
                    }
                }

                containerBoard.RemovePieceFromCurPosition(this);

                int w = _w;
                int h = _h;
                this._w = h;
                this._h = w;
                this._brickArr = newBrickArr;

                if (containerBoard.TestPieceOnPosition(this, this.X, this.Y))
                {
                    containerBoard.PutPieceOnPosition(this, this.X, this.Y);
                }
                else
                {
                    this._w = w;
                    this._h = h;
                    this._brickArr = oldBrickArr;
                    containerBoard.PutPieceOnPosition(this, this.X, this.Y);
                }
            }
            return test;
        }

        public bool Rotate270()
        {
            bool test = false;
            if (!anchored)
            {
                if (containerBoard == null)
                    throw new NullContainerBoardException();

                containerBoard.RemovePieceFromCurPosition(this);

                IBrick[,] newShapeArray = new IBrick[_h, _w];
                IBrick[,] oldShapeArray = new IBrick[_w, _h];
                for (int row = 0; row < _h; row++)
                {
                    for (int column = 0; column < _w; column++)
                    {
                        newShapeArray[row, _w - column - 1] = _brickArr[column, row];
                        oldShapeArray[column, row] = _brickArr[column, row];
                    }
                }

                int w = _w;
                int h = _h;
                this._w = h;
                this._h = w;
                this._brickArr = newShapeArray;

                if (containerBoard.TestPieceOnPosition(this, this.X, this.Y))
                {
                    containerBoard.PutPieceOnPosition(this, this.X, this.Y);
                }
                else
                {
                    this._w = w;
                    this._h = h;
                    this._brickArr = oldShapeArray;
                    containerBoard.PutPieceOnPosition(this, this.X, this.Y);
                }
            }
            return test;
        }


        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public ShapeKind ShapeKind { get; private set; }

        public IBoard ContainerBoard
        {
            get { return containerBoard; }
            set { containerBoard = value; }
        }

        public bool Anchored
        { 
            get {return anchored;}
        }

    }
}
