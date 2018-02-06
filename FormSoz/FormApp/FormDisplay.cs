using BomberSoz.Core;
using BomberSoz.Model;
using BomberSoz.NewFolder1;
using FormSoz.Core;
using FormSoz.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormSoz.FormApp
{
    public class FormDisplay : IDisplay
    {

        private Form _mainForm;
        private MonsterImage _monsterImage;
        private BomberImage _bomberImage;
        private BackgroundImage _backgroundImage;
        private RemotBombUpgradeImage _remoteBombUpgrade;
        private MultibombUpgradeImage _multiBombUpgrade;
        private MoreFlameUpgradeImage _moreFlameUpgrade;
        private Flame_Image _flameImage;
        private WallImage _wallImage;
        private BombImage _bombImage;
        private NotRushWall _notRushImage;
        private Graphics gfx;
        private FormMap _map;
        private FormMap _prevMap;
        private BufferedGraphics _myBufer;
        private Bomber _bomber;
        private List<PictureBox> PictureBoxCollection = new List<PictureBox>();
        private SoundPlayer _mainSound;



        public FormDisplay(FormMap map, Form mainForm, Graphics gfx, BufferedGraphics myBufer, Bomber bomber, SoundPlayer mainSound)
        {

            this._map = map;
            this._mainForm = mainForm;
            this.gfx = gfx;
            _bomberImage = new BomberImage();
            _backgroundImage = new BackgroundImage();
            _wallImage = new WallImage();
            _notRushImage = new NotRushWall();
            _bombImage = new BombImage();
            _monsterImage = new MonsterImage();
            _remoteBombUpgrade = new RemotBombUpgradeImage();
            _multiBombUpgrade = new MultibombUpgradeImage();
            _moreFlameUpgrade = new MoreFlameUpgradeImage();
            _flameImage = new Flame_Image();
            _myBufer = myBufer;
            _bomber = bomber;
            _mainSound = mainSound;

        }
        public void CleanMap()
        {

        }

        public void DrawMap()
        {




            _myBufer.Graphics.DrawImage(Resources.background, 0, 0, _mainForm.Width, _mainForm.Height);
            _myBufer.Graphics.DrawImage(Resources.LifeCntR, 0, _map.Height * 45, 300, 300);
            ShowLive(_bomber.LiveCount);
            for (int rowCnt = 0; rowCnt <= _map.Width; rowCnt++)
            {
                for (int colCnt = 0; colCnt <= _map.Height; colCnt++)
                {
                    var item = _map.ArrayMap[colCnt, rowCnt];






                    var wall = item as Wall;
                    if (wall != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.block, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);


                        continue;

                    }
                    var bombItem = item as Bomb;
                    if (bombItem != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.bomb, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }

                    var wallUpgradeFar = item as WallWithUpgradeFar;
                    if (wallUpgradeFar != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.block, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }
                    var wallUpgradeRemote = item as WallWithUpgradeRemote;
                    if (wallUpgradeRemote != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.block, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);

                        continue;
                    }
                    var wallUpgradeMultiBomb = item as WallWithUpgradeMultiBomb;
                    if (wallUpgradeMultiBomb != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.block, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }
                    var upgradeFar = item as UpgradeFar;
                    if (upgradeFar != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.flame_upgrade, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt, ImagePath.IMAGE_SIZE, ImagePath.IMAGE_SIZE);
                        continue;
                    }
                    var upgradeRemote = item as UpgradeRemote;
                    if (upgradeRemote != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.remote_bomb_upgrade, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }
                    var upgradeMultiBomb = item as UpgradeMultiBomb;
                    if (upgradeMultiBomb != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.bomb_upgrade, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt, ImagePath.IMAGE_SIZE, ImagePath.IMAGE_SIZE);
                        continue;
                    }
                    var space = item as Space;
                    if (space != null)
                    {


                        continue;
                    }

                    var monster_fly = item as MonsterFly;
                    if (monster_fly != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.monster, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }
                    var monster_not_fly = item as MonsterNotFly;
                    if (monster_not_fly != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.monster, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }
                    var monsterClever = item as MonsterClever;
                    if (monsterClever != null)
                    {
                        _myBufer.Graphics.DrawImage(Resources.monster, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }

                    var bomber = item as Bomber;
                    if (bomber != null)
                    {

                        _myBufer.Graphics.DrawImage(Resources.bomberman, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);

                        continue;
                    }

                    var wallAngle = item as WallAngle;
                    if (wallAngle != null)
                    {

                        _myBufer.Graphics.DrawImage(Resources.dont_rush_block, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);

                        continue;
                    }
                    var wallVert = item as WallVerticale;
                    if (wallVert != null)
                    {

                        _myBufer.Graphics.DrawImage(Resources.dont_rush_block, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }

                    var wallHoriz = item as WallHorizontal;
                    if (wallHoriz != null)
                    {

                        _myBufer.Graphics.DrawImage(Resources.dont_rush_block, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }
                    var bombBoom = item as BombBoom;
                    if (bombBoom != null)
                    {

                        _myBufer.Graphics.DrawImage(Resources.flame, ImagePath.IMAGE_SIZE * rowCnt, ImagePath.IMAGE_SIZE * colCnt);
                        continue;
                    }

                }
            }
            _myBufer.Render(gfx);
        }





        public void ShowGameOver()
        {
            
            _myBufer.Graphics.DrawImage(Resources.gameOver, 0, 0, _mainForm.Width, _mainForm.Height);
            _myBufer.Render(gfx);
        }

        public void ShowLive(int liveCount)
        {
            switch (liveCount)
            {
                case 1:
                    {
                        _myBufer.Graphics.DrawImage(Resources.live1, 100, _map.Height * 55, 200, 200);
                        break;
                    }
                case 2:
                    {
                        _myBufer.Graphics.DrawImage(Resources.live2, 100, _map.Height * 55, 200, 200);
                        break;
                    }
                case 3:
                    {
                        _myBufer.Graphics.DrawImage(Resources.live3, 100, _map.Height * 55, 200, 200);
                        break;
                    }
            }
        }

        public void ShowYouWin()
        {

            _myBufer.Graphics.DrawImage(Resources.youWin, 0, 0, _mainForm.Width, _mainForm.Height);
            _myBufer.Render(gfx);
        }

        public void SignalOnDead()
        {
            ;
            SoundPlayer simpleSound = new SoundPlayer(Resources.die);
            simpleSound.Play();

        }
        public void SignalOnBombBoom()
        {
            SoundPlayer simpleSound = new SoundPlayer(Resources.exp);
            simpleSound.Play();
        }

        public void SignalOnGetBonus()
        {
            SoundPlayer simpleSound = new SoundPlayer(Resources.bonus);
            simpleSound.Play();
        }
    }
}
