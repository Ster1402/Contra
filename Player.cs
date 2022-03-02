﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Contra.Map;
using Contra.components;

namespace Contra
{

    public enum Directions
    {
        Left, Top, Right, Bottom
    }

    public class Player : PictureBox
    {

        #region Properties

        private GameMap Map; //Map où se situe le joueur

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
        private List<Image> imagesMoveTop;
        private List<Image> imagesMoveBottom;

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

        public int jumpingDelay { get; set; }

        public bool end { get; set; }


        #endregion

        #region Constructors
        public Player(GameMap parent)
        {
            //Init Map
            Map = parent;

            name = "Player 1";
            speed = 10; //Player speed
            direction = Directions.Right; //Il regarde à droite par défaut
            isLookingRight = true;

            isOnGround = true;
            jumpingDelay = 2000; //2s de saut pour montrer

            //Init PictureBox
            Size = new Size(45, 90);
            SizeMode = PictureBoxSizeMode.StretchImage;
            Location = new Point(90, 170);
            
            imageIndex = 0;       
            imagesMoveRight = new List<Image> { Properties.Resources.PicRight1, Properties.Resources.PicRight2, Properties.Resources.PicRight3 };
            imagesMoveLeft = new List<Image> { Properties.Resources.PicLeft1, Properties.Resources.PicLeft2, Properties.Resources.PicLeft3 };
            imagesMoveTop = new List<Image>();
            imagesMoveBottom = new List<Image>();

            Image = imagesMoveRight[0];

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

        #region Moving Thread

        //Thread déplacement
        public void ThreadMouvement()
        {
            Thread.CurrentThread.Name = "MouvementPlayer";
            Console.WriteLine(Thread.CurrentThread.Name + " : Début déplacement...");

            while (!end)
            {

                while (shouldMove)
                {
                    int numberOfImagesToShow = 3;
                    while (numberOfImagesToShow > 0)
                    {
                        Map.Invoke(delegateThreadPlayerMouvement);
                        Thread.Sleep(95);
                        numberOfImagesToShow--;
                    }

                    if (Left >= Map.Parent.Width / 2 + 50)
                    {
                        if (direction == Directions.Right)
                        {
                            Map.direction = direction;
                            Map.couldMove = true;
                        }
                        else
                        {
                            Map.couldMove = false;
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

        //Move Player
        private void movePlayer()
        {
            Console.WriteLine(Thread.CurrentThread.Name + " : Player begin to move... Image Index : " + imageIndex);

            imageIndex = (imageIndex + 1) % 3; //On change l'index de la prochaine image

            if (isMovingLeft)
            {
                    Image = imagesMoveLeft[imageIndex];

                    if (Left > Map.Left + 20)
                        Left -= speed;

            }
            
            if (isMovingUp)
            {
                    //Image = imagesMoveTop[imageIndex];

            }

            if (isMovingDown)
            {
                //Image = imagesMoveTop[imageIndex];

            }

            if (isMovingRight)
            {
                Image = imagesMoveRight[imageIndex];
                    
                if (Left < Map.Width - 270)
                    Left += speed;

            }

        }

        //Jump

        private void ThreadJump()
        {
            Thread.CurrentThread.Name = "PlayerJump";
            Console.WriteLine("PlayerJump : looking....");

            int maxJumpingHeight = Top - 120;
            int initialTop = Top;

            while (!end)
            {
                if (isOnGround)
                {
                    if (isMovingUp)
                    {
                        isOnGround = false;
                        shouldMove = false;
                        couldChangeDirection = false;

                        DateTime end = DateTime.Now.AddMilliseconds(jumpingDelay);

                        try
                        {

                            while (Top > maxJumpingHeight && end > DateTime.Now)
                            {
                                Map.Invoke(delegateThreadPlayerJump);
                                Thread.Sleep(50);
                                isMovingUp = true;
                            }

                            end = DateTime.Now.AddMilliseconds(jumpingDelay);

                            isMovingUp = false;
                            isMovingDown = true;

                            while (Top < initialTop && end > DateTime.Now)
                            {
                                Map.Invoke(delegateThreadPlayerJump);
                                Thread.Sleep(50);
                                isMovingDown = true;
                            }

                            isMovingDown = false;
                        }
                        catch (Exception) { }

                        isOnGround = true;
                        shouldMove = true;
                        couldChangeDirection = true;
                    }
                }
                else
                {

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
            else if (isMovingDown)
            {
                falling();
            }

        }

        private void jump()
        {
            Top -= 5;
            if (isMovingLeft && Left > Map.Left + 20)
            {
                Left -= 5;
            
            }if (isMovingRight && Left < Map.Width - 270)
            {
                Left += 5;
            }
        }

        private void falling()
        {
            Top += 5;

            if (isMovingLeft && Left > Map.Left + 20)
            {
                Left -= 5;
            }
            if (isMovingRight && Left < Map.Width - 270)
            {
                Left += 5;
            }
        }

        #endregion

        #region Shoot Bullet

        public void shoot()
        {
            bool bulletShouldGoLeft, bulletShouldGoRight;

            bulletShouldGoLeft = isLookingLeft;
            bulletShouldGoRight = isLookingRight;

            if (isMovingUp && !isMovingLeft)
                bulletShouldGoLeft = false;
           
            if (isMovingUp && !isMovingRight)
                bulletShouldGoRight = false;

            if (isMovingDown && !isMovingLeft)
                bulletShouldGoLeft = false;

            if (isMovingDown && !isMovingRight)
                bulletShouldGoRight = false;

            Bullet bullet = new Bullet(Map, this)
            {
                isGoingLeft = bulletShouldGoLeft,
                isGoingUp = isLookingUp,
                isGoingRight = bulletShouldGoRight,
                isGoingDown = isLookingDown
            };

            //On tire
            bullet.taskMovingBullet.Start();

        }

        #endregion
    
    }

}