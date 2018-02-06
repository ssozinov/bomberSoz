using BomberSoz.NewFolder1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormSoz;

namespace FormSoz.FormApp
{
    public class FormGamePad : IGamePad
    {
        public Action PressLeft { get; set; }
        public Action PressRight { get; set; }
        public Action PressUp { get; set; }
        public Action PressDown { get; set; }


        public Action PressSpace { get; set; }
        public Action PressR { get; set; }
        public Action PressP { get; set; }
        public void Start()
        {



         

        }
        public void Start(KeyEventArgs e)
        {



          

        }
    }
}
