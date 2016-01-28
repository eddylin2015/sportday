using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace SportDayLib
{
    public partial class Form_Input_RC : Form
    {
        private OdbcConnection conn = null;
        private string feildnames=null;
        private string c_feildnames=null;
        private string col_span = null;
        private string sport_item = null;
        private string sport_id = null;
        private string sport_rcx = null;
        private string temp_Out_Html_filename = null;
        private List<string> Porm_Names_List = null;
        private iForm_Input_RC_ACT iAct = null;
        public Form_Input_RC(string si_id, string item, string rcx, iForm_Input_RC_ACT Act)
            : base()
        {
            InitializeComponent();
            iAct = Act;
            this.Text = String.Format("{0}:{1}",si_id,item);
            sport_item = item;
            sport_id = si_id;
            sport_rcx = rcx;
            temp_Out_Html_filename = Basic_HTB_Info.cfg.output_result_html + "\\" + String.Format("{0}_{1}.htm", sport_id, sport_item);
            conn = new OdbcConnection(Basic_HTB_Info.Conn_Str);
            conn.Open();
            this.lbl_GR.Text = iAct.Get_GR_STR(si_id, conn);
            if (!Basic_HTB_Info.signflag_sql.Contains(rcx)) throw new Exception(rcx);
            feildnames = Basic_HTB_Info.signflag_sql[rcx].ToString();
            c_feildnames = Basic_HTB_Info.Get_Signflag_s(rcx);
            Basic_HTB_Info.Adjust_FieldName(ref feildnames,ref c_feildnames,ref col_span);
            string[] s_ar = c_feildnames.Split(',');
            int col_cnt = s_ar.Length;
            int row_cnt = iAct.Get_Item_Cnt(si_id, conn) + 1;
 
            this.tablePanel.ColumnCount = col_cnt;
            this.tablePanel.RowCount = row_cnt;
            iAct.Set_TablePanelStyle(tablePanel);
            iAct.Set_TablePanelColumnHead(tablePanel, s_ar);
            iAct.Set_TablePanel_TextBox_ForData(tablePanel, feildnames, si_id, conn);
            foreach (Control c in tablePanel.Controls)
            {
                if (c.Name.Contains("rank") || c.Name.Contains("rc") || c.Name.Contains("grk") || c.Name.Contains("note"))
                {
                    TextBox tb = (TextBox)c; tb.TextChanged += new EventHandler(tb_TextChanged); tb.KeyUp += new KeyEventHandler(tb_KeyUp);
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

        private void Form_Input_RC_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (conn != null)
            {
                if(conn.State==ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        private void btn_RC_Html_Output_Click(object sender, EventArgs e)
        {
            RC_Html_Output h_o_xls = new RC_Html_Output(sport_id, sport_item, sport_rcx, conn, temp_Out_Html_filename.Replace(".htm",".xls"), new RC_S_GR());
            
            RC_Html_Output h_o = new RC_Html_Output(sport_id, sport_item, sport_rcx, conn, temp_Out_Html_filename,new RC_S_GR());
            Form_RC_OUT_RESULT out_r = new Form_RC_OUT_RESULT(h_o.html_filename);          
            out_r.MdiParent = this.MdiParent;
            out_r.Show();
            RC_Html_Output_BlankTable h_o_blanktable = new RC_Html_Output_BlankTable(sport_id, sport_item, sport_rcx, conn, temp_Out_Html_filename, new RC_S_GR());
        }
        /// <summary>
        /// 輸出賽果,并輸出進級名單(Porm_Names_List)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RC_Prom_Html_Output_Click(object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            string title_item = sport_item.Split('(')[0];
            MessageBox.Show(title_item);
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select si_id,s_item,rcx from sport_item where s_item like '{0}%' and lock_item is null ;", title_item), conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                    lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                }
            }
            if (lbf.ShowDialog() == DialogResult.OK && lbf.lb.SelectedIndex>-1)
            {
                string[] s_ar=lbf.lb.SelectedItem.ToString().Split(':');
                if(s_ar.Length==3)
                {
                    RC_Prom_Html_Output rp_o_xls = new RC_Prom_Html_Output(this.sport_id, this.sport_item, this.sport_rcx, s_ar[0], s_ar[1], s_ar[2], conn, out Porm_Names_List, temp_Out_Html_filename.Replace(".htm",".xls"), new RC_S_GR());
                    RC_Prom_Html_Output rp_o = new RC_Prom_Html_Output(this.sport_id, this.sport_item, this.sport_rcx, s_ar[0], s_ar[1], s_ar[2], conn, out Porm_Names_List, temp_Out_Html_filename,new RC_S_GR());
                    Form_RC_OUT_RESULT out_r = new Form_RC_OUT_RESULT(rp_o.html_filename);
                    out_r.MdiParent = this.MdiParent;
                    out_r.Show();
                }
            }
        }
        
        private void btn_Result_to_Public_And_Lock_Click(object sender, EventArgs e)
        {
            string ds_n = "0";

            using (OdbcCommand cmd = new OdbcCommand(string.Format("select ds_n from sport_item where si_id={0}", sport_id), conn))
            {
                OdbcDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ds_n = dr.GetString(0);
                }
            }
            using (OdbcCommand cmd = new OdbcCommand(string.Format("update sport_item set lock_item=1 where si_id={0}",sport_id), conn))
            {
                FileInfo xlsinfo = new FileInfo(temp_Out_Html_filename.Replace(".htm",".xls"));
                if (xlsinfo.Exists)
                {
                    xlsinfo.CopyTo(Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}賽果.xls", sport_item), true);
                }


                FileInfo info = new FileInfo(temp_Out_Html_filename);
                
                if (info.Exists)
                {
                    info.CopyTo(Basic_HTB_Info.cfg.report_to_public+string.Format("\\{0}賽果.htm",sport_item),true);

                    MessageBox.Show("檔案CopyTo report_to_public");
                    try {
                        FileInfo binfo = new FileInfo(temp_Out_Html_filename + "blank");
                        if (binfo.Exists)
                        {
                            binfo.CopyTo(Basic_HTB_Info.cfg.blanktable_to_public+string.Format("\\{0}.dat",sport_item),true);
                        }
                    }
                    catch (Exception exp1)
                    {
                                           }
                    try {
                      MessageBox.Show( WebPost.postfile(ds_n,sport_item,Basic_HTB_Info.cfg.report_to_public+string.Format("\\{0}賽果.htm",sport_item)));

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("檔案不存在");
                }
                MessageBox.Show(String.Format("{0}加鎖 更新{1}筆!", sport_item, cmd.ExecuteNonQuery()));
            }
        }
        /// <summary>
        /// 進級名(insert)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Insert_Prom_Names_Click(object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            string title_item = sport_item.Split('(')[0];
            MessageBox.Show(title_item);
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select si_id,s_item,rcx from sport_item where s_item like '{0}%' and lock_item is null;", title_item), conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                    lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                }
            }
            if (lbf.ShowDialog() == DialogResult.OK)
            {
                string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                
                if (s_ar.Length == 3)
                {
                    string p_id = s_ar[0];
                    string p_item = s_ar[1];
                    button6.Text = p_id;
                    if (MessageBox.Show("del " + p_id + p_item+" records?", "p_item", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        using (OdbcCommand cmd = new OdbcCommand(String.Format("delete from sport_rc where si_id={0};", p_id), conn))
                        {
                            MessageBox.Show(String.Format("del {0} {1}筆",p_id,cmd.ExecuteNonQuery()));
                            if (Porm_Names_List != null)
                            {
                                MPPFORM.MsgBox msg = new MPPFORM.MsgBox(Porm_Names_List.ToArray());
                                msg.Show();

                                foreach (string s in Porm_Names_List)
                                {
                                    string[] s_ar0 = s.Split(',');
                                    string sql = String.Format("select s_number,number,classno,name from sport_rc where rc_id={0}", s_ar0[s_ar0.Length - 1]);
                                    using (OdbcDataReader dr = new OdbcCommand(sql,conn).ExecuteReader())
                                    {
                                        if (dr.Read())
                                        {
                                            List<string> ls = new List<string>();
                                            ls.Add(s_ar0[0]); ls.Add(s_ar0[1]); ls.Add(s_ar0[2]);
                                            for (int i = 0; i < dr.FieldCount; i++) if (!dr.IsDBNull(i)) { ls.Add(dr.GetString(i)); } else { ls.Add(""); }// 
                                            Lib.inc_cmd("si_id,group_id,road,s_number,number,classno,name", "sport_rc", ls.ToArray(), conn);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btn_Print_RC_HTML_FORM_CLICK(object sender, EventArgs e)
        {
            string html_filename = Basic_HTB_Info.baseFilePath + "\\temp\\" + String.Format("{0}_{1}.htm", sport_id, sport_item);

            RC_Html_Print h_o = new RC_Html_Print(sport_id, sport_item, sport_rcx, conn, html_filename,new RC_S_GR());
            Form_RC_OUT_RESULT out_r = new Form_RC_OUT_RESULT(h_o.html_filename);          
            out_r.MdiParent = this.MdiParent;
            out_r.Show();
              FileInfo info = new FileInfo(html_filename);
              if (info.Exists)
              {
                  if(MessageBox.Show("是否公布名單?","",MessageBoxButtons.YesNo)==DialogResult.Yes)
                  {
                      info.CopyTo(Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}名單.htm", sport_item), true);
                      String tempfilename = Basic_HTB_Info.cfg.report_to_public + string.Format("\\{0}名單.xls", sport_item);
                      RC_Html_Print rc_html_p = new RC_Html_Print(sport_id, sport_item, sport_rcx, conn, tempfilename, new RC_S_GR());
                  }
              }
        }

        private void LinkGRTB_Click(object sender, EventArgs e)
        {
            String si_id = sport_id;
            int cnt = 0;
            using (OdbcDataReader cnt_dr = new OdbcCommand("select count(*) from sport_gr where si_id="+si_id,conn).ExecuteReader())
            {
                cnt_dr.Read();
                cnt = int.Parse(cnt_dr[0].ToString());
                if (cnt > 0)
                {
                   int temp_int= new OdbcCommand("delete from sport_gr where si_id=" + si_id, conn).ExecuteNonQuery();
                }
            }
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();

            using (OdbcDataReader gr_dr = new OdbcCommand("select s_item,gr_rc,name,classno,gr_period,gr_date from sport_gr_tb order by s_item;",conn).ExecuteReader())
            {
                while (gr_dr.Read())
                {
                    lbf.lb.Items.Add(String.Format("{0};{1};{2};{3};{4};{5}",gr_dr[0],gr_dr[1],gr_dr[2],gr_dr[3],gr_dr[4],gr_dr[5]));
                }
            }
            if (lbf.ShowDialog() == DialogResult.OK)
            {
                String[] str = lbf.lb.SelectedItem.ToString().Split(';');
                String[] value_s = new String[str.Length + 1];
                value_s[0]=si_id;
                int temp_int = 1;
                foreach(String s in str)
                {
                    value_s[temp_int] = s;
                    temp_int++;
                }
                Lib.inc_cmd("si_id,s_item, gr_rc, name, classno, gr_period, gr_date","sport_gr",value_s,conn);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int temp_int;
            if (!int.TryParse(button6.Text,out temp_int)) return; 
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                
                OdbcDataReader dr = new OdbcCommand("select si_id,s_item,rcx from sport_item where si_id= "+button6.Text, conn).ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                    lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                }
                conn.Close();
            }
            if (lbf.ShowDialog() == DialogResult.OK)
            {
                string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                string si_id = s_ar[0];
                string item = s_ar[1];
                string rcx = s_ar[2];
                SportDayLib.Form_Input_RC f_RC = new Form_Input_RC(si_id, item, rcx, new Form_Input_RC_ACT_ForAdmin());
                f_RC.MdiParent = Basic_HTB_Info.cfg.MDIparent;
                f_RC.Show();
            }

        }


    }
}
