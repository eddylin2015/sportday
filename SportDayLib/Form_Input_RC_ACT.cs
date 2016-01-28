using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using System.Drawing;

namespace SportDayLib
{
    public interface iForm_Input_RC_ACT
    {
        /// <summary>
        /// GR 大會記錄
        /// </summary>
        /// <param name="si_id">比賽項目編號</param>
        /// <param name="conn">db_Connection</param>
        /// <returns>該項大會記錄</returns>
        string Get_GR_STR(string si_id,OdbcConnection conn);
        /// <summary>
        /// 參賽人數
        /// </summary>
        /// <param name="si_id">比賽項目編號</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        int Get_Item_Cnt(string si_id, OdbcConnection conn);
        /// <summary>
        /// 設TableLayoutPanel統一Style樣式 
        /// </summary>
        /// <param name="tablePanel">tablelayoutpanel</param>
        void Set_TablePanelStyle(System.Windows.Forms.TableLayoutPanel tablePanel);
        /// <summary>
        /// 設標題
        /// </summary>
        /// <param name="tablePanel">tablelayoutpanel</param>
        /// <param name="s_ar">文字串</param>
        void Set_TablePanelColumnHead(System.Windows.Forms.TableLayoutPanel tablePanel,string[] s_ar);
        /// <summary>
        /// KeyUp操作
        /// </summary>
        /// <param name="tablePanel"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tablePanel_KeyUp(System.Windows.Forms.TableLayoutPanel tablePanel, object sender, System.Windows.Forms.KeyEventArgs e);
        /// <summary>
        /// 設table textbox和data
        /// </summary>
        /// <param name="tablePanel"></param>
        /// <param name="feildnames"></param>
        /// <param name="si_id"></param>
        /// <param name="conn"></param>
        void Set_TablePanel_TextBox_ForData(TableLayoutPanel tablePanel, string feildnames, string si_id, OdbcConnection conn);
        /// <summary>
        /// datachanged
        /// </summary>
        /// <param name="tablePanel"></param>
        /// <param name="conn"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tablePanel_TextChanged(TableLayoutPanel tablePanel, OdbcConnection conn, object sender, EventArgs e);
        /// <summary>
        /// 核實值格式
        /// </summary>
        /// <param name="FeildName"></param>
        /// <param name="Value"></param>
        /// <returns>true 返回 null,false 返回error字串</returns>
        String VerifyFieldValueFormate(String FieldName, String Value);
        /// <summary>
        /// 可編輯textbox
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        bool Editable_Control(Control c);

    }
    public class Form_Input_RC_ACT:iForm_Input_RC_ACT
    {
        public String Get_GR_STR(string si_id,OdbcConnection conn)
        {
            string GR_TXT = null;
            using (OdbcDataReader dr = new OdbcCommand(string.Format("select gr_rc,name,classno,gr_period,gr_date from sport_gr where si_id={0};", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                    GR_TXT = string.Format("{0}  {1}  {2}  {3}   {4}", dr[0], dr[1], dr[2], dr[3], dr[4]);
            }
            return GR_TXT;

        }
        public int Get_Item_Cnt(string si_id, OdbcConnection conn)
        {
            int row_cnt = 0;
            using (OdbcDataReader dr = new OdbcCommand(string.Format("select count(*) from sport_rc where si_id={0};", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                    row_cnt = int.Parse(dr[0].ToString());
            }
            return row_cnt;
        }
        public void Set_TablePanelStyle(System.Windows.Forms.TableLayoutPanel tablePanel)
        {
            tablePanel.ColumnStyles.Clear();
            tablePanel.RowStyles.Clear();
            for (int i = 0; i < tablePanel.ColumnCount; i++)
            {
                tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            }
            for (int i = 0; i < tablePanel.RowCount; i++)
            {
                tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            }

        }
        public void Set_TablePanelColumnHead(System.Windows.Forms.TableLayoutPanel tablePanel, string[] s_ar)
        {
            for (int c_i = 0; c_i < tablePanel.ColumnCount; c_i++)
            {
                System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                lb.Text = s_ar[c_i];
                tablePanel.Controls.Add(lb, c_i, 0);
            }
        }
        public void tablePanel_KeyUp(System.Windows.Forms.TableLayoutPanel tablePanel, object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                TextBox tb = (TextBox)sender;
                string name_head = tb.Name.Split('_')[0];
                bool flag = false;
                foreach (Control c in tablePanel.Controls)
                {
                    if (flag && c.Name.Contains(name_head)) { c.Focus(); break; }
                    if (c.Name == tb.Name) flag = true;
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                TextBox tb = (TextBox)sender;
                string name_head = tb.Name.Split('_')[0];
                Control p_ctr = null;
                foreach (Control c in tablePanel.Controls)
                {
                    if (p_ctr == null && c.Name.Contains(name_head)) p_ctr = c;
                    if (c.Name == tb.Name) { p_ctr.Focus(); break; }
                    if (c.Name.Contains(name_head)) { p_ctr = c; }
                }
            }
        }
        public void tablePanel_TextChanged(TableLayoutPanel tablePanel,OdbcConnection conn,object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string[] s_ar = tb.Name.Split('_');
            string rc_id = s_ar[s_ar.Length - 1];
            string field_name = s_ar[0];
            string value = tb.Text;

            for (int i = 1; i < s_ar.Length - 1; i++)
            {
                field_name += "_" + s_ar[i];
            }
            
            string sql = string.Format("update sport_rc set {0}=? where rc_id={1};", field_name, rc_id);
            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                cmd.Parameters.Add("@" + field_name, OdbcType.VarChar, 32, field_name);
                cmd.Parameters["@" + field_name].Value = value.ToUpper() ;
                cmd.ExecuteNonQuery();
            }
            
            //throw new NotImplementedException();
        }

        public void Set_TablePanel_TextBox_ForData(TableLayoutPanel tablePanel, string feildnames, string si_id, OdbcConnection conn)
        {
            string sql = string.Format("select rc_id,{0} from sport_rc where si_id={1} order by length(group_id), group_id,road;", feildnames, si_id);
            int r_i = 1;
            Font fnt = new Font("標楷體", 10);
            using (OdbcDataReader dr = new OdbcCommand(sql, conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    string rc_id = dr[0].ToString();
                    int min_int = tablePanel.ColumnCount;

                    if (dr.FieldCount - 3 < min_int) min_int = dr.FieldCount - 3;
                    for (int i = 0; i < min_int; i++)
                    {

                        if (dr.GetName(i + 3) == "rank" || dr.GetName(i + 3) == "rc" || dr.GetName(i + 3) == "grk" || dr.GetName(i + 3) == "note")
                        {
                            TextBox tb = new TextBox();
                            tb.TextAlign = HorizontalAlignment.Right;
                            tb.Font = fnt;
                            if (Lib.sysfont != null) tb.Font = Lib.sysfont ;
                            tb.Name = String.Format("{0}_{1}", dr.GetName(i + 3), rc_id);
                            tb.Text = dr[i + 3].ToString();
                            tablePanel.Controls.Add(tb, i, r_i);
                        }
                        else
                        {
                            TextBox lb = new TextBox(); if (Lib.sysfont != null) lb.Font = Lib.sysfont;
                            lb.ReadOnly = true; lb.Text = dr[i + 3].ToString(); tablePanel.Controls.Add(lb, i, r_i);
                        }
                    }
                    r_i++;
                }
            }
        }
        public String VerifyFieldValueFormate(String FieldName,String Value)
        {
            if (FieldName == "rc")
            {
                string pattern0 = @"^\d{2}''\d{2}$";
                string pattern1 = @"\d{1}'\d{2}''\d{2}$";
                if (Regex.IsMatch(Value, pattern0) || Regex.IsMatch(Value, pattern1) || Value == "")
                {
                    return null;
                }
                else
                {
                    return "格式錯誤!";
                }
            }
            return null;
        }
        public virtual bool Editable_Control(Control c)
        {
            if (c.Name.Contains("rank") || c.Name.Contains("rc") || c.Name.Contains("grk") || c.Name.Contains("note"))
            {
                return true;
            }
            return false;
        }

    }
    public class Form_Input_RC_ACT_ForAdmin : Form_Input_RC_ACT
    {

    }
    public class Form_Input_RC_ACT_ForClient : Form_Input_RC_ACT
    {

    }
    public class Form_Input_FIELD_RC_ACT : iForm_Input_RC_ACT
    {
        public string Get_GR_STR(string si_id, OdbcConnection conn)
        {
            string GR_TXT = null;
            using (OdbcDataReader dr = new OdbcCommand(string.Format("select gr_rc,name,classno,gr_period,gr_date from field_gr where fi_id={0};", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                    GR_TXT = string.Format("{0}  {1}  {2}  {3}   {4}", dr[0], dr[1], dr[2], dr[3], dr[4]);
            }
            return GR_TXT;
        }
        public int Get_Item_Cnt(string si_id, OdbcConnection conn) 
        {
            int row_cnt = 0;
            using (OdbcDataReader dr = new OdbcCommand(string.Format("select count(*) from field_rc where fi_id={0};", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                    row_cnt = int.Parse(dr[0].ToString());
            }
            return row_cnt;
        }
        public virtual void Set_TablePanelStyle(System.Windows.Forms.TableLayoutPanel tablePanel) 
        {
            tablePanel.ColumnStyles.Clear();
            tablePanel.RowStyles.Clear();
            for (int i = 0; i < tablePanel.ColumnCount; i++)
            {
                switch(i)
                {
                    case 0:
                    case 1:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
                        break;
                    case 2:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
                        break;
                    case 3:
                    case 4:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
                        break;
                    case 21:
                    case 22:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
                        break;
                    default:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
                        break;
                }
            }
            for (int i = 0; i < tablePanel.RowCount; i++)
            {
                tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            }
        }
        
        public virtual void Set_TablePanelColumnHead(System.Windows.Forms.TableLayoutPanel tablePanel, string[] s_ar)
        {
            for (int c_i = 0; c_i < tablePanel.ColumnCount; c_i++)
            {
                System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                lb.Text = s_ar[c_i];
                tablePanel.Controls.Add(lb, c_i, 0);
            }
        }
        public void tablePanel_KeyUp(System.Windows.Forms.TableLayoutPanel tablePanel, object sender, System.Windows.Forms.KeyEventArgs e) 
        {
            TextBox tb = (TextBox)sender;
            int posi_c = -1;
            int posi_r = -1;
            for(int i=0;i<tablePanel.ColumnCount;i++)
                for(int j=0;j<tablePanel.RowCount;j++)
                    if (TB_ARR[i, j] == tb)
                    {
                        posi_c = i;
                        posi_r = j;
                    }
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                if (posi_r + 1 < tablePanel.RowCount && TB_ARR[posi_c, posi_r + 1] != null)
                {
                    TB_ARR[posi_c, posi_r + 1].Focus();
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                if (posi_r  > 0 && TB_ARR[posi_c, posi_r - 1] != null)
                {
                    TB_ARR[posi_c, posi_r - 1].Focus();
                }
            }
            else if(e.KeyCode==Keys.Right)
            {
                e.Handled = true;
                if(posi_c+1<tablePanel.ColumnCount&&TB_ARR[posi_c+1,posi_r]!=null)
                {
                    TB_ARR[posi_c + 1, posi_r].Focus();
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                e.Handled = true;
                if(posi_c>0&& TB_ARR[posi_c-1,posi_r]!=null)
                {
                    TB_ARR[posi_c - 1, posi_r].Focus();
                }

            }
        }
        protected TextBox[,] TB_ARR = null;
        public virtual void Set_TablePanel_TextBox_ForData(TableLayoutPanel tablePanel, string feildnames, string si_id, OdbcConnection conn) 
        {
            TB_ARR=new TextBox[tablePanel.ColumnCount,tablePanel.RowCount];
            string sql = string.Format("select frc_id,{0} from field_rc where fi_id={1} order by BIT_LENGTH( s_number),s_number;", feildnames, si_id);
            int r_i = 1;
            Font fnt = new Font("標楷體", 10);
            using (OdbcDataReader dr = new OdbcCommand(sql, conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    string rc_id = dr[0].ToString();
                    int min_int = tablePanel.ColumnCount;

                    if (dr.FieldCount - 3 < min_int) min_int = dr.FieldCount - 3;
                    for (int i = 0; i < min_int; i++)
                    {
                        if (dr.GetName(i + 3) == "rank" || dr.GetName(i + 3) == "rc" || dr.GetName(i + 3) == "grk" || dr.GetName(i + 3) == "note")
                        {
                            TextBox tb = new TextBox();
                            tb.TextAlign = HorizontalAlignment.Right;
                            tb.Font = fnt;
                            tb.Name = String.Format("{0}_{1}", dr.GetName(i + 3), rc_id);
                            tb.Text = dr[i + 3].ToString();
                            tablePanel.Controls.Add(tb, i, r_i);
                            TB_ARR[i, r_i] = tb;
                        }
                        else if (dr.GetName(i + 3)[0] == 'h' || dr.GetName(i + 3)[0] == 'b')
                        {
                            TextBox tb = new TextBox();
                            tb.TextAlign = HorizontalAlignment.Right;
                            tb.Font = fnt;
                            tb.Name = String.Format("{0}_{1}", dr.GetName(i + 3), rc_id);
                            tb.Text = dr[i + 3].ToString();
                            tablePanel.Controls.Add(tb, i, r_i);
                            TB_ARR[i, r_i] = tb;
                        }else
                        {
                            TextBox lb = new TextBox(); lb.ReadOnly = true; lb.Text = dr[i + 3].ToString(); tablePanel.Controls.Add(lb, i, r_i);
                        }
                    }
                    r_i++;
                }
            }
        }
        public void tablePanel_TextChanged(TableLayoutPanel tablePanel, OdbcConnection conn, object sender, EventArgs e) 
        {
            TextBox tb = (TextBox)sender;
            string[] s_ar = tb.Name.Split('_');
            string rc_id = s_ar[s_ar.Length - 1];
            string field_name = s_ar[0];
            string value = tb.Text;

            for (int i = 1; i < s_ar.Length - 1; i++)
            {
                field_name += "_" + s_ar[i];
            }

            string sql = string.Format("update field_rc set {0}=? where frc_id={1};", field_name, rc_id);
            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                cmd.Parameters.Add("@" + field_name, OdbcType.VarChar, 32, field_name);
                cmd.Parameters["@" + field_name].Value = value;
                cmd.ExecuteNonQuery();
            }
        }
        private readonly string hx_pattern = @"^h\d$";
        private readonly string hxx_pattern = @"^h\d{2}$";
        private readonly string bx_pattern = @"^b\d$";
        
        public virtual String VerifyFieldValueFormate(String FieldName, String Value)
        {

            if (FieldName == "rc" || Regex.IsMatch(FieldName, hx_pattern) || Regex.IsMatch(FieldName, hxx_pattern) || Regex.IsMatch(FieldName, bx_pattern))
            {
                string pattern0 = @"^\d{1}.\d{2}$";
                string pattern1 = @"^\d{2}.\d{2}$";
                string pattern2 = @"^\d{3}.\d{2}$";

                if (Regex.IsMatch(Value, pattern0) || Regex.IsMatch(Value, pattern1) || Regex.IsMatch(Value, pattern2) || Value == "" || Value == "X")
                {
                    return null;
                }
                else
                {
                    return "格式錯誤!";
                }
            }
            return null;
        }
        public virtual bool Editable_Control(Control c)
        {
            String[] str_a = c.Name.Split('_');
            String fieldname = "";
            if (str_a.Length > 0) fieldname = str_a[0];
            if (c.Name.Contains("rank") || c.Name.Contains("rc") || c.Name.Contains("grk") || c.Name.Contains("note") || Regex.IsMatch(fieldname,hx_pattern) || Regex.IsMatch(fieldname,hxx_pattern)||Regex.IsMatch(fieldname,bx_pattern))
            {
                return true;
            }
            return false;
        }

    }
    public class Form_Input_FIELD_RC_ACT_ForAdmin : Form_Input_FIELD_RC_ACT
    {

    }
                 
    public class Form_Input_FIELD_RC_ACT_ForClient : Form_Input_FIELD_RC_ACT
    {

    }
    public class Form_Input_FIELDJUMPHIGH_RC_ACT_ForClient : Form_Input_FIELDJUMPHIGH_RC_ACT
    {
    }
    public class Form_Input_FIELDJUMPHIGH_RC_ACT_ForAdmin : Form_Input_FIELDJUMPHIGH_RC_ACT
    {

    }
    public class Form_Input_FIELDJUMPHIGH_RC_ACT : Form_Input_FIELD_RC_ACT
    {
        public override void Set_TablePanelColumnHead(System.Windows.Forms.TableLayoutPanel tablePanel, string[] s_ar)
        {

        }
        public override void Set_TablePanelStyle(System.Windows.Forms.TableLayoutPanel tablePanel)
        {
            tablePanel.ColumnStyles.Clear();
            tablePanel.RowStyles.Clear();
            for (int i = 0; i < tablePanel.ColumnCount; i++)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
                        break;
                    case 2:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
                        break;
                    case 3:
                    case 4:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
                        break;
                    case 21:
                    case 22:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
                        break;
                    default:
                        tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
                        break;
                }
            }
            for (int i = 0; i < tablePanel.RowCount; i++)
            {
                tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            }
        }
        private void hx_changed(Object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string[] s_ar = tb.Name.Split('_');
            string rc_id = s_ar[s_ar.Length - 1];
            string field_name = s_ar[1];
            string value = tb.Text;

            for (int i = 2; i < s_ar.Length - 1; i++)
            {
                field_name += "_" + s_ar[i];
            }

            string sql = string.Format("update field_item set {0}=? where fi_id={1};", field_name, rc_id);
            using (OdbcCommand cmd = new OdbcCommand(sql, db_conn))
            {
                cmd.Parameters.Add("@" + field_name, OdbcType.VarChar, 32, field_name);
                cmd.Parameters["@" + field_name].Value = value;
                cmd.ExecuteNonQuery();
            }
        }
        private OdbcConnection db_conn = null;
        public override void Set_TablePanel_TextBox_ForData(TableLayoutPanel tablePanel, string feildnames, string si_id, OdbcConnection conn)
        {
            TB_ARR=new TextBox[tablePanel.ColumnCount,tablePanel.RowCount];
            db_conn = conn;
            String c_fieldnames = "名\n次,次\n序,比賽\n號,姓名,班級,";
            c_fieldnames += "h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11,h12,h13,h14,h15,h16,";
            c_fieldnames += "成績,備註";
            String[] s_ar = c_fieldnames.Split(',');

            string hxsql = string.Format("select h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11,h12,h13,h14,h15,h16 from field_item where fi_id={0} ;",  si_id);
            using (OdbcDataReader hxdr= new OdbcCommand(hxsql, conn).ExecuteReader())
            {
                hxdr.Read();
                for (int c_i = 0; c_i < tablePanel.ColumnCount; c_i++)
                {
                    if (s_ar[c_i][0] == 'h')
                    {
                        System.Windows.Forms.TextBox lb = new System.Windows.Forms.TextBox();
                        lb.Name=String.Format("fieldname_{0}_{1}", s_ar[c_i], si_id);
                        lb.Text = hxdr[s_ar[c_i]].ToString();
                        lb.TextAlign = HorizontalAlignment.Right;
                        lb.TextChanged += hx_changed;
                        tablePanel.Controls.Add(lb, c_i, 0);
                        TB_ARR[c_i, 0] = lb;
                    }
                    else
                    {
                        System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                        lb.Text = s_ar[c_i];
                        tablePanel.Controls.Add(lb, c_i, 0);
                    }
                }
            }

            string sql = string.Format("select frc_id,{0} from field_rc where fi_id={1} order by BIT_LENGTH( s_number),s_number;", feildnames, si_id);
            int r_i = 1;
            Font fnt = new Font("標楷體", 10);
            using (OdbcDataReader dr = new OdbcCommand(sql, conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    string rc_id = dr[0].ToString();
                    int min_int = tablePanel.ColumnCount;
                    if (dr.FieldCount - 3 < min_int) min_int = dr.FieldCount - 3;
                    for (int i = 0; i < min_int; i++)
                    {
                        if (dr.GetName(i + 3) == "rank" || dr.GetName(i + 3) == "rc" || dr.GetName(i + 3) == "grk" || dr.GetName(i + 3) == "note")
                        {
                            TextBox tb = new TextBox();
                            tb.TextAlign = HorizontalAlignment.Right;
                            tb.Font = fnt;
                            tb.Name = String.Format("{0}_{1}", dr.GetName(i + 3), rc_id);
                            tb.Text = dr[i + 3].ToString();
                            tablePanel.Controls.Add(tb, i, r_i);
                            TB_ARR[i, r_i] = tb;
                        }
                        else if (dr.GetName(i + 3)[0] == 'h')
                        {
                            TextBox tb = new TextBox();
                            tb.TextAlign = HorizontalAlignment.Left;
                            tb.Font = fnt;
                            tb.MaxLength = 3;
                            tb.Name = String.Format("{0}_{1}", dr.GetName(i + 3), rc_id);
                            tb.Text = dr[i + 3].ToString();
                            tablePanel.Controls.Add(tb, i, r_i);
                            TB_ARR[i, r_i] = tb;
                        }
                        else
                        {
                            TextBox lb = new TextBox(); lb.ReadOnly = true; lb.Text = dr[i + 3].ToString(); tablePanel.Controls.Add(lb, i, r_i);
                        }
                    }
                    r_i++;
                }
            }

        }
        public override string VerifyFieldValueFormate(string FieldName, string Value)
        {
            string pattern0 = @"^h\d$";
            string pattern1 = @"^h\d{2}$";

            if (Regex.IsMatch(FieldName, pattern0)||Regex.IsMatch(FieldName,pattern1))
            {
                string uv=Value.ToUpper();
                if (uv == "" || uv == "-" || uv == "X" || uv == "XX" || uv == "O" || uv == "XO" || uv == "XXO" || uv == "XXX")
                {
                    return null;
                }
                return "大寫X 和 大寫O,格式錯誤!";
            }
            return base.VerifyFieldValueFormate(FieldName, Value);
        }
    }
}
