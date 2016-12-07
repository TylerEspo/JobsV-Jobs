using GTA;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsV_Jobs
{
    public class Debug
    {

         public static void CustomDraw(String text, int x, int y, Color color, GTA.Font font, float size)
        {
            
            UIText ui = new UIText(text, new Point(checked((int)Math.Round(unchecked((double)UI.WIDTH / x))), y), size, color, font, false);
            ui.Draw();
        }
    }
}
