using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace Contra.Map
{
    public class GameMap : PictureBox
    {
        # region Properties
        
        public List<SingleMap> maps;
        public int numberOfMaps;
        public TabPage parent;

        public Label scoreLabel { get; set; }
        public ProgressBar lifeBar { get; set; }
        public Label playerName { get; set; }
        public PictureBox heartLifeIcon { get; set; }
        
        public bool couldMove;
        public Directions direction;

        public Thread ThreadMovingMap;
        public delegate void DelegateThreadMovingMap();
        public DelegateThreadMovingMap delegateThreadMoveMap;
        public bool end { get; set; }

        #endregion

        #region Constructor
        public GameMap()
        {
            maps = new List<SingleMap>();
            numberOfMaps = 4;

            end = false;

            Image = Properties.Resources.background_map;
            SizeMode = PictureBoxSizeMode.StretchImage;
            Size = new Size(900, 900);
            Location = new Point(0, 0);

            for(int i = 0; i < numberOfMaps; i++)
            {
                maps.Add( new SingleMap() );
                Controls.Add(maps[i]);
            }

            #region Score Label
            // 
            // scoreLabel
            //
            scoreLabel = new Label();

            scoreLabel.Font = new Font("Ravie", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            scoreLabel.ForeColor = Color.Ivory;
            scoreLabel.BackColor = Color.Transparent;
            scoreLabel.Location = new Point(6, 6);
            scoreLabel.Name = "scoreLabel";
            scoreLabel.Size = new Size(149, 26);
            scoreLabel.Text = "Score : 000";
            #endregion

            #region Heart Life Icon
            // 
            // Heart Life Icon
            //
            heartLifeIcon = new PictureBox();

            heartLifeIcon.Image = Properties.Resources.heart_full;
            heartLifeIcon.BackColor = Color.Transparent;
            heartLifeIcon.Location = new Point(775, 6);
            heartLifeIcon.Name = "heartLifeIcon";
            heartLifeIcon.Size = new Size(27, 23);
            heartLifeIcon.SizeMode = PictureBoxSizeMode.StretchImage;

            #endregion
            
            #region Player Name

            playerName = new Label();

            playerName.Font = new Font("Courier New", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            playerName.ForeColor = Color.Ivory;
            playerName.BackColor = Color.Transparent;
            playerName.Location = new Point(775, 6);
            playerName.Name = "PlayerName";
            playerName.Size = new Size(149, 26);
            playerName.Text = "Player 1";

            #endregion

            #region Life Bar
            // 
            // lifeBar
            // 
            lifeBar = new ProgressBar();

            //lifeBar.BackColor = Color.Gray;
            lifeBar.ForeColor = Color.Red;
            lifeBar.Location = new Point(790, 5);
            lifeBar.Name = "lifeBar";
            lifeBar.Size = new Size(212, 23);
            lifeBar.Style = ProgressBarStyle.Continuous;
            lifeBar.Value = 100;
            #endregion

            Controls.Add(scoreLabel);
            Controls.Add(playerName);
            Controls.Add(heartLifeIcon);
            Controls.Add(lifeBar);

            playerName.BringToFront();
            scoreLabel.BringToFront();
            heartLifeIcon.BringToFront();
            lifeBar.BringToFront();

            #region Thread

            ThreadMovingMap = new Thread(ThreadMoving);
            delegateThreadMoveMap = new DelegateThreadMovingMap(MoveMap);

            couldMove = false;
            //ThreadMovingMap.Start();

            #endregion

        }

        #endregion

        #region Reload

        public void reload()
        {
            int i = 0;
            foreach (SingleMap map in maps)
            {
                //SingleMap
                map.Left = i * map.Width;
                i++;
            }
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

        #endregion

        #region Thread
        public void ThreadMoving()
        {
            Thread.CurrentThread.Name = "GameMapThread";
            Console.WriteLine(Thread.CurrentThread.Name + " : Moving start thread... ");

            while (!end)
            {

                if (couldMove)
                {
                    Parent.Invoke(delegateThreadMoveMap);
                    Console.WriteLine(Thread.CurrentThread.Name + " : Moving... ");
                }

                if (end)
                {
                    return;
                }

                Thread.Sleep(200);
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

                    for(int i=0; i < maps.Count; i++)
                    {
                        maps[i].Left += 15;
                    }

                    break;

                case Directions.Right:

                    for (int i = 0; i < maps.Count; i++)
                    {
                        maps[i].Left -= 15;
                    }

                    break;
            }

            }

            Console.WriteLine(Thread.CurrentThread.Name + " : MoveMap End.");
        }

        #endregion

    }
}
