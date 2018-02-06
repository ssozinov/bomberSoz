using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BomberSoz.NewFolder1;
using System.Configuration;
using BomberSoz;
using BomberSoz.Manager;
using BomberSoz.ConsoleApp;
using FormSoz.FormApp;
using System.Diagnostics;
using BomberSoz.Core;
using FormSoz.Core;
using System.Media;
using FormSoz.Properties;

namespace FormSoz
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public MotionController mC;
        public FormGamePad gamepadForm;
        public FormDisplay displayForm;
        BomberImage bI = new BomberImage();
        private void MainForm_Load(object sender, EventArgs e)
        {
            BufferedGraphicsContext currentContext;
            BufferedGraphics myBuffer;

            SoundPlayer mainSound = new SoundPlayer(Resources.go_goom);
            //  mainSound.PlaySync();

            currentContext = BufferedGraphicsManager.Current;

            myBuffer = currentContext.Allocate(this.CreateGraphics(),
               this.DisplayRectangle);
            var mapManager = new MapManager(ConfigurationManager.AppSettings["pathToMapDirectory"], ConfigurationManager.AppSettings["pathToMapFile"]);
            var map = new FormMap(int.Parse(ConfigurationManager.AppSettings["widthField"]), int.Parse(ConfigurationManager.AppSettings["heightField"]), int.Parse(ConfigurationManager.AppSettings["timeToBoom"]));
            var Bomber = new BomberSoz.Model.Bomber(int.Parse(ConfigurationManager.AppSettings["liveCount"]), map.FirstPositionBomber);
            this.Size = new System.Drawing.Size((map.Width + 1) * ImagePath.IMAGE_SIZE + 20, (map.Height + 1) * ImagePath.IMAGE_SIZE + 150);
            map.Load(mapManager.GetFirstMap());
            map.SetUpgradeOnMap(1, 1, 1);
            this.DoubleBuffered = true;
            displayForm = new FormDisplay(map, this, this.CreateGraphics(), myBuffer, Bomber, mainSound);
            gamepadForm = new FormGamePad();
            mC = new MotionController(displayForm, gamepadForm, map, Bomber, mapManager);

            mC.SetPositionForPerson(2, 1);
            mC.SetPositionForPerson(3, 2);
            mC.SetPositionForPerson(4, 1);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();

            sw.Stop();
            Logging.WriteTrace("Первая отрисовка" + (sw.ElapsedMilliseconds / 100.0).ToString());
            Task.Run(() => mC.Start());






        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {



        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {

                case Keys.Left:
                    {
                        gamepadForm.PressLeft();
                        ;
                        break;
                    }

                case Keys.Right:
                    {
                        gamepadForm.PressRight();

                        break;
                    }

                case Keys.Down:
                    {
                        gamepadForm.PressDown();

                        break;
                    }
                case Keys.Up:
                    {
                        gamepadForm.PressUp();

                        break;
                    }

                case Keys.Space:
                    {
                        gamepadForm.PressSpace();
                        break;
                    }
                case Keys.R:
                    {
                        gamepadForm.PressR();
                        break;
                    }

                case Keys.P:
                    {
                        gamepadForm.PressP();
                        break;
                    }

            }


        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            displayForm.DrawMap();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }


        protected override void OnPaint(PaintEventArgs e)

        {





        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}


