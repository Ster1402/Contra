using Contra.components;
using Contra.Map;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Contra
{
    public class Enemy : PictureBox
    {

        #region Properties

        public DateTime coolDown { get; set; }

        public static int index;
        public int rank { get; set; }
        public bool couldUpgrade { get; set; }

        public Bitmap imageLeft { get; set; }
        public Bitmap imageRight { get; set; }


        public Bitmap bullet_image { get; set; }
        public int degat { get; set; }

        public static object tag { get; set; }

        public ProgressBar life { get; set; }

        public bool end { get; set; }

        public int speed { get; set; }

        public bool isLookingUp { get; set; }
        public bool isLookingDown { get; set; }
        
        public bool isMovingLeft { get; set; }
        public bool isMovingRight { get; set; }

        public static object BulletTag = "enemy_bullet";

        //Thread
        public Thread threadMoveEnemy { get; set; }
        public delegate void DelegateMoveEnemy();
        public DelegateMoveEnemy delegateMoveEnemy { get; set; }

        #endregion

        #region Constructors

        public Enemy()
        {
            index++;

            rank = 1;
            couldUpgrade = true;
            degat = 5;
            bullet_image = Properties.Resources.bullet_rank_1;

            isLookingDown = isLookingUp = false;

            Size = new Size(65, 65);
            tag = Tag = "Enemy";

            imageLeft = Properties.Resources.enemy;
            imageRight = Properties.Resources.enemy_rank_1_right;

            Image = Properties.Resources.enemy;
            SizeMode = PictureBoxSizeMode.StretchImage;

            life = new ProgressBar
            {
                Size = new Size(Width, 15),
                BackColor = Color.Red,
                ForeColor = Color.DarkGreen,
                Maximum = 100,
                Value = 100
            };

            Controls.Add(life);

            life.Top = -5;
            life.Left = 0;

            life.BringToFront();


            speed = 10;
            end = false;

            isMovingLeft = true;
            isMovingRight = false;

            threadMoveEnemy = new Thread(MoveEnemyThread);
            delegateMoveEnemy = new DelegateMoveEnemy(MoveEnemy);

        }

        #endregion

        public void subirDegat(int degat)
        {
            life.Value = ((life.Value - degat) >= 0) ? (life.Value - degat) : 0;
        }

        public void upgrade()
        {
            if (couldUpgrade)
            {

                if (rank == 1)
                {
                    life.Maximum = 200;
                    life.Value = 200;

                    imageLeft = Properties.Resources.enemy_rank_2;
                    imageRight = Properties.Resources.enemy_rank_2_right;

                    rank = 2;
                    degat = 10;
                    Image = Properties.Resources.enemy_rank_2;
                    bullet_image = Properties.Resources.bullet_rank_2;
                }
                else if (rank == 2)
                {
                    life.Maximum = 300;
                    life.Value = 300;

                    imageLeft = Properties.Resources.enemy_rank_3;
                    imageRight = Properties.Resources.enemy_rank_3_right;

                    rank = 3;
                    degat = 20;
                    Image = Properties.Resources.enemy_rank_3;
                    bullet_image = Properties.Resources.bullet_rank_3;
                }
                else if (rank == 3)
                {
                    degat = 25;
                    couldUpgrade = false;
                    bullet_image = Properties.Resources.bullet_rank_3_above;
                }

            }

        }

        #region Task Move Enemy

        public void MoveEnemyThread()
        {
            Thread.CurrentThread.Name = "Enemy " + index;
            Console.WriteLine(Thread.CurrentThread.Name);

            int i = 0;

            while (!end)
            {
                Thread.Sleep(500);

                if (life.Value == 0)
                {
                    end = true;
                }

                if (end)
                {
                    return;
                }

                try
                {

                    if (isMovingLeft)
                    {

                        i = 0;
                        while (i <= 3)
                        {
                            Thread.Sleep(150);
                            try
                            {
                                if (end)
                                {
                                    return;
                                }

                                Parent.Invoke(delegateMoveEnemy);

                            }
                            catch (NullReferenceException) { }

                            i++;
                        }

                        isMovingLeft = false;
                        isMovingRight = true;

                        Thread.Sleep(2500);
                    }
                    else if (isMovingRight)
                    {
                        i = 0;
                        while (i <= 3)
                        {
                            Thread.Sleep(150);

                            try
                            {
                                if (end)
                                {
                                    return;
                                }

                                Parent.Invoke(delegateMoveEnemy);

                            }
                            catch (NullReferenceException) { }

                            i++;
                        }

                        isMovingLeft = true;
                        isMovingRight = false;

                        Thread.Sleep(2500);
                    }

                }
                catch (Exception) { }
            }

        }
        public void MoveEnemy()
        {
            if (isMovingLeft)
            {
                Image = imageRight;

                Left -= 11;
            }
            else
            {
                Image = imageLeft;

                Left += 11;
            }

        }

        #endregion

        #region Shoot Bullet
        public void shoot()
        {
            if (coolDown == null || coolDown > DateTime.Now)
                return;

            if (Parent == null)
                return;

            bool bulletShouldGoLeft = isMovingLeft, bulletShouldGoRight = isMovingRight;

            Bullet bullet = new Bullet((GameMap)Parent, this)
            {
                degat = this.degat,
                isGoingLeft = bulletShouldGoLeft,
                isGoingRight = bulletShouldGoRight,
                isGoingDown = isLookingDown,
                isGoingUp = isLookingUp,
                Tag = BulletTag,
                Image = bullet_image
            };

            try
            {
                bullet.taskMovingBullet.Start();
            }catch (Exception) { }   

        }

        #endregion
    }
}
