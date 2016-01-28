using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;

namespace SportDayLib
{   
    /// <summary>
    /// 運動會核心運算
    /// </summary>
    public class Lib
    {
        public static System.Drawing.Font sysfont =null;
        public static void SetFont()
        {
            FontDialog fd = new FontDialog();
            if(sysfont!=null)fd.Font = sysfont;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                sysfont = fd.Font;
            }
        }

        public static void Load_Sport_GR_TB_TXT_TO_DB(string table_name,string field_names)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
                {
                    conn.Open();
                    MessageBox.Show(String.Format("TRUNCATE TABLE {0};",table_name));
                    using (OdbcCommand cmd = new OdbcCommand(String.Format("TRUNCATE TABLE {0};",table_name), conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    StreamReader sr = new StreamReader(ofd.FileName);
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {

                        string[] str_a = line.Split(',');
                        if (str_a.Length > 5)
                        {
                            Lib.inc_cmd(field_names, table_name, str_a, conn);
                        }
                    }
                    conn.Close();
                }
            }
        }
        public static void Load_Sport_TXT_TO_DB_ByTableNameAndFieldName(string table_name, string field_names)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (OdbcConnection conn = new OdbcConnection(Basic_HTB_Info.Conn_Str))
                {
                    conn.Open();
                    //MessageBox.Show(String.Format("TRUNCATE TABLE {0};", table_name));
                    //using (OdbcCommand cmd = new OdbcCommand(String.Format("TRUNCATE TABLE {0};", table_name), conn))
                    //{
                     //   cmd.ExecuteNonQuery();
                    //}
                    StreamReader sr = new StreamReader(ofd.FileName,Encoding.Default);
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] str_a = line.Split(';');
                        String[] fieldnames = field_names.Split(',');
                        if (str_a.Length == fieldnames.Length)
                        {
                            Lib.inc_cmd(field_names, table_name, str_a, conn);
                        }
                    }
                    conn.Close();
                }
            }
        }
        public static void inc_cmd(string title_s,string table_name, string[] value_s, OdbcConnection conn)
        {
            string sql_head = "insert into "+table_name+" ({0}) values ({1});";
            string qm_s = "?";
            string[] s_ar = title_s.Split(',');
            for (int i = 1; i < s_ar.Length; i++) qm_s += ",?";
            string sql = string.Format(sql_head, title_s, qm_s);
            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                for (int i = 0; i < s_ar.Length; i++)
                    cmd.Parameters.Add("@" + s_ar[i], OdbcType.VarChar, 32, s_ar[i]);
                for (int i = 0; i < s_ar.Length; i++)
                    cmd.Parameters["@" + s_ar[i]].Value = value_s[i];
                cmd.ExecuteNonQuery();
            }
        }
        public static void convertXlsTxt(string s_file, string d_file)
        {
            ProcessStartInfo pi = new ProcessStartInfo("wscript.exe");
            pi.UseShellExecute = true;
            //   pi.RedirectStandardOutput = true;
            pi.Arguments = String.Format(Basic_HTB_Info.baseFilePath + @"\convertTxt.vbs" + String.Format(" {0} {1}", s_file, d_file));
            //  Process p = Process.Start(pi);
            Process p = Process.Start(pi);
            p.WaitForExit();
        }
        public static void convertXlsPdf(String s_file, String d_file)
        {
            ProcessStartInfo pi = new ProcessStartInfo("wscript.exe");
            pi.UseShellExecute = true;
            //   pi.RedirectStandardOutput = true;
            if (s_file.Contains("跳高"))
            {
                pi.Arguments = String.Format(Basic_HTB_Info.baseFilePath + @"\convertPDF70.vbs" + String.Format(" {0} {1}", s_file, d_file));

            }
            else
            {
                pi.Arguments = String.Format(Basic_HTB_Info.baseFilePath + @"\convertPDF.vbs" + String.Format(" {0} {1}", s_file, d_file));
            }
            //  Process p = Process.Start(pi);
            Process p = Process.Start(pi);
            p.WaitForExit();
        }
    }

}
