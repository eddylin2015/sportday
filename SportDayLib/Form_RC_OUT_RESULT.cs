using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SportDayLib
{
    public partial class Form_RC_OUT_RESULT : Form
    {
        public Form_RC_OUT_RESULT(string filename):base()
        {
            InitializeComponent();
            this.webBrowser1.Navigate(filename);
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Refresh();

        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }
    }
}
