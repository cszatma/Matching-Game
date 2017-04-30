using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace MatchingGameAssignment
{
    using R = Properties.Resources;
    using Settings = Properties.Settings;
    public partial class GameForm : Form
    {
        Game game;
        GameDifficulty selectedDifficulty;
        GameModes selectedMode;
        Bitmap[] pics;
        PictureBox SelectedImage1; //Stores first card flipped
        PictureBox SelectedImage2; //Stores second card flipped
        SoundPlayer correctMatch = new SoundPlayer(Resources.Resource1.DingSound);
        SoundPlayer incorrectMatch = new SoundPlayer(Resources.Resource1.WrongBuzzerSound);
        List<Point> points = new List<Point>();
        Random picLocation = new Random();

        public GameForm()
        {
            InitializeComponent();
        }

        #region Main Menu
        private void lblExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblNewGame_Click(object sender, EventArgs e)
        {
            panelChooseGame.Visible = true;
        }

        private void lblSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = true;
        }
        #endregion

        private void lblBack_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
        }

        private void lblAudio_Click(object sender, EventArgs e)
        {
            
            if (Settings.Default.Audio == true)
            {
                lblAudio.ForeColor = Color.Red;
                lblAudio.Text = "Enable Audio";
            }
            else if (Settings.Default.Audio == false)
            {
                lblAudio.ForeColor = Color.LimeGreen;
                lblAudio.Text = "Disable Audio";
            }
            Settings.Default.Audio = !Settings.Default.Audio;
            Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            panelSettings.Dock = DockStyle.Fill;
            panelChooseGame.Dock = DockStyle.Fill;
            panelPaused.Dock = DockStyle.Fill;
            panelGameWin.Dock = DockStyle.Fill;
            panelPlay.Dock = DockStyle.Fill;

            lblAudio.ForeColor = Settings.Default.Audio == true ? Color.LimeGreen : Color.Red;
            lblAudio.Text = Settings.Default.Audio == true ? "Disable Audio" : "Enable Audio";

            lblSpace.ForeColor = Settings.Default.Design == 0 ? Color.LimeGreen : Color.Red;
            lblAnimals.ForeColor = Settings.Default.Design == 1 ? Color.LimeGreen : Color.Red;
            lblFruits.ForeColor = Settings.Default.Design == 2 ? Color.LimeGreen : Color.Red;

            setUpEvents(this);
            setUpEvents(cardsHolder);
            setUpEvents(cardsHolderHard);
            setUpEvents(panelSettings);
            setUpEvents(panelChooseGame);
            setUpEvents(panelConfirmQuit);
            setUpEvents(panelPaused);
            setUpEvents(panelGameWin);
            setUpEvents(panelPlay);
            FillImageArray();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
                this.Close();
        }

        private void startGame()
        {
            game = new Game(selectedMode, selectedDifficulty);

            //Resets score, attempts and labels to 0
            if (game.GameMode == GameModes.Singleplayer)
            {
                panelSPScore.Visible = true;
                panel2PScore.Visible = false;
                lblSPScoreNum.Text = "0";
                lblSPAttemptsNum.Text = "0";
            }
            else
            {
                panel2PScore.Visible = true;
                panelSPScore.Visible = false;
                lblP1ScoreNum.Text = "0";
                lblP1AttemptsNum.Text = "0";
                lblP2ScoreNum.Text = "0";
                lblP2AttemptsNum.Text = "0";
            }

            lblTimeCount.Text = "0:00";

            //Adds the background image to each picture in easy mode
            var container = game.Difficulty == GameDifficulty.Easy ? cardsHolder : cardsHolderHard;
            setupPictureBoxes(container);

            SelectedImage1 = null;
            SelectedImage2 = null;

            panelPlay.Visible = true;
            timerGameTime.Start();
        }

        private void setupPictureBoxes(Panel container)
        {
            foreach (PictureBox picture in container.Controls)
            {
                picture.Visible = true;
                picture.Image = R.HBackgroundCard;
                points.Add(picture.Location);
            }

            //Puts each picturebox in a random location in easy mode
            foreach (PictureBox picture in container.Controls)
            {
                var next = picLocation.Next(points.Count);
                Point p = points[next];
                picture.Location = p;
                points.Remove(p);
                picture.Enabled = true;
            }
        }

        private void handleWin()
        {

            //Single player win events
            if (game.GameMode == GameModes.Singleplayer)
            {
                panelSPWin.Visible = true;
                panel2PWin.Visible = false;
                lblSPWinTime.Text = game.Time.ToString();
                lblSPWinScore.Text = game.Player1.Score.ToString();
                lblSPWinAttempts.Text = game.Player1.Attempts.ToString();
            }

            //2 player win events
            else
            {
                panel2PWin.Visible = true;
                panelSPWin.Visible = false;
                lbl2PWinTime.Text = game.Time.ToString();
                lblP1WinScoreAmt.Text = game.Player1.Score.ToString();
                lblP1WinAttpsAmt.Text = game.Player1.Attempts.ToString();
                lblP2WinScoreAmt.Text = game.Player2.Score.ToString();
                lblP2WinAttpsAmt.Text = game.Player2.Attempts.ToString();
                lblWinner.ForeColor = (game.Player1.Score == game.Player2.Score) ? Color.Blue : Color.LimeGreen;
                lblWinner.Location = (game.Player1.Score == game.Player2.Score) ? new Point(258, 22) : new Point(203, 27);

                Color p1Color = Color.Blue;
                Color p2Color = Color.Blue;
                lblWinner.Text = "It's a Tie!";

                if (game.Player1.Score > game.Player2.Score)
                {
                    //Make all P1 labels green if P1 wins
                    lblWinner.Text = "Player 1 Wins!";
                    p1Color = Color.LimeGreen;
                    p2Color = Color.Red;
                }
                else if (game.Player1.Score < game.Player2.Score)
                {
                    //Make all P2 labels green if P2 wins
                    lblWinner.Text = "Player 2 Wins!";
                    p1Color = Color.Red;
                    p2Color = Color.LimeGreen;
                }

                lblP1Win.ForeColor = p1Color;
                lblP1WinScore.ForeColor = p1Color;
                lblP1WinScoreAmt.ForeColor = p1Color;
                lblP1WinAttempts.ForeColor = p1Color;
                lblP1WinAttpsAmt.ForeColor = p1Color;
                
                lblP2Win.ForeColor = p2Color;
                lblP2WinScore.ForeColor = p2Color;
                lblP2WinScoreAmt.ForeColor = p2Color;
                lblP2WinAttempts.ForeColor = p2Color;
                lblP2WinAttpsAmt.ForeColor = p2Color;

            }
            
        }

        //Compare pictures
        private async void timerFlipCards_Tick(object sender, EventArgs e)
        {
            timerFlipCards.Stop();
            //Executed if images are the same
            if (SelectedImage1.Tag == SelectedImage2.Tag)
            {
                game.CardsMatched++;
                if (Settings.Default.Audio == true)
                {
                    correctMatch.Play();
                }
                SelectedImage1.Visible = false;
                SelectedImage2.Visible = false;

                game.IncrementStats(true);

                if (game.GameMode == GameModes.Singleplayer)
                {
                    lblSPScoreNum.Text = game.Player1.Score.ToString();
                    lblSPAttemptsNum.Text = game.Player1.Attempts.ToString();
                }

                else
                {
                    if (game.CurrentPlayer == game.Player1)
                    {
                        lblP1ScoreNum.Text = game.Player1.Score.ToString();
                        lblP1AttemptsNum.Text = game.Player1.Attempts.ToString();
                    }
                    else
                    {
                        lblP2ScoreNum.Text = game.Player2.Score.ToString();
                        lblP2AttemptsNum.Text = game.Player2.Attempts.ToString();
                    }
                }
             }
                
             //Executes if images are different
             else
             {
                if (Settings.Default.Audio == true)
                {
                    incorrectMatch.Play();
                }
                SelectedImage1.Image = R.EBackgroundCard;
                SelectedImage2.Image = R.EBackgroundCard;

                game.IncrementStats(false);

                if (game.GameMode == GameModes.Singleplayer)
                {
                    lblSPAttemptsNum.Text = game.Player1.Attempts.ToString();
                }
                else
                {
                    if (game.CurrentPlayer == game.Player1)
                    {
                        lblP1AttemptsNum.Text = game.Player1.Attempts.ToString();
                        lblP1.ForeColor = Color.Red;
                        lblP2.ForeColor = Color.LimeGreen;
                    }
                    else
                    {
                        lblP2AttemptsNum.Text = game.Player2.Attempts.ToString();
                        lblP1.ForeColor = Color.LimeGreen;
                        lblP2.ForeColor = Color.Red;
                    }
                }
             }
             
             SelectedImage1.Enabled = true;
             SelectedImage2.Enabled = true;
             SelectedImage1 = null;
             SelectedImage2 = null;

            //Determines if player has won
            if ((game.Difficulty == GameDifficulty.Easy && game.CardsMatched == 8) || (game.Difficulty == GameDifficulty.Hard && game.CardsMatched == 18))
            {
                timerGameTime.Stop();
                await Task.Delay(1000);
                panelGameWin.Visible = true;
                panelPlay.Visible = false;
                handleWin();
            }

            switch (game.Difficulty)
            {
                case GameDifficulty.Easy:
                    cardsHolder.Enabled = true;
                    break;
                case GameDifficulty.Hard:
                    cardsHolderHard.Enabled = true;
                    break;
            }
        }

        //Used to count the length of the game
        private void timerGameTime_Tick(object sender, EventArgs e)
        {
            game.Time.IncrementTime();
            if (game.Difficulty == GameDifficulty.Easy)
            {
                lblTimeCount.Text = game.Time.ToString();
            }
            else
            {
                lblTimeCount.Text = game.Time.ToString();
            }
        }

        #region PauseButton & QuitButton
        private void lblPause_Click(object sender, EventArgs e)
        {
            if (panelConfirmQuit.Visible != true)
            {
                timerGameTime.Stop();
                panelPlay.Visible = false;
                panelPaused.Visible = true;
            }
        }

        private void lblQuit_Click(object sender, EventArgs e)
        {
            panelConfirmQuit.Visible = true;
            panelConfirmQuit.BringToFront();
            panelPlay.Enabled = false;
            panelConfirmQuit.Location = new Point(
                    this.ClientSize.Width / 2 - panelConfirmQuit.Size.Width / 2,
                    this.ClientSize.Height / 2 - panelConfirmQuit.Size.Height / 2);
            panelConfirmQuit.Anchor = AnchorStyles.None;
            timerGameTime.Stop();
        }
        #endregion

        #region PauseMenu
        private void lblResume_Click(object sender, EventArgs e)
        {
            panelPaused.Visible = false;
            panelPlay.Visible = true;
            timerGameTime.Start();
        }

        private void lblRestart_Click(object sender, EventArgs e)
        {
            panelPaused.Visible = false;
            startGame();
        }

        private void lblMainMenu_Click(object sender, EventArgs e)
        {
            panelPaused.Visible = false;
        }
        #endregion

        #region WinMenu
        private void lblWinPlayAgain_Click(object sender, EventArgs e)
        {
            panelGameWin.Visible = false;
            startGame();
        }

        private void lblWinMainMenu_Click(object sender, EventArgs e)
        {
            panelGameWin.Visible = false;
        }
        #endregion

        #region QuitMenu
        private void lblQuitYes_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblQuitNo_Click(object sender, EventArgs e)
        {
            panelConfirmQuit.Visible = false;
            if (game.Difficulty == GameDifficulty.Easy)
            {
                cardsHolder.Enabled = true;
            }
            else if (game.Difficulty == GameDifficulty.Hard)
            {
                cardsHolderHard.Enabled = true;
            }
            timerGameTime.Start();
        }
        #endregion

        /// <summary>
        /// Adds events to controls.
        /// </summary>
        /// <param name="container">Container with controls to gain events.</param>
        private void setUpEvents(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (container == panelSettings && c != lblBack)
                {
                    c.Click += HandleSettingsClicks;
                }
                else if (c is Label)
                {
                    c.MouseEnter += HandleMouseEvents;
                    c.MouseLeave += HandleMouseEvents;
                }
                else if (c is PictureBox)
                {
                    c.Click += HandleImageClicks;
                }
            }

            lblEasy.Click += HandleSetDifficulty;
            lblHard.Click += HandleSetDifficulty;
            lblSinglePlayer.Click += HandleSetMode;
            lbl2Players.Click += HandleSetMode;
        }

        /// <summary>
        /// Sets chosen difficulty.
        /// </summary>
        private void HandleSetDifficulty(object sender, EventArgs e)
        {
            selectedDifficulty = sender == lblEasy ? GameDifficulty.Easy : GameDifficulty.Hard;
            lblEasy.Visible = false;
            lblHard.Visible = false;
            lblSinglePlayer.Visible = true;
            lbl2Players.Visible = true;
        }

        /// <summary>
        /// Sets chosen gamemode.
        /// </summary>
        private void HandleSetMode(object sender, EventArgs e)
        {
            selectedMode = sender == lblSinglePlayer ? GameModes.Singleplayer : GameModes.Multiplayer;
            panelChooseGame.Visible = false;
            startGame();
        }

        /// <summary>
        /// Events for changing the color of labels when hovered over.
        /// </summary>
        private void HandleMouseEvents(object sender, EventArgs e)
        {
            var lbl = (Label)sender;
            if ((string)lbl.Tag != "nohighlight")
            {
                lbl.ForeColor = lbl.ForeColor == Color.Red ? Color.LimeGreen : Color.Red;
            }
        }

        /// <summary>
        /// Handles the pictureboxes being clicked.
        /// </summary>
        private void HandleImageClicks(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            pb.Image = pics[Convert.ToInt32(pb.Tag)];
            if (SelectedImage1 == null)
            {
                SelectedImage1 = pb;
                pb.Enabled = false;
                return;
            }
            else if (SelectedImage2 == null)
            {
                SelectedImage2 = pb;
                pb.Enabled = false;
            }

            if (game.Difficulty == GameDifficulty.Easy)
            {
                cardsHolder.Enabled = false;
            }
            else
            {
                cardsHolderHard.Enabled = false;
            }
            timerFlipCards.Start();
        }

        /// <summary>
        /// Event Handler for changing the game settings.
        /// </summary>
        private void HandleSettingsClicks(object sender, EventArgs e)
        {
            if (sender == lblSpace)
            {
                Settings.Default.Design = 0;
            }
            else if (sender == lblAnimals)
            {
                Settings.Default.Design = 1;
            }
            else if (sender == lblFruits)
            {
                Settings.Default.Design = 2;
            }

            Settings.Default.Save();
            lblSpace.ForeColor = Settings.Default.Design == 0 ? Color.LimeGreen : Color.Red;
            lblAnimals.ForeColor = Settings.Default.Design == 1 ? Color.LimeGreen : Color.Red;
            lblFruits.ForeColor = Settings.Default.Design == 2 ? Color.LimeGreen : Color.Red;
        }

        private void FillImageArray()
        {
            if (Settings.Default.Design == 0)
            {
                pics = new Bitmap[] { R.HBackgroundCard, R.Hcard1Space, R.Hcard2Space, R.Hcard3Space, R.Hcard4Space, R.Hcard5Space, R.Hcard6Space, R.Hcard7Space,
                    R.Hcard8Space, R.Hcard9Space, R.Hcard10Space, R.Hcard11Space, R.Hcard12Space, R.Hcard13Space, R.Hcard14Space, R.Hcard15Space, R.Hcard16Space,
                    R.Hcard17Space, R.Hcard18Space };
            }
            else if (Settings.Default.Design == 1)
            {
                pics = new Bitmap[] { R.HBackgroundCard, R.Hcard1Animal, R.Hcard2Animal, R.Hcard3Animal, R.Hcard4Animal, R.Hcard5Animal, R.Hcard6Animal, R.Hcard7Animal,
                    R.Hcard8Animal, R.Hcard9Animal, R.Hcard10Animal, R.Hcard11Animal, R.Hcard12Animal, R.Hcard13Animal, R.Hcard14Animal, R.Hcard15Animal, R.Hcard16Animal,
                    R.Hcard17Animal, R.Hcard18Animal };
            }
            else if (Settings.Default.Design == 2)
            {
                pics = new Bitmap[] { R.HBackgroundCard, R.Hcard1Fruit, R.Hcard2Fruit, R.Hcard3Fruit, R.Hcard4Fruit, R.Hcard5Fruit, R.Hcard6Fruit, R.Hcard7Fruit,
                    R.Hcard8Fruit, R.Hcard9Fruit, R.Hcard10Fruit, R.Hcard11Fruit, R.Hcard12Fruit, R.Hcard13Fruit, R.Hcard14Fruit, R.Hcard15Fruit, R.Hcard16Fruit,
                    R.Hcard17Fruit, R.Hcard18Fruit };
            }
        }
    }
}
