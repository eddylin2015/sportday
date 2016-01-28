using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SportDayLib
{
    public class InputTextBox:System.Windows.Forms.Form
    {
        private System.Windows.Forms.FlowLayoutPanel pl = new System.Windows.Forms.FlowLayoutPanel();
        private System.Windows.Forms.Button oktb = new System.Windows.Forms.Button();
        public System.Windows.Forms.TextBox tb = new System.Windows.Forms.TextBox();
        public InputTextBox()
        {
            pl.Controls.Add(tb);
            oktb.DialogResult = DialogResult.OK;
            oktb.Text = "OK";
            pl.Controls.Add(oktb);
            this.Controls.Add(pl);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // InputTextBox
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "InputTextBox";
            this.ResumeLayout(false);

        }
    }
}
