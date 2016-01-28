using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SportDayLib
{
    public partial class Form_Reg_Stud_Item : Form
    {
        private OdbcConnection oconn=null;
        private String studref = null;
        private Font btnfont = new Font("新細明體", 18.0f);
        public Form_Reg_Stud_Item(String pstudref,String mtext)
        {
            InitializeComponent();
            studref = pstudref;
            this.Text = mtext;
            
            oconn = new OdbcConnection(Basic_HTB_Info.Conn_Str);
            oconn.Open();
            String cntsql = String.Format("Select count(*) from stud_gti where stud_ref ='{0}';", studref);
            OdbcDataReader dr = new OdbcCommand(cntsql, oconn).ExecuteReader();
            if (dr.Read())
            {
                int cnt = int.Parse(dr[0].ToString());
                if (cnt == 0)
                {
                    string upsql = String.Format("update student set logdatetime=now() where stud_ref='{0}';", studref);
                    OdbcCommand upcmd = new OdbcCommand(upsql, oconn);
                    upcmd.ExecuteNonQuery();
                    OdbcDataReader gtdr = new OdbcCommand(String.Format("SELECT gi FROM gt_item WHERE gt ='null' or gt IN (SELECT gt FROM `student` WHERE stud_ref = '{0}');", studref), oconn).ExecuteReader();
                    while (gtdr.Read())
                    {
                        String gi = gtdr[0].ToString();
                        OdbcCommand inscmd = new OdbcCommand(String.Format("insert into stud_gti values('{0}',{1},0)", studref, gi), oconn);
                        inscmd.ExecuteNonQuery();
                    }
                }
            }
            string gti_sql =String.Format( "select a.gi,b.gid,a.reg from stud_gti a inner join gt_item b on a.gi=b.gi where stud_ref='{0}';",studref);
            OdbcDataReader gti_dr = new OdbcCommand(gti_sql,oconn).ExecuteReader();
            while (gti_dr.Read())
            {
                CheckBox cb = new CheckBox();
                cb.Name = gti_dr[0].ToString();
                if (gti_dr[2].ToString() == "1")
                {
                    cb.Checked = true;
                }
                cb.Size = new System.Drawing.Size(160,160);
               
                cb.Font = btnfont;
                cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
                this.tableLayoutPanel1.Controls.Add(cb);
                Label lbl = new Label();
                lbl.Font = btnfont;
                lbl.Text = gti_dr[0].ToString() + gti_dr[1].ToString() + gti_dr[2];
                lbl.AutoSize = true;
                this.tableLayoutPanel1.Controls.Add(lbl);
            }
        }

        void cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            string gi = c.Name;
            string ch="0";
            if(c.Checked)
                ch="1";
            String sql = String.Format("update stud_gti set reg='{0}' where stud_ref='{1}' and gi='{2}';", ch, studref, gi);
            OdbcCommand cmd = new OdbcCommand(sql, oconn);
            cmd.ExecuteNonQuery();
            //throw new NotImplementedException();
        }

        private void Form_Reg_Stud_Item_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (oconn != null)
            {
                oconn.Close();
                oconn.Dispose();
            }
        }

        
    }
}
