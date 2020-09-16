using System;
using System.Collections.Generic;
using System.Text;
using BlazorBricks.Core.Shapes;

namespace BlazorBricks.Core
{
  public class Brick : IBrick
  {
	private double _x = 0;
	private double _y = 0;
	private double _left = 0;
	private double _top = 0;
	private double _marginLeft = 4;
	private double _marginTop = 4;
	private int _brickSize = 20;
	private ShapeKind _kind = ShapeKind.I;

	public Brick(double x, double y, ShapeKind color)
	{
	  this._x = x;
	  this._y = y;
	  this._kind = color;
	}

	public double X
	{
	  get { return _x; }
	  set
	  {
		_x = value;
		Left = _x * _brickSize + _marginLeft;
	  }
	}

	public double Y
	{
	  get { return _y; }
	  set
	  {
		_y = value;
		Top = _y * _brickSize + _marginTop;
	  }
	}

	public ShapeKind Kind
	{
	  get
	  {
		return _kind;
	  }
	  set
	  {
		_kind = value;
	  }
	}

	public double Left
	{
	  get { return _left; }
	  set
	  {
		_left = value;
	  }
	}

	public double Top
	{
	  get { return _top; }
	  set
	  {
		_top = value;
	  }
	}

  }
}
