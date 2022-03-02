using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Contra.Map
{
    public class SingleMap : PictureBox
    {
        public Ground ground { get; set; }

        public SingleMap()
        {
            // 
            // Map
            // 
            Image = Properties.Resources.background_map;
            Tag = "SingleMap";
            Size = new Size(982, 514);
            SizeMode = PictureBoxSizeMode.StretchImage;
            
            ground = new Ground(this);
            
        }
    }
}
