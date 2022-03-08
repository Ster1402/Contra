using Contra.components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace Contra.Map
{
    public class GameMap : PictureBox
    {
        #region Properties
        public bool gameStarted { get; set; }
        public Label labelLevelName { get; set; }
        public bool shouldShowWelcomeLabel { get; set; }

        public List<SingleMap> maps;

        public List<Enemy> enemies;

        public int numberOfMaps;
        public int platformWidth;

        public Ground ground { get; set; }

        public TabPage parent;

        public Label scoreLabel { get; set; }
        public int score { get; set; }
        public ProgressBar lifeBar { get; set; }
        public Label playerName { get; set; }
        public PictureBox heartLifeIcon { get; set; }

        public bool couldMove;
        public Directions direction;

        public Thread ThreadMovingMap { get; set; }
        public delegate void DelegateThreadMovingMap();
        public DelegateThreadMovingMap delegateThreadMoveMap;

        public delegate void DelegatePrintWelcomeLabel();
        public DelegatePrintWelcomeLabel delegatePrintWelcomeLabel;

        public Thread ThreadBulletCollisionMap;

        public delegate void DelegateThreadBulletCollisionMapEnemy(Enemy enemy, Bullet bullet);
        public DelegateThreadBulletCollisionMapEnemy delegateThreadBulletCollisionMapEnemy;

        public delegate void DelegateThreadBulletCollisionMapPlayer(Player player, Bullet bullet);
        public DelegateThreadBulletCollisionMapPlayer delegateThreadBulletCollisionMapPlayer;

        public delegate void DelegateThreadEnemyCollisionMapPlayer(Player player, Enemy enemy);
        public DelegateThreadEnemyCollisionMapPlayer delegateThreadEnemyCollisionMapPlayer;

        public Thread threadAttackPlayer { get; set; }
        public delegate void DelegateThreadEnemyShootMapPlayer(Enemy enemy, Player player);
        public DelegateThreadEnemyShootMapPlayer delegateThreadEnemyShootMapPlayer;


        public bool end { get; set; }

        #endregion

        #region Constructor
        public GameMap()
        {
            maps = new List<SingleMap>();
            numberOfMaps = 4;

            end = false; //End thread

            Image = Properties.Resources.background_map;
            SizeMode = PictureBoxSizeMode.StretchImage;
            Size = new Size(1328, 900);
            Location = new Point(0, 0);

            for (int i = 0; i < numberOfMaps; i++)
            {
                maps.Add(new SingleMap());
                Controls.Add(maps[i]);
            }

            #region Welcome Label

            labelLevelName = new Label
            {
                AutoSize = true,
                Font = new Font("Old English Text MT", 65F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.Ivory,
                BackColor = Color.Black,
                Size = new Size(868, 104),
                Text = "The Forest Of Greed !",
                Padding = new Padding(5),
                TextAlign = ContentAlignment.MiddleCenter,

            };

            labelLevelName.Left = Width / 2 - labelLevelName.Width / 2;
            labelLevelName.Top = 100;

            #endregion

            #region Score Label
            // 
            // scoreLabel
            //
            score = 0;

            scoreLabel = new Label
            {
                Font = new Font("Courier New", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.Ivory,
                BackColor = Color.Transparent,
                Location = new Point(6, 6),
                Name = "scoreLabel",
                AutoSize = true,
                Text = "Score : 0"
            };
            #endregion

            #region Heart Life Icon
            // 
            // Heart Life Icon
            //
            heartLifeIcon = new PictureBox
            {
                Image = Properties.Resources.heart_full,
                BackColor = Color.Transparent,
                Location = new Point(775, 6),
                Name = "heartLifeIcon",
                Size = new Size(27, 23),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            #endregion

            #region Player Name

            playerName = new Label
            {
                Font = new Font("Courier New", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.Ivory,
                BackColor = Color.Transparent,
                Name = "PlayerName",
                AutoSize = true,
                Text = "Player 1"
            };

            #endregion

            #region Life Bar
            // 
            // lifeBar
            // 
            lifeBar = new ProgressBar
            {
                Location = new Point(790, 5),
                Name = "lifeBar",
                Size = new Size(212, 23),
                Style = ProgressBarStyle.Continuous,
                Value = 100
            };

            #endregion

            Controls.Add(scoreLabel);
            Controls.Add(playerName);
            Controls.Add(heartLifeIcon);
            Controls.Add(lifeBar);

            ground = new Ground(this);

            groundLayout_Level_1();
            MakeEnnemies();

            playerName.BringToFront();
            scoreLabel.BringToFront();
            heartLifeIcon.BringToFront();
            lifeBar.BringToFront();

            #region Thread

            ThreadMovingMap = new Thread(ThreadMoving);
            delegateThreadMoveMap = new DelegateThreadMovingMap(MoveMap);

            couldMove = false;

            ThreadBulletCollisionMap = new Thread(BulletCollisionMap);
            threadAttackPlayer = new Thread(AttackPlayer);

            delegateThreadBulletCollisionMapPlayer = new DelegateThreadBulletCollisionMapPlayer(shootPlayer);
            delegateThreadBulletCollisionMapEnemy = new DelegateThreadBulletCollisionMapEnemy(shootEnemy);
            delegateThreadEnemyCollisionMapPlayer = new DelegateThreadEnemyCollisionMapPlayer(shootPlayer);
            delegateThreadEnemyShootMapPlayer = new DelegateThreadEnemyShootMapPlayer(EnemyShoot);

            //Print label
            delegatePrintWelcomeLabel = new DelegatePrintWelcomeLabel(printLevelStageWelcome);

            #endregion

        }

        #endregion

        #region Map structure

        public void groundLayout_Level_1()
        {

            List<int> lefts = new List<int>() { 5, 6, 7, 15, 16, 30, 31, 35, 36, 37, 20, 21, 22, 23, 24, 25, 26, 27 };
            List<int> tops = new List<int>() { 170, 170, 170, 380, 380, 380, 170, 170, 170, 370, 420, 420, 370, 370, 370, 420, 470, 470 };

            for (int i = 0; i < lefts.Count; i++)
            {
                ground.platforms.Add(new Platform(this, lefts[i] * platformWidth, tops[i]));
            }

            int j = 1;
            foreach (Platform platform in ground.platforms)
            {

            }

        }
        public void MakeEnnemies()
        {
            enemies = new List<Enemy>();

            int index = 1;
            foreach (Platform platform in ground.platforms)
            {
                if (index % 9 == 0 || index % 5 == 0 || index % 13 == 0 || index % 7 == 0)
                {
                    Enemy enemy = new Enemy();

                    enemies.Add(enemy);

                    enemy.Left = platform.Left + 15;
                    enemy.Top = platform.Top - enemy.Height;

                    Controls.Add(enemy);
                    enemy.BringToFront();

                    if (index > 20)
                    {
                        enemy.upgrade();

                        if (index > 30)
                        {
                            enemy.upgrade();
                        }
                    }
                }


                index++;
            }


            //enemy.threadMoveEnemy.Start();

        }
        public void EraseEnnemies()
        {

            foreach (Enemy enemy in enemies)
            {
                Controls.Remove(enemy);
            }

            enemies.Clear();
        }

        #endregion

        #region Thread Enemies

        public void startEnemiesThread()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.threadMoveEnemy.Start();
            }
        }

        public void pauseEnemiesThread()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.threadMoveEnemy.Suspend();
            }
        }

        public void resumeEnemiesThread()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.threadMoveEnemy.Resume();
            }
        }

        public void endEnemiesThread()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.end = true;
            }
        }

        #endregion

        #region Reload

        public void reload()
        {
            int i = 0;

            foreach (SingleMap map in maps)
            {
                if (!Controls.Contains(map))
                {
                    Controls.Add(map);
                }

                map.Left = i * map.Width;
                map.BringToFront();
                i++;
            }

            foreach (Platform platform in ground.platforms)
            {
                if (!Controls.Contains(platform))
                {
                    Controls.Add(platform);
                }
            }

            i = 0;
            foreach (Platform platform in ground.platforms)
            {
                platform.Left = i * platform.Width;
                platform.BringToFront();
                i++;
            }

            scoreLabel.Text = "Score : 0";
            score = 0;
            lifeBar.Value = 100;

            scoreLabel.BringToFront();
            playerName.BringToFront();
            lifeBar.BringToFront();
            heartLifeIcon.BringToFront();

            groundLayout_Level_1();

            EraseEnnemies();
            MakeEnnemies();
        }

        #endregion

        #region Resize Maps
        public void resizeMap()
        {
            int i = 0;
            foreach (SingleMap map in maps)
            {
                //SingleMap
                map.Width = Width;
                map.Height = Height;

                map.Left = i * map.Width;
                map.Top = 0;

                i++;
            }
        }

        public void resizePlayerName()
        {
            playerName.Left = Width / 2 - playerName.Width / 2;
        }

        #endregion

        public void printLevelStageWelcome()
        {

            if (shouldShowWelcomeLabel)
            {
                shouldShowWelcomeLabel = false;
                gameStarted = false;

                Controls.Add(labelLevelName);
                labelLevelName.BringToFront();
            }
            else
            {
                Controls.Remove(labelLevelName);
            }


        }

        #region Map Moving Management
        public void ThreadMoving()
        {
            Thread.CurrentThread.Name = "GameMapThread";
            Console.WriteLine(Thread.CurrentThread.Name + " : Moving start thread... ");

            ThreadBulletCollisionMap.Start();

            while (!end)
            {

                if (gameStarted)
                {
                    Thread.Sleep(1500);
                    shouldShowWelcomeLabel = true;
                    Parent?.Invoke(delegatePrintWelcomeLabel);
                    Thread.Sleep(3500);
                    Parent?.Invoke(delegatePrintWelcomeLabel);
                }

                if (couldMove)
                {
                    Parent?.Invoke(delegateThreadMoveMap);
                    Console.WriteLine(Thread.CurrentThread.Name + " : Moving... ");
                }

                if (end)
                {
                    return;
                }

                Thread.Sleep(250);
            }

            Console.WriteLine(Thread.CurrentThread.Name + " : End ! ");

        }

        public void MoveMap()
        {
            Console.WriteLine(Thread.CurrentThread.Name + " : MoveMap....");

            if (maps.Last().Left > 10)
            {
                switch (direction)
                {
                    case Directions.Left:

                        MoveGroundLeft();

                        MoveEnemiesLeft();

                        for (int i = 0; i < maps.Count; i++)
                        {
                            maps[i].Left += 15;
                        }

                        break;

                    case Directions.Right:

                        MoveGroundRight();

                        MoveEnemiesRight();

                        for (int i = 0; i < maps.Count; i++)
                        {
                            maps[i].Left -= 15;

                            if (maps[i].Right <= 0)
                            {
                                Controls.Remove(maps[i]);
                            }
                        }

                        break;
                }

            }

            Console.WriteLine(Thread.CurrentThread.Name + " : MoveMap End.");
        }

        public void MoveEnemiesLeft()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Left += 15;

                if (enemy.Right <= 0)
                {
                    enemy.end = true;
                    //Controls.Remove(enemy);
                }
            }
        }

        public void MoveEnemiesRight()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Left -= 15;

                if (enemy.Right <= 0)
                {
                    enemy.end = true;
                    //Controls.Remove(enemy);
                }
            }
        }

        private void MoveGroundLeft()
        {
            foreach (Platform platform in ground.platforms)
            {
                platform.Left += 15;

                if (platform.Right <= 0)
                {
                    Controls.Remove(platform);
                }
            }
        }

        private void MoveGroundRight()
        {
            foreach (Platform platform in ground.platforms)
            {
                platform.Left -= 15;

                if (platform.Right <= 0)
                {
                    Controls.Remove(platform);
                }
            }
        }

        #endregion

        #region Check for collision

        public void BulletCollisionMap()
        {
            threadAttackPlayer.Start();

            Thread.CurrentThread.Name = "Collision check !";

            while (!end)
            {

                //Check if an enemy has been shooted
                foreach (Control control in Controls)
                {
                    if (control.Tag != Enemy.tag)
                    {
                        continue;
                    }

                    Enemy enemy = (Enemy)control;

                    //Check for player bullets
                    foreach (Control control1 in Controls)
                    {
                        if (control1.Tag == Player.BulletTag)
                        {

                            Bullet bullet = (Bullet)control1;

                            if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                            {
                                Parent.Invoke(delegateThreadBulletCollisionMapEnemy, enemy, bullet);

                            }

                        }
                    }

                }

                //Check the player has been shooted
                foreach (Control control in Controls)
                {
                    if (control.Tag == Player.PlayerTag)
                    {

                        Player player = (Player)control;

                        //Check for enemies bullets
                        foreach (Control control1 in Controls)
                        {
                            if (control1.Tag == Enemy.BulletTag)
                            {
                                Bullet bullet = (Bullet)control1;

                                if (bullet.Bounds.IntersectsWith(player.Bounds))
                                {
                                    Parent.Invoke(delegateThreadBulletCollisionMapPlayer, player, bullet);

                                }

                            }
                        }

                    }

                }

                Thread.Sleep(50);

            }
        }

        public void AttackPlayer()
        {
            Thread.CurrentThread.Name = "Attack player !";

            while (!end)
            {
                //Check the player has been shooted
                foreach (Control control in Controls)
                {
                    if (control.Tag == Player.PlayerTag)
                    {

                        Player player = (Player)control;

                        foreach (Control control_enemy in Controls)
                        {
                            if (control_enemy.Tag == Enemy.tag)
                            {
                                Enemy enemy = (Enemy)control_enemy;

                                if (player.Bounds.IntersectsWith(enemy.Bounds))
                                {
                                    Thread.Sleep(200);
                                    Parent.Invoke(delegateThreadEnemyCollisionMapPlayer, player, enemy);
                                    break;
                                }

                                if (player.Right >= (enemy.Left - 400) && player.Right <= (enemy.Right + 100))
                                {
                                    Thread.Sleep(500);
                                    Parent.Invoke(delegateThreadEnemyShootMapPlayer, enemy, player);
                                }
                            }
                        }

                    }

                }

                Thread.Sleep(100);
            }
        }

        public void shootPlayer(Player player, Bullet bullet)
        {
            player.subirDegat(bullet.degat);

            bullet.end = true;
            Controls.Remove(bullet);

        }

        public void shootPlayer(Player player, Enemy enemy)
        {
            player.subirDegat( enemy.degat / 5 );
        }

        public void EnemyShoot(Enemy enemy, Player player)
        {
            if (player.Right < enemy.Left)
            {
                if (enemy.isMovingLeft)
                {
                    if (player.Bottom < enemy.Top - 10)
                    {
                        enemy.isLookingUp = true;
                        enemy.isLookingDown = false;
                        enemy.coolDown.AddSeconds(enemy.rank + 2);
                        enemy.shoot();
                    }
                    else if (player.Bottom > enemy.Bottom + 10)
                    {
                        enemy.isLookingUp = false;
                        enemy.isLookingDown = true;
                        enemy.coolDown.AddSeconds(enemy.rank + 2);
                        enemy.shoot();
                    }
                    else
                    {
                        enemy.isLookingUp = false;
                        enemy.isLookingDown = false;
                        enemy.coolDown.AddSeconds(enemy.rank + 2);
                        enemy.shoot();
                    }
                }
                
            }
            else if (player.Left > enemy.Right){
                
                if (enemy.isMovingRight)
                {
                    if (player.Bottom < enemy.Top - 10)
                    {
                        enemy.isLookingUp = true;
                        enemy.isLookingDown = false;
                        enemy.coolDown.AddSeconds(enemy.rank + 2);
                        enemy.shoot();
                    }
                    else if (player.Bottom > enemy.Bottom + 10)
                    {
                        enemy.isLookingUp = false;
                        enemy.isLookingDown = true;
                                                enemy.coolDown.AddSeconds(enemy.rank + 2);
                        enemy.shoot();
                    }
                    else
                    {
                        enemy.isLookingUp = false;
                        enemy.isLookingDown = false;
                                                enemy.coolDown.AddSeconds(enemy.rank + 2);
                        enemy.shoot();
                    }
                }
            }
        }

        public void shootEnemy(Enemy enemy, Bullet bullet)
        {

            enemy.subirDegat(bullet.degat);
            bullet.end = true;
            Controls.Remove(bullet);
            Invalidate();
            Update();

            if (enemy.life.Value == 0)
            {

                if (enemy.rank == 1)
                {
                    score += 5;
                }
                else if (enemy.rank == 2)
                {
                    score += 10;
                }
                else if (enemy.rank == 3)
                {
                    score += 20;
                }

                scoreLabel.Text = "Score : " + score;

                enemy.end = true;

                Controls.Remove(enemy);
            }

        }

        #endregion

    }
}
