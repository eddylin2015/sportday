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
using System.IO;
namespace SportDayLib
{
    public partial class Form_Input_Field_RC_Admin : Form
    {
        private OdbcConnection conn = null;
        private iForm_Input_RC_ACT iAct = null;

        private String psi_id="";
        private String pitem = "";
        private String prcx="";
        public Form_Input_Field_RC_Admin(string si_id, string item, string rcx, iForm_Input_RC_ACT Act)
            : base()
        {
            InitializeComponent();
            psi_id = si_id;
            pitem = item;
            prcx = rcx;

            this.Text = String.Format("{0}:{1}:{2}", si_id, item,rcx);
            conn = new OdbcConnection(Basic_HTB_Info.Conn_Str);
            conn.Open();
            iAct = Act;
            this.lbl_GR.Text = iAct.Get_GR_STR(si_id, conn);
            string feildnames = null;
            string c_feildnames=null;
            if (pitem.Contains("跳高"))
            {
                feildnames = "frc_id,rcx,rank,s_number,number,classno,name,h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11,h12,h13,h14,h15,h16,rc,note";
                c_feildnames="名次,次序,比賽號,姓名,班級,";
                c_feildnames += "h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11,h12,h13,h14,h15,h16,";
                c_feildnames+="成績,備註";
            }
            else
            {
                feildnames = "frc_id,rcx,rank,s_number,number,classno,name,h1,h2,h3,b3,h4,h5,b5,h6,rc,note";
                c_feildnames="名\n次,次\n序,比賽\n號,姓名,班級,一,二,三,B3,四,五,B5,六,成績,備註";
            }
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
            {
                iAct.tablePanel_TextChanged(tablePanel, conn, sender, e);
            }
            errorProvider1.SetError(tb, ErrorStr);
        }

        private void Form_Input_RC_Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Basic_HTB_Info.cfg.temp_dir != null)
            {
               
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

        private void button1_Click(object sender, EventArgs e)
        {
            string html_filename = Basic_HTB_Info.baseFilePath + "\\temp\\" + String.Format("{0}_{1}.htm", psi_id, pitem);
            Field_Html_Print h_o = null;
            Field_Html_Print h_o_xls = null;
            if (prcx == "RCFJH") 
            {
                h_o = new FieldJH_Html_Print(psi_id, pitem, prcx, conn, html_filename,new RC_F_GR(),false);
                h_o_xls = new FieldJH_Html_Print(psi_id, pitem, prcx, conn, html_filename.Replace(".htm",".xls"), new RC_F_GR(), false);
            }
            else
            {
                h_o=new Field_Html_Print(psi_id, pitem, prcx, conn, html_filename,new RC_F_GR(),false);
                h_o_xls = new Field_Html_Print(psi_id, pitem, prcx, conn, html_filename.Replace(".htm",".xls"), new RC_F_GR(), false);
            }
            Form_RC_OUT_RESULT out_r = new Form_RC_OUT_RESULT(h_o.html_filename);
            out_r.MdiParent = this.MdiParent;
            out_r.Show();
            FileInfo info = new FileInfo(html_filename);
            FileInfo info_xls = new FileInfo(html_filename.Replace(".htm",".xls"));
            if (info.Exists)
            {
                if (MessageBox.Show("是否公布名單?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    info.CopyTo(Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}名單.htm", pitem), true);
                    info_xls.CopyTo(Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}名單.xls", pitem), true);
                }
                // MessageBox.Show("檔案CopyTo Temp");
            }

        }
        private String html_blankformat_filename = null;
        private String tempOutput()
        {
            string html_filename = Basic_HTB_Info.baseFilePath + "\\temp\\" + String.Format("{0}_{1}.htm", psi_id, pitem);
             html_blankformat_filename = Basic_HTB_Info.baseFilePath + "\\temp\\" + String.Format("{0}_{1}.htmblank", psi_id, pitem);
            Field_Html_Output h_o = null;
            Field_Html_Output h_o_xls = null;
            if (prcx == "RCFJH")
            {
                h_o = new FieldJH_Html_Output(psi_id, pitem, prcx, conn, html_filename,new RC_F_GR(),false);
                h_o_xls = new FieldJH_Html_Output(psi_id, pitem, prcx, conn, html_filename.Replace(".htm",".xls"), new RC_F_GR(), false);
                h_o = new FieldJH_Html_Output(psi_id, pitem, prcx, conn, html_blankformat_filename, new RC_F_GR(), true);
            }
            else
            {
                h_o = new Field_Html_Output(psi_id, pitem, prcx, conn, html_filename,new RC_F_GR(),false);
                h_o_xls = new Field_Html_Output(psi_id, pitem, prcx, conn, html_filename.Replace(".htm",".xls"), new RC_F_GR(), false);
                h_o = new Field_Html_Output(psi_id, pitem, prcx, conn, html_blankformat_filename, new RC_F_GR(),true);
            }
            return html_filename;


        }
        private void button2_Click(object sender, EventArgs e)
        {
            string ds_n = "0";

            using (OdbcCommand cmd = new OdbcCommand(string.Format("select ds_n from field_item where fi_id={0}", psi_id), conn))
            {
                OdbcDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ds_n = dr.GetString(0);
                }
            }


            String tempfilename=tempOutput();
            Form_RC_OUT_RESULT out_r = new Form_RC_OUT_RESULT(tempfilename);
            out_r.MdiParent = this.MdiParent;
            out_r.Show();
            FileInfo info = new FileInfo(tempfilename);
            FileInfo info_xls = new FileInfo(tempfilename.Replace(".htm",".xls"));
            if (info.Exists)
            {
                if (MessageBox.Show("是否公布名單?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    /////////////////
                    using (OdbcCommand cmd = new OdbcCommand(string.Format("update field_item set lock_item=1 where fi_id={0}", psi_id), conn))
                    {
                        
                        info.CopyTo(Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}賽果.htm", pitem), true);
                        if (info_xls.Exists)
                        {
                            info_xls.CopyTo(Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}賽果.xls", pitem), true);
                        }
                        if (html_blankformat_filename != null)
                        {
                            FileInfo info_blnak = new FileInfo(html_blankformat_filename);
                            info_blnak.CopyTo(Basic_HTB_Info.cfg.blanktable_to_public + string.Format("\\{0}.dat", pitem), true);
                        }
                        else
                        {
                            MessageBox.Show("檔案不存在");
                        }

                        ////////////////
                      
                            try
                            {
                                MessageBox.Show(
                                   WebPost.postfile(ds_n, pitem, Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}賽果.htm", pitem)));
                            }
                            catch (Exception ex) { }
                      

                        MessageBox.Show(String.Format("{0}加鎖 更新{1}筆!", pitem, cmd.ExecuteNonQuery()));
                    }
                    ///////////////////

                   
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ds_n = "0";

            using (OdbcCommand cmd = new OdbcCommand(string.Format("select ds_n from field_item where fi_id={0}", psi_id), conn))
            {
                OdbcDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ds_n = dr.GetString(0);
                }
            }
            using (OdbcCommand cmd = new OdbcCommand(string.Format("update field_item set lock_item=1 where fi_id={0}", psi_id), conn))
            {
                String temp_Out_Html_filename = tempOutput();
                FileInfo info = new FileInfo(temp_Out_Html_filename);
                if (info.Exists)
                {
                    info.CopyTo(Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}賽果.htm", pitem), true);
                    MessageBox.Show("檔案CopyTo report_to_public");
                    try
                    {MessageBox.Show(
                        WebPost.postfile(ds_n, pitem, Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}賽果.htm", pitem)));
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    MessageBox.Show("檔案不存在");
                }

                MessageBox.Show(String.Format("{0}加鎖 更新{1}筆!", pitem, cmd.ExecuteNonQuery()));
            }
        }
    }
}
