using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using BlasmBricks;

namespace BlasmBricks.Pages
{
  public class IndexBase: ComponentBase
  {
    protected BlazorBricks.Core.BoardViewModel boardViewModel;

	protected override async Task OnInitializedAsync()
	{
	  OnKeyUp.Action = async value =>
	  {
		ConsoleKey consoleKey = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), value);

		var presenter = BlazorBricks.Core.GameManager.Instance.Presenter;

		switch (consoleKey)
		{
		  case ConsoleKey.LeftArrow:
			presenter.MoveLeft();
			break;
		  case ConsoleKey.RightArrow:
			presenter.MoveRight();
			break;
		  case ConsoleKey.UpArrow:
			presenter.Rotate90();
			break;
		  case ConsoleKey.DownArrow:
			presenter.MoveDown();
			break;
		  default:
			break;
		}
		this.StateHasChanged();
	  };

	  boardViewModel = BlazorBricks.Core.GameManager.Instance.CurrentBoard;

	  Object thisLock = new Object();
	  BlazorBricks.Core.GameManager.Instance.Presenter.Updated
		+= (obj, e) =>
		{
		  lock (thisLock)
		  {
			boardViewModel = BlazorBricks.Core.GameManager.Instance.CurrentBoard;
			this.StateHasChanged();
		  };
		};

	  InitializeBoard();
	}

	void InitializeBoard()
	{
	  BlazorBricks.Core.GameManager.Instance.Presenter.InitializeBoard();
	  boardViewModel = BlazorBricks.Core.GameManager.Instance.CurrentBoard;

	  this.StateHasChanged();
	}

	public void StartTickLoop()
	{
	  BlazorBricks.Core.GameManager.Instance.Presenter.StartTickLoop();
	}
  }
}
