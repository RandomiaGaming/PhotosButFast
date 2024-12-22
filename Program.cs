using System;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace PhotosButFast
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            args = new string[1] { "D:\\ImportantData\\Adult Content Archive\\Hentai\\Test\\Untitled.png" };
            ImageViewerForm imageViewerForm = new ImageViewerForm();
            if (args.Length is 1)
            {
                imageViewerForm.OpenFile(args[0]);
            }
            Application.Run(imageViewerForm);
        }
    }
    public class ImageViewerForm : Form
    {
        private PictureBox pictureBox;

        private string folderPath;
        private ImageMeta[] folderContents;
        private int currentIndex;

        public ImageViewerForm()
        {
            BackColor = Color.Black;
            Width = Screen.PrimaryScreen.Bounds.Width / 2;
            Height = Screen.PrimaryScreen.Bounds.Height / 2;
            Location = new Point(Screen.PrimaryScreen.Bounds.Width / 4, Screen.PrimaryScreen.Bounds.Height / 4);

            pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(pictureBox);

            KeyPreview = true;
            KeyDown += OnKeyDown;
            ResizeBegin += OnWMSize;
            ResizeEnd += OnWMSize;
            Resize += OnWMSize;

            folderPath = null;
            folderContents = new ImageMeta[0];
            currentIndex = -1;
        }
        private void OnWMSize(object sender, EventArgs e)
        {
            RefreshPictureBox();
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                Next();
            }
            else if (e.KeyCode == Keys.Left)
            {
                Previous();
            }
            else if (e.KeyCode == Keys.O && e.Control)
            {
                Open();
            }
            else if (e.KeyCode == Keys.F5)
            {
                Reopen();
            }
        }
        private void RefreshPictureBox()
        {
            if (folderPath == null)
            {
                pictureBox.Image = null;
                return;
            }

            pictureBox.Image = folderContents[currentIndex].image;
            double inverseWindowAspect = ClientSize.Height / (double)ClientSize.Width;
            if (inverseWindowAspect > folderContents[currentIndex].aspectRatio)
            {
                pictureBox.Width = ClientSize.Width;
                pictureBox.Height = (int)(ClientSize.Width * folderContents[currentIndex].aspectRatio);
            }
            else
            {
                pictureBox.Width = (int)(ClientSize.Height / folderContents[currentIndex].aspectRatio);
                pictureBox.Height = ClientSize.Height;
            }
            pictureBox.Location = new Point((ClientSize.Width - pictureBox.Width) / 2, (ClientSize.Height - pictureBox.Height) / 2);
        }
        public void Next()
        {
            if (folderPath is null)
            {
                return;
            }
            currentIndex++;
            if (currentIndex >= folderContents.Length)
            {
                currentIndex -= folderContents.Length;
            }
            imagePath = folderContents[currentIndex];
            image = new Bitmap(imagePath);
            imageAspect = image.Width / (double)image.Height;
            RefreshPictureBox();
        }
        public void Previous()
        {
            if (folderPath is null)
            {
                return;
            }
            imageIndex--;
            if (imageIndex < 0)
            {
                imageIndex += folderContents.Length;
            }
            imagePath = folderContents[imageIndex];
            image = new Bitmap(imagePath);
            imageAspect = image.Width / (double)image.Height;
            RefreshPictureBox();
        }
        public void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();
            OpenFile(openFileDialog.FileName);
        }
        public void Reopen()
        {
            OpenFile(folderContents[imageIndex]);
        }
        public void OpenFile(string filePath)
        {
            imagePath = filePath;
            folderPath = Path.GetDirectoryName(filePath);
            Text = folderPath;
            folderContents = Directory.GetFiles(folderPath);
            for (int i = 0; i < folderContents.Length; i++)
            {
                if (folderContents[i].ToLower() == imagePath.ToLower())
                {
                    imageIndex = i;
                    break;
                }
            }
            image = new Bitmap(imagePath);
            imageAspect = image.Width / (double)image.Height;
            RefreshPictureBox();
        }
        private enum ImageMetaState
        {
            NotLoaded,
            Loading,
            Loaded,
            Unloading
        }
        private struct ImageMeta
        {
            public ImageMetaState state;
            public string filePath;
            public Bitmap image;
            public double aspectRatio;
        }
    }
}
