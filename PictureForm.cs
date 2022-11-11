using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class PictureForm : Form
    {
        Image picture;

        public Image Picture
        {
            get
            {
                return picture;
            }
            set
            {
                picture = value;
                pictureBox1.Image = picture;
            }
        }
        
        public PictureForm()
        {
            InitializeComponent();

        }

        private void PictureForm_Load(object sender, EventArgs e)
        {         

        }

        
    }
}
