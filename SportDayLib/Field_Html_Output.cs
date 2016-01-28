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

    /// <summary>
    /// 田賽
    /// </summary>
    public class FIELD_Html_Basic
    {
        public string html_filename = null;
        protected OdbcConnection conn = null;
        protected string si_id = null;
        protected string s_item = null;
        protected string rcx = null;
        protected string fieldnames = null;
        protected string[] th_ar = null;
        protected string[] colspan_ar = null;
        protected iGR aagr = null;
        public FIELD_Html_Basic(string si_id, string s_item, string rcx, OdbcConnection conn, string file_name,iGR pGR,bool blankformat)
        {
            this.conn = conn;
            this.s_item = s_item;
            this.si_id = si_id;
            aagr = pGR;
            if (rcx != null)
            {
                this.rcx = rcx;
            }
            else
            {
                if (s_item.Contains("跳高"))
                {
                    this.rcx = "RCFJH";
                }
                else
                {
                    this.rcx="RCFIE";
                }
            }
            
            string col_span = null;
            string c_fieldnames = null;
            if (rcx == "RCFJH")
            {
                fieldnames = "frc_id,rcx,rank,s_number,number,classno,name,h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11,h12,h13,h14,h15,h16,rc,note";
                c_fieldnames = "名次,次序,比賽號,姓名,班級,";
                c_fieldnames += "h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11,h12,h13,h14,h15,h16,";
                c_fieldnames += "成績,備註";

            }
            else if (rcx == "RCFIE")
            {
                fieldnames = "frc_id,rcx,rank,s_number,number,classno,name,h1,h2,h3,b3,h4,h5,b5,h6,rc,note";
                c_fieldnames = "名\n次,次\n序,比賽\n號,姓名,班級,一,二,三,B3,四,五,B5,六,成績,備註";
            }
            else
            {
                throw new Exception("RCFJH,RCFIE");
            }
            Basic_HTB_Info.Adjust_FieldName(ref fieldnames, ref c_fieldnames, ref col_span);
            th_ar = c_fieldnames.Split(',');
            colspan_ar = col_span.Split(',');
            html_filename = file_name;

        }
        /// <summary>
        /// GR
        /// </summary>
        /// <param name="si_id"></param>
        /// <param name="conn"></param>
        /// <param name="rcx"></param>
        /// <returns></returns>
        public string GR_STR(string si_id, OdbcConnection conn, string rcx)
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
            /*
            for (int i = 0; i < th_ar.Length; i++) { r += String.Format("<td colspan={0}>{1}", colspan_ar[i], th_ar[i]); }
            */

            if (rcx == "RCFJH")
            {
                r = "<tr><td rowspan=2>名次<td rowspan=2>次序<td rowspan=2>比賽號<td rowspan=2>姓名<td rowspan=2>班級";
                string hxsql = string.Format("select h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11,h12,h13,h14,h15,h16 from field_item where fi_id={0} ;",  si_id);
                using (OdbcDataReader hxdr = new OdbcCommand(hxsql, conn).ExecuteReader())
                {
                    if (hxdr.Read())
                    {
                        for(int i=0;i<hxdr.FieldCount;i++)
                        {
                            
                            r += String.Format("<td colspan=3>{0}&nbsp;", hxdr[i]);
                            if (i == 7)
                            {
                                r += "<td rowspan=2>成績<td rowspan=2>備註<tr  bgcolor=Silver>";
                            }
                        }
                    }
                }
                
            }
            else if (rcx == "RCFIE")
            {

                r = "<tr align=center><td rowspan=2>名次<td rowspan=2>次序<td rowspan=2>比賽號<td rowspan=2>姓名<td rowspan=2>班級<td colspan=3>前三次成績     <td rowspan=2>B3<td colspan=2>後二次成績<td rowspan=2>B5<td>&nbsp;<td rowspan=2>成績<td rowspan=2>備註";
                r += "<tr align=center>                                         <td>一<td>二<td>三       <td>四<td>五        <td>六";
            }
            return r;
        }
    }
    public class FieldJH_Html_Output : Field_Html_Output
    {
        public FieldJH_Html_Output(string si_id, string s_item, string rcx, OdbcConnection conn, string file_name,iGR pGR,bool blankformat)
            : base(si_id, s_item, rcx, conn, file_name,pGR,blankformat) { }
        public override void main_content(string si_id, string s_item, string rcx, string gr_out_str, OdbcConnection conn, StreamWriter sw)
        {
           // base.main_content(si_id, s_item, rcx, gr_out_str, conn, sw);
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select frc_id,{0}  from field_rc where fi_id='{1}' order by BIT_LENGTH( rank),rank;", fieldnames, si_id), conn).ExecuteReader())
            {
                SortedList slist = new SortedList();
                int cnt = 0;
                while (dr.Read())
                {
                    decimal temp_d = GETKEY(dr["rc"].ToString(),dr["note"].ToString(),cnt);
                    cnt++;
                    string val = "<tr>";
                    for (int i = 3; i < 8; i++)
                    {
                        if(dr.IsDBNull(i)||dr.GetString(i).Trim().Equals("")){
                               val += String.Format("<td align=\"right\" rowspan=2 colspan={0}>{1}&nbsp;", colspan_ar[i - 3], dr[i]);
                        }else{
                            val += String.Format("<td align=\"right\" rowspan=2 colspan={0}>{1}", colspan_ar[i - 3], dr[i]);
                        }
                        
                    }
                    String Silverf = "White";
                    for (int i = 8; i < dr.FieldCount - 2; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (j < dr[i].ToString().Length)
                            {
                                val += String.Format("<td align=\"right\">{0}", dr[i].ToString().ToUpper()[j]);
                            }
                            else
                            {
                                val += String.Format("<td><font color={0}>.</font>",Silverf);
                            }
                        }
                        if (i == 8 + 7)
                        {
                            val += String.Format("<td align=right rowspan=2>{0}&nbsp;<td rowspan=2>{1}&nbsp;<tr bgcolor=Silver>", dr["rc"], dr["note"]);
                            Silverf = "Silver";
                        }
                    }
                    slist.Add(temp_d, val);
                }
                for (int i = 0; i < slist.Count; i++)
                {
                    sw.WriteLine("{0}", slist.GetByIndex(i));
                }
            }

        }
    }
    /// <summary>
    /// 賽果
    /// </summary>
    public class Field_Html_Output : FIELD_Html_Basic
    {
        protected Decimal GETKEY(String rc,String note,int cnt)
        {
            decimal temp_d;
            if (rc.ToUpper() == "X"||rc=="")
            {
            }
            else if (note == "DNS" || note == "DR" || note == "NM" || note == "DQ" || note == "NDF")
            {
            }
            else if (decimal.TryParse(rc, out temp_d))
            {
                return (100 - temp_d) * 10000 + cnt;
            }
            return 999900+cnt;
        }
        public virtual void main_content(string si_id, string s_item, string rcx, string gr_out_str, OdbcConnection conn, StreamWriter sw)
        {
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select frc_id,{0}  from field_rc where fi_id='{1}' order by BIT_LENGTH(rank) , rank ;", fieldnames, si_id), conn).ExecuteReader())
            {
                SortedList slist = new SortedList();
                int cnt = 0;
                while (dr.Read())
                {
                    decimal temp_d=GETKEY(dr["rc"].ToString(),dr["note"].ToString(),cnt);
                    cnt++;
                    string val = "<tr>";
                    for (int i = 3; i < dr.FieldCount; i++)
                    {
                        val += string.Format("<td align=\"right\" colspan={0}>{1}</td>", colspan_ar[i - 3], dr[i]);
                    }

                    slist.Add(temp_d, val);
                }
                for (int i = 0; i < slist.Count; i++)
                {
                    sw.WriteLine("{0}", slist.GetByIndex(i));
                }
            }
        }
        public Field_Html_Output(string si_id, string s_item, string rcx, OdbcConnection conn, string file_name,iGR pGR,bool blankformat)
            : base(si_id, s_item, rcx, conn, file_name,pGR,blankformat)
        {
            StreamWriter sw = new StreamWriter(html_filename, false, Encoding.Default);
            if (file_name.ToLower().Contains(".htm"))
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.html_head);
            }
            else
            {
                sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            }
            //sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            sw.WriteLine("<B><center>成績公告<br>{0}</center></b>", s_item);
            sw.WriteLine("<table border=1 width=100%>{0}", TH_STR());
            string gr_out_str = GR_STR(si_id, conn, rcx);
            main_content(si_id, s_item, rcx, gr_out_str, conn, sw);
            sw.WriteLine("</table><br><hr>");
            sw.WriteLine(GR_STR(si_id, conn, rcx));
            sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_END);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

    }
    public class FieldJH_Html_Print : Field_Html_Print
    {
        public FieldJH_Html_Print(string si_id, string s_item, string rcx, OdbcConnection conn, string file_name,iGR pgr,bool blankformat)
            :  base(si_id, s_item, rcx, conn, file_name,pgr,blankformat)
        {

        }
        public override void main_content(string si_id, string s_item, string rcx, string gr_out_str, OdbcConnection conn, StreamWriter sw)
        {
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select frc_id,{0}  from field_rc where fi_id='{1}' order by BIT_LENGTH( s_number),s_number;", fieldnames, si_id), conn).ExecuteReader())
            {
                while (dr.Read())
                {
                    sw.Write("<tr>");
                    for (int i = 3; i < 8; i++)
                    {
                        sw.Write("<td class=x133r align=\"right\" rowspan=2 colspan={0}>{1}&nbsp;", colspan_ar[i - 3], dr[i]);
                    }
                    for (int i = 8; i < dr.FieldCount-2; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (j < dr[i].ToString().Length)
                            {
                                sw.Write("<td class=x133r align=\"right\">{0}",  dr[i].ToString().ToUpper()[j]);
                            }
                            else
                            {
                                sw.Write("<td class=x133r align=\"right\" >&nbsp;");
                            }
                        }
                        if (i == 8 + 7)
                        {
                            sw.Write("<td align=right rowspan=2>{0}&nbsp;<td rowspan=2>{1}&nbsp;<tr bgcolor=Silver>", dr["rc"], dr["note"]);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 成績公告
    /// </summary>
    public class Field_Html_Print : FIELD_Html_Basic
    {
        public virtual void main_content(string si_id, string s_item, string rcx,String gr_out_str,OdbcConnection conn, StreamWriter sw)
        {
            int group_cnt = 0;
            using (OdbcDataReader dr = new OdbcCommand(String.Format("select frc_id,{0}  from field_rc where fi_id='{1}' order by BIT_LENGTH( s_number),s_number;", fieldnames, si_id), conn).ExecuteReader())
            {
                string group_str = null;
                bool group_flag = false;

                for (int i = 0; i < dr.FieldCount; i++) if (dr.GetName(i) == "group_id") { group_flag = true; break; }
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
                            sw.WriteLine("<table border=1 width=100%><tr>{0}", TH_STR());
                        }
                    }
                    sw.Write("<tr>");
                    for (int i = 3; i < dr.FieldCount; i++)
                        sw.Write("<td class=x133r align=\"right\" colspan={0}>{1}&nbsp;", colspan_ar[i - 3], dr[i]);
                }
            }
        }
        public Field_Html_Print(string si_id, string s_item, string rcx, OdbcConnection conn, string file_name,iGR pGR,bool blankformat)
            : base(si_id, s_item, rcx, conn, file_name,pGR,blankformat)
        {
            //   html_filename = Basic_HTB_Info.baseFilePath + "\\temp\\" + String.Format("{0}_{1}.htm", si_id, s_item);
            StreamWriter sw = new StreamWriter(html_filename, false, Encoding.GetEncoding(950));
            if (!blankformat)
            {
                if (file_name.ToLower().Contains(".htm"))
                {
                    sw.WriteLine(RC_Html_EXCEL_TAG.html_head);
                }
                else
                {
                    sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
                }
                //sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_HEAD);
            }
            sw.WriteLine("<B><center>成績公告<br>{0}</center></b>", s_item);
            sw.WriteLine("<table border=1 width=100%><tr>{0}", TH_STR());

            string gr_out_str = GR_STR(si_id, conn, rcx);
            main_content(si_id, s_item, rcx, gr_out_str, conn, sw);
            sw.WriteLine("</table><br><hr>");
            sw.WriteLine(gr_out_str);
            if(!blankformat)
            sw.WriteLine(RC_Html_EXCEL_TAG.EXCEL_END);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
