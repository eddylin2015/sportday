using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Collections;
using System.Text.RegularExpressions;




namespace SportDayLib
{
    public partial class Form_Input_RC_Client : Form
    {
        private OdbcConnection conn = null;
        private iForm_Input_RC_ACT iAct = null;

        private String psi_id="";
        private String pitem = "";
        private String prcx="";
        public Form_Input_RC_Client(string si_id,string item,string rcx,iForm_Input_RC_ACT Act):base()
        {
            InitializeComponent();
            psi_id = si_id;
            pitem = item;
            prcx = rcx;

            this.Text = String.Format("{0}:{1}", si_id, item);
            conn = new OdbcConnection(Basic_HTB_Info.Conn_Str);
            conn.Open();
            iAct = Act;
            this.lbl_GR.Text = iAct.Get_GR_STR(si_id, conn);
     
            if (!Basic_HTB_Info.signflag_sql.Contains(rcx)) throw new Exception(rcx);
            
            string feildnames = Basic_HTB_Info.signflag_sql[rcx].ToString();
            string c_feildnames = Basic_HTB_Info.Get_Signflag_s(rcx);

            string span_sr=null;
            Basic_HTB_Info.Adjust_FieldName(ref feildnames, ref c_feildnames,ref span_sr);

            string[] s_ar = c_feildnames.Split(',');
            
            int col_cnt = s_ar.Length;
            int row_cnt = iAct.Get_Item_Cnt(si_id, conn) + 1;
            this.tablePanel.ColumnCount = col_cnt;
            this.tablePanel.RowCount = row_cnt;
            iAct.Set_TablePanelStyle(tablePanel);
            iAct.Set_TablePanelColumnHead(tablePanel, s_ar);
            iAct.Set_TablePanel_TextBox_ForData(tablePanel, feildnames, si_id,conn);

            foreach (Control c in tablePanel.Controls)
            {
                if (iAct.Editable_Control(c))
                {
                    TextBox tb = (TextBox)c;
                    tb.TextChanged += new EventHandler(tb_TextChanged);
                    tb.KeyUp += new KeyEventHandler(tb_KeyUp);
                }

            }
        }

        void tb_KeyUp(object sender, KeyEventArgs e)
        {
            iAct.tablePanel_KeyUp(tablePanel, sender, e);
        }

        void tb_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string[] s_ar = tb.Name.Split('_');
            string rc_id = s_ar[s_ar.Length - 1];
            string field_name = s_ar[0];
            string value = tb.Text;
            String ErrorStr = iAct.VerifyFieldValueFormate(field_name, value);
            if(ErrorStr==null)
                iAct.tablePanel_TextChanged(tablePanel, conn, sender, e);
            errorProvider1.SetError(tb, ErrorStr);
        }

        private void Form_Input_RC_Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Basic_HTB_Info.cfg.temp_dir != null)
            {
                string html_filename = Basic_HTB_Info.cfg.temp_dir + "\\" + String.Format("{0}_{1}.htm", psi_id, pitem);

                RC_Html_Print h_o = new RC_Html_Print(psi_id, pitem, prcx, conn, html_filename,new RC_S_GR());
                try
                {
                    String url = "ftp://" + Basic_HTB_Info.cfg.DB_HOST + "/" + String.Format("{0}_{1}.htm", psi_id, pitem);
                    MPPNET.MPPFtp.ftp_up_file(url, h_o.html_filename, "sportday", "701100", Encoding.GetEncoding(950));
                }
                catch (Exception ftp_e)
                {
                    MessageBox.Show(ftp_e.Message+":多於一個用戶登入;");
                }
                Form_RC_OUT_RESULT out_r = new Form_RC_OUT_RESULT(h_o.html_filename);
                out_r.MdiParent = this.MdiParent;
                out_r.Show();
                
            }
            
            if (conn != null)
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}
