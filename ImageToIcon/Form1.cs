using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wmgCMS
{
    public partial class Form1 : Form
    {

        private String[] imageToConvertPath;

        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
        }

        public void convert()
        {
            if (imageToConvertPath != null) {
                Bitmap thumb = (Bitmap)Image.FromFile(imageToConvertPath[0]);
                int w;
                int h;
                if (tbHeight.Text != "" && tbWidth.Text != "")
                {
                    w = Int32.Parse(tbWidth.Text);
                    h = Int32.Parse(tbHeight.Text);
                    thumb = ResizeImage(thumb, w, h);
                    
                } else {
                    w = thumb.Width;
                    h = thumb.Height;
                }
                thumb.MakeTransparent();

                Graphics g = Graphics.FromImage(thumb); // allow drawing to it
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic; // make the image shrink nicely by using HighQualityBicubic mode
                g.DrawImage(thumb, 0, 0, w, h);
                g.Flush();

                //Generate save file dialog
                SaveFileDialog sfd = new SaveFileDialog();
                //Set dialog filter
                sfd.Filter = "Icon (*.ico)|*.ico|All files (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Stream IconStream = System.IO.File.Create(sfd.FileName);

                    //DEPRECATED DUE TO LOW QUALITY ICON
                    /*Icon icon = Icon.FromHandle(thumb.GetHicon());
                    icon.Save(IconStream);
                    IconStream.Close();*/

                    Icon icon = ImageHelper.Convert(thumb, new Size(w, h));
                    this.Icon = icon;
                    this.Icon.Save(IconStream);
                    IconStream.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select an image by dragging it into the window or by clicking 'Browse...'");
            }
        }

        //Handles cursor effects when dragging over data
        private void panel5_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        //Handles what to do when data is dropped. Bitmap var is assigned the image.
        private void panel5_DragDrop(object sender, DragEventArgs e)
        {
            imageToConvertPath = (String[])e.Data.GetData(DataFormats.FileDrop);
            //MessageBox.Show(imageToConvertPath[0]);
            pictureBox1.BackgroundImage = Image.FromFile(imageToConvertPath[0]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            convert();
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void tbWidth_Enter(object sender, EventArgs e)
        {
            panel3.BackColor = ColorTranslator.FromHtml("#3E7EF8");
            panel4.BackColor = Color.DarkGray;
        }

        private void tbHeight_Enter(object sender, EventArgs e)
        {
            panel3.BackColor = Color.DarkGray;
            panel4.BackColor = ColorTranslator.FromHtml("#3E7EF8");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.BackgroundImage = Image.FromFile(ofd.FileName);
                } catch(Exception i)  {
                    MessageBox.Show("Invalid image file. Please select a valid file. Exception: " + i.Message);
                }

            
                imageToConvertPath = new String[1]; //Must instantiate array to prevent null reference exception
                imageToConvertPath[0] = ofd.FileName;
            }
        }

        private void panel5_Click(object sender, EventArgs e)
        {
            //Allow user to click on upload area to select file
            button1_Click(this, new EventArgs());
        }
    }
}
