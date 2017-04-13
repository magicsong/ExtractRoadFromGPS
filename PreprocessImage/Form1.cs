using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace PreprocessImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap curBitmap;
        private void 去除黑边ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap curBitmap = pictureBox1.Image as Bitmap;
            int width = curBitmap.Width;
            int height = curBitmap.Height;
            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = curBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//curBitmap.PixelFormat

            IntPtr ptr = bmpData.Scan0;
            int bytesCount = bmpData.Stride * bmpData.Height;
            byte[] arrDst = new byte[bytesCount];
            System.Runtime.InteropServices.Marshal.Copy(ptr, arrDst, 0, bytesCount);

            for (int i = 0; i < bytesCount; i += 3)
            {
                //这里处理图片
                byte colorTemp = (byte)(arrDst[i + 2] * 0.299 + arrDst[i + 1] * 0.587 + arrDst[i] * 0.114);
                arrDst[i] = arrDst[i + 1] = arrDst[i + 2] = (byte)colorTemp;

            }
            System.Runtime.InteropServices.Marshal.Copy(arrDst, 0, ptr, bytesCount);
            curBitmap.UnlockBits(bmpData);
            pictureBox1.Image = curBitmap;

        }

        private void cCLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curBitmap = pictureBox1.Image as Bitmap;
            AForge.Imaging.Filters.Threshold th = new AForge.Imaging.Filters.Threshold(120);

            MyNewCCLFilter.ConnectedComponentsLabeling ccl = new MyNewCCLFilter.ConnectedComponentsLabeling();
            curBitmap = ccl.Apply(th.Apply(curBitmap));
            pictureBox1.Image = curBitmap;
        }

        private void shrinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shrink sh = new Shrink(Color.Black);
            curBitmap = sh.Apply(curBitmap);
            pictureBox1.Image = curBitmap;

        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curBitmap.Save("结果.bmp");
        }
    }
}
