using Contra.components;
using Contra.Map;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Contra
{

    public enum Directions
    {
        Left, Top, Right, Bottom
    }

    public class Player : PictureBox
    {

        #region Properties

        public static object PlayerTag = "player";
        public static object BulletTag = "player_bullet";

        private GameMap Map; //Map où se situe le joueur
        public Platform currentPlatform; //Platform sur laquelle le joueur se situe

        public int life { get; set; }
        public ProgressBar lifeBar { get; set; }
        public string name { get; set; } //Nom du joueur
        public int speed { get; set; } //Vitesse du joueur

        //Mouvement
        public Directions direction { get; set; } //Direction de déplacement du joueur
        public Thread ThreadPlayerMouvement;
        private delegate void DelegateThreadPlayerMouvement();
        private DelegateThreadPlayerMouvement delegateThreadPlayerMouvement;

        public Thread ThreadPlayerJump;
        private delegate void DelegateThreadPlayerJump();
        private DelegateThreadPlayerJump delegateThreadPlayerJump;

        private int imageIndex; //Pour le mouvement;
        public bool shouldMove { get; set; }

        private List<Image> imagesMoveRight;
        private List<Image> imagesMoveLeft;

        private List<Image> imagesShootRight;
        private List<Image> imagesShootLeft;

        private List<Image> imagesShootTopRight;
        private List<Image> imagesShootTopLeft;

        private List<Image> imagesShootDownRight;
        private List<Image> imagesShootDownLeft;

        public bool couldChangeDirection { get; set; }
        public bool isMovingLeft { get; set; }
        public bool isMovingUp { get; set; }
        public bool isMovingRight { get; set; }
        public bool isMovingDown { get; set; }
        public bool isShooting { get; set; }
        public bool isOnGround { get; set; }

        public bool isLookingLeft { get; set; }
        public bool isLookingUp { get; set; }
        public bool isLookingRight { get; set; }
        public bool isLookingDown { get; set; }

        public bool isJumping { get; set; }
        public int jumpingDelay { get; set; }

        public bool end { get; set; }

        #endregion

        #region Constructors

        public Player(GameMap parent)
        {
            //Init Map
            Map = parent;

            lifeBar = parent.lifeBar;

            name = "SterDevs";
            Tag = PlayerTag;
            life = 300;
            lifeBar.Maximum = 300;
            lifeBar.Value = life;

            speed = 15; //Player speed
            direction = Directions.Right; //Il regarde à droite par défaut
            isLookingRight = true;

            isOnGround = true;
            jumpingDelay = 1500; //2s de saut pour montrer

            //Init PictureBox
            Size = new Size(28, 90);

            SizeMode = PictureBoxSizeMode.StretchImage;
            Location = new Point(90, 170);

            imageIndex = 0;

            imagesMoveRight = new List<Image> { Properties.Resources.PicRight1, Properties.Resources.PicRight2, Properties.Resources.PicRight3 };
            imagesMoveLeft = new List<Image> { Properties.Resources.PicLeft1, Properties.Resources.PicLeft2, Properties.Resources.PicLeft3 };

            imagesShootRight = new List<Image>() { Properties.Resources.shoot_right_1, Properties.Resources.shoot_right_2, Properties.Resources.shoot_right_3 };
            imagesShootLeft = new List<Image>() { Properties.Resources.shoot_left_1, Properties.Resources.shoot_left_2, Properties.Resources.shoot_left_3 };

            imagesShootTopRight = new List<Image>() { Properties.Resources.shoot_top_right_1, Properties.Resources.shoot_top_right_2, Properties.Resources.shoot_top_right_3 };
            imagesShootTopLeft = new List<Image>() { Properties.Resources.shoot_top_left_1, Properties.Resources.shoot_top_left_2, Properties.Resources.shoot_top_left_3 };

            imagesShootDownRight = new List<Image>() { Properties.Resources.shoot_down_right_1, Properties.Resources.shoot_down_right_2, Properties.Resources.shoot_down_right_3 };
            imagesShootDownLeft = new List<Image>() { Properties.Resources.shoot_down_left_1, Properties.Resources.shoot_down_left_2, Properties.Resources.shoot_down_left_3 };

            Image = imagesMoveRight[2];

            currentPlatform = Map.ground.platforms.First();

            Map.Controls.Add(this);
            BringToFront();

            //Init thread
            initThread();
        }
        private void initThread()
        {
            ThreadPlayerMouvement = new Thread(ThreadMouvement);

            delegateThreadPlayerMouvement = new DelegateThreadPlayerMouvement(movePlayer);

            shouldMove = false; //Par défaut il est stable

            ThreadPlayerMouvement.Start(); //On lance le thread pour son déplacement

            //Jump 
            couldChangeDirection = true;

            ThreadPlayerJump = new Thread(ThreadJump);
            delegateThreadPlayerJump = new DelegateThreadPlayerJump(jumping);

            ThreadPlayerJump.Start();

        }

        #endregion

        #region Move and Jump Thread

        //Thread déplacement
        public void ThreadMouvement()
        {
            Thread.CurrentThread.Name = "MouvementPlayer";
            Console.WriteLine(Thread.CurrentThread.Name + " : Début déplacement...");

            while (!end)
            {

                while (shouldMove)
                {
                    int numberOfImagesToShow = 4;
                    while (numberOfImagesToShow > 0)
                    {
                        Map.Invoke(delegateThreadPlayerMouvement);
                        Thread.Sleep(90);
                        numberOfImagesToShow--;
                    }

                    if (Right >= (Map.Parent.Width / 2 + 25))
                    {
                        if (direction == Directions.Right)
                        {
                            Map.direction = direction;
                            Map.couldMove = true;
                            speed = 10;
                        }
                        else
                        {
                            Map.couldMove = false;
                            speed = 15;
                        }
                    }

                }

                Map.couldMove = false;

                if (end)
                {
                    return;
                }

                Thread.Sleep(40);

            }

            Console.WriteLine(Thread.CurrentThread.Name + " : Fin thread !");

        }

        //Check if player is on ground
        public bool IsPlayerIsOnGround()
        {
            foreach (Platform platform in Map.ground.platforms)
            {
                //Check the top
                if ((platform.Top + platform.Height - 5) > Bottom && Bottom > (platform.Top - 10))
                {
                    //Check the Left
                    //if ( Bounds.IntersectsWith( platform.Bounds ) )
                    if ((platform.Left - 25) < Left && Right < (platform.Right + 25))
                    {
                        currentPlatform = platform;
                        Top = currentPlatform.Top - Height;

                        return true;
                    }

                }
            }

            return false;
        }

        //Move Player
        private void movePlayer()
        {
            Console.WriteLine(Thread.CurrentThread.Name + " : Player begin to move... Image Index : " + imageIndex);

            imageIndex = (imageIndex + 1) % 3; //On change l'index de la prochaine image

            //Image
            if (!isShooting)
            {
                Width = 28;

                if (isMovingLeft)
                {
                    Image = imagesMoveLeft[imageIndex];
                }

                if (isMovingRight)
                {
                    Image = imagesMoveRight[imageIndex];
                }

            }
            else
            {
                //Size = new Size(75, 90);
                Width = 35;

                if (isMovingLeft && !isLookingUp && !isLookingDown)
                {
                    Image = imagesShootLeft[imageIndex];

                }

                if (isMovingRight && !isLookingUp && !isLookingDown)
                {
                    Image = imagesShootRight[imageIndex];

                }

                if (isLookingUp && isLookingRight)
                {
                    Image = imagesShootTopRight[imageIndex];

                }

                if (isLookingUp && isLookingLeft)
                {
                    Image = imagesShootTopLeft[imageIndex];

                }

                if (isLookingDown && isLookingRight)
                {
                    Image = imagesShootDownRight[imageIndex];

                }

                if (isLookingDown && isLookingLeft)
                {
                    Image = imagesShootDownLeft[imageIndex];
                }

            }

            if (isMovingLeft)
            {
                if (Left > Map.Left + 20)
                {
                    Left -= speed;
                }
            }

            if (isMovingDown)
            {
                isMovingDown = false;
                Top += currentPlatform.Height / 2 + 30;

            }

            if (isMovingRight)
            {
                if (Left < Map.Width - 450)
                {
                    Left += speed;
                }
            }

        }

        //Jump
        private void ThreadJump()
        {
            Thread.CurrentThread.Name = "PlayerJump";
            Console.WriteLine("PlayerJump : looking....");

            while (!end)
            {
                if (isOnGround)
                {
                    if (isMovingUp)
                    {
                        isJumping = true;
                        isOnGround = false;
                        shouldMove = false;
                        couldChangeDirection = false;

                        int maxJumpingHeight;

                        if ((Top - 80) > (Map.scoreLabel.Bottom - 20))
                        {
                            maxJumpingHeight = Top - 110;
                        }
                        else
                        {
                            maxJumpingHeight = Top - 10;
                        }

                        try
                        {

                            while (Top >= maxJumpingHeight)
                            {
                                Map.Invoke(delegateThreadPlayerJump);
                                Thread.Sleep(30);
                                isMovingUp = true;
                            }

                            //end = DateTime.Now.AddMilliseconds(jumpingDelay);

                            isMovingUp = false;

                            /* 
                                isMovingDown = true;

                                while ( end > DateTime.Now)
                                {
                                    Map.Invoke(delegateThreadPlayerJump);
                                    Thread.Sleep(50);
                                    isMovingDown = true;
                                }

                                isMovingDown = false; 
                            */

                        }
                        catch (Exception) { }

                        isOnGround = true;
                        shouldMove = true;
                        isJumping = false;
                        couldChangeDirection = true;
                    }
                }

                Thread.Sleep(150);
            }

            Console.WriteLine("PlayerJump : End");

        }

        private void jumping()
        {

            if (isMovingUp)
            {
                jump();
            }
            /*else if (isMovingDown)
            {
                falling();
            }*/

        }

        private void jump()
        {
            Top -= 10;
            if (isMovingLeft && Left > Map.Left + 20)
            {
                Left -= 5;

            }
            if (isMovingRight && Left < Map.Width - 270)
            {
                Left += 5;
            }
        }

        /*private void falling()
        {
            //Top += 5; the gravity will make him fall later

            if (isMovingLeft && Left > Map.Left + 20)
            {
                Left -= 2;
            }
            if (isMovingRight && Left < Map.Width - 270)
            {
                Left += 2;
            }
        }
*/

        #endregion

        #region Bullet Management

        public void subirDegat(int degat)
        {
            lifeBar.Value = life = ((life - degat) >= 0) ? (life - degat) : 0;
        }

        public void shoot()
        {
            if (Map == null) return;

            bool bulletShouldGoLeft, bulletShouldGoRight;

            bulletShouldGoLeft = isLookingLeft;
            bulletShouldGoRight = isLookingRight;

            Width = 35;

            if (!isMovingLeft && !isMovingRight && !isMovingUp && !isMovingDown)
            {
                if (isLookingLeft && !isLookingUp && !isLookingDown)
                {
                    Image = imagesShootLeft[2];

                }

                if (isLookingRight && !isLookingUp && !isLookingDown)
                {
                    Image = imagesShootRight[2];

                }

                if (isLookingUp && isLookingRight)
                {
                    Image = imagesShootTopRight[2];

                }

                if (isLookingUp && isLookingLeft)
                {
                    Image = imagesShootTopLeft[2];

                }

                if (isLookingDown && isLookingRight)
                {
                    Image = imagesShootDownRight[2];
                }

                if (isLookingDown && isLookingLeft)
                {
                    Image = imagesShootDownLeft[2];
                }
            }

            Bullet bullet = new Bullet(Map, this)
            {
                isGoingLeft = bulletShouldGoLeft,
                isGoingUp = isLookingUp,
                isGoingRight = bulletShouldGoRight,
                isGoingDown = isLookingDown,
                Tag = "player_bullet",
                Image = Properties.Resources.bullet_player
            };

            try
            {
                //On tire
                bullet.taskMovingBullet.Start();
            }
            catch (Exception) { }
        }

        #endregion

    }

}
