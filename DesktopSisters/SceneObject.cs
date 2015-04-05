using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*namespace DesktopSistersCSharpForm
{
    public class SceneObject
    {
        public SceneObject(String fileName, Rectangle rect, int zplane)
        {
            FileName = fileName;
            Rect = rect;
            Plane = zplane;
        }

        public int Plane { get; set; }

        private string FileName { get; }
        private Rectangle Rect { get; }

        public virtual void Update() { }


        public virtual void Draw(Graphics g, Image imageOverride = null)
        {
            var image = imageOverride;

            if (image == null)
            {
                image = ImageController.LoadEventImage(FileName);
            }

            if (image == null)
                return;

            g.DrawImage(image, Rect);
        }

    }
}*/
