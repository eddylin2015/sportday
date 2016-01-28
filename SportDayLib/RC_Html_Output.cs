using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using System.IO;
using System.Collections;
namespace SportDayLib
{
    public interface iGR
    {
        string GR_STR(string si_id, OdbcConnection conn, string rcx);
    }
    public class RC_S_GR : iGR
    {
        public string GR_STR(string si_id, OdbcConnection conn, string rcx)
        {
            string gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR7;
            if (rcx == "RC0" || rcx == "RC2" || rcx == "RC4") { }
            else if (rcx == "RC1" || rcx == "RC3") { gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR9; }
            else if (rcx == "RCFIE") { gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR15; }
            else if (rcx == "RCFJH") gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR31;
            int gi = -1;
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select gi from sport_item where si_id='{0}'", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                {
                    
                    if(int.TryParse(dr[0].ToString(),out gi) && gi>0)
                    {
                    using (OdbcDataReader dr0 = new OdbcCommand(String.Format("select gr_rc, name,classno,gr_period,gr_date from gt_item where gi='{0}'", gi), conn).ExecuteReader())
                    {
                        if (dr0.Read())
                        {
                            return string.Format(gr_format, "GR", dr0[0], dr0[1], dr0[2], dr0[3], dr0[4]);
                        }
                    }
                    }
                }
            }
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select gr_rc, name,classno,gr_period,gr_date from sport_gr where si_id='{0}'", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                {
                    return string.Format(gr_format, "GR", dr[0], dr[1], dr[2], dr[3], dr[4]);
                }
            }
            return "";
        }
    }
    public class RC_F_GR : iGR
    {
        public string GR_STR(string si_id, OdbcConnection conn, string rcx)
        {
            string gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR7;
            if (rcx == "RC0" || rcx == "RC2" || rcx == "RC4") { }
            else if (rcx == "RC1" || rcx == "RC3") { gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR9; }

            else if (rcx == "RCFIE") { gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR15; }
            else if (rcx == "RCFJH") gr_format = RC_Html_EXCEL_TAG.EXCEL_GR_TR31;
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select gi  from field_item where fi_id='{0}' and not gi is null;", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                {
                    int gi=-1;
                    if (int.TryParse(dr[0].ToString(),out gi) && gi > 0)
                    {
                        using (OdbcDataReader dr0 = new OdbcCommand(String.Format("select gr_rc, name,classno,gr_period,gr_date  from gt_item where gi='{0}'", gi), conn).ExecuteReader())
                        {
                            if (dr0.Read())
                            {
                                return string.Format(gr_format, "GR", dr0[0], dr0[1], dr0[2], dr0[3], dr0[4]);
                            }
                        }
                    }
                }
            }

            using (OdbcDataReader dr = new OdbcCommand(String.Format("select gr_rc, name,classno,gr_period,gr_date  from field_gr where fi_id='{0}'", si_id), conn).ExecuteReader())
            {
                if (dr.Read())
                {
                    return string.Format(gr_format, "GR", dr[0], dr[1], dr[2], dr[3], dr[4]);
                }
            }
            return "";
        }
    }



    /// <summary>
    /// rc_html
    /// </summary>
    public class RC_Html_Basic
    {
        public string html_filename = null;
        protected OdbcConnection conn = null;
        protected string si_id = null;
        protected string s_item=null;
        protected string rcx = null;
        protected string fieldnames = null;
        protected string[] th_ar = null;
        protected string[] colspan_ar=null;
        public iGR aagr = null;
        public RC_Html_Basic(string si_id,string s_item,string rcx,OdbcConnection conn,string file_name,iGR pGR)
        {
            string col_span = null;
            string c_fieldnames = null;
            c_fieldnames = Basic_HTB_Info.Get_Signflag_s(rcx);
            fieldnames = Basic_HTB_Info.signflag_sql[rcx].ToString();
            aagr = pGR;
            Basic_HTB_Info.Adjust_FieldName(ref fieldnames, ref c_fieldnames, ref col_span);
            th_ar = c_fieldnames.Split(',');
            colspan_ar = col_span.Split(',');
            html_filename = file_name;
            this.conn = conn;
            this.s_item = s_item;
            this.rcx = rcx;
        }
        /// <summary>
        /// GR
        /// </summary>
        /// <param name="si_id"></param>
        /// <param name="conn"></param>
        /// <param name="rcx"></param>
        /// <returns></returns>
        public string GR_STR(string si_id,OdbcConnection conn,string rcx)
        {
            return aagr.GR_STR(si_id,conn,rcx);
          
        }
        /// <summary>
        /// Head 標題
        /// </summary>
        /// <returns></returns>
        public string TH_STR()
        {
            string r = "";
            for (int i = 0; i < th_ar.Length; i++) { r+=String.Format("<td colspan={0}>{1}", colspan_ar[i], th_ar[i]); }
            return r;
        }
    }
  
    /// <summary>
    /// 成績公告
    /// </summary>
    public class RC_Html_Print : RC_Html_Basic
    {
        public RC_Html_Print(string si_id, string s_item, string rcx, OdbcConnection conn,string file_name,iGR pGR)
            : base(si_id, s_item, rcx,conn,file_name,pGR)
        {
         //   html_filename = Basic_HTB_Info.baseFilePath + "\\temp\\" + String.Format("{0}_{1}.htm", si_id, s_item);
            StreamWriter sw = new StreamWriter(html_filename,false,Encoding.GetEncoding(950));
            if (file_name.ToLower().Contains(".htm"))
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.html_head);
            }
            else
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            }
            sw.WriteLine("<B><center>成績公告<br>{0}</center></b>",s_item);
            sw.WriteLine("<table border=1 width=100%><tr>{0}",TH_STR());
            
            string gr_out_str=GR_STR(si_id, conn,rcx);
            int group_cnt = 0;
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select rc_id,{0}  from sport_rc where si_id='{1}' order by group_id,road;", fieldnames, si_id), conn).ExecuteReader())
            {
                string group_str = null;
                bool group_flag=false;
                
                for(int i=0;i<dr.FieldCount;i++)   if(dr.GetName(i)=="group_id"){group_flag=true;break;}
                while (dr.Read())
                {
                    if (group_flag)
                    {
                        if (group_str == null) group_str = dr["group_id"].ToString(); if (group_str != dr["group_id"].ToString())
                        {
                            sw.WriteLine("</table><br>");
                            group_str = dr["group_id"].ToString();
                            group_cnt++;
                            if (group_cnt % 2 == 0)
                            {
                                sw.WriteLine(gr_out_str);
                                sw.WriteLine("<br class=break>");
                                sw.WriteLine("<B><center>成績公告<br>{0}</center></b>", s_item);
                            }
                            sw.WriteLine("<table border=1 width=100%><tr>{0}",TH_STR());
                        }
                    }
                    sw.Write( "<tr>");
                    for(int i=3; i<dr.FieldCount;i++)
                        sw.Write("<td colspan={0}>{1}&nbsp;",colspan_ar[i-3], dr[i]);
                }
            }
            sw.WriteLine("</table><br><br>");
            sw.WriteLine(gr_out_str);
            sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_END);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
    /// <summary>
    /// 進級結果并輸出進級名單porm_list
    /// </summary>
    class RC_Prom_Html_Output:RC_Html_Basic
    {
        /// <summary>
        /// 組別中賽道
        /// </summary>
        public int group_p_cnt = 8;
        /// <summary>
        /// 進級結果并輸出進級名單porm_list
        /// </summary>
        /// <param name="si_id"></param>
        /// <param name="s_item"></param>
        /// <param name="rcx"></param>
        /// <param name="p_si_id"></param>
        /// <param name="p_s_item"></param>
        /// <param name="p_rcx"></param>
        /// <param name="conn"></param>
        /// <param name="porm_list">進級名單porm_list</param>
        /// <param name="file_name"></param>
        /// <param name="pGR"></param>
        public RC_Prom_Html_Output(string si_id,string s_item,string rcx,string p_si_id,string p_s_item,string p_rcx,OdbcConnection conn,out List<string> porm_list,string file_name,iGR pGR):base(si_id,s_item,rcx,conn,file_name,pGR)
        {
            
            int p_cnt = 0;
            if (p_s_item.Contains("(初賽)")) p_cnt = group_p_cnt * 4;
            if (p_s_item.Contains("(複賽)")) p_cnt = group_p_cnt * 2;
            if (p_s_item.Contains("(決賽)")) p_cnt = group_p_cnt;
            MPPFORM.InputStrBox inputtxt = new MPPFORM.InputStrBox("進級人數:",p_cnt.ToString());
            if (inputtxt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                int out_int=p_cnt;
                if (int.TryParse(inputtxt.textbox_str, out out_int))
                {
                    p_cnt = out_int;
                    System.Windows.Forms.MessageBox.Show("人數修改為:" + p_cnt);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("數字格式錯誤:"+inputtxt.textbox_str+" 使用默認值:"+p_cnt);
                }
            }
            StreamWriter sw = new StreamWriter(html_filename, false, Encoding.Default);
            if (file_name.ToLower().Contains(".htm"))
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.html_head);
            }
            else
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            }
            sw.WriteLine("<B><center>成績公告<br>{0}</center></b>", s_item);
            sw.WriteLine("<table border=1 width=100%><tr>{0}",TH_STR());
            string[] f_ar = fieldnames.Split(',');
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select rc_id,{0}  from sport_rc where si_id='{1}' order by rc;", fieldnames, si_id), conn).ExecuteReader())
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("rc_key"); dt.Columns.Add("rc_id");
                foreach (string s in f_ar) dt.Columns.Add(s);
                while (dr.Read())
                {
                     DataRow d_row=dt.NewRow();
                    string key = null;
                    if (dr["rc"].ToString().Trim() == "" ) { key = "NULL"; }
                    else if (dr["note"].ToString().Contains("GR") || dr["note"].ToString() == "")
                    {
                        key = String.Format("{0}-{1}", dr["rc"], dr["rc_id"]);
                    }
                    else
                    {
                        key = String.Format("{0}-{1}-{2}", dr["note"], dr["rc"], dr["rc_id"]);
                    }
                    d_row["rc_key"] = key;
                    for (int i = 0; i < dr.FieldCount; i++)
                        d_row[dr.GetName(i)] = dr[i];
                    d_row["grk"] = d_row["rank"];
                    dt.Rows.Add(d_row);
                }
                DataView dv    =dt.DefaultView;
                dv.Sort = "rc_key";
                int drv_cnt = 0;
                porm_list = new List<string>();

                string[] porm_road_s=null;
                if(group_p_cnt==7)
                {
                    porm_road_s= new String[]{ ",4;,5;,3;,6;,2;,7;,8", "二,4;一,4;一,5;二,5;二,3;一,3;一,6;二,6;二,2;一,2;二,7;一,7;一,8;二,8" };
                }else if(group_p_cnt==6)
                {
                    porm_road_s = new String[] { ",4;,5;,3;,6;,2;,7;,8", "二,4;一,4;一,5;二,5;二,3;一,3;一,6;二,6;二,2;一,2;二,7;一,7;一,8;二,8" };
                }
                else
                {
                    porm_road_s =new String[]{",4;,5;,3;,6;,2;,1;,7;,8","二,4;一,4;一,5;二,5;二,3;一,3;一,6;二,6;二,2;一,2;一,1;二,1;二,7;一,7;一,8;二,8",
                        "三,4;二,4;一,4;一,5;二,5;三,5;三,3;二,3;一,3;一,6;二,6;三,6;三,2;二,2;一,2;一,1;二,1;三,1;一,7;二,7;三,7;一,8;二,8;三,8",
            "四,4;三,4;二,4;一,4;一,5;二,5;三,5;四,5;四,3;三,3;二,3;一,3;一,6;二,6;三,6;四,6;四,2;三,2;二,2;一,2;一,1;二,1;三,1;四,1;四,7;三,7;二,7;一,7;一,8;二,8;三,8;四,8"

                        };
                }
                int temp_ = p_cnt / group_p_cnt - 1;
                if (temp_ < 0) temp_ = 0;
                string[] porm_road=porm_road_s[temp_].Split(';');
                foreach (DataRowView drv in dv)
                {
                    drv_cnt++;
                    sw.WriteLine("<tr>");
                    if (drv["rank"].ToString().Trim() != "") { sw.Write("<td>{0}&nbsp;", drv_cnt); } else { sw.Write("<td>&nbsp;"); }
                    for (int j = 5; j < dv.Table.Columns.Count; j++)
                    {
                        sw.Write("<td colspan={0}>{1}&nbsp;", colspan_ar[j-4],drv[j]);
                    }
                    if (drv_cnt <= p_cnt)
                    {
                        sw.Write("Q");
                        porm_list.Add(string.Format("{0},{1},{2}", p_si_id, porm_road[(drv_cnt - 1)],drv["rc_id"]));
                    }
                }
            }
            sw.WriteLine("</table><br><br>");
            sw.WriteLine(GR_STR(si_id,conn,rcx));
            sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_END);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
    public class RC_Html_Output_BlankTable : RC_Html_Basic
    {
        public RC_Html_Output_BlankTable(String si_id,String s_item,String rcx,OdbcConnection conn,String file_name,iGR pGR)
            : base(si_id,s_item,rcx,conn,file_name,pGR)
        {
            StreamWriter sw = new StreamWriter(html_filename+"blank", false, Encoding.Default);
            //sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            /*if (file_name.ToLower().Contains(".htm"))
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.html_head);
            }
            else
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            }*/
            sw.WriteLine("<B><center>成績公告<br>{0}</center></b>", s_item);
            sw.WriteLine("<table border=1 width=100%><tr>{0}", TH_STR());
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select rc_id,{0}  from sport_rc where si_id='{1}' order by rc;", fieldnames, si_id), conn).ExecuteReader())
            {
                SortedList slist = new SortedList();
                while (dr.Read())
                {
                    string key = null;
                    if (dr["note"].ToString().Contains("GR"))
                    {
                        key = String.Format("-{0}-{1}", dr["rc"], dr["rc_id"]);
                    }
                    else
                    {
                        key = String.Format("{0}-{1}-{2}", dr["note"], dr["rc"], dr["rc_id"]);
                    }
                    string val = "<tr>";
                    for (int i = 3; i < dr.FieldCount; i++)
                    {
                        val += string.Format("<td colspan={0}>{1}&nbsp;", colspan_ar[i - 3], dr[i]);
                    }
                    slist.Add(key, val);
                }
                for (int i = 0; i < slist.Count; i++)
                {
                    sw.WriteLine("{0}", slist.GetByIndex(i));
                }
            }
            sw.WriteLine("</table><br><br>");
            sw.WriteLine(GR_STR(si_id, conn, rcx));
            //sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_END);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

    }
    /// <summary>
    /// 賽果
    /// </summary>
    public class RC_Html_Output : RC_Html_Basic
    {
        public RC_Html_Output(string si_id,string s_item,string rcx,OdbcConnection conn,string file_name,iGR pGR):base(si_id,s_item,rcx,conn,file_name,pGR)
        {
            StreamWriter sw = new StreamWriter(html_filename,false,Encoding.Default);

            if (file_name.ToLower().Contains(".htm")) {
                sw.WriteLine(RC_Html_EXCEL_TAG.html_head);
            }
            else
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            }
            sw.WriteLine("<B><center>成績公告<br>{0}</center></b>",s_item);
            sw.WriteLine("<table border=1 width=100%><tr>{0}",TH_STR());
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select rc_id,{0}  from sport_rc where si_id='{1}' order by rc;", fieldnames,si_id), conn).ExecuteReader())
            {
                
                SortedList slist = new SortedList();
                while (dr.Read())
                {
                    string key = null;
                    if (dr["note"].ToString().Contains("GR")) 
                    { 
                        key = String.Format("-{0}-{1}",dr["rc"] , dr["rc_id"]);
                    }
                    else if (dr["note"].ToString().Trim() == "")
                    {
                        key = String.Format("-{0}-{1}", dr["rc"], dr["rc_id"]);
                    }
                    else if (dr["rc"].ToString().Trim() == "")
                    {
                        key = String.Format("ZZZZZZZZZ-{0}-{1}", dr["rc"], dr["rc_id"]);
                    }
                    else
                    {
                        key = String.Format("{0}-{1}-{2}", dr["note"].ToString().TrimStart(), dr["rc"], dr["rc_id"]);
                    }
                    string val = "<tr>";
                    for (int i = 3; i < dr.FieldCount; i++)
                    {
                        val += string.Format("<td colspan={0}>{1}&nbsp;", colspan_ar[i - 3], dr[i]);
                    }
                    slist.Add(key, val);
                }
                for (int i = 0; i < slist.Count; i++)
                {
                    sw.WriteLine("{0}",slist.GetByIndex(i));
                }
            }
            sw.WriteLine("</table><br><hr>");
            sw.WriteLine(GR_STR(si_id,conn,rcx));
            sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_END);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

    }
      class  RC_Html_EXCEL_TAG{

          public static string html_head = @"<!DOCTYPE html>
<html><head>
<style>
table,th,td
{
border-collapse:collapse;
border:1px solid black;
font-size:100%
}
</style>
</head>
<body>";
       public static string EXCEL_HEAD = @"
<html xmlns:o=urn:schemas-microsoft-com:office:office 
xmlns:x=urn:schemas-microsoft-com:office:excel
xmlns=http://www.w3.org/TR/REC-html40>
<head>
<meta http-equiv=Content-Type content='text/html; charset=big5'>
<meta name=ProgId content=Excel.Sheet>
<meta name=Generator content='Microsoft Excel 11'>
<style>
.break { page-break-after: always; }
<!--table
@page
	{margin:1.0in .75in 1.0in .75in;
	mso-header-margin:.5in;
	mso-footer-margin:.5in;}
.font0
	{color:windowtext;
	font-size:12.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:新細明體, serif;
	mso-font-charset:136;}
.font6
	{color:windowtext;
	font-size:9.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:新細明體, serif;
	mso-font-charset:136;}
tr
	{mso-height-source:auto;
	mso-ruby-visibility:none;}
col
	{mso-width-source:auto;
	mso-ruby-visibility:none;}
br
	{mso-data-placement:same-cell;}
.style0
	{mso-number-format:General;
	text-align:general;
	vertical-align:bottom;
	white-space:nowrap;
	mso-rotate:0;
	mso-background-source:auto;
	mso-pattern:auto;
	color:windowtext;
	font-size:10.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:Helv, sans-serif;
	mso-font-charset:0;
	border:none;
	mso-protection:locked visible;
	mso-style-name:"";}
.style0
	{mso-number-format:General;
	text-align:general;
	vertical-align:bottom;
	white-space:nowrap;
	mso-rotate:0;
	mso-background-source:auto;
	mso-pattern:auto;
	color:windowtext;
	font-size:10.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:Helv, sans-serif;
	mso-font-charset:0;
	border:none;
	mso-protection:locked visible;
	mso-style-name:一般;
	mso-style-id:0;}
td
	{mso-style-parent:style0;
	padding-top:1px;
	padding-right:1px;
	padding-left:1px;
	mso-ignore:padding;
	color:windowtext;
	font-size:12.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:新細明體, serif;
	mso-font-charset:136;
	mso-number-format:General;
	text-align:general;
	vertical-align:middle;
	border:none;
	mso-background-source:auto;
	mso-pattern:auto;
	mso-protection:locked visible;
	white-space:nowrap;
	mso-rotate:0;}
.xl25
	{mso-style-parent:style0;
	text-align:center;
	border:.5pt solid windowtext;}
.xl26
	{mso-style-parent:style0;
	font-size:14.0pt;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;}
.xl27
	{mso-style-parent:style0;
	color:black;
	font-size:14.0pt;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;}
.xl28
	{mso-style-parent:style0;
	color:black;
	font-size:14.0pt;
	text-align:center;
	border:.5pt solid windowtext;}
.xl29
	{mso-style-parent:style0;
	font-size:14.0pt;}
.xl30
	{mso-style-parent:style0;
	font-size:14.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl31
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl32
	{mso-style-parent:style0;
	text-align:center;
	border:.5pt solid windowtext;
	background:gray;
	mso-pattern:auto none;}
.xl33
	{mso-style-parent:style0;
	color:black;
	font-size:14.0pt;
	text-align:center;
	border:.5pt solid windowtext;
	background:gray;
	mso-pattern:auto none;}
.xl33r
	{mso-style-parent:style0;
	text-align:right;
	border:.5pt solid black;
	white-space:normal;}
.xl34
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;
	background:gray;
	mso-pattern:auto none;}
.xl35
	{mso-style-parent:style0;
	font-size:14.0pt;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;
	background:white;
	mso-pattern:auto none;}
.xl36
	{mso-style-parent:style0;
	font-size:14.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;
	background:white;
	mso-pattern:auto none;}
.xl37
	{mso-style-parent:style0;
	background:white;
	mso-pattern:auto none;}
.xl38
	{mso-style-parent:style0;
	font-size:14.0pt;
	background:white;
	mso-pattern:auto none;}
.xl40
	{mso-style-parent:style0;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl41
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl42
	{mso-style-parent:style0;
	font-weight:700;
	background:white;
	mso-pattern:auto none;}
.xl43
	{mso-style-parent:style0;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:.5pt solid windowtext;
	background:white;
	mso-pattern:auto none;}
.xl44
	{mso-style-parent:style0;
	font-size:14.0pt;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl45
	{mso-style-parent:style0;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl46
	{mso-style-parent:style0;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl47
	{mso-style-parent:style0;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;
	background:white;
	mso-pattern:auto none;}
.xl48
	{mso-style-parent:style0;
	font-size:14.0pt;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl49
	{mso-style-parent:style0;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl50
	{mso-style-parent:style0;
	text-align:right;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl51
	{mso-style-parent:style0;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
.xl52
	{mso-style-parent:style0;
	font-family:細明體, monospace;
	mso-font-charset:136;
	background:white;
	mso-pattern:auto none;}
.xl53
	{mso-style-parent:style0;
	font-family:細明體, monospace;
	mso-font-charset:136;
	text-align:center;
	background:white;
	mso-pattern:auto none;}
.xl54
	{mso-style-parent:style0;
	font-size:18.0pt;
	font-weight:700;
	text-align:center;
	background:white;
	mso-pattern:auto none;}
.xl55
	{mso-style-parent:style0;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:auto none;}
ruby
	{ruby-align:left;}
rt
	{color:windowtext;
	font-size:9.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:細明體, monospace;
	mso-font-charset:136;
	mso-char-type:none;
	display:none;}
-->
</style>
</head>
<body>"
;
      public  static string EXCEL_END = @"
</body>
</html>
";
      public static string EXCEL_GR_TR31 = @"
<table border=1 width=100%>
<tr><td colspan=2>紀錄<td colspan=2>成績<td colspan=2>姓名<td colspan=7>班別<td colspan=7>屆<td colspan=11>日期
<tr><td colspan=2>{0} <td colspan=2>{1} <td colspan=2>{2}<td colspan=7>{3}<td colspan=7>{4}<td colspan=11>{5}
</table>  
<b>註釋</B>
<table border=1 width=100%>
<tr><td colspan=4>GR  大會紀錄<td colspan=9>TGR  平大會紀錄<td colspan=18>NGR  破大會紀錄
<tr><td colspan=4>DNS  棄權<td colspan=9>DR  請假<td colspan=18>
</table> 
<table border=0 width=100%>
<tr><td colspan=6>徑賽裁判長:<td colspan=14>編排記錄主裁判:<td colspan=11>記錄員:
</table>
";
      public static string EXCEL_GR_TR15 = @"
<table border=1 width=100%>
<tr><td colspan=2>紀錄<td  colspan=2>成績<td colspan=2>姓名<td colspan=3>班別<td  colspan=3>屆<td colspan=3>日期
<tr><td colspan=2>{0} <td colspan=2>{1} <td colspan=2>{2}<td colspan=3>{3}<td colspan=3>{4}<td colspan=3>{5}
</table>  
<b>註釋</B>
<table border=1 width=100%>
<tr><td colspan=4>B3  前三次最好<td colspan=4>B5  前五次最好<td colspan=7>
<tr><td colspan=4>GR  大會紀錄<td colspan=4>TGR  平大會紀錄<td colspan=7>NGR  破大會紀錄
<tr><td colspan=2>DNS  棄權<td colspan=4>NM  無有效成績<td colspan=7>DR  請假<td colspan=1>
</table> 
<table border=0 width=100%>
<tr><td colspan=5>徑賽裁判長:<td colspan=5>編排記錄主裁判:<td colspan=5>記錄員:
</table>
";
      public  static string EXCEL_GR_TR9 = @"
<table border=1 width=100%>
<tr><td>紀錄<td>成績<td colspan=3>姓名<td colspan=2>班別<td>屆<td colspan=1>日期
<tr><td>{0} <td>{1} <td colspan=3>{2}<td colspan=2>{3}<td>{4}<td colspan=1>{5}
</table>  
<b>註釋</B>
<table border=1 width=100%>
<tr><td colspan=2>GR  大會紀錄<td colspan=2>TGR  平大會紀錄<td colspan=2>NGR  破大會紀錄<td colspan=2>GRK 組內排名<td colspan=1>Q  晉級
<tr><td colspan=2>DQ  犯規<td colspan=2>DNF  中退<td colspan=2>DNS  棄權<td colspan=2>DR  請假<td colspan=1>
</table> 
<table border=0 width=100%>
<tr><td colspan=3>徑賽裁判長:<td colspan=4>編排記錄主裁判:<td colspan=2>記錄員:
</table>
";
      public static string EXCEL_GR_TR7 = @"
<table border=1 width=100%>
<tr><td>紀錄<td>成績<td colspan=2>姓名<td colspan=1>班別<td>屆<td colspan=1>日期
<tr><td>{0} <td>{1} <td colspan=2>{2}<td colspan=1>{3}<td>{4}<td colspan=1>{5}
</table>  
<b>註釋</B>
<table border=1 width=100%>
<tr><td colspan=2>GR 大會紀錄<td colspan=2>TGR 平大會紀錄<td colspan=1>NGR 破大會紀錄<td colspan=1>GRK 組內排名<td colspan=1>Q  晉級
<tr><td colspan=2>DQ 犯規<td colspan=2>DNF  中退<td colspan=2>DNS  棄權<td colspan=1>DR  請假
</table> 
<table border=0 width=100%>
<tr><td colspan=3>徑賽裁判長:<td colspan=2>編排記錄主裁判:<td colspan=2>記錄員:
</table>
";  
    }

}
