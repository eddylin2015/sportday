using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using MPPFORM;
using System.Data;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;


namespace SportDayLib
{
    /// <summary>
    /// 共用字典
    /// </summary>
    public class Basic_HTB_Info
    {
        private static ConfigIni private_cfgini = null;
        /// <summary>
        /// Config Ini 
        /// </summary>
        public static ConfigIni cfg  { get{ if (private_cfgini == null) private_cfgini = new ConfigIni(); return private_cfgini; } }
        /// <summary>
        /// Connection String
        /// </summary>
        public static string Conn_Str
        {
            get
            {
                if (__conn_str == null)
                {
                    FileInfo finfo0 = new FileInfo(initxt);
                    if (finfo0.Exists)
                    {
                        using (StreamReader file = new StreamReader(initxt, Encoding.Default))
                        {
                            __conn_str = file.ReadToEnd().Replace("\r", "").Replace("\n", "");
                        }
                    }
                    else
                    {
                        MessageBox.Show("error not db_config.txt");
                        throw new Exception("not config.txt");
                    }
                    //__conn_str= "Driver={MySQL ODBC 5.1 Driver};Server=" + cfg.DB_HOST + ";Database=SportDay2015;UID=xxxxx;PWD=xxxxx;OPTION=3";
                }
                    return __conn_str;
            }
        }
        private static string __conn_str = null;
        private static string initxt = @"c:\code\config\sportday_dbconnection_str.txt";
        /// <summary>
        /// 系統目錄
        /// </summary>
        private static string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        /// <summary>
        /// 文件目錄
        /// </summary>
        public static string baseFilePath { get{ return appPath.Substring(6); } }
        /// <summary>
        /// 運動項目字典
        /// </summary>
        public readonly Hashtable Race_Events = new Hashtable();

        private Basic_HTB_Info()
        {
            Load_Race_Event_ITEMS_FROM_FILE();
        }

        private static Basic_HTB_Info instance = null;
        /// <summary>
        /// One Instance;
        /// </summary>
        /// <returns>Basic_HTB_Info</returns>
        public static Basic_HTB_Info GetInstance
        {
            get{if (instance == null) instance = new Basic_HTB_Info();return instance; }
        }
        /// <summary>
        /// 從檔案載入運動項目字典
        /// </summary>
        public void Load_Race_Event_ITEMS_FROM_FILE()
        {
            string race_event_item_file = Basic_HTB_Info.baseFilePath + @"\RACE_EVENT_ITEMS.txt";
            FileInfo finfo = new FileInfo(race_event_item_file);
            if (!finfo.Exists) return;
            Race_Events.Clear();
            StreamReader sr = new StreamReader(race_event_item_file, Encoding.Default);
            string input = null;
            while ((input = sr.ReadLine()) != null)
            {
                string[] strA = input.Split(';');
                if (strA.Length > 1)
                {
                    int basekey = int.Parse(strA[0]);
                    string baseFieldEvent = strA[1];
                    try
                    {
                        Race_Events.Add(basekey, baseFieldEvent);
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show(e.Message);
                    }
                }
            }
            sr.Close();
            sr.Dispose();
        }
        /// <summary>
        /// 匯入excel檔案,write to RACE_EVENT_ITEMS.txt
        /// </summary>
        public static  void Import_Race_Event_XLS_FileName()
        {
            int SI_id = 0;
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = Basic_HTB_Info.baseFilePath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                // Process the list of files found in the directory.
                List<string> ls = new List<string>();
                string[] fileEntries = Directory.GetFiles(fbd.SelectedPath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains(".XLS"))
                    {
                        FileInfo finfo = new FileInfo(fileName);
                        string si_name = finfo.Name.Substring(0, finfo.Name.Length - 4);
                        ls.Add(string.Format("{0};{1};{2}",++SI_id,si_name,finfo.FullName));
                    }
                }
                String[] dirEntries = Directory.GetDirectories(fbd.SelectedPath);
                foreach (string dirName in dirEntries)
                {
                    string[] subdir_fileEntries=Directory.GetFiles(dirName);
                    foreach (string fileName in subdir_fileEntries)
                    {
                        if (fileName.ToUpper().Contains(".XLS"))
                        {
                            FileInfo finfo = new FileInfo(fileName);
                            string si_name = finfo.Name.Substring(0, finfo.Name.Length - 4);
                            ls.Add(string.Format("{0};{1};{2}",++SI_id,si_name,finfo.FullName));
                        }
                    }
                }
                MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
                foreach (string s in ls)
                {
                    lbf.lb.Items.Add(s);
                }
                if (lbf.ShowDialog() == DialogResult.OK)
                {
                    string filename = string.Format(@"{0}\RACE_EVENT_ITEMS.txt", Basic_HTB_Info.baseFilePath);
                    StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);
                    foreach (string s in lbf.lb.CheckedItems)
                        sw.WriteLine(s);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                    Basic_HTB_Info.GetInstance.Load_Race_Event_ITEMS_FROM_FILE();
                }
            }
        }
        public static void Import_Field_Event_XLS_FileName()
        {
            int FI_id = 0;
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = Basic_HTB_Info.baseFilePath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                // Process the list of files found in the directory.
                List<string> ls = new List<string>();
                string[] fileEntries = Directory.GetFiles(fbd.SelectedPath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.ToUpper().Contains(".XLS"))
                    {
                        FileInfo finfo = new FileInfo(fileName);
                        string si_name = finfo.Name.Substring(0, finfo.Name.Length - 4);
                        ls.Add(string.Format("{0};{1};{2}", ++FI_id, si_name, finfo.FullName));
                    }
                }
                String[] dirEntries = Directory.GetDirectories(fbd.SelectedPath);
                foreach (string dirName in dirEntries)
                {
                    string[] subdir_fileEntries = Directory.GetFiles(dirName);
                    foreach (string fileName in subdir_fileEntries)
                    {
                        if (fileName.ToUpper().Contains(".XLS"))
                        {
                            FileInfo finfo = new FileInfo(fileName);
                            string si_name = finfo.Name.Substring(0, finfo.Name.Length - 4);
                            ls.Add(string.Format("{0};{1};{2}", ++FI_id, si_name, finfo.FullName));
                        }
                    }
                }
                MPPFORM.ListBoxFilterForm lbf = new MPPFORM.ListBoxFilterForm();
                foreach (string s in ls)
                {
                    lbf.lb.Items.Add(s);
                }
                if (lbf.ShowDialog() == DialogResult.OK)
                {
                    string filename = string.Format(@"{0}\FIELD_EVENT_ITEMS.txt", Basic_HTB_Info.baseFilePath);
                    StreamWriter sw = new StreamWriter(filename, false, Encoding.Default);
                    foreach (string s in lbf.lb.CheckedItems)
                        sw.WriteLine(s);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                   // Basic_HTB_Info.GetInstance.Load_Race_Event_ITEMS_FROM_FILE();
                }
            }
        }
        private static Hashtable private_rcx_htb=null;
        private static Hashtable private_rcx_sql_htb = null;
        public static Hashtable signflag_sql
        {
            get
            {
                if (private_rcx_sql_htb == null)
                {
                    private_rcx_sql_htb = new Hashtable();
                    private_rcx_sql_htb.Add("RC5", "si_id,rcx,rank,s_number,number,name,classno,rc,note");
                    private_rcx_sql_htb.Add("RC0", "si_id,rcx,rank,s_number,number,classno,name,rc,note");
                    private_rcx_sql_htb.Add("RC1", "si_id,rcx,rank,group_id,road,number,classno,name,rc,grk,note");
                    private_rcx_sql_htb.Add("RC2", "si_id,rcx,rank,road,number,classno,name,rc,note");
                    private_rcx_sql_htb.Add("RC3", "si_id,rcx,rank,group_id,road,name,number,rc,classno,grk,note");
                    private_rcx_sql_htb.Add("RC4", "si_id,rcx,rank,road,name,number,rc,classno,note");
                    private_rcx_sql_htb.Add("RC6", "si_id,rcx,rank,group_id,road,number,name,classno,rc,note");
                    private_rcx_sql_htb.Add("RC7", "si_id,rcx,rank,group_id,s_number,number,name,classno,rc,grk,note");

                }
                return private_rcx_sql_htb;
            }
        }

        public static Hashtable signflag_s
        {
            get
            {
                if (private_rcx_htb == null)
                {
                    private_rcx_htb = new Hashtable();
                    private_rcx_htb.Add("名次,小號,號碼,姓名,班別,成績,備註","RC5");//2011ver 800以上
                    private_rcx_htb.Add("名次,小號,號碼,姓名,班級,成績,備註", "RC5");//2011ver 800以上
                    private_rcx_htb.Add("名次,小號,比賽號,班別,姓名,成績,備註", "RC0");//800以上
                    private_rcx_htb.Add("名次,小號,號碼,班別,姓名,成績,備註", "RC0");//800以上 比賽號,號碼 決
                    private_rcx_htb.Add("名次,組次,小號,號碼,姓名,班別,成績,GRK,備註", "RC7");//800以上 比賽號,號碼 分組決
                    private_rcx_htb.Add("名次,組次,道次,號碼,姓名,班級,成績,GRK,備註", "RC1");//短程100 200 400 初,複
                    private_rcx_htb.Add("名次,組次,道次,號碼,姓名,班別,成績,GRK,備註", "RC1");//短程100 200 400 初,複
                    private_rcx_htb.Add("名次,組次,道次,號碼,姓名,班級,成績,備註", "RC6");//短程100 200 400 初,複
                    private_rcx_htb.Add("名次,道次,號碼,姓名,班級,成績,備註", "RC2");//短程100 200 400 決
                    private_rcx_htb.Add("名次,組次,道次,隊名,,成績,,GRK,備註", "RC3");//4x初, 複
                    private_rcx_htb.Add("名次,道次,隊名,,成績,,備註", "RC4");//4x決賽
                } 
                return private_rcx_htb;
            }
        }
        public static string Get_Signflag_s(string rcx)
        {
            foreach (DictionaryEntry e in Basic_HTB_Info.signflag_s)
            {
                if (e.Value.ToString() == rcx)
                {
                    return e.Key.ToString();
                }
            }
            return null;
        }
        public static void Adjust_FieldName(ref string fieldnames, ref string c_fieldnames, ref string colspan_str)
        {

            string[] cf_ar = c_fieldnames.Split(',');
            string[] f_ar = fieldnames.Split(',');
            string n_cf = cf_ar[0];
            string n_f = String.Format("{0},{1},{2}", f_ar[0], f_ar[1], f_ar[2]);
            colspan_str = "1";
            int colspan_cnt = 1;
            for (int i = 1; i < cf_ar.Length; i++)
            {
                if (i+1<cf_ar.Length && cf_ar[i+1] == "")  colspan_cnt++;
                if(cf_ar[i]=="")    continue; 
                n_cf += "," + cf_ar[i];
                n_f += "," + f_ar[i + 2];
                colspan_str +=","+colspan_cnt ;
                colspan_cnt = 1;
            }
            
            fieldnames = n_f;
            c_fieldnames = n_cf;
        }
        private static Hashtable Get_Item_To_si_id(OdbcConnection conn)
        {
            Hashtable rc_e_tb = new Hashtable();
            using (OdbcDataReader dr = new OdbcCommand("select si_id,s_item from sport_item;", conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    rc_e_tb.Add(dr["s_item"].ToString(), dr["si_id"].ToString());
                }
            }
            return rc_e_tb;
        }
        private static Hashtable Get_Item_To_fi_id(OdbcConnection conn)
        {
            Hashtable rc_e_tb = new Hashtable();
            using (OdbcDataReader dr = new OdbcCommand("select fi_id,f_item from field_item;", conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    rc_e_tb.Add(dr["f_item"].ToString(), dr["fi_id"].ToString());
                }
            }
            return rc_e_tb;
        }
        /// <summary>
        /// 將txt 成績記錄匯總入一個TXT File
        /// </summary>
        public static void Sport_RC_TXT_pre_process_TXT()
        {
            string signflag = "RC0";
            StreamWriter sw = new StreamWriter(Basic_HTB_Info.baseFilePath+@"\PreProcessSportRC.txt", false);
            string[] file_contain_txt = {  "跳高", "跳遠", "鉛球", "壘球" };
            string b_path = Basic_HTB_Info.baseFilePath + @"\xlstxt\";
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\RACE_EVENT_ITEMS.txt", Encoding.Default);

            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s_ar = line.Split(';');
                if (s_ar.Length > 2)
                {
                    bool flag = true;
                    foreach (string s in file_contain_txt)
                    {
                        if (s_ar[2].Contains(s)) { flag = false; break; }
                    }
                    if (flag)
                    {
                        string s_filename = b_path + s_ar[1] + ".txt";
                        sw.WriteLine("FILE: {0}", s_ar[1]);
                        StreamReader sr_rc = new StreamReader(s_filename, Encoding.Default);
                        String rc_line = null;
                        while ((rc_line = sr_rc.ReadLine()) != null)
                        {
                            string[] s_rc_ar = rc_line.Split(',');

                            if (s_rc_ar.Length > 5)
                            {
                                if (s_rc_ar[0].Length > 3 && (s_rc_ar[0][0] == '女' || s_rc_ar[0][0] == '男')) continue;
                                if (s_rc_ar[0].Contains("成績")
                                    || s_rc_ar[0].Contains("紀錄")
                                    || s_rc_ar[0].Contains("GR")
                                    || s_rc_ar[0].Contains("DQ")
                                    || s_rc_ar[0].Contains("註釋")
                                    || s_rc_ar[0].Contains("徑賽")
                                    ) continue;
                                if (s_rc_ar[0].Contains("名次"))
                                {
                                    string[] title_ar=rc_line.Split(',');
                                    string title = title_ar[0];
                                    for (int i = 1; i < title_ar.Length;i++ )
                                    {
                                        title += "," + title_ar[i];
                                        if (title_ar[i].Contains("備註")) break;
                                    }
                                    sw.WriteLine("TITLE: {0}", title);
                                    if (signflag_s.Contains(title))
                                    {
                                        signflag = signflag_s[title].ToString();
                                    }
                                    else
                                    {
                                        signflag = "RCX";
                                    }
                                    continue;
                                }
                                // sw.WriteLine(rc_line);
                                bool l_flag = false;
                                int len = s_rc_ar.Length; if (len > 9) len = 9;
                                for (int i = 0; i < len; i++)
                                    if (s_rc_ar[i] == "") { l_flag = l_flag || false; } else { l_flag = l_flag || true; }
                                if (!l_flag) continue;
                                string out_str = signflag;
                                for (int i = 0; i < len; i++) out_str += "\t" + s_rc_ar[i];
                                sw.WriteLine(out_str);
                            }
                        }
                        sr_rc.Close();
                        sr_rc.Dispose();
                        sw.WriteLine("END");
                    }
                }
            }
            sr.Close();
            sr.Dispose();
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 田賽將txt 成績記錄匯總入一個TXT File
        /// </summary>
        public static void Field_RC_TXT_pre_process_TXT()
        {
            string signflag = "RC0";
            StreamWriter sw = new StreamWriter(Basic_HTB_Info.baseFilePath + @"\PreProcessFieldRC.txt", false);
            string[] file_contain_txt = { "跳高", "跳遠", "鉛球", "壘球" };
            string b_path = Basic_HTB_Info.baseFilePath + @"\xlstxt\";
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\FIELD_EVENT_ITEMS.txt", Encoding.Default);

            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s_ar = line.Split(';');
                if (s_ar.Length > 2)
                {
                    bool flag = false;
                    foreach (string s in file_contain_txt)
                    {
                        if (s_ar[2].Contains(s)) { flag = true; break; }
                    }
                    if (flag)
                    {
                        string s_filename = b_path + s_ar[1] + ".txt";
                        sw.WriteLine("FILE: {0}", s_ar[1]);
                        StreamReader sr_rc = new StreamReader(s_filename, Encoding.Default);
                        String rc_line = null;
                        while ((rc_line = sr_rc.ReadLine()) != null)
                        {
                            string[] s_rc_ar = rc_line.Trim().Split(',');
                            
                            if (s_rc_ar.Length > 1)
                            {
                                if (s_rc_ar[0].Length > 0 && (
                                    s_rc_ar[0][0] == '女'
                                    || s_rc_ar[0][0] == '男'
                                    || s_rc_ar[0].Contains("成績")
                                    || s_rc_ar[0][0] == '名'
                                    || s_rc_ar[0].Contains("紀錄")
                                    || s_rc_ar[0].Contains("GR") 
                                    || s_rc_ar[0].Contains("DQ")
                                    || s_rc_ar[0].Contains("註釋") 
                                    || s_rc_ar[0].Contains("徑賽")
                                    || s_rc_ar[0].Contains("田賽裁判長")
                                    
                                    ))
                                {
                                    continue;
                                }
                                else if (
                                           s_rc_ar[1].Contains("GR")
                                        || s_rc_ar[1].Contains("註釋")
                                        || s_rc_ar[1].Contains("DNS")
                                      || s_rc_ar[1].Contains("B3")
                                    
                                    )
                                {
                                    continue;
                                }
                                if (s_rc_ar[0].Length > 0&&s_rc_ar[0][0]=='次')
                                {

                                    string title = "名次,次序,比賽號,姓名,班級";

                                    sw.WriteLine("TITLE: {0}", title);
                                    if (signflag_s.Contains(title))
                                    {
                                        signflag = signflag_s[title].ToString();
                                    }
                                    else
                                    {
                                        signflag = "RCX";
                                    }
                                    continue;
                                }
                                // sw.WriteLine(rc_line);
                                bool l_flag = false;
                                int len = s_rc_ar.Length; 
                                if (len > 5) len = 5;
                                for (int i = 0; i < len; i++)
                                    if (s_rc_ar[i] == "") { l_flag = l_flag || false; } else { l_flag = l_flag || true; }
                                if (!l_flag) continue;
                                string out_str = signflag;
                                for (int i = 0; i < len; i++) out_str += "\t" + s_rc_ar[i];
                                sw.WriteLine(out_str);
                            }
                        }
                        sr_rc.Close();
                        sr_rc.Dispose();
                        sw.WriteLine("END");
                    }
                }
            }
            sr.Close();
            sr.Dispose();
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 將txt GR 成績記錄匯總入一個TXT File
        /// </summary>
        public static void Sport_GR_TXT_pre_process_TXT()
        {
            StreamWriter sw = new StreamWriter(Basic_HTB_Info.baseFilePath + @"\PreProcessSportGR.txt", false);
            string[] file_contain_txt = {  "跳高", "跳遠", "鉛球", "壘球" };
            string b_path = Basic_HTB_Info.baseFilePath + @"\xlstxt\";
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\RACE_EVENT_ITEMS.txt", Encoding.Default);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s_ar = line.Split(';');
                if (s_ar.Length > 2)
                {
                    bool flag = true;
                    foreach (string s in file_contain_txt)
                    {
                        if (s_ar[2].Contains(s)) { flag = false; break; }
                    }
                    if (flag)
                    {
                        string s_filename = b_path + s_ar[1] + ".txt";
                        sw.WriteLine("FILE: {0}", s_ar[1]);
                        StreamReader sr_rc = new StreamReader(s_filename, Encoding.Default);
                        String rc_line = null;
                        while ((rc_line = sr_rc.ReadLine()) != null)
                        {
                            string[] s_rc_ar = rc_line.Split(',');

                            if (s_rc_ar.Length > 3)
                            {
                                if (s_rc_ar[0].Trim() == "GR")
                                {
                                    int cnt = 0;
                                    string out_s="GR";
                                    int i=1;
                                    while (i < s_rc_ar.Length)
                                    {
                                        if (s_rc_ar[i] != "")
                                        {
                                            cnt++;
                                            out_s += "," + s_rc_ar[i];
                                            if (s_rc_ar[i][0] == '"' && s_rc_ar[i][s_rc_ar[i].Length - 1] != '"')
                                            {
                                                while (true)
                                                {
                                                    i++;
                                                    out_s += " " + s_rc_ar[i];
                                                    if (i > s_rc_ar.Length - 1) break;
                                                    if (s_rc_ar[i][s_rc_ar[i].Length - 1] == '"') break;
                                                }
                                            }
                                        }
                                       
                                        if (cnt > 4) break;
                                        i++;
                                    }
                                   
                                    sw.WriteLine(out_s);
                                }
                            }
                        }
                        sr_rc.Close();
                        sr_rc.Dispose();
                        sw.WriteLine("END");
                    }
                }
            }
            sr.Close();
            sr.Dispose();
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void Field_GR_TXT_pre_process_TXT()
        {
            StreamWriter sw = new StreamWriter(Basic_HTB_Info.baseFilePath + @"\PreProcessFieldGR.txt", false);
            string[] file_contain_txt = { "跳高", "跳遠", "鉛球", "壘球" };
            string b_path = Basic_HTB_Info.baseFilePath + @"\xlstxt\";
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\FIELD_EVENT_ITEMS.txt", Encoding.Default);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s_ar = line.Split(';');
                if (s_ar.Length > 2)
                {
                    bool flag = false;
                    foreach (string s in file_contain_txt)
                    {
                        if (s_ar[2].Contains(s)) { flag = true; break; }
                    }
                    if (flag)
                    {
                        string s_filename = b_path + s_ar[1] + ".txt";
                        sw.WriteLine("FILE: {0}", s_ar[1]);
                        StreamReader sr_rc = new StreamReader(s_filename, Encoding.Default);
                        String rc_line = null;
                        while ((rc_line = sr_rc.ReadLine()) != null)
                        {
                            string[] s_rc_ar = rc_line.Split(',');

                            if (s_rc_ar.Length > 3)
                            {
                                if (s_rc_ar[0].Trim() == "GR")
                                {
                                    int cnt = 0;
                                    string out_s = "GR";
                                    int i = 1;
                                    while (i < s_rc_ar.Length)
                                    {
                                        if (s_rc_ar[i] != "")
                                        {
                                            cnt++;
                                            out_s += "," + s_rc_ar[i];
                                            if (s_rc_ar[i][0] == '"' && s_rc_ar[i][s_rc_ar[i].Length - 1] != '"')
                                            {
                                                while (true)
                                                {
                                                    i++;
                                                    out_s += " " + s_rc_ar[i];
                                                    if (i > s_rc_ar.Length - 1) break;
                                                    if (s_rc_ar[i][s_rc_ar[i].Length - 1] == '"') break;
                                                }
                                            }
                                        }

                                        if (cnt > 4) break;
                                        i++;
                                    }

                                    sw.WriteLine(out_s);
                                }
                            }
                        }
                        sr_rc.Close();
                        sr_rc.Dispose();
                        sw.WriteLine("END");
                    }
                }
            }
            sr.Close();
            sr.Dispose();
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 從預處理文檔,載入DB sport_item
        /// </summary>
        public static void Load_Sport_Item_To_DB()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName="RACE_EVENT_ITEMS.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using(OdbcConnection conn = new OdbcConnection(Conn_Str))
                {
                    conn.Open();
                    MessageBox.Show("TRUNCATE TABLE sport_item;");
                    using (OdbcCommand cmd = new OdbcCommand("TRUNCATE TABLE sport_item;",conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    StreamReader sr = new StreamReader(ofd.FileName,Encoding.Default);
                    string line=null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] str_a = line.Split(';');
                        if (str_a.Length > 2)
                        {
                            OdbcCommand cmd = new OdbcCommand(String.Format("insert into sport_item (si_id,s_item,filename) values('{0}','{1}','{2}');", str_a[0], str_a[1], str_a[2].Replace('\\','/')), conn);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
            }
        }
        public static void Load_Field_Item_To_DB()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "FIELD_EVENT_ITEMS.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (OdbcConnection conn = new OdbcConnection(Conn_Str))
                {
                    conn.Open();
                    if (MessageBox.Show("TRUNCATE TABLE field_item;","",MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (OdbcCommand cmd = new OdbcCommand("TRUNCATE TABLE field_item;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    StreamReader sr = new StreamReader(ofd.FileName, Encoding.Default);
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] str_a = line.Split(';');
                        if (str_a.Length > 2)
                        {
                            String rcx = "RCFIE";
                            if (str_a[1].Contains("跳高")) { rcx = "RCFJH"; }
                            OdbcCommand cmd = new OdbcCommand(String.Format("insert into field_item (fi_id,f_item,filename,rcx) values('{0}','{1}','{2}','{3}');", str_a[0], str_a[1], str_a[2].Replace('\\', '/'),rcx), conn);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
            }
        }

        private static void upd_item(string rcx,string title,int si_id,OdbcConnection conn)
        {
            string sql_head = "update sport_item set rcx='{1}' ,title='{2}'  where si_id = '{0}'; ";
            OdbcCommand cmd=new OdbcCommand(string.Format(sql_head,si_id,rcx,title),conn);
            cmd.ExecuteNonQuery();
        }
        
        private static void inc_rc0(string[] value_s,int si_id,OdbcConnection conn)
        {
            string sql_head = "insert into sport_rc ({0}) values ({1});";
            string rcx=value_s[0];
            if (rcx == "") return;
            string sql_f = null;
            if (signflag_sql.Contains(rcx))
            {
                sql_f = signflag_sql[rcx].ToString();
                
            }
            else { //cool
              

                throw new Exception(rcx); }
            string qm_s = "?";
            string[] s_ar = sql_f.Split(',');
            for (int i = 1; i < s_ar.Length; i++) qm_s += ",?";
            string sql = string.Format(sql_head, sql_f, qm_s);
            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                cmd.Parameters.Add("@si_id", OdbcType.Int,0,"si_id");                
                for (int i = 1; i < s_ar.Length; i++)
                    cmd.Parameters.Add("@" + s_ar[i], OdbcType.VarChar, 32,s_ar[i]);
                cmd.Parameters["@si_id"].Value = si_id;
                for (int i = 1; i < s_ar.Length; i++)
                    cmd.Parameters["@" + s_ar[i]].Value = value_s[i - 1];
                cmd.ExecuteNonQuery();
            }
        }
        private static void inc_field_rc0(string[] value_s, int fi_id, OdbcConnection conn)
        {
            string sql_head = "insert into field_rc ({0}) values ({1});";
            string rcx = value_s[0];
            if (rcx == "") return;
            string sql_f =  "fi_id,rank,s_number,number,name,classno";

            string qm_s = "?";
            string[] s_ar = sql_f.Split(',');
            for (int i = 1; i < s_ar.Length; i++) qm_s += ",?";
            string sql = string.Format(sql_head, sql_f, qm_s);
            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                cmd.Parameters.Add("@fi_id", OdbcType.Int, 0, "fi_id");
                for (int i = 1; i < s_ar.Length; i++)
                    cmd.Parameters.Add("@" + s_ar[i], OdbcType.VarChar, 32, s_ar[i]);
                cmd.Parameters["@fi_id"].Value = fi_id;
                for (int i = 1; i < s_ar.Length; i++)
                    cmd.Parameters["@" + s_ar[i]].Value = value_s[i ];
                cmd.ExecuteNonQuery();
            }
        }
        public static void Load_field_rc_From_txt_To_DB()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = Basic_HTB_Info.baseFilePath + @"\PreProcessfieldRC.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (OdbcConnection conn = new OdbcConnection(Conn_Str))
                {
                    conn.Open();

                    if (MessageBox.Show("TRUNCATE TABLE field_rc;", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (OdbcCommand cmd = new OdbcCommand("TRUNCATE TABLE field_rc;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Hashtable rc_e_tb = Get_Item_To_fi_id(conn);
                    StreamReader sr = new StreamReader(ofd.FileName);
                    string line = null;
                    string file_name = "";
                    //string rc_title = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("FILE: ")) file_name = line.Substring(6);
                        if (line.Contains("TITLE: "))
                        {
                           //rc_title = line.Substring(7);
                           // upd_item(signflag_s[rc_title].ToString(), rc_title, int.Parse(rc_e_tb[file_name].ToString()), conn);
                        }
                        string[] str_a = line.Split('\t');
                        if (str_a.Length > 2)
                        {
                            inc_field_rc0(str_a, int.Parse(rc_e_tb[file_name].ToString()), conn);
                        }
                    }
                    conn.Close();
                }
            }
        }
        public static void Load_field_rc_gr_From_txt_To_DB()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = Basic_HTB_Info.baseFilePath + @"\PreProcessfieldGR.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (OdbcConnection conn = new OdbcConnection(Conn_Str))
                {
                    conn.Open();
                    if (MessageBox.Show("TRUNCATE TABLE field_gr;", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (OdbcCommand cmd = new OdbcCommand("TRUNCATE TABLE field_gr;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Hashtable rc_e_tb = Get_Item_To_fi_id(conn);
                    StreamReader sr = new StreamReader(ofd.FileName);
                    string line = null;
                    string file_name = "";
                    string s_item = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("FILE: ")) { file_name = line.Substring(6); s_item = file_name.Split('(')[0]; }
                        string[] str_a = line.Split(',');
                        if (str_a.Length > 5)
                        {
                            inc_field_gc0(str_a, s_item, int.Parse(rc_e_tb[file_name].ToString()), conn);
                        }
                    }
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 從預處理文檔,載入DB sport_rc
        /// </summary>
        public static void Load_sport_rc_From_txt_To_DB()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = Basic_HTB_Info.baseFilePath + @"\PreProcessSportRC.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (OdbcConnection conn = new OdbcConnection(Conn_Str))
                {
                    conn.Open();
                    if (MessageBox.Show("TRUNCATE TABLE sport_rc;","",MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (OdbcCommand cmd = new OdbcCommand("TRUNCATE TABLE sport_rc;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Hashtable rc_e_tb = Get_Item_To_si_id(conn);
                    StreamReader sr = new StreamReader(ofd.FileName);
                    string line = null;
                    string file_name = "";
                    string rc_title = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("FILE: ")) file_name = line.Substring(6);
                        if (line.Contains("TITLE: "))
                        {
                            rc_title = line.Substring(7);
                            upd_item(signflag_s[rc_title].ToString(), rc_title, int.Parse(rc_e_tb[file_name].ToString()), conn);
                        }
                        string[] str_a = line.Split('\t');
                        if (str_a.Length > 2)
                        {
                            if (str_a[1] == "男" || str_a[1] == "女")
                            {
                            }
                            else
                            {
                                inc_rc0(str_a, int.Parse(rc_e_tb[file_name].ToString()), conn);
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
        public static void Load_sport_rc_From_GR_txt_To_DB()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = Basic_HTB_Info.baseFilePath + @"\PreProcessSportGR.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (OdbcConnection conn = new OdbcConnection(Conn_Str))
                {
                    conn.Open();
                    if (MessageBox.Show("TRUNCATE TABLE sport_gr;", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (OdbcCommand cmd = new OdbcCommand("TRUNCATE TABLE sport_gr;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Hashtable rc_e_tb = Get_Item_To_si_id(conn);
                    StreamReader sr = new StreamReader(ofd.FileName);
                    string line = null;
                    string file_name = "";
                    string s_item="";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("FILE: ")){ file_name = line.Substring(6);s_item=file_name.Split('(')[0];}
                        string[] str_a = line.Split(',');
                        if (str_a.Length >5)
                        {
                            inc_gc0(str_a, s_item,int.Parse(rc_e_tb[file_name].ToString()), conn);
                        }
                    }
                    conn.Close();
                }
            }
        }
        private static void inc_gc0(string[] value_s,string s_item,int si_id,OdbcConnection conn)
        {
            string sql_head = "insert into sport_gr ({0}) values ({1});";
            string titile_s="si_id,s_item,gr_rc,name,classno,gr_period,gr_date";
            string qm_s = "?";
            string[] s_ar = titile_s.Split(',');
            for (int i = 1; i < s_ar.Length; i++) qm_s += ",?";
            string sql = string.Format(sql_head, titile_s, qm_s);
            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                cmd.Parameters.Add("@si_id", OdbcType.Int,0,"si_id");
                cmd.Parameters.Add("@s_item", OdbcType.VarChar, 64, "s_item");
                for (int i = 2; i < s_ar.Length; i++)
                    cmd.Parameters.Add("@" + s_ar[i], OdbcType.VarChar, 32,s_ar[i]);
                cmd.Parameters["@si_id"].Value = si_id;
                cmd.Parameters["@s_item"].Value = s_item;
                for (int i = 2; i < s_ar.Length; i++)
                    cmd.Parameters["@" + s_ar[i]].Value = value_s[i-1];
                cmd.ExecuteNonQuery();
            }
        }
        private static void inc_field_gc0(string[] value_s, string s_item, int si_id, OdbcConnection conn)
        {
            string sql_head = "insert into field_gr ({0}) values ({1});";
            string titile_s = "fi_id,f_item,gr_rc,name,classno,gr_period,gr_date";
            string qm_s = "?";
            string[] s_ar = titile_s.Split(',');
            for (int i = 1; i < s_ar.Length; i++) qm_s += ",?";
            string sql = string.Format(sql_head, titile_s, qm_s);
            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                cmd.Parameters.Add("@fi_id", OdbcType.Int, 0, "fi_id");
                cmd.Parameters.Add("@f_item", OdbcType.VarChar, 64, "f_item");
                for (int i = 2; i < s_ar.Length; i++)
                    cmd.Parameters.Add("@" + s_ar[i], OdbcType.VarChar, 32, s_ar[i]);
                cmd.Parameters["@fi_id"].Value = si_id;
                cmd.Parameters["@f_item"].Value = s_item;
                for (int i = 2; i < s_ar.Length; i++)
                    cmd.Parameters["@" + s_ar[i]].Value = value_s[i - 1];
                cmd.ExecuteNonQuery();
            }
        }


      

    }
}
