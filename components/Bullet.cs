using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Contra.Map;

namespace Contra.components
{
    public class Bullet : PictureBox
    {

        #region Properties

        public GameMap Map;

        public static int index { get; set; }

        private bool isOutOfBox;
        public int degat { get; set; }
        public bool isGoingLeft { get; set; }
        public bool isGoingUp { get; set; }
        public bool isGoingRight { get; set; }
        public bool isGoingDown { get; set; }
        public int speed { get; set; }

        public bool end { get; set; }

        //
        //Thread
        //
        public delegate void DelegateMovingTask();
        public DelegateMovingTask delegateMoving { get; set; }
        public Task taskMovingBullet { get; set; }

        public static object BulletTag = "bullet";

        #endregion

        #region Constructor

        public Bullet(GameMap map, PictureBox shooter)
        {
            index++;

            speed = 50;

            Map = map;
            Size = new Size(20,20);
            SizeMode = PictureBoxSizeMode.StretchImage;
            Tag = BulletTag;

            Left = shooter.Left;
            Top = shooter.Top + shooter.Height / 2 - 11;

            degat = 10;
            isGoingLeft = false;
            isGoingUp = false;
            isGoingRight = false;
            isGoingDown = false;

            isOutOfBox = false;
            end = false;

            Map.Controls.Add(this);
            BringToFront();
            shooter.BringToFront();

            taskMovingBullet = new Task(TaskBulletGoing);
            delegateMoving = new DelegateMovingTask(moveBullet);
        }

        #endregion

        #region Thread

        public void TaskBulletGoing()
        {
            Thread.CurrentThread.Name = "Task create : Bullet " + index;
            Console.WriteLine(Thread.CurrentThread.Name);

            while (!isOutOfBox && !end)
            {
                try
                {
                    Map.Invoke(delegateMoving);
                }
                catch (Exception) { }

                Thread.Sleep(speed);
            }

            Console.WriteLine(Thread.CurrentThread.Name + " : Is out of box");
           
        }

        public void moveBullet()
        {
            /*//Image
            if (isGoingLeft && !isGoingUp && !isGoingDown)
            {
                Image = Properties.Resources.bullet_left;

            }
            else if (isGoingRight && !isGoingUp && !isGoingDown)
            {
                Image = Properties.Resources.bullet_right;

            }
            else if (isGoingLeft && isGoingUp)
            {
                Image = Properties.Resources.bullet_top_left;

            }
            else if (isGoingRight && isGoingUp)
            {
                Image = Properties.Resources.bullet_top_right;
            }
            else if (isGoingLeft && isGoingDown)
            {
                Image = Properties.Resources.bullet_bottom_left;
            }
            else if (isGoingRight && isGoingDown)
            {
                Image = Properties.Resources.bullet_bottom_right;
            }
            else if (!isGoingLeft && !isGoingRight && isGoingUp && !isGoingDown)
            {
                Image = Properties.Resources.bullet_up;
            }
            else if (!isGoingLeft && !isGoingRight && !isGoingUp && isGoingDown)
            {
                Image = Properties.Resources.bullet_down;
            }*/

            if (isGoingLeft)
            {
                Left -= 30;
            }

            if (isGoingUp)
            {
                Top -= 30;
            }

            if (isGoingRight)
            {
                Left += 30;
            }

            if (isGoingDown)
            {
                Top += 30;
            }

            isOutOfBox = (Left < 0) || (Left >= Map.Width ) || (Top < 0) || (Top >= Map.Height);

            if (isOutOfBox && !end)
                Map.Controls.Remove(this);

        }

        #endregion

    }
}
