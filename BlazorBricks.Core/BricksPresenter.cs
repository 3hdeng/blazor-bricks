using System;
using System.Threading;
using System.Threading.Tasks;
using BlazorBricks.Core.Shapes;

namespace BlazorBricks.Core
{
    public class BricksPresenter : IPresenter
    {
        public event EventHandler Updated;

        private IView _view;
        private BricksBoard _bricksBoard;
        private TimeSpan _accTimeSpan = TimeSpan.FromMilliseconds(0);
        private CancellationTokenSource _cancelTokenSource;
        private const int TICK_MS_INTERVAL = 30;
        private const int PROCESS_NEXT_MOVEMENT_MS_INTERVAL = 150;
        private Object _thisLock = new Object();

        public bool IsGameOver { get; set; } = true;

        public BricksPresenter(IView view)
        {
            this._view = view;
            _bricksBoard = new BricksBoard(this);
            _bricksBoard.Updated += (obj, e) =>
            {
                Updated?.Invoke(this, e);
            };
        }

        public async Task StartTickLoop()
        {
            IsGameOver = false;
            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;
            Task task = Task.Run(async () =>
               {
                   while (!IsGameOver)
                   {
                       bool processMove = await ExecuteTickLoop();
                       Tick(processMove);
                   }
               }, token);

            await task;
        }

        private async Task<bool> ExecuteTickLoop()
        {
            await Task.Delay(TICK_MS_INTERVAL);
            bool processMove = false;

            lock (_thisLock)
            {
                processMove = _bricksBoard.DownPressed;
                _accTimeSpan = _accTimeSpan.Add(TimeSpan.FromMilliseconds(TICK_MS_INTERVAL));
                if (_accTimeSpan.TotalMilliseconds >= PROCESS_NEXT_MOVEMENT_MS_INTERVAL)
                {
                    processMove = true;
                    _accTimeSpan = TimeSpan.FromMilliseconds(0);
                }
            }
            return processMove;
        }

        public IView View
        {
            get { return _view; }
            set { _view = value; }
        }

        public void UpdateBoardView(string ArrayString, IBrick[,] brickArray, int width, int height)
        {
            if (_view == null)
                throw new ArgumentNullException("View");

            _view.DisplayBoard(ArrayString, brickArray, width, height);
        }

        public void HighlightCompletedRow(int row)
        {
            if (_view == null)
                throw new ArgumentNullException("View");
        }

        public void UpdateScoreView(int score, int hiScore, int lines, int level, IShape next)
        {
            if (_view == null)
                throw new ArgumentNullException("View");

            _view.DisplayScore(score, hiScore, lines, level, next);
        }

        public bool MoveLeft()
        {
            if (IsGameOver) return false;
            return _bricksBoard.MoveLeft();
        }

        public bool MoveRight()
        {
            if (IsGameOver) return false;
            return _bricksBoard.MoveRight();
        }
        
        public bool MoveDown()
        {
            if (IsGameOver) return false;
            return _bricksBoard.MoveDown();
        }

        public bool Rotate90()
        {
            if (IsGameOver) return false;
            return _bricksBoard.Rotate90();
        }

        public bool Rotate270()
        {
            if (IsGameOver) return false;
            return _bricksBoard.Rotate270();
        }

        public void InitializeBoard()
        {
            _bricksBoard.Init();
        }

        public void GameOver()
        {
            _cancelTokenSource.Cancel();
            _view.GameOver();
        }

        public void Tick(bool processMove = false)
        {
            if (processMove)
            {
                _bricksBoard.ProcessNextMove();
            }
            Updated?.Invoke(this, new EventArgs());
        }

        public int Width
        {
            get { return _bricksBoard.Width; }
        }

        public int Height
        {
            get { return _bricksBoard.Height; }
        }

        public int Level
        {
            get { return _bricksBoard.Level; }
        }
    }
}
