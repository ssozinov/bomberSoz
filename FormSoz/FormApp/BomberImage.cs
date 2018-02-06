using FormSoz.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSoz.FormApp
{
    public class BomberImage:WorkImage
    {
        public BomberImage():base (Resources.bomberman)
            {
            _bomberSpot.X =X + 100;
            _bomberSpot.Y = Y + 100;
            _bomberSpot.Width = 50;
            _bomberSpot.Height = 50;
            }
           


            public Rectangle _bomberSpot = new Rectangle(); 

        public void Update (int x,int y)
        {
            X = x;
            Y = y;
            _bomberSpot.X = X;
            _bomberSpot.Y = Y;
          
        }

    
    }
}
