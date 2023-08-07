using Metris.Classes;
using Metris.Classes.Game;
using Metris.Classes.Blocks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace Metris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage (new Uri("Classes/Assets/Tiles/TileEmpty.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/TileCyan.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/TileBlue.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/TileOrange.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/TileYellow.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/TileGreen.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/TilePurple.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/TileRed.png", UriKind.Relative)),
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-Empty.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-I.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-J.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-L.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-O.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-S.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-T.png", UriKind.Relative)),
            new BitmapImage (new Uri("Classes/Assets/Tiles/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;
        private GameState gameState = new GameState();
        private MediaPlayer mediaPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.PlayingField);
            mediaPlayer.Open(new Uri(string.Format("{0}\\tetris-background.mp3", AppDomain.CurrentDomain.BaseDirectory)));
            mediaPlayer.Play();
            
        }

        private Image[,] SetupGameCanvas(PlayingField field)
        {
            Image[,] imageControls = new Image[field.Rows, field.Columns];
            int cellSize = 25;
            
            for (int r = 0; r < field.Rows; r++)
            {
                for (int c = 0; c < field.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r -2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;

                }
            }

            return imageControls;
        }

        private void DrawGrid(PlayingField field)
        {
            for (int r = 0; r < field.Rows; r++)
            {
                for (int c = 0; c < field.Columns; c++)
                {
                    int id = field[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawNextBlock(GameState gameState)
        {
            BlockQueue blockQueue = gameState.BlockQueue;

            if (gameState.CurrentBlock == blockQueue.nextBlock)
            {
                while (gameState.CurrentBlock == blockQueue.nextBlock)
                {
                    blockQueue.GetAndUpdate();
                }
            }

            Block nextBlock = blockQueue.nextBlock;
            NextImage.Source = blockImages[nextBlock.Id];
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.PlayingField);
            DrawGhostBlock(gameState.CurrentBlock); 
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(gameState);
            while (!gameState.GameOver)
            {
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }    
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch(e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockACW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }
    }
}
