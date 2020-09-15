using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BlazorBricks.Core.Shapes;


namespace BlazorBricks.Core
{
    public abstract class BaseBricksArray : IBricksArray
    {
        #region attributes
        protected int _w = 0;
        protected int _h = 0;
        protected string _shapeStr = "";
        protected IBrick[,] _brickArr = null;
        protected IPresenter _presenter = null;
        #endregion attributes

        #region methods
        public string GetShapeString()
        {
            string ret = "";
            for (int row = 0; row < _h; row++)
            {
                for (int column = 0; column < _w; column++)
                {
                    if (_brickArr[column, row] != null)
                    {
                        //ret += ((int)(shapeArray[column, row].Color)).ToString();
                        ret += "1";
                    }
                    else
                    {
                        ret += "0";
                    }
                }
            }
            return ret;
        }

        public virtual void Init()
        {
            _brickArr = new IBrick[_w, _h];
            for (int row = 0; row < _h; row++)
            {
                for (int column = 0; column < _w; column++)
                {
                    _brickArr[column, row] = null;
                }
            }
        }

        private string Replicate(string s, int n)
        {
            string ret = "";
            for (int i = 0; i < n; i++)
            {
                ret += s;
            }
            return ret;
        }

        public int Width
        {
            get { return _w; }
        }

        public int Height
        {
            get { return _h; }
        }

        public string ShapeString
        {
            get { return GetShapeString(); }
        }

        public IBrick[,] BrickArr
        {
            get { return _brickArr; }
        }

        public IPresenter Presenter
        {
            get { return _presenter; }
            set { _presenter = value; }
        }
        #endregion methods
    }
}
