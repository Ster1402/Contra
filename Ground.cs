using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using Contra.Map;

namespace Contra
{
    public class Ground
    {
        public List<Platform> platforms;
        public SingleMap map;

        public Ground(SingleMap map)
        {
            this.map = map;

            platforms = new List<Platform>();

            for(int i=0; i < 14; i++)
            {
                if (i % 5 == 0) continue;

                platforms.Add(new Platform(this.map, i * 118, 260));
            }

        }

    }

    public class Platform : PictureBox
    {
        public SingleMap map;

        public Platform(SingleMap map, int left, int top)
        {
            this.map = map;
            Left = left;
            Image = Properties.Resources.Platform;
            SizeMode = PictureBoxSizeMode.StretchImage;
            Top = top;
            Size = new Size(118, 50);

            this.map.Controls.Add(this);
        }
    }

}
