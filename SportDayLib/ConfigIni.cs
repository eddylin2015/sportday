using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace SportDayLib
{
    public class ConfigIni
    {
        public Form MDIparent = null;

        public string DB_HOST = null;

        public string output_result_html = null;
        public string temp_dir = null;

        public string report_to_public = null;

        public string blanktable_to_public = null;

        public Hashtable tempdata= new Hashtable();
        public ConfigIni()
        {
            StreamReader sr = new StreamReader(Basic_HTB_Info.baseFilePath + @"\Config.txt", Encoding.Default);
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s_ar = line.Split('=');
                if (s_ar[0] == "DB_HOST") DB_HOST = s_ar[1];
                if (s_ar[0] == "output_result_html") output_result_html = s_ar[1];
                if (s_ar[0] == "report_to_public") report_to_public = s_ar[1];
                if (s_ar[0] == "temp_dir") temp_dir = s_ar[1];
                if (s_ar[0] == "blanktable_to_public") blanktable_to_public = s_ar[1];
            }
            sr.Close();
            sr.Dispose();
            using (FileStream xmlfs = new FileStream(Basic_HTB_Info.baseFilePath + @"\tempdata.txt",FileMode.Open))
            {
                XmlTextReader xmlr = new XmlTextReader(xmlfs);
                String tempstr = null;
                while (xmlr.Read())
                {
                    if (xmlr.NodeType == XmlNodeType.Element)
                    {
                        if (xmlr.Name == "tempdata") continue;
                        tempstr = xmlr.Name;
                    }
                    else if (xmlr.NodeType == XmlNodeType.Text)
                    {
                        tempdata.Add(tempstr, xmlr.Value);
                    }
                }
                xmlfs.Close();
                xmlfs.Dispose();
            }
        }
        public String TempdataKey(String key)
        {
            if (!tempdata.ContainsKey(key)) { tempdata.Add(key, ""); return ""; } else return tempdata[key].ToString();
        }
        public void FlashTempData()
        {
            using (FileStream xmlfs = new FileStream(Basic_HTB_Info.baseFilePath + @"\tempdata.txt", FileMode.Create))
            {
                XmlTextWriter w = new XmlTextWriter(xmlfs, Encoding.UTF8);
                w.WriteStartDocument();
                w.WriteStartElement("tempdata");
                foreach (DictionaryEntry entry in tempdata)
                {
                    w.WriteElementString(entry.Key.ToString(),entry.Value.ToString());
                }
                w.WriteEndElement();
                w.WriteEndDocument();
                w.Flush();
                w.Close();
                xmlfs.Close();
                xmlfs.Dispose();
            }
        }
    }
}
