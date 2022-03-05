using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Contra.Map;
using Contra.utils;

namespace Contra
{
    public partial class GameWindows : Form
    {
        #region Game Properties
        
        //Variables de controles booléennes
        public bool IsGameActive;
        public bool IsGamePaused;
        public bool IsGameResumed;
        public bool IsGameOver;

        public bool IsMainMenuPageActive;
        public bool IsGamePageActive;
        public bool IsScoresPageActive;

        public bool IsSettingsPageActive;
        public bool selectingCommands;
        public string command;
        public bool IsPopUpActive;

        public bool ButtonStartIsFocused;
        public bool ButtonScoresIsFocused;
        public bool ButtonSettingsIsFocused;
        public bool ButtonExitIsFocused;

        public bool CouldPlayBackgroundMusic;
        public bool CouldPlayHoverButtonMusic;
        public bool CouldPlayMusic;

        //Liste des scores
        private List<int> scores;

        //Settings
        public Settings settings { get; set; }
        private Settings tempSettings;

        //Joueurs
        internal Player player;

        //Map
        //public GameMap gameMap;

        #endregion

        #region Constructors
        public GameWindows()
        {
            Thread.CurrentThread.Name = "Windows Thread";

            InitializeComponent();
            init();

            showMainMenuPage();

            resizeItem();
        }
        #endregion

        #region Init Variables
        private void init()
        {
            //Settings
            settings = new Settings();
            tempSettings = new Settings();

            //Init booleans
            IsGameActive = IsGamePaused = IsGameResumed = IsGameOver = false;
            IsMainMenuPageActive = true;
            IsGamePageActive = IsScoresPageActive = IsSettingsPageActive = false;
            selectingCommands = false;

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = false;

            CouldPlayBackgroundMusic = true;
            CouldPlayHoverButtonMusic = true;
            CouldPlayMusic = true;

            //Init scores list
            scores = new List<int>();

        }

        #endregion

        #region On Windows size changed
        public void resizeItem()
        {
            soundBackground.Size = new Size(0,0);
            soundBackground.SendToBack();

            soundHover.Size = new Size(0, 0);
            soundHover.SendToBack();

            //Ajust TabControl
            GamePages.Left = -24;
            GamePages.Top = -5;
            GamePages.Height = Height + 10;
            GamePages.Width = Width + 30;

            //Ajust TabLayoutPanel
            if (mainMenuPage.Contains(tableLayoutPanel))
            {
                mainMenuPage.Controls.Remove(tableLayoutPanel);
                backgroundPictureMainMenu.Controls.Add(tableLayoutPanel);
            }

            backgroundPictureMainMenu.Width = tableLayoutPanel.Width = GamePages.Width;
            backgroundPictureMainMenu.Height = tableLayoutPanel.Height = GamePages.Height;
            backgroundPictureMainMenu.Left = tableLayoutPanel.Left = 0;
            backgroundPictureMainMenu.Top = tableLayoutPanel.Top = 0;


            //Scores Pages
            backgroundPictureScoresMenu.Width = GamePages.Width;
            backgroundPictureScoresMenu.Height = GamePages.Height;
            backgroundPictureScoresMenu.Left = 0;
            backgroundPictureScoresMenu.Top = 0;

            //Settings pages
            backgroundPictureSettingsMenu.Width = GamePages.Width;
            backgroundPictureSettingsMenu.Height = GamePages.Height;
            backgroundPictureSettingsMenu.Left = 0;
            backgroundPictureSettingsMenu.Top = 0;

            //Settings Box
            settingsBox.Width = groupBoxAudio.Width * 2 + 40;
            settingsBox.Height = Height - 100;
            settingsBox.Location = new Point(GamePages.Width / 2 - settingsBox.Width / 2, 10 + buttonBack.Height);

            settingsTitle.Left = settingsBox.Width/2 - settingsTitle.Width/2;
            settingsTitle.Top = 5;

            groupBoxAccount.Top = settingsTitle.Height + settingsTitle.Top;
            groupBoxAccount.Left = settingsBox.Width/2 - groupBoxAccount.Width / 2;

            groupBoxAudio.Top = settingsTitle.Height + groupBoxAccount.Height + 20;
            groupBoxAudio.Left = 10;

            groupBoxCommands.Top = groupBoxAudio.Top;
            groupBoxCommands.Left = 5 + groupBoxAudio.Left + groupBoxAudio.Width; ;


            //settingsTitle.Location = new Point(settingsBox.Width / 2 - settingsTitle.Width / 2, settingsTitle.Location.Y - 45);
            //groupBoxAccount.Location = new Point(settingsBox.Width / 2 - groupBoxAccount.Width / 2, groupBoxAccount.Location.Y - 47);

            //groupBoxAudio.Location = new Point(groupBoxAccount.Location.X - (groupBoxAccount.Width / 2) - 5, groupBoxAudio.Location.Y - 52);
            //groupBoxCommands.Location = new Point(groupBoxAudio.Location.X + (groupBoxAudio.Width / 2) + 5, groupBoxCommands.Location.Y - 52);

            settingsBox.BringToFront();
            settingsTitle.BringToFront();
            groupBoxAudio.BringToFront();
            groupBoxAccount.BringToFront();
            groupBoxCommands.BringToFront();

            buttonReset.Location = new Point( settingsBox.Width / 2 + settingsBox.Left - buttonReset.Width / 2, buttonReset.Location.Y );
            buttonSave.Left = buttonReset.Left - buttonSave.Width - 10;
            buttonCancel.Left = buttonReset.Left + buttonReset.Width + 10;

            buttonReset.Top = buttonSave.Top = buttonCancel.Top = settingsBox.Height - buttonSave.Height + 10;

            //Map
            gameMap.Width = GamePages.Width;
            gameMap.Height = GamePages.Height;
            gameMap.Left = 0;
            gameMap.Top = 0;

            //Score Location , Life Bar Location and player name
            gameMap.scoreLabel.Left = 10;
            gameMap.scoreLabel.Top = 10;

            gameMap.lifeBar.Left = Width - gameMap.lifeBar.Width - 30;
            gameMap.lifeBar.Top = 10;

            gameMap.heartLifeIcon.Left = Width - gameMap.heartLifeIcon.Width - gameMap.lifeBar.Width - 35;
            gameMap.heartLifeIcon.Top = 10;

            gameMap.playerName.Left = Width - gameMap.playerName.Width - gameMap.heartLifeIcon.Width - gameMap.lifeBar.Width - 15;
            gameMap.playerName.Top = 10;

            gameMap.resizeMap();

        }
        public void resizeMap()
        {
            //Map
            gameMap.Width = GamePages.Width;
            gameMap.Height = GamePages.Height;
            gameMap.Left = 0;
            gameMap.Top = 0;

            //Score Location , Life Bar Location and player name
            gameMap.scoreLabel.Left = 10;
            gameMap.scoreLabel.Top = 10;

            gameMap.lifeBar.Left = Width - gameMap.lifeBar.Width - 30;
            gameMap.lifeBar.Top = 10;

            gameMap.heartLifeIcon.Left = Width - gameMap.heartLifeIcon.Width - gameMap.lifeBar.Width - 35;
            gameMap.heartLifeIcon.Top = 10;

            gameMap.playerName.Left = Width - gameMap.playerName.Width - gameMap.heartLifeIcon.Width - gameMap.lifeBar.Width - 15;
            gameMap.playerName.Top = 10;

            gameMap.resizeMap();
        }
        private void handleSizeChanged(object sender, EventArgs e)
        {
            resizeItem();
        }
        #endregion

        #region Click Main Menu Handlers
        private void OnButtonStartGameClicked(object sender, EventArgs e)
        {
            showGamePage();
            StartGame();
        }
        private void OnButtonScoresClicked(object sender, EventArgs e)
        {
            showScoresPage();
        }
        private void OnButtonSettingsClicked(object sender, EventArgs e)
        {
            showSettingsPage();
        }
        private void OnButtonExitClicked(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (player != null)
                    player.ThreadPlayerMouvement.Resume();
            }
            catch (Exception) { }
            finally
            {

                if (player != null)
                {
                    player.end = true;
                }
            }

            try
            {
                gameMap.ThreadMovingMap.Resume();
            }
            catch (Exception) { }
            finally
            {

                if (gameMap != null)
                {
                    gameMap.end = true;
                }
            }

            Application.Exit();

        }
        #endregion
        
        #region Click Scores Menu Handlers
        private void OnButtonBackClicked(object sender, EventArgs e)
        {
            showMainMenuPage();
        }
        #endregion

        #region Show Page
        private void showMainMenuPage()
        {
            tempSettings.update(this);
            settings.refresh(this);

            try
            {
                
                playHoverButtonMusic("Allumette.mp3");
                playBackgroundMusic("Intro.mp3");
            }
            catch (Exception) { }

            if (!GamePages.Contains(mainMenuPage))
            {
                GamePages.TabPages.Add(mainMenuPage);
            }

            GamePages.TabPages.Remove(gamePage);
            GamePages.TabPages.Remove(scoresPage);
            GamePages.TabPages.Remove(settingsPage);
    
            IsMainMenuPageActive = true;
            IsGameActive = false;
            IsScoresPageActive = false;
            IsSettingsPageActive = false;
        }
        private void showGamePage()
        {
            try
            {
                stopBackgroundMusic();
            }
            catch (Exception)
            {
            }
            finally
            {
                playHoverButtonMusic("Allumette.mp3");
                playBackgroundMusic("Level_1.mp3"); //Pour l'instant
            }

            if (!GamePages.Contains(gamePage))
            {
                GamePages.TabPages.Add(gamePage);
            }

            GamePages.TabPages.Remove(mainMenuPage);
            GamePages.TabPages.Remove(scoresPage);
            GamePages.TabPages.Remove(settingsPage);

            IsMainMenuPageActive = false;
            IsGameActive = true;
            IsScoresPageActive = false;
            IsSettingsPageActive = false;
        }
        private void showScoresPage()
        {
            try
            {
                stopBackgroundMusic();
            }
            catch (Exception)
            {
            }
            finally
            {
                playHoverButtonMusic("Allumette.mp3");
                playBackgroundMusic("backgroundScores.mp3");
            }

            if (!GamePages.Contains(scoresPage))
            {
                GamePages.TabPages.Add(scoresPage);
            }

            scoresPage.Controls.Remove(arrowBack);
            scoresPage.Controls.Remove(buttonBack);

            backgroundPictureScoresMenu.Controls.Add(arrowBack);
            backgroundPictureScoresMenu.Controls.Add(buttonBack);

            GamePages.TabPages.Remove(mainMenuPage);
            GamePages.TabPages.Remove(gamePage);
            GamePages.TabPages.Remove(settingsPage);

            IsMainMenuPageActive = false;
            IsGameActive = false;
            IsScoresPageActive = true;
            IsSettingsPageActive = false;
        }
        private void showSettingsPage()
        {
            tempSettings.update(this);
            settings.refresh(this);

            try
            {
                stopBackgroundMusic();
            }
            catch (Exception) {
            }
            finally
            {
                playHoverButtonMusic("shoot.mp3");
                playBackgroundMusic("horror.mp3");
            }


            if (!GamePages.Contains(settingsPage))
            {
                GamePages.TabPages.Add(settingsPage);
            }

            settingsPage.Controls.Remove(arrowBack);
            settingsPage.Controls.Remove(buttonBack);

            backgroundPictureSettingsMenu.Controls.Add(arrowBack);
            backgroundPictureSettingsMenu.Controls.Add(buttonBack);


            GamePages.TabPages.Remove(mainMenuPage);
            GamePages.TabPages.Remove(gamePage);
            GamePages.TabPages.Remove(scoresPage);

            IsMainMenuPageActive = false;
            IsGameActive = false;
            IsScoresPageActive = false;
            IsSettingsPageActive = true;
        }
        #endregion

        #region Audio
       
        public void playBackgroundMusic(string music) {
            soundBackground.settings.volume = settings.volumeBackgroundMusic;
            soundBackground.URL = "Resources/sounds/" + music; 
            soundBackground.settings.setMode("loop", true);
            soundBackground.Ctlcontrols.play();
        }
        public void stopBackgroundMusic() {
            soundBackground.Ctlcontrols.stop();
        }
        public void pauseBackgroundMusic() {
            try
            {
                soundBackground.Ctlcontrols.pause();

            }
            catch (Exception) { }
        }
        public void resumeBackgroundMusic()
        {
            try
            {
                soundBackground.Ctlcontrols.play();

            }
            catch (Exception) { }
        }
        public void playHoverButtonMusic(string music) {
            soundHover.settings.volume = settings.volumeSFXMusic;
            soundHover.URL = "Resources/sounds/" + music;
            soundHover.Ctlcontrols.play();
        }
        private void OnCheckedStateSoundChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender) == checkBackgroundMusic)
            {
                trackBarBackgroundMusic.Enabled = checkBackgroundMusic.Checked;
                if (!checkBackgroundMusic.Checked)
                    soundBackground.settings.volume = 0;

            }
            else
            {
                settings.SFXMusicEnabled = trackBarSFXMusic.Enabled = checkSFXMusic.Checked;
                if (!checkSFXMusic.Checked)
                    soundHover.settings.volume = 0;
            }

        }
        private void OnSoundValueChanged(object sender, EventArgs e)
        {
            if ( ((TrackBar)sender) == trackBarBackgroundMusic )
            {
                soundBackground.settings.volume = ((TrackBar)sender).Value;
            }
            else if ( ((TrackBar)sender) == trackBarSFXMusic)
            {
                soundHover.settings.volume = ((TrackBar)sender).Value;
            }

        }
        
        #endregion

        #region File Management
        private void fileSettingsInit() {
            //Initialisation des variables en fonction du fichier de paramètres

        }
        private void fileScoresInit()
        {
            //Initialisation des scores en fonction du fichier de scores

        }
        private void filePlayerNameInit() { }
        #endregion

        #region Test Data
        //Initialisation des données tests
        private void fileScoresInitTest() { }
        private void filePlayerNameInitTest() { }
        #endregion

        #region Show Eyes On Button Menu
        private void showEyes(string eyesTagName, bool ok)
        {
            foreach (Control eyes in tableLayoutPanel.Controls)
            {
                if (eyes.Tag == eyesTagName)
                {
                    if (ok)
                        ((PictureBox)eyes).Image = Properties.Resources.eye;
                    else
                        ((PictureBox)eyes).Image = null;
                }
            }
        }

        #endregion

        #region Mouse Enter Main Menu Handlers
        private void OnMouseEnterButtonStart(object sender, EventArgs e)
        {
            showEyes("EyesStart", true);
            playHoverButtonMusic("button_hover.wav");

            ButtonStartIsFocused = true;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = false;

            ((Label)sender).ForeColor = Color.Brown;
        }
        private void OnMouseEnterButtonScores(object sender, EventArgs e)
        {
            showEyes("EyesScores", true);
            playHoverButtonMusic("button_hover.wav");

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = true;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = false;

            ((Label)sender).ForeColor = Color.Brown;
        }
        private void OnMouseEnterButtonSettings(object sender, EventArgs e)
        {
            showEyes("EyesSettings", true);
            playHoverButtonMusic("button_hover.wav");

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = true;
            ButtonExitIsFocused = false;

            ((Label)sender).ForeColor = Color.Brown;
        }
        private void OnMouseEnterButtonExit(object sender, EventArgs e)
        {
            showEyes("EyesExit", true);
            playHoverButtonMusic("button_hover.wav");

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = true;

            ((Label)sender).ForeColor = Color.Brown;
        }
        #endregion
        
        #region Mouse Leave Main Menu Handlers
        private void OnMouseLeaveButtonStart(object sender, EventArgs e)
        {
            showEyes("EyesStart", false);

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = false;

            ((Label)sender).ForeColor = Color.Ivory;
        }
        private void OnMouseLeaveButtonScores(object sender, EventArgs e)
        {
            showEyes("EyesScores", false);

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = false;

            ((Label)sender).ForeColor = Color.Ivory;
        }
        private void OnMouseLeaveButtonSettings(object sender, EventArgs e)
        {
            showEyes("EyesSettings", false);

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = false;

            ((Label)sender).ForeColor = Color.Ivory;
        }
        private void OnMouseLeaveButtonExit(object sender, EventArgs e)
        {
            showEyes("EyesExit", false);

            ButtonStartIsFocused = false;
            ButtonScoresIsFocused = false;
            ButtonSettingsIsFocused = false;
            ButtonExitIsFocused = false;

            ((Label)sender).ForeColor = Color.Ivory;
        }
        #endregion

        #region KeysDown and KeysUp Events Handlers

        private void OnKeysDown(object sender, KeyEventArgs e) 
        {
            if (selectingCommands && IsPopUpActive)
            {
                selectingCommands = false;

                popUpSettings.Visible = false;
                IsPopUpActive = false;
                buttonYesPopUp.Visible = buttonYesPopUp.Enabled = true;
                buttonNoPopUp.Visible = buttonNoPopUp.Enabled = true;

                settings.playerKeys[command] = e.KeyCode.ToString();
                 
                //Update command
                commandsSelectLeft.Text = settings.playerKeys["KeyLeft"];
                commandsSelectUp.Text = settings.playerKeys["KeyUp"];
                commandsSelectRight.Text = settings.playerKeys["KeyRight"];
                commandsSelectDown.Text = settings.playerKeys["KeyDown"];
                commandsSelectShoot.Text = settings.playerKeys["KeyShoot"];

                //Restore PopUp modification
                foreach (Control control in settingsPage.Controls)
                {
                    control.Enabled = true;
                }

                popUpSettings.Visible = IsPopUpActive = false;
                popUpSettings.Size = new Size(552, 214);

                titlePopUp.Text = "Quit without saving ?";

            }
            else if (!IsPopUpActive)    
            {
                if (IsGameActive && player != null)
                {
                    try
                    {
                        if (e.KeyCode.ToString() == settings.playerKeys["KeyShoot"])
                        {
                            player.isShooting = true;
                            player.shoot();

                        }else if (e.KeyCode == Keys.Enter )
                        {
                            //Gestion de l'action Entrer au clavier

                        }
                        else if (e.KeyCode.ToString() == settings.playerKeys["KeyLeft"] && player.couldChangeDirection)
                        {
                            player.direction = Directions.Left;
                            player.shouldMove = true;
                            player.isMovingLeft = true;
                            player.isMovingRight = false;

                            player.isLookingLeft = true;
                            player.isLookingRight = false;
                        }
                        else if (e.KeyCode.ToString() == settings.playerKeys["KeyRight"] && player.couldChangeDirection)
                        {
                            player.direction = Directions.Right;
                            player.shouldMove = true;
                            player.isMovingRight = true;
                            player.isMovingLeft = false;

                            player.isLookingRight = true;
                            player.isLookingLeft = false;
                        }
                        else if (e.KeyCode.ToString() == settings.playerKeys["KeyUp"] && player.couldChangeDirection)
                        {
                            player.direction = Directions.Top;
                            player.shouldMove = true;
                            player.isMovingUp = true;
                            player.isMovingDown = false;

                            player.isLookingUp = true;
                            player.isLookingDown = false;

                        }
                        else if (e.KeyCode.ToString() == settings.playerKeys["KeyDown"] && player.couldChangeDirection)
                        {
                            player.direction = Directions.Bottom;
                            player.shouldMove = true;
                            player.isMovingDown = true;
                            player.isMovingUp = false;
                            
                            player.isLookingUp = false;
                            player.isLookingDown = true;
                        
                        }
                        else if (e.KeyCode == Keys.Escape)
                        {
                            if (IsGameActive)
                            {
                                IsGamePaused = true;
                                pauseGame();

                            }
                        }
                    
                    }
                    catch (Exception) { }
                }
                else if (!IsMainMenuPageActive && !IsGamePaused && e.KeyCode == Keys.Escape)
                {
                    if (IsSettingsPageActive)
                    {
                        confirmSettings();
                    }
                    else if (IsScoresPageActive)
                        showMainMenuPage();
                }
                else if (IsGamePaused && e.KeyCode == Keys.Escape )
                {
                    resumeGame();
                }

            }   
        }
        private void OnKeysUp(object sender, KeyEventArgs e) {
            if (IsGameActive && player != null)
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {

                    }
                    else if (e.KeyCode.ToString() == settings.playerKeys["KeyLeft"])
                    {
                        player.shouldMove = false;
                        player.isMovingLeft = false;
                    }
                    else if (e.KeyCode.ToString() == settings.playerKeys["KeyRight"])
                    {
                        player.shouldMove = false;
                        player.isMovingRight = false;
                    }
                    else if (e.KeyCode.ToString() == settings.playerKeys["KeyUp"])
                    {
                        player.shouldMove = false;
                        player.isMovingUp = false;
                        player.isLookingUp = false;
                    }
                    else if (e.KeyCode.ToString() == settings.playerKeys["KeyDown"])
                    {
                        player.shouldMove = false;
                        player.isMovingDown = false;
                        player.isLookingDown = false;
                    }

                }
                catch (Exception) { }
            }
        }

        #endregion

        #region Game Thread Mangament

        #endregion

        #region On Application Closed (Before)
        private void BeforeWindowsClose(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (player != null)
                    player.ThreadPlayerMouvement.Resume();
            }
            catch (Exception) { }
            finally
            {

                if (player != null)
                {
                    player.end = true;
                }
            }

            try
            {
                gameMap.ThreadMovingMap.Resume();
            }
            catch (Exception) { }
            finally
            {
                if (gameMap != null)
                {
                    gameMap.end = true;
                }
            }

            Application.Exit();

        }
        #endregion

        #region On game started

        private void GameLoad(object sender, EventArgs e)
        {
            playBackgroundMusic("Intro.mp3");
        }
        private void pauseGame() {

            IsGamePaused = true;
            IsGameActive = false;
            IsGameResumed = false;

            player.shouldMove = false;
            player.isMovingDown = false;
            player.isMovingLeft = false;
            player.isMovingRight = false;
            player.isMovingUp = false;
            player.isShooting = false;

            gameGravity.Enabled = false;

            gameMenu.Visible = true;

            try
            {
                player.ThreadPlayerMouvement.Suspend();
                gameMap.ThreadMovingMap.Suspend();
            
            }catch (Exception) { }

        }
        private void resumeGame() {
            
            IsGameResumed = true;
            IsGamePaused = false;
            IsGameActive = true;
            gameMenu.Visible = false;
            player.shouldMove = false;

            gameGravity.Enabled = true;

            try
            {
                player.ThreadPlayerMouvement.Resume();
                gameMap.ThreadMovingMap.Resume();

            }
            catch (Exception) { }

        }
        private void stopGame()
        {
            IsGameActive = false;
            IsGamePageActive = false;
            IsGamePaused = false;
            IsGameResumed = false;
            IsGameOver = false;
            player.shouldMove = false;

            try
            {
                gameMap.ThreadMovingMap.Suspend();
            }
            catch (Exception) { }

            if (player != null)
            {
                gameMap.Controls.Remove(player);
                try
                {
                    player.ThreadPlayerMouvement.Resume();
                }
                catch (Exception) { }
                finally
                {

                    player.end = true;
                    
                }
            }

            gameMap.reload();
            gameGravity.Enabled = false;
            gameMenu.Visible = false;
            showMainMenuPage();
        }
        private void StartGame()
        {
            if ( gameMap.ThreadMovingMap.ThreadState == ThreadState.Unstarted )
            {
                try
                {
                    gameMap.ThreadMovingMap.Start();
                }
                catch (Exception) { }

            }else if (gameMap.ThreadMovingMap.ThreadState == ThreadState.Suspended)
            {
                try
                {
                    gameMap.ThreadMovingMap.Resume();
                }
                catch (Exception) { }

            }

            player = new Player(gameMap);
            gameGravity.Enabled = true;
            IsGameActive = true;
            
        }
        private void restart()
        {

            IsGameResumed = true;
            IsGamePaused = false;

            gameMenu.Visible = false;

            try
            {
                gameMap.ThreadMovingMap.Resume();
            }
            catch (Exception) { }

            if (player != null)
            {
                gameMap.Controls.Remove(player);

                try
                {
                    player.ThreadPlayerMouvement.Resume();
                }
                catch (Exception) { }
                finally
                {

                    player.end = true;
                    
                }

            }

            gameMap.reload();

            StartGame();
        }
        public void gameOver()
        {

        }

        #endregion

        #region Settings Events

        #region On Commands Settings Select
        private void OnCommandsSelectClicked(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.DarkSlateGray;
            ((Label)sender).ForeColor = Color.Ivory;

            IsPopUpActive = selectingCommands = true;
            popUpSettings.Visible = true;
            buttonYesPopUp.Visible = buttonYesPopUp.Enabled = false;
            buttonNoPopUp.Visible = buttonNoPopUp.Enabled = false;

            titlePopUp.Text = "Waiting for a key...";
            popUpSettings.Height = titlePopUp.Height + 120;
            popUpSettings.Width = titlePopUp.Width + 150;
            confirmSettings();

            command = ((Label)sender).Tag.ToString();

        }
        #endregion

        #region On Mouse Enter and Leave Commands and button Settings Box

        private void OnMouseEnterButtonSettingsBox(object sender, EventArgs e)
        {
            if (((Label)sender) == buttonSave)
            {
                ((Label)sender).BackColor = Color.DarkGreen;

            }
            else if (((Label)sender) == buttonReset)
            {
                ((Label)sender).BackColor = Color.Aquamarine;

            }
            else if (((Label)sender) == buttonCancel)
            {
                ((Label)sender).BackColor = Color.DarkRed;
            }
            else
                ((Label)sender).BackColor = Color.Ivory;

            ((Label)sender).ForeColor = Color.Black;
        }
        private void OnMouseLeaveButtonSettingsBox(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.FromArgb(238, 25, 25, 25);
            ((Label)sender).ForeColor = Color.Ivory;
        }
        private void OnMouseEnterCommandsSettings(object sender, EventArgs e)
        {

            ((Label)sender).BackColor = Color.Ivory;

            ((Label)sender).ForeColor = Color.Black;
        
        }
        private void OnMouseLeaveCommandsSettings(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.FromArgb(238, 25, 25, 25);
            ((Label)sender).ForeColor = Color.Ivory;
        }

        #endregion

        #region save settings

        private void confirmSettings()
        {
            playHoverButtonMusic("modalWindows.wav");

            foreach (Control control in settingsPage.Controls)
            {
                control.Enabled = false;
            }

            popUpSettings.Visible = IsPopUpActive = true;
            popUpSettings.Enabled = true;

            if (!selectingCommands)
            {
                buttonYesPopUp.Visible = buttonYesPopUp.Enabled = true;
                buttonNoPopUp.Visible = buttonNoPopUp.Enabled = true;
            }
        }

        public void OnSaveSettingsClicked(object sender, EventArgs e)
        {
            settings.refresh(this);
            tempSettings = new Settings(settings);

            showMainMenuPage();
        }

        public void OnResetSettingsClicked(object sender, EventArgs e)
        {
            settings.reset(this);
        }

        private void buttonYesPopUp_Click(object sender, EventArgs e)
        {
            foreach (Control control in settingsPage.Controls)
            {
                control.Enabled = true;
            }

            showMainMenuPage();
            popUpSettings.Visible = IsPopUpActive = false;
        }

        private void buttonNoPopUp_Click(object sender, EventArgs e)
        {
            foreach (Control control in settingsPage.Controls)
            {
                control.Enabled = true;
            }

            popUpSettings.Visible = IsPopUpActive = false;
        }

        #endregion

        private void OnAddNewPlayer(object sender, EventArgs e)
        {
            int index = playerNames.Items.Add(textBoxPlayerName.Text);
            playerNames.SelectedIndex = index;
            playerNames.Text = "  " + playerNames.Text;
            textBoxPlayerName.Text = "";
        }

        #endregion

        #region On Mouse Enter and Leave Button Back
        private void OnMouseEnterButtonBack(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.Brown;
        }
        private void OnMouseLeaveButtonBack(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.Ivory;
        }

        #endregion
        
        #region Game Menu Events
        
        private void OnGameMenuButtonClicked(object sender, EventArgs e)
        {
            if ( ((Label)sender) == buttonRestart)
            {
                restart();
            
            }else if (((Label)sender) == buttonResume)
            {
                resumeGame();
            }else if (((Label)sender) == buttonQuit)
            {
                stopGame();
            }
        }
        private void OnMouseEnterGameMenuButton(object sender, EventArgs e)
        {

            ((Label)sender).ForeColor = Color.Black;

            if (((Label)sender) == buttonRestart)
                buttonRestart.BackColor = Color.Aquamarine;
            else if (((Label)sender) == buttonResume)
                buttonResume.BackColor = Color.DarkGreen;
            else if (((Label)sender) == buttonQuit)
                buttonQuit.BackColor = Color.DarkRed;
        }
        private void OnMouseLeaveGameMenuButton(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.Ivory;
            ((Label)sender).BackColor = Color.Teal;
        }




        #endregion

        #region Gravity Management

        private void Gravity(object sender, EventArgs e)
        {
            if (player != null)
            {
                if (player.Bottom < Height && !player.isJumping)
                {
                    player.currentPlatform.BackColor = Color.Transparent;

                    if ( !player.IsPlayerIsOnGround())
                    {
                        player.Top += 7;

                        player.isOnGround = false;
                        player.shouldMove = false;
                        player.couldChangeDirection = false;
                    }
                    else
                    {
                        player.isOnGround = true;
                        player.couldChangeDirection = true;

                        player.currentPlatform.BackColor = Color.Green;
                        
                        //Console.WriteLine("Current platform : left = " + player.currentPlatform.Left + " right = " + player.currentPlatform.Right);
                        //Console.WriteLine("Player : left = " + player.Left + " right = " + player.Right);

                    }
                }
            }
        }

        #endregion


    }
}
