using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BomberSoz.NewFolder1
{
    public interface IGamePad
    {
        Action PressLeft { get; set; }
        Action PressRight { get; set; }
        Action PressUp { get; set; }
        Action PressDown { get; set; }
        Action PressP { get; set; }
        Action PressSpace { get; set; }
        Action PressR { get; set; }
        void Start();
       

    }
}
