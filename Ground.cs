using Contra.Map;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Contra
{
    public class Ground
    {
        public List<Platform> platforms;
        public GameMap map;

        public Ground(GameMap map)
        {
            this.map = map;

            platforms = new List<Platform>();

            int platformsWidth = this.map.platformWidth = this.map.Width / 10;

            for (int i = 0; i < 10 * map.numberOfMaps + 4; i++)
            {
                platforms.Add(new Platform(this.map, i * platformsWidth, 260));

            }

        }

    }

    public class Platform : PictureBox
    {
        public GameMap map;

        public Platform(GameMap map, int left, int top)
        {
            this.map = map;
            Left = left;
            Image = Properties.Resources.Platform;
            SizeMode = PictureBoxSizeMode.StretchImage;
            Top = top;

            int platformsWidth = this.map.Width / 10;
            Size = new Size(platformsWidth, 50);

            this.map.Controls.Add(this);
            BringToFront();
        }
    }

}
