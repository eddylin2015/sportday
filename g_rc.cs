using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

using System.Net;
using System.IO;
namespace ConsoleApplication1
{
    class Program
    {
        public static string Conn_Str = "Driver={MySQL ODBC 5.1 Driver};Server=localhost;Database=SportDay2015;UID=xxxx;PWD=xxxx;OPTION=3";
		static string initxt = @"c:\code\config\sportday_dbconnection_str.txt";
        static void Main(string[] args)
        {
			FileInfo finfo0 = new FileInfo(initxt);
            if (finfo0.Exists)
            {
                using (StreamReader file = new StreamReader(initxt, Encoding.Default))
                {
                    Conn_Str = file.ReadToEnd().Replace("\r","").Replace("\n","");
                }
            }
            else
            {
			Console.WriteLine("error not db_config.txt");
			return;
			}
            String[] f1 = {"男A","男B","男C","男D","女A","女B","女C","女D"};
            String[] f2 =  {"男子A","男子B","男子C","男子D","女子A","女子B","女子C","女子D"};
            Object[] o = new Object[8];
            o[0] = new List<String>();
            o[1] = new List<String>();
            o[2] = new List<String>();
            o[3] = new List<String>();
            o[4] = new List<String>();
            o[5] = new List<String>();
            o[6] = new List<String>();
            o[7] = new List<String>();
            

            OdbcConnection conn = new OdbcConnection(Conn_Str);
            conn.Open();
            {
                String sql = "select s_item,rank, name  ,classno  ,rc,note from sport_rc a inner join sport_item b on a.si_id=b.si_id where lock_item=1 and trim(rank) in ('1','2','3') order by s_item,rank";
                OdbcDataReader dr = new OdbcCommand(sql, conn).ExecuteReader();
                Console.WriteLine("<Table>");
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    Console.Write(dr.GetName(i));
                    Console.Write("\t");
                }
                Console.WriteLine();
                while (dr.Read())
                {
                    if (dr[0].ToString().Contains("初") || dr[0].ToString().Contains("複"))
                    {
                    }
                    else
                    {
                        for(int j=0;j<f1.Length;j++){
                        if (dr[0].ToString().Contains(f1[j]) || dr[0].ToString().Contains(f2[j]))
                        {
                            
                            String line0 = "";
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                            
                                line0+=dr[i].ToString()+"\t";
                            }
                            List<String> li = (List<string>)o[j];
                            li.Add(line0);
                        }
                        }
                    }
                }

            }
            {
                String sql = "select f_item,rank,classno,name,rc,note from field_rc a inner join field_item b on a.fi_id=b.fi_id where lock_item=1 and trim(rank) in ('1','2','3') and rc >'' order by f_item,rank";
                OdbcDataReader dr = new OdbcCommand(sql, conn).ExecuteReader();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    Console.Write(dr.GetName(i));
                    Console.Write("\t");
                }
		Console.WriteLine();
                while (dr.Read())
                {

                    if (dr[0].ToString().Contains("初") || dr[0].ToString().Contains("複"))
                    {
                    }
                    else
                   {
                        for(int j=0;j<f1.Length;j++){
                        if (dr[0].ToString().Contains(f1[j]) || dr[0].ToString().Contains(f2[j]))
                        {
                            String line0 = "";
                            for (int i = 0; i < dr.FieldCount; i++)
                            {

                                line0 += dr[i].ToString() + "\t";
                            }
                            List<String> li = (List<string>)o[j];
                            li.Add(line0);
                        }
                        }
                    }
                }

                conn.Close();

                for (int i = 0; i < f1.Length; i++)
                {
                    List<string> li =(List<String>) o[i];
                    foreach (String s in li)
                    {
                        Console.WriteLine(s);
                    }
                }
               // Console.Read();
            }

        }
    }
}

