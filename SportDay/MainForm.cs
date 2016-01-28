using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Security;
using System.Threading;
using SportDayLib;


namespace SportDay
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
            MainMenu fmainMenu = new MainMenu();
            MenuItem msys = fmainMenu.MenuItems.Add("項目定義");
            MenuItem def_item = msys.MenuItems.Add("檔案分析");
            def_item.MenuItems.Add(new MenuItem("競賽項目名_Read_File_Name", this.menuImport_Race_Event_XLS_FileName));
            def_item.MenuItems.Add(new MenuItem("XLS_Convert_TXT_File_Name", this.menu_XLS_FileName_Convert_TXT));
            def_item.MenuItems.Add("讀取Read Txt to Txt", this.menu_Read_TXT_to_TXT);
            def_item.MenuItems.Add("讀取Read GR Txt to Txt", this.menu_Read_GR_TXT_to_TXT);
            def_item.MenuItems.Add("-");
            def_item.MenuItems.Add("田賽項目名_Read_File_Name", menuImport_Field_Event_XLS_FileName);
            def_item.MenuItems.Add("XLS_Convert_TXT_FILE_NAME", menu_FIELDXLS_FileName_Convert_TXT);
            def_item.MenuItems.Add("讀取Read Txt to Txt",menu_Read_Field_TXT_to_TXT);
            def_item.MenuItems.Add("讀取Read GR Txt to Txt",menu_Read_Field_GR_TXT_to_TXT);
            MenuItem db_item = msys.MenuItems.Add("資料庫");
            db_item.MenuItems.Add("競賽項目Load file to DB",this.menu_Load_Sport_item_to_db);
            db_item.MenuItems.Add("讀取Read Txt to DB",this.menu_load_rc_item_to_db);//Load_sport_rc_From_txt_To_DB
            db_item.MenuItems.Add("讀取Read Txt GR  to DB", this.menu_read_GR_TXT_TO_DB);
            db_item.MenuItems.Add("讀取Read Txt Sport GR_TB to DB", this.menu_read_Sport_GR_TB_TXT_TO_DB);
            db_item.MenuItems.Add("讀取Read Txt Sport_ITEM(複賽)(決賽) to DB", this.menu_read_Sport_item_for_Prom_TB_TXT_TO_DB);
            db_item.MenuItems.Add("-");
            db_item.MenuItems.Add("田賽項目Load file to DB",this.menu_Load_Field_item_to_db);
            db_item.MenuItems.Add("田賽Read Txt to DB",menu_load_field_rc_item_txt_to_db);
            db_item.MenuItems.Add("田賽Read Txt GR to DB",menu_load_field_rc_gr_txt_to_db);
            msys.MenuItems.Add("-");
            msys.MenuItems.Add("字型", menu_SetFont);
            MenuItem inputsys = fmainMenu.MenuItems.Add("輸入");
            inputsys.MenuItems.Add("輸入競賽成績", menu_Input_RC);
            inputsys.MenuItems.Add("輸入田賽成績", menu_Input_Field_RC);
            MenuItem upgradesys = fmainMenu.MenuItems.Add("晉級");
            upgradesys.MenuItems.Add("晉級競賽成績", menu_Upgrade_RC);
            upgradesys.MenuItems.Add("下載競賽xls名單", menu_DownXls_S_ITEM_NAME);
            upgradesys.MenuItems.Add("下載競賽Htm名單", menu_DownHtm_S_ITEM_NAME);
            upgradesys.MenuItems.Add("下載競賽xls賽果", menu_DownXls_S_ITEM_Result);
            upgradesys.MenuItems.Add("-");
            upgradesys.MenuItems.Add("處理田賽成績", menu_Upgrade_Field);
            upgradesys.MenuItems.Add("下載田賽xls名單", menu_DownXls_F_ITEM_NAME);
            upgradesys.MenuItems.Add("下載田賽Htm名單", menu_DownHtm_F_ITEM_NAME);
            upgradesys.MenuItems.Add("下載田賽xls賽果", menu_DownXls_F_ITEM_Result);
            upgradesys.MenuItems.Add("-");
            upgradesys.MenuItems.Add("解鎖_UNLOCK", menu_UNLOCK_S_ITEM);
            MenuItem addmenu = fmainMenu.MenuItems.Add("處理");
            addmenu.MenuItems.Add("後補報名",menu_reg_student_gi);
            addmenu.MenuItems.Add("gt_item->預賽項目", menu_gtitem2spitem);
            addmenu.MenuItems.Add("stud_gti->預賽參賽名單", menu_studgti2sprc);
            addmenu.MenuItems.Add("stud_gti->田賽預賽參賽名單", menu_studgti2fieldrc);
            
            MenuItem sqlitemenu = fmainMenu.MenuItems.Add("SQLITE");
            sqlitemenu.MenuItems.Add("Syn SQLite",menu_Syn_From_SQLite);
            MenuItem webitemenu = fmainMenu.MenuItems.Add("公佈至website");
            webitemenu.MenuItems.Add("XLStoPDF", menu_XLStoPDF);
            webitemenu.MenuItems.Add("race",menu_pulishtowebsite_race);
            webitemenu.MenuItems.Add("field", menu_pulishtowebsite_field);

            

        //    msys.MenuItems.Add(new MenuItem("Test", this.menutest));
            this.Menu = fmainMenu;
            this.FormClosing += Form_Closing;
            Basic_HTB_Info.cfg.MDIparent = this;
        }
        private void menu_SetFont(Object sender, EventArgs e)
        {
            Lib.SetFont();
        }
        private void Form_Closing(Object sender, EventArgs e)
        {
            Basic_HTB_Info.cfg.FlashTempData();
        }
        private void menu_XLStoPDF(Object sender,EventArgs e)
        {
            String sourceDirectory = @"C:\AppServ\www\report_to_public\files";
            if (Directory.Exists(sourceDirectory) == true)
            {
                MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
                DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
                foreach(FileInfo f in diSource.GetFiles())
                {
                    System.TimeSpan diff1 = DateTime.Now.Subtract(f.LastWriteTime);
                    if (f.Name.Contains(".xls") && diff1.Hours <1)
                    {
                        lbf.lb.Items.Add(f.Name);
                        //eddy pause
                    }
                }
                if(lbf.ShowDialog()==DialogResult.OK){
                    String a=sourceDirectory+@"\"+lbf.lb.SelectedItem.ToString();
                    String b = a.ToUpper().Replace(".XLS", ".PDF");
                    Lib.convertXlsPdf(a, b);
                   // convertXlsPdf
                }
            }

        }
        private void DownXls_S_ITEM_NAME(String extra_filename)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Basic_HTB_Info.cfg.TempdataKey("path1");
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Basic_HTB_Info.cfg.tempdata["path1"] = fbd.SelectedPath;
                MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
                using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
                {
                    conn.Open();
                    OdbcDataReader dr = new OdbcCommand("select si_id,s_item,rcx from sport_item;", conn).ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                        lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                    }
                    if (lbf.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string s in lbf.lb.CheckedItems)
                        {
                            string[] s_ar = s.Split(':');
                            string si_id = s_ar[0];
                            string item = s_ar[1];
                            string rcx = s_ar[2];
                            string f_name = String.Format("{0}\\{1}.{2}", fbd.SelectedPath, item,extra_filename);
                            RC_Html_Print rc_html_p = new RC_Html_Print(si_id, item, rcx, conn, f_name,new RC_S_GR());
                        }
                    }
                    conn.Close();
                }
            }
        }
        private void menu_pulishtowebsite_race(Object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                OdbcDataReader dr = new OdbcCommand("select si_id,s_item,rcx from sport_item where lock_item is null order by si_id;", conn).ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                    lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                }
                dr.Dispose();
                if (lbf.ShowDialog() == DialogResult.OK)
                {
                    string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                    string si_id = s_ar[0];
                    string item = s_ar[1];
                    string rcx = s_ar[2];
                    dr = new OdbcCommand("select rc_id,si_id,rank,group_id,road,s_number,number,classno,name,rc,grk,note from sport_rc where si_id=" + si_id, conn).ExecuteReader();
                    StringBuilder sb = new StringBuilder("[");
                    while (dr.Read())
                    {
                        sb.Append("{");
                        for (int i = 0; i < dr.FieldCount; i++)
                        {

                            
                            if (!dr.IsDBNull(i))
                            {
                                if (i > 0) { sb.Append(","); }
                                if (i < 2) { sb.Append(String.Format("\"{0}\":\"{1}\"", Convert.ToChar(65 + i), dr.GetValue(i))); } 

                                else {
                                    string v=dr.GetString(i).Replace("''","s").Replace("'","m").Replace(" ", "").Replace("\t","").Replace("\n","").Trim();
                                    sb.Append(String.Format("\"{0}\":\"{1}\"", Convert.ToChar(65 + i), v)); 
                                }
                                
                            }
                        }
                        sb.Append("}");
                    }
                    sb.Append("]");
                    MessageBox.Show(sb.ToString());
                    webpost.post(sb.ToString());
                }
                conn.Close();
            }
            
        }
        private void menu_pulishtowebsite_field(Object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                OdbcDataReader dr = new OdbcCommand("select fi_id,f_item,rcx from field_item where lock_item is null order by fi_id;", conn).ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球"))
                    {
                        lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                    }
                }
                if (lbf.ShowDialog() == DialogResult.OK)
                {
                    string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                    string si_id = s_ar[0];
                    dr = new OdbcCommand("select frc_id,fi_id,rank,group_id,road,s_number,number,classno,name,rc,grk,note from field_rc where fi_id=" + si_id, conn).ExecuteReader();
                    StringBuilder sb = new StringBuilder("[");
                    while (dr.Read())
                    {
                        sb.Append("{");
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            if (!dr.IsDBNull(i))
                            {
                                
                                if (i > 0) { sb.Append(","); }
                                if (i == 0) { sb.Append(String.Format("\"{0}\":\"{1}\"", Convert.ToChar(65 + i), dr.GetInt32(i)+2000)); }
                                else if (i < 2) { sb.Append(String.Format("\"{0}\":\"{1}\"", Convert.ToChar(65 + i), dr.GetValue(i))); }
                                else { sb.Append(String.Format("\"{0}\":\"{1}\"", Convert.ToChar(65 + i), dr.GetString(i).Replace(" ", "_").Trim())); }

                            }
                        }
                        sb.Append("}");
                    }
                    sb.Append("]");
                    MessageBox.Show(sb.ToString());
                    webpost.post(sb.ToString());
                }

                conn.Close();
            }
            
        }
        private void menu_DownXls_S_ITEM_NAME(Object sender,EventArgs e)
        {
            DownXls_S_ITEM_NAME("xls");
        }
        private void menu_DownHtm_S_ITEM_NAME(Object sender, EventArgs e)
        {
            DownXls_S_ITEM_NAME("htm");
        }
        private void DownXls_F_ITEM_NAME(String extra_filename)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Basic_HTB_Info.cfg.TempdataKey("path1");
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Basic_HTB_Info.cfg.tempdata["path1"] = fbd.SelectedPath;
                MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
                using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
                {
                    conn.Open();
                    OdbcDataReader dr = new OdbcCommand("select fi_id,f_item,rcx from field_item;", conn).ExecuteReader();
                    while (dr.Read())
                    {
                        lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                    }
                    if (lbf.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string s in lbf.lb.CheckedItems)
                        {
                            string[] s_ar = s.Split(':');
                            string si_id = s_ar[0];
                            string item = s_ar[1];
                            string rcx = s_ar[2];
                            if (item.Contains("跳高"))
                            {
                                string f_name = String.Format("{0}\\{1}.{2}", fbd.SelectedPath, item, extra_filename);
                                Field_Html_Print rc_html_p = new FieldJH_Html_Print(si_id, item, rcx, conn, f_name,new RC_F_GR(),false);
                            }
                            else
                            {
                                string f_name = String.Format("{0}\\{1}.{2}", fbd.SelectedPath, item, extra_filename);
                                Field_Html_Print rc_html_p = new Field_Html_Print(si_id, item, rcx, conn, f_name,new RC_F_GR(),false);
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
        private void menu_DownHtm_F_ITEM_NAME(Object sender, EventArgs e)
        {
            DownXls_F_ITEM_NAME("htm");
        }
        private void menu_DownXls_F_ITEM_NAME(Object sender, EventArgs e)
        {
            DownXls_F_ITEM_NAME("xls");
        }
        private void DownXls_F_ITEM_Result(String extra_filename)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Basic_HTB_Info.cfg.TempdataKey("path2");
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Basic_HTB_Info.cfg.tempdata["path2"] = fbd.SelectedPath;
                MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
                using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
                {
                    conn.Open();
                    OdbcDataReader dr = new OdbcCommand("select fi_id,f_item,rcx from field_item;", conn).ExecuteReader();
                    while (dr.Read())
                    {
                        lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                    }
                    if (lbf.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string s in lbf.lb.CheckedItems)
                        {
                            string[] s_ar = s.Split(':');
                            string si_id = s_ar[0];
                            string item = s_ar[1];
                            string rcx = s_ar[2];
                            if (item.Contains("跳高"))
                            {
                                string f_name = String.Format("{0}\\{1}.{2}", fbd.SelectedPath, item, extra_filename);
                                FieldJH_Html_Output rc_html_p = new FieldJH_Html_Output(si_id, item, rcx, conn, f_name,new RC_F_GR(),false);
                            }
                            else
                            {
                                string f_name = String.Format("{0}\\{1}.{2}", fbd.SelectedPath, item, extra_filename);
                                Field_Html_Output rc_html_p = new Field_Html_Output(si_id, item, rcx, conn, f_name,new RC_F_GR(),false);
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
        private void menu_DownXls_F_ITEM_Result(Object sender,EventArgs e)
        {
            DownXls_F_ITEM_Result("xls");
        }
        private void menu_DownXls_S_ITEM_Result(Object sender, EventArgs e) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath=  Basic_HTB_Info.cfg.TempdataKey("path2");
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Basic_HTB_Info.cfg.tempdata["path2"] = fbd.SelectedPath;
                MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
                using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
                {
                    conn.Open();
                    OdbcDataReader dr = new OdbcCommand("select si_id,s_item,rcx from sport_item;", conn).ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                        lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                    }
                    if (lbf.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string s in lbf.lb.CheckedItems)
                        {
                            string[] s_ar = s.Split(':');
                            string si_id = s_ar[0];
                            string item = s_ar[1];
                            string rcx = s_ar[2];
                            string f_name = String.Format("{0}\\{1}.xls", fbd.SelectedPath, item);
                            RC_Html_Output rc_html_p = new RC_Html_Output(si_id, item, rcx, conn, f_name,new RC_S_GR());
                        }
                    }
                    conn.Close();
                }
            }
        }
        private void menu_UNLOCK_S_ITEM(Object sender, EventArgs e)
        {
            MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                OdbcDataReader dr = new OdbcCommand("select si_id,s_item,rcx from sport_item where lock_item =1;", conn).ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                    lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                }
                if (lbf.ShowDialog() == DialogResult.OK)
                {
                    string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                    string si_id = s_ar[0];
                    string item = s_ar[1];
                    string rcx = s_ar[2];
                    using (OdbcCommand cmd = new OdbcCommand(String.Format("update sport_item set lock_item=NULL where si_id={0};", si_id), conn))
                    {
                        MessageBox.Show(string.Format("{0}_{1}_UNLOCK {2}筆", si_id, item, cmd.ExecuteNonQuery()));
                    }
                }
                conn.Close();
            }
        }
        private void menu_Input_RC(Object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                OdbcDataReader dr = new OdbcCommand("select si_id,s_item,rcx from sport_item where lock_item is null order by si_id;", conn).ExecuteReader();
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
                SportDayLib.Form_Input_RC_Client f_RC = new Form_Input_RC_Client(si_id, item, rcx,new Form_Input_RC_ACT_ForClient());
                f_RC.MdiParent = this;
                f_RC.Show();
            }
        }
        private void menu_Input_Field_RC(Object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                OdbcDataReader dr = new OdbcCommand("select fi_id,f_item,rcx from field_item where lock_item is null order by fi_id;", conn).ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球"))
                    {
                        lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                    }
                }
                conn.Close();
            }
            if (lbf.ShowDialog() == DialogResult.OK)
            {
                string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                string si_id = s_ar[0];
                string item = s_ar[1];
                string rcx = s_ar[2];
                Form_Input_FIELD_RC_ACT fact = null;
                if (item.Contains("跳高")) 
                {
                    fact = new Form_Input_FIELDJUMPHIGH_RC_ACT_ForClient();
                }
                else 
                {
                    fact = new Form_Input_FIELD_RC_ACT_ForClient();
                }
                SportDayLib.Form_Input_Field_RC_Client f_RC = new Form_Input_Field_RC_Client(si_id, item, rcx,fact);
                f_RC.MdiParent = this;
                f_RC.Show();
            }
        }
        private void menu_Upgrade_RC(Object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            using(OdbcConnection conn=new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                OdbcDataReader dr = new OdbcCommand("select si_id,s_item,rcx from sport_item order by si_id ", conn).ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球")) continue;
                    lbf.lb.Items.Add(String.Format("{0}:{1}:{2}",dr[0],dr[1],dr[2]));
                }
                conn.Close();
            }
            if (lbf.ShowDialog() == DialogResult.OK)
            {
                string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                string si_id=s_ar[0];
                string item=s_ar[1];
                string rcx = s_ar[2];
                SportDayLib.Form_Input_RC f_RC = new Form_Input_RC(si_id, item,rcx,new Form_Input_RC_ACT_ForAdmin());
                f_RC.MdiParent = this;
                f_RC.Show();
            }
        }
        private void menu_Upgrade_Field(Object sender, EventArgs e)
        {
            MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                OdbcDataReader dr = new OdbcCommand("select fi_id,f_item,rcx from field_item  order by fi_id;", conn).ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1].ToString().Contains("跳") || dr[1].ToString().Contains("球"))
                    {
                        lbf.lb.Items.Add(String.Format("{0}:{1}:{2}", dr[0], dr[1], dr[2]));
                    }
                }
                conn.Close();
            }
            if (lbf.ShowDialog() == DialogResult.OK)
            {
                string[] s_ar = lbf.lb.SelectedItem.ToString().Split(':');
                string si_id = s_ar[0];
                string item = s_ar[1];
                string rcx = s_ar[2];
                Form_Input_FIELD_RC_ACT fact = null;
                if (item.Contains("跳高"))
                {
                    fact = new Form_Input_FIELDJUMPHIGH_RC_ACT_ForAdmin();
                }
                else
                {
                    fact = new Form_Input_FIELD_RC_ACT_ForAdmin();
                }
                SportDayLib.Form_Input_Field_RC_Admin f_RC = new Form_Input_Field_RC_Admin(si_id, item, rcx, fact);
                f_RC.MdiParent = this;
                f_RC.Show();
            }
        }
        private void menu_read_Sport_GR_TB_TXT_TO_DB(Object sender, EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text == "2011")
            
            Lib.Load_Sport_GR_TB_TXT_TO_DB("sport_gr_tb","gr_rc,name,classno,gr_period,gr_date,s_item");
        }
        /// <summary>
        /// 新增進級項目
        /// input:file.txt
        /// si_id;s_item;rcx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_read_Sport_item_for_Prom_TB_TXT_TO_DB(Object sender, EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text == "2011")

                Lib.Load_Sport_TXT_TO_DB_ByTableNameAndFieldName("sport_item", "si_id,s_item,rcx");
        }

        private void menu_read_GR_TXT_TO_DB(Object sender, EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text == "2011")
            
            Basic_HTB_Info.Load_sport_rc_From_GR_txt_To_DB();
        }
        private void menu_Read_GR_TXT_to_TXT(Object sender, EventArgs e)
        {
            Basic_HTB_Info.Sport_GR_TXT_pre_process_TXT();
        }
        private void menu_Read_Field_GR_TXT_to_TXT(Object sender, EventArgs e)
        {
            Basic_HTB_Info.Field_GR_TXT_pre_process_TXT();
        }
        private void menu_load_rc_item_to_db(Object sender, EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text == "2011")
            
            Basic_HTB_Info.Load_sport_rc_From_txt_To_DB();
        }
        private void menu_Read_TXT_to_TXT(Object sender,EventArgs e)
        {
            Basic_HTB_Info.Sport_RC_TXT_pre_process_TXT(); 
        }
        private void menu_Read_Field_TXT_to_TXT(Object sender, EventArgs e)
        {
            Basic_HTB_Info.Field_RC_TXT_pre_process_TXT();
        }

        private void menu_XLS_FileName_Convert_TXT(Object sender,EventArgs e)
        {
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\RACE_EVENT_ITEMS.txt", Encoding.Default);
            String line = null;
            while((line=sr.ReadLine())!=null)
            {
                string[] s_arr = line.Split(';');
                if (s_arr.Length > 2)
                {
                    string s_filename=s_arr[2].Replace('/','\\');
                    string d_fileanme=Basic_HTB_Info.baseFilePath+ @"\xlstxt\" + s_arr[1]+ ".txt";
                    //用ACTIONSCRIPT excel save AS text
                    Lib.convertXlsTxt(s_filename,d_fileanme);
                }
            }
            sr.Close();
            sr.Dispose();
        }
        private void menu_FIELDXLS_FileName_Convert_TXT(Object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\FIELD_EVENT_ITEMS.txt", Encoding.Default);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s_arr = line.Split(';');
                if (s_arr.Length > 2)
                {
                    string s_filename = s_arr[2].Replace('/', '\\');
                    string d_fileanme = Basic_HTB_Info.baseFilePath + @"\xlstxt\" + s_arr[1] + ".txt";
                    Lib.convertXlsTxt(s_filename, d_fileanme);
                }
            }
            sr.Close();
            sr.Dispose();
        }

        private void menu_Load_Sport_item_to_db(Object sender, EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text=="2011")
            
                Basic_HTB_Info.Load_Sport_Item_To_DB();
            
        }
        private void menu_Load_Field_item_to_db(Object sender, EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text == "2011")
                Basic_HTB_Info.Load_Field_Item_To_DB();
        }
        private void menutest(Object sender,EventArgs e)
        {
         /*   OdbcConnection con = new OdbcConnection(Basic_HTB_Info.Conn_Str);
            try
            {
                con.Open();
                con.Close();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            return;
            FormGirdPanel fgp=new FormGirdPanel("day1");
            fgp.MdiParent = this;
            fgp.Show();
            return;
            ProcessStartInfo pi = new ProcessStartInfo(  "wscript.exe");
            pi.UseShellExecute = true;
         //   pi.RedirectStandardOutput = true;
            pi.Arguments = String.Format(Basic_HTB_Info.baseFilePath+@"\(10101)男子A組鉛球(決賽).vbs");
          //  Process p = Process.Start(pi);
            Process p = Process.Start(pi);
            p.WaitForExit();
          * */
        }
        /// <summary>
        /// 從excel檔入定義項目
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void menuImport_Race_Event_XLS_FileName(Object sender, EventArgs e)
        {
            Basic_HTB_Info.Import_Race_Event_XLS_FileName();
        }
        private void menuImport_Field_Event_XLS_FileName(Object sender, EventArgs e)
        {
            Basic_HTB_Info.Import_Field_Event_XLS_FileName();
        }


        private void menu_load_field_rc_item_txt_to_db(Object sender,EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text == "2011")

                Basic_HTB_Info.Load_field_rc_From_txt_To_DB();
        }

        private void menu_load_field_rc_gr_txt_to_db(Object sender,EventArgs e)
        {
            InputTextBox itb = new InputTextBox();
            itb.Text = "PASSWORD";
            if (itb.ShowDialog() == DialogResult.OK && itb.tb.Text == "2011")

                Basic_HTB_Info.Load_field_rc_gr_From_txt_To_DB();
        }

        public void menu_Syn_From_SQLite (Object Sender,EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.DefaultExt = ".db";
            ofd.Filter = "SQLite (.DB)|*.db";
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                String filepath = ofd.FileName;
                using(SQLiteConnection sqliteconn=new SQLiteConnection(String.Format("Data Source={0}",filepath)))
                {
                    sqliteconn.Open();
                    String sql = "SELECT name From sqlite_master Where type in('table','view') AND name Not Like 'sqlite_%' UNION ALL SELECT name From sqlite_temp_master Where type IN('table','view') ORDER BY 1";
                    SQLiteDataReader dr = new SQLiteCommand(sql,sqliteconn).ExecuteReader();
                    MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
                    while (dr.Read())
                    {
                        lbf.lb.Items.Add(dr[0].ToString());
                    }
                    if (lbf.ShowDialog() == DialogResult.OK)
                    {
                        OdbcConnection odbcconn = new OdbcConnection(Basic_HTB_Info.Conn_Str);
                        odbcconn.Open();
                        foreach (string s in lbf.lb.CheckedItems)
                        {
                            string sql_tbl = "Select * from " + s;
                            SQLiteDataReader dr_tbl = new SQLiteCommand(sql_tbl,sqliteconn).ExecuteReader();
                            
                            String inc_sql = String.Format("insert into {0} values (?",s);
                            for (int i = 1; i < dr_tbl.FieldCount; i++)
                            {
                                inc_sql += ",?";
                            }
                            inc_sql += ");";
                            OdbcCommand inccmd = new OdbcCommand(inc_sql, odbcconn);
                            for (int i = 0; i < dr_tbl.FieldCount; i++)
                            {
                                inccmd.Parameters.Add(new OdbcParameter());
                            }
                            while (dr_tbl.Read())
                            {
                                for (int i = 0; i < dr_tbl.FieldCount; i++)
                                {
                                    inccmd.Parameters[i].Value = dr_tbl[i].ToString();
                                }
                                inccmd.ExecuteNonQuery();
                            }
                        }
                        odbcconn.Close();
                        odbcconn.Dispose();
                    }
                }
            }
        }

        private void menu_reg_student_gi(Object Sender,EventArgs e) 
        {
            Form_Search fs = new Form_Search();
            fs.MdiParent = this;

            fs.Show();
        }
        /// <summary>
        /// gt_item 中比賽項目設定為預賽項目
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void menu_gtitem2spitem(Object Sender,EventArgs e)
        {
            using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                String sql = "select gi,gid from gt_item;";
                using (OdbcDataReader dr = new OdbcCommand(sql,conn).ExecuteReader())
                {
                    MPPFORM.ListBoxForm lb = new MPPFORM.ListBoxForm();
                    while (dr.Read())
                    {
                        lb.lb.Items.Add(String.Format("{0}.{1}(預賽)",dr[0],dr[1]));
                    }
                    if (lb.ShowDialog() == DialogResult.OK)
                    {
                        String fieldmaxid_sql="select max(fi_id) m from field_item; ";
                        int fieldmaxid=0;
                        int sportmaxid=0;
                        String sportmaxid_sql="select max(si_id) m from sport_item;";
                        using(OdbcDataReader dr0=new OdbcCommand(fieldmaxid_sql,conn).ExecuteReader())
                        {
                            if(dr0.Read())
                            {
                                fieldmaxid=int.Parse(dr0[0].ToString());
                            }
                        }
                        using(OdbcDataReader dr0=new OdbcCommand(sportmaxid_sql,conn).ExecuteReader())
                        {
                            if(dr0.Read())
                            {
                                sportmaxid=int.Parse(dr0[0].ToString());
                            }
                        }
                        int cnt=0;
                        foreach (string s in lb.lb.CheckedItems)
                        {
                            string RC1 = "RC1";
                            string title="名次,組次,道次,號碼,姓名,班級,成績,GRK,備註";
                            string[] s_a=s.Split('.');
                            string gi=s_a[0];
                            string item=s_a[1];

                            if (s.Contains("球") || s.Contains("跳"))
                            {
                                fieldmaxid++;
                                RC1 = "RCFIE";if(s.Contains("跳高")) RC1="RCFJH";
                                string incsql=string.Format("insert into field_item (fi_id,f_item,rcx,gi)values({0},'{1}','{2}',{3})",fieldmaxid,item,RC1,gi);
                                OdbcCommand cmd=new OdbcCommand(incsql,conn);
                                cnt+=cmd.ExecuteNonQuery();
                                
                            }
                            else
                            {
                                sportmaxid++;
                                String incsql=string.Format("insert into sport_item(si_id,s_item,rcx,title,gi)values({0},'{1}','{2}','{3}',{4});",sportmaxid,item,RC1,title,gi);
                                OdbcCommand cmd=new OdbcCommand(incsql,conn);
                                cnt+=cmd.ExecuteNonQuery();
                               
                            }
                        }
                        MessageBox.Show(cnt.ToString());
                    }

                }
                conn.Close();
            }
        }
        /// <summary>
        /// stud_gti 設定為參賽名單
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void menu_studgti2sprc(Object Sender,EventArgs e)
        {
            String[] groupname = {"一","二","三","四","五","六","七","八","九","十","十一","十二","十三","十四","十五","十六","十七","十八","十九","廿","廿一" };
            int road_p = 5;
            
            String sql = "select si_id,s_item,gi from sport_item where not gi is null;";
            MPPFORM.ListBoxForm lb = new MPPFORM.ListBoxForm();
            using(OdbcConnection conn=new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                using (OdbcDataReader dr = new OdbcCommand(sql, conn).ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lb.lb.Items.Add(String.Format("{0}.{1}.{2}",dr[0],dr[1],dr[2]));
                    }
                }
                if (lb.ShowDialog() == DialogResult.OK)
                {
                    String fmt = " SELECT a.stud_ref,gi, number,name_c,concat(grade,classno) class_no FROM stud_gti a" +
" INNER JOIN student b ON a.stud_ref = b.stud_ref and a.reg=1 and gi={0} order by concat(grade,classno)";
                    string cntfmt = " SELECT count(*) cnt FROM stud_gti a INNER JOIN student b ON a.stud_ref = b.stud_ref and a.reg=1 and gi={0} order by concat(grade,classno)";
                    int inc_cnt = 0;
                    foreach (string s in lb.lb.CheckedItems)
                    {
                        String[] s_a = s.Split('.');
                        string fiid = s_a[0];
                        string fitem = s_a[1];
                        string gi = s_a[2];
                        int pcount = 0;
                        using (OdbcDataReader dr = new OdbcCommand(String.Format(cntfmt, gi), conn).ExecuteReader())
                        {
                            if(dr.Read())  pcount = int.Parse(dr[0].ToString());
                        }
                        int groupnum=pcount / road_p;
                        if(pcount % road_p >0) groupnum++;
                        int[] group_road = new int[groupnum];

                        using (OdbcDataReader dr = new OdbcCommand(String.Format(fmt,gi),conn).ExecuteReader())
                        {
                            int cnt=0;
                            while (dr.Read())
                            {
                                int temp_g = cnt % groupnum;
                                group_road[temp_g]++;
                                String incsqlfmt = "insert into  sport_rc (si_id,group_id,road,s_number, number,classno,name,stud_ref,gi) values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');";
                                cnt++;
                                string g_n=temp_g.ToString();
                                if (temp_g < groupname.Length) g_n = groupname[temp_g];

                                string incsql = string.Format(incsqlfmt,fiid,g_n,group_road[temp_g],cnt,dr["number"],dr["class_no"],dr["name_c"],dr["stud_ref"],gi);
                                OdbcCommand cmd = new OdbcCommand(incsql, conn);
                                inc_cnt += cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    MessageBox.Show(inc_cnt.ToString());
                }

                conn.Close();
            }
        }

        
        private void menu_studgti2fieldrc(Object Sender, EventArgs e)
        {
            
            String sql = "select fi_id,f_item,gi from field_item where not gi is null;";
            MPPFORM.ListBoxForm lb = new MPPFORM.ListBoxForm();
            using(OdbcConnection conn=new OdbcConnection(Basic_HTB_Info.Conn_Str))
            {
                conn.Open();
                using (OdbcDataReader dr = new OdbcCommand(sql, conn).ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lb.lb.Items.Add(String.Format("{0}.{1}.{2}",dr[0],dr[1],dr[2]));
                    }
                }
                if (lb.ShowDialog() == DialogResult.OK)
                {
                    String fmt = " SELECT a.stud_ref,gi, number,name_c,concat(grade,classno) class_no FROM stud_gti a" +
" INNER JOIN student b ON a.stud_ref = b.stud_ref and a.reg=1 and gi={0} order by concat(grade,classno)";
                    int inc_cnt = 0;
                    foreach (string s in lb.lb.CheckedItems)
                    {
                        String[] s_a = s.Split('.');
                        string fiid = s_a[0];
                        string fitem = s_a[1];
                        string gi = s_a[2];
                        using (OdbcDataReader dr = new OdbcCommand(String.Format(fmt,gi),conn).ExecuteReader())
                        {
                            int cnt=1;
                            while (dr.Read())
                            {
                                String incsqlfmt = "insert into  field_rc (fi_id,s_number, number,classno,name,stud_ref,gi) values({0},'{1}','{2}','{3}','{4}','{5}',{6});";
                                string incsql = string.Format(incsqlfmt,fiid,cnt,dr["number"],dr["class_no"],dr["name_c"],dr["stud_ref"],gi);
                                OdbcCommand cmd = new OdbcCommand(incsql, conn);
                                inc_cnt += cmd.ExecuteNonQuery();
                                cnt++;
                            }
                        }
                    }
                    MessageBox.Show(inc_cnt.ToString());
                }

                conn.Close();
            }
        }
    }
}