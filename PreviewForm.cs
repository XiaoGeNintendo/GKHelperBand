using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GKHelper
{
    public partial class PreviewForm : Form
    {
        public PreviewForm()
        {
            InitializeComponent();
        }

        private void PreviewForm_Load(object sender, EventArgs e)
        {
            var myc = new TaskbarApplet();
            this.Controls.Add(myc);
            this.Width = 476;
            this.Height = 45;

        }
    }
}
