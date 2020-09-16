using System.Collections.Generic;

namespace BlazorBricks.Core
{
    public class GameManager : IView
    {
        private static GameManager _instance = null;
        private static BricksPresenter _presenter = null;
        private static BoardViewModel _currentBoard = null;

        private GameManager()
        {
            _currentBoard = new BoardViewModel();
            _currentBoard.Bricks = new BrickViewModel[] { };

            _presenter = new BricksPresenter(this);
        }

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        public BricksPresenter Presenter
        {
            get { return _presenter; }
        }

        public BoardViewModel CurrentBoard
        {
            get { return _currentBoard; }
        }

        public void InitializeBoard()
        {
            //presenter.InitializeBoard();
            //presenter.IsGameOver = false;
        }

        public void DisplayBoard(string arrayString, IBrick[,] brickArray, int width, int height)
        {
            _currentBoard.Bricks = GetBricksArray(height, width, brickArray);
        }

        public void DisplayScore(int score, int hiScore, int lines, int level, BlazorBricks.Core.Shapes.IShape next)
        {
            _currentBoard.Score = score;
            _currentBoard.HiScore = hiScore;
            _currentBoard.Lines = lines;
            _currentBoard.Level = level;
            _currentBoard.Next = GetBricksArray(next.BrickArr.GetUpperBound(1) + 1, next.BrickArr.GetUpperBound(0) + 1, next.BrickArr);
        }

/// 2D IBrick arr -> 1D BrickView arr
        private BrickViewModel[] GetBricksArray(int rowCount, int colCount, IBrick[,] array)
        {
            var bricksList = new List<BrickViewModel>();

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var b = array[col, row];
                    if (b != null)
                    {
                        bricksList.Add(new BrickViewModel()
                        {
                            Row = row,
                            Col = col,
                            Kind = b.Kind.ToString()
                        });
                    }
                    else
                    {
                        bricksList.Add(new BrickViewModel()
                        {
                            Row = row,
                            Col = col,
                            Kind = "0"
                        });
                    }
                }
            }
            return bricksList.ToArray();
        }

        public void GameOver()
        {
            _presenter.IsGameOver = true;
        }
    }

    public class BrickViewModel
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Kind { get; set; }
    }

    public class BoardViewModel
    {
        public BoardViewModel()
        {
        }

        public BrickViewModel[] Bricks { get; set; }
        public int Score { get; set; }
        public int HiScore { get; set; }
        public int Lines { get; set; }
        public int Level { get; set; }
        public BrickViewModel[] Next { get; set; }
    }
}
