using Contra.Map;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            if (map == null || shooter == null)
                return;
            
            index++;

            speed = 50;

            Map = map;
            Size = new Size(20, 20);
            SizeMode = PictureBoxSizeMode.StretchImage;
            Tag = BulletTag;

            Left = shooter.Left;
            Top = shooter.Top + shooter.Height / 3 - 7;

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

            isOutOfBox = (Left < 0) || (Left >= Map.Width) || (Top < 0) || (Top >= Map.Height);

            if (isOutOfBox && !end)
            {
                Map.Controls.Remove(this);
            }
        }

        #endregion

    }
}
