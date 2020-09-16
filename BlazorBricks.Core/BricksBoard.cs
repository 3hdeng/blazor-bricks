using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BlazorBricks.Core.Exceptions;
using BlazorBricks.Core.Shapes;

using System.Threading.Tasks;

namespace BlazorBricks.Core
{
    public class BricksBoard : BaseBricksArray, IBoard
    {
        public event EventHandler Updated;

        #region attribute        
        private IShape _shape = null;
        private int _score = 0;
        private int _hiScore = 0;
        private int _level = 1;
        private int _lines = 0;
        private IShape _next = null;

        #endregion attribute

        #region constructors
        public BricksBoard(IPresenter presenter)
        {
            this._presenter = presenter;
            this._w = 10;
            this._h = 20;
            Init();
            _next = GetRandomShape();
        }

        public BricksBoard(IPresenter presenter, int width, int height)
        {
            this._presenter = presenter;

            if (width < 0)
                throw new ArgumentOutOfRangeException("width");

            if (height < 0)
                throw new ArgumentOutOfRangeException("height");

            this._w = width;
            this._h = height;
            Init();
            _next = GetRandomShape();
        }

        #endregion constructors

        #region methods

        public override void Init()
        {
            _score = 0;
            _level = 1;
            _lines = 0;
            if (_shape != null)
            {
                _shape.Y = 0;
            }
            _next = GetRandomShape();
            _presenter.UpdateScoreView(_score, _hiScore, _lines, _level, _next);
            base.Init();
        }

        public bool TestPieceOnPosition(IShape shape, int x, int y)
        {
            for (int row = 0; row < shape.Height; row++)
            {
                for (int column = 0; column < shape.Width; column++)
                {
                    //is the position out of range?
                    if (column + x < 0)
                        return false;

                    if (row + y < 0)
                        return false;

                    if (column + x >= _w)
                        return false;

                    if (row + y >= _h)
                        return false;

                    //will the shape collide in the board?
                    if (
                        _brickArr[column + x, row + y] != null &&
                        shape.BrickArr[column, row] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void RemovePieceFromCurPosition(IShape shape)
        {
            for (int row = 0; row < shape.Height; row++)
            {
                for (int column = 0; column < shape.Width; column++)
                {
                    if (shape.BrickArr[column, row] != null)
                    {
                        _brickArr[column + shape.X, row + shape.Y] = null;
                    }
                }
            }
        }

        public void PutPieceOnPosition(IShape shape, int x, int y)
        {
            if (!TestPieceOnPosition(shape, x, y))
                throw new CantSetShapePosition();

            for (int row = 0; row < shape.Height; row++)
            {
                for (int column = 0; column < shape.Width; column++)
                {
                    if (shape.BrickArr[column, row] != null)
                    {
                        _brickArr[column + x, row + y] = shape.BrickArr[column, row];
                    }
                }
            }
            shape.X = x;
            shape.Y = y;

            if (_presenter != null)
            {
                _presenter.UpdateBoardView(GetShapeString(), _brickArr, _w, _h);
            }
        }

        private bool RemoveCompletedRows()
        {
            bool completed = false;
            int row = _h - 1;
            while (row >= 0)
            {
                completed = true;
                for (int column = 0; column < _w; column++)
                {
                    if (_brickArr[column, row] == null)
                    {
                        completed = false;
                        break;
                    }
                }

                if (completed)
                {
                    //presenter.HighlightCompletedRow(row);

                    IBrick[] removedBricks = new IBrick[_w];
                    for (int column = 0; column < _w; column++)
                    {
                        removedBricks[column] = _brickArr[column, row];
                    }

                    _shape = null;
                    for (int innerRow = row; innerRow > 0; innerRow--)
                    {
                        for (int innerColumn = 0; innerColumn < _w; innerColumn++)
                        {
                            _brickArr[innerColumn, innerRow] = _brickArr[innerColumn, innerRow - 1];
                            _brickArr[innerColumn, innerRow - 1] = null;
                        }
                    }

                    _score += 10 * _level;
                    if (_score > _hiScore)
                    {
                        _hiScore = _score;
                    }
                    _lines++;
                    _level = 1 + (_lines / 10);
                    _presenter.UpdateScoreView(_score, _hiScore, _lines, _level, _next);
                }
                else
                {
                    row--;
                }
            }

            if (_presenter != null)
            {
                _presenter.UpdateBoardView(GetShapeString(), _brickArr, _w, _h);
            }

            if (completed)
            {
                RemoveCompletedRows();
            }
            return completed;
        }

        public void ProcessNextMove()
        {
            if (_shape == null)
            {
                StartRandomShape();
            }

            bool couldMoveDown = true;

            if (!_shape.Anchored)
            {
                RemovePieceFromCurPosition(_shape);
                couldMoveDown = _shape.MoveDown();
            }
            else
            {
                bool full = !StartRandomShape();
                if (full)
                {
                    Init();
                    GameOver();
                    return;
                }
                else
                {
                    couldMoveDown = _shape.MoveDown();
                }
            }

            if (!couldMoveDown)
            {
                RemoveCompletedRows();
                DownPressed = false;
            }

            if (_presenter != null)
            {
                _presenter.UpdateBoardView(GetShapeString(), _brickArr, _w, _h);
            }
        }

        private void GameOver()
        {
            _level = 1;
            _lines = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            _presenter.UpdateBoardView(this._shapeStr, _brickArr, _w, _h);
            _presenter.GameOver();
        }

        public bool StartRandomShape()
        {
            if (_shape != null && !_shape.Anchored)
            {
                this.RemovePieceFromCurPosition(_shape);
            }

            _shape = _next;
            
            _next = GetRandomShape();
            _shape.ContainerBoard = this;
            int x = (this.Width - _shape.Width) / 2;

            bool ret = this.TestPieceOnPosition(_shape, x, 0);
            if (ret)
            {
                try
                {
                    this.PutPieceOnPosition(_shape, x, 0);
                }
                catch {}
            }
            return ret;
        }

        private IShape GetRandomShape()
        {
            IShape newShape = null;
            Random randomClass = new Random();
            int randomCode = randomClass.Next((int)ShapeKind.I, (int)ShapeKind.Z + 1);

            switch (randomCode)
            {
                case (int)ShapeKind.I:
                    newShape = new StickShape();
                    break;
                case (int)ShapeKind.J:
                    newShape = new JShape();
                    break;
                case (int)ShapeKind.L:
                    newShape = new LShape();
                    break;
                case (int)ShapeKind.O:
                    newShape = new OShape();
                    break;
                case (int)ShapeKind.S:
                    newShape = new SShape();
                    break;
                case (int)ShapeKind.T:
                    newShape = new TShape();
                    break;
                case (int)ShapeKind.Z:
                    newShape = new ZShape();
                    break;
            }

            ((BaseShape)newShape).Presenter = _presenter;

            _presenter.UpdateScoreView(_score, _hiScore, _lines, _level, newShape);
            return newShape;
        }

        public bool MoveLeft()
        {
            if (_shape == null)
            {
                return false;
            }
            else
            {
                return _shape.MoveLeft();
            }
        }

        public bool MoveRight()
        {
            if (_shape == null)
            {
                return false;
            }
            else
            {
                return _shape.MoveRight();
            }
        }

        public bool MoveDown()
        {
            if (_shape == null || DownPressed)
            {
                return false;
            }

            DownPressed = true;
            bool ret = _shape.MoveDown();
            if (_shape.Anchored)
            {
                DownPressed = false;
                RemoveCompletedRows();
            }
            return ret;
        }

        public bool Rotate90()
        {
            if (_shape == null)
            {
                return false;
            }
            else
            {
                return _shape.Rotate90();
            }
        }

        public bool Rotate270()
        {
            if (_shape == null)
            {
                return false;
            }
            else
            {
                return _shape.Rotate270();
            }
        }

        //public bool IsShapeAnchored() => _shape.Anchored;

        #endregion methods

        #region properties
        
        public int Score
        {
            get { return _score; }
        }

        private int HiScore
        {
            get { return _hiScore; }
        }

        public int Level
        {
            get { return _level; }
        }

        public bool DownPressed { get; private set; } = false;

        #endregion properties
    }
}
