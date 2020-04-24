using System.Drawing;
using System.Windows.Forms;

namespace Processing
{
    public partial class CanvasForm : Form
    {
        public PictureBox FormPictureBox => pictureBox;

        public CanvasForm()
        {
            InitializeComponent();
        }

        public void SetSize(int width, int height)
        {
            pictureBox.Size = new Size(width, height);
            Size = new Size(width + 16, height + 39);
        }
    }
}