using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Windows.Forms;


namespace SportDayLib
{
    public class Basic_HTB_Info
    {
        /// <summary>
        /// 系統目錄
        /// </summary>
        public static string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        /// <summary>
        /// 文件目錄
        /// </summary>
        public static string baseFilePath
        {
            get
            {
                return appPath.Substring(6);
            }
        }
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
            get
            {
                if (instance == null) instance = new Basic_HTB_Info();
                return instance;
            }
        }
        /// <summary>
        /// 從檔案載入運動項目字典
        /// </summary>
        public void Load_Race_Event_ITEMS_FROM_FILE()
        {
            FileInfo finfo = new FileInfo(Basic_HTB_Info.baseFilePath + @"\RACE_EVENT_ITEMS.txt");
            if (!finfo.Exists) return;
            Race_Events.Clear();
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\RACE_EVENT_ITEMS.txt", Encoding.Default);

            string input = null;
            while ((input = sr.ReadLine()) != null)
            {
                string[] strA = input.Split(':');
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
        public void Import_Race_Event_XLS_FileName()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            char[] splitchr = { '(', ')' };
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
                        string[] strA = finfo.Name.Split(splitchr);
                        if (strA.Length > 2)
                        {
                            string ts = "";
                            foreach (string tmps in strA)
                            {
                                ts += tmps + ":";
                            }
                            int i = 1;
                            int re_id = 0;
                            if (!int.TryParse(strA[i], out re_id)) ;// continue;
                            string re_name = strA[i + 1];
                            string re_type = strA[i + 2];
                            ls.Add(string.Format("{0}:{1}({2})", re_id, re_name, re_type));
                        }
                    }
                }
                MPPFORM.ListBoxForm lbf = new MPPFORM.ListBoxForm();
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

    }

}
