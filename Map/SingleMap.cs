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

        public SingleMap()
        {
            // 
            // Map
            // 
            Image = Properties.Resources.background_map;
            Tag = "SingleMap";
            Size = new Size(1328, 900);
            SizeMode = PictureBoxSizeMode.StretchImage;
            
        }
    }
}
