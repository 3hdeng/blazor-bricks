using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazorBricks.Core.Shapes;

namespace BlazorBricks.Core
{
    public class BricksPresenter : IPresenter
    {
        public event EventHandler Updated;

        private IView view;
        private BricksBoard BricksBoard;
        private TimeSpan accumulatedTimeSpan = TimeSpan.FromMilliseconds(0);
        private const int TICK_MS_INTERVAL = 25;
        private const int PROCESS_NEXT_MOVEMENT_MS_INTERVAL = 300;
        
        public BricksPresenter(IView view)
        {
            this.view = view;
            BricksBoard = new BricksBoard(this);
            BricksBoard.Updated += (obj, e) =>
            {
                Updated?.Invoke(this, e);
            };

            TickLoop();
        }

        private async Task TickLoop()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TICK_MS_INTERVAL);
                    bool processMove = BricksBoard.DownPressed;
                    accumulatedTimeSpan = accumulatedTimeSpan.Add(TimeSpan.FromMilliseconds(TICK_MS_INTERVAL));
                    if (accumulatedTimeSpan.TotalMilliseconds >= PROCESS_NEXT_MOVEMENT_MS_INTERVAL)
                    {
                        processMove = true;
                        accumulatedTimeSpan = TimeSpan.FromMilliseconds(0);
                    }
                    Tick(processMove);
                }
            });
        }

        public IView View
        {
            get { return view; }
            set { view = value; }
        }

        public void UpdateBoardView(string ArrayString, IBrick[,] brickArray, int width, int height)
        {
            if (view == null)
                throw new ArgumentNullException("View");

            view.DisplayBoard(ArrayString, brickArray, width, height);
        }

        public void HighlightCompletedRow(int row)
        {
            if (view == null)
                throw new ArgumentNullException("View");

            view.HighlightCompletedRow(row);
        }

        public void UpdateScoreView(int score, int hiScore, int lines, int level, IShape next)
        {
            if (view == null)
                throw new ArgumentNullException("View");

            view.DisplayScore(score, hiScore, lines, level, next);
        }

        public bool MoveLeft()
        {
            return BricksBoard.MoveLeft();
        }

        public bool MoveRight()
        {
            return BricksBoard.MoveRight();
        }
        
        public bool MoveDown()
        {
            return BricksBoard.MoveDown();
        }

        public bool Rotate90()
        {
            return BricksBoard.Rotate90();
        }

        public bool Rotate270()
        {
            return BricksBoard.Rotate270();
        }

        public void InitializeBoard()
        {
            BricksBoard.InitializeArray();
        }

        public void GameOver()
        {
            view.GameOver();
        }

        public void Tick(bool processMove = false)
        {
            if (processMove)
            {
                BricksBoard.ProcessNextMove();
            }
            Updated?.Invoke(this, new EventArgs());
        }

        public int Width
        {
            get { return BricksBoard.Width; }
        }

        public int Height
        {
            get { return BricksBoard.Height; }
        }

        public int Level
        {
            get { return BricksBoard.Level; }
        }

        public bool IsPlaying
        {
            get { return BricksBoard.IsPlaying; }
        }

    }

    public enum ShapeCodes
    {
        I = 1,
        J,
        L,
        O,
        S,
        T,
        Z
    }
}
