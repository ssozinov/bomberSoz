using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSoz.FormApp
{
    public class WorkImage : IDisposable
    {


        private bool disposed = false;
        Bitmap _bitmap;
        public int X { get; set; }
        public int Y { get; set; }
        public WorkImage(Bitmap _resourse)

        {
            this._bitmap = new Bitmap(_resourse);
        }
        public void DrawImage(Graphics gfx)
        {

            gfx.DrawImage(_bitmap, X, Y);



        }
        public void Dispose()
        {
            Dispose(true);


            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                _bitmap.Dispose();
            }
            disposed = true;
        }

    }
}
