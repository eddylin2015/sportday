using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;

namespace SportDayLib
{
    public partial class Form_Search : Form
    {
        public Form_Search()
        {
            InitializeComponent();
        }
        private System.Drawing.Size btnsize = new System.Drawing.Size(300, 60);
        private Font btnfont = new Font("新細明體", 12.0f);
        private void button1_Click(object sender, EventArgs e)
        {
 

            using(OdbcConnection oconn=new OdbcConnection(Basic_HTB_Info.Conn_Str)){
                oconn.Open();
                Form_Search_Res fsr = new Form_Search_Res();
                fsr.MdiParent = Basic_HTB_Info.cfg.MDIparent;
            if (textBox1.Text.Length > 0)
            {
                string sql = String.Format("select stud_ref,name_c,name_p,grade,classno,c_no from student where stud_ref like '{0}' ", textBox1.Text);
                binding_btn(fsr, sql, oconn);
            }
            if (textBox2.Text.Length > 0)
            {
                string sql = String.Format("select stud_ref,name_c,name_p,grade,classno,c_no from student where code like '{0}' ", textBox2.Text);
                binding_btn(fsr, sql, oconn);
            }
            if (textBox3.Text.Length > 0)
            {
                string sql = String.Format("select stud_ref,name_c,name_p,grade,classno,c_no from student where name_c like '{0}' ", textBox3.Text);
                binding_btn(fsr, sql, oconn);
            }
            if (textBox4.Text.Length > 0)
            {
                String grade = textBox4.Text.Substring(0, textBox4.Text.Length - 1).ToUpper();
                String classno = textBox4.Text[textBox4.Text.Length - 1].ToString().ToUpper();
                string sql = String.Format("select stud_ref,name_c,name_p,grade,classno,c_no from student where grade='{0}' and classno='{1}' ", grade,classno);
                binding_btn(fsr, sql, oconn);
            }
                oconn.Close();
                oconn.Dispose();
                fsr.Show();
            }
        }
        private void binding_btn(Form_Search_Res fsr, String sql, OdbcConnection oconn)
        {
            OdbcDataReader dr = new OdbcCommand(sql, oconn).ExecuteReader();
            while (dr.Read())
            {
                String btntxt = String.Format("{0}:{1}:{2}:{3}{4}{5}", dr[0], dr[1], dr[2], dr[3], dr[4], dr[5]);
                Button btn = new Button();
                btn.Text = btntxt;
                btn.Size = btnsize;
                btn.Font = btnfont;
                btn.Click += btn_click;
                fsr.flowLayoutPanel1.Controls.Add(btn);
            }
        }
        private void btn_click(Object sender, EventArgs e)
        {
                Control c = (Control)sender;
                String studref = c.Text.Split(':')[0];
                Form_Reg_Stud_Item frsi = new Form_Reg_Stud_Item(studref,c.Text);
                frsi.MdiParent = Basic_HTB_Info.cfg.MDIparent;
                frsi.Show();


            
        }


    }
}
