using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Specialized;

namespace postGAE
{
    class Program
    {
        static string localhost = "http://localhost/report_to_public/files/";
        static string remothost = "";
        static string initxt = @"c:\code\config\postGAE.txt";
        static void Main(string[] args)
        {

            FileInfo finfo0 = new FileInfo(initxt);
            if (finfo0.Exists)
            {
                using (StreamReader file = new StreamReader(initxt, Encoding.Default))
                {
                    string[] temp = file.ReadToEnd().Split('\n');
                    localhost = temp[0].Replace("\r","");
                    remothost = temp[1];
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("please input localhost:");
                    localhost = Console.ReadLine();
                    Console.WriteLine("please input remotehost:");
                    remothost = Console.ReadLine();
                    if (remothost.Contains("http://") && localhost.Contains("http://"))
                    {
                        using (System.IO.StreamWriter file =
                           new System.IO.StreamWriter(initxt, false))
                        {
                            file.WriteLine(localhost);
                            file.WriteLine(remothost);
                            file.Flush();
                        }

                        break;
                    }
                }
            }

            string[] items ={                            
"女A_100M_(初賽)","509",
"女A_100M_(決賽)","552",
"女A_100M_(複賽)","522",
"女A_100米欄_(決賽)","544",
"女A_1500M_(決賽)","540",
"女A_200M_(初賽)","517",
"女A_200M_(決賽)","532",
"女A_3000M_(決賽)","559",
"女A_400M_(初賽)","526",
"女A_400M_(決賽)","565",
"女A_4x100M_(決賽)","571",
"女A_4x400M_(決賽)","556",
"女A_800M_(決賽)","503",
"女B_100M_(初賽)","607",
"女B_100M_(決賽)","650",
"女B_100M_(複賽)","620",
"女B_1500M_(決賽)","638",
"女B_200M_(初賽)","615",
"女B_200M_(決賽)","630",
"女B_400M_(決賽)","663",
"女B_4x100M_(決賽)","670",
"女B_800M_(決賽)","601",
"女C_100M_(初賽)","705",
"女C_100M_(決賽)","748",
"女C_100M_(複賽)","724",
"女C_1500M_(決賽)","736",
"女C_200M_(初賽)","719",
"女C_200M_(決賽)","728",
"女C_4x100M_(決賽)","769",
"女D_1000M_(決賽)","854",
"女D_4x50M_(初賽)","861",
"女D_4x50M_(決賽)","867",
"女D_60M_(初賽)","811",
"女D_60M_(複賽)","834",
"女D_60M_(決賽)","846",
"男A_100M_(初賽)","110",
"男A_100M_(決賽)","153",
"男A_100M_(複賽)","123",
"男A_110米欄_(初賽)","113",
"男A_110米欄_(決賽)","145",
"男A_1500M_(分組決)","141",
"男A_200M_(初賽)","118",
"男A_200M_(決賽)","133",
"男A_400M_(初賽)","127",
"男A_400M_(決賽)","166",
"男A_4x100M_(決賽)","172",
"男A_4x400M_(決賽)","157",
"男A_5000M_(決賽)","160",
"男A_800M_(決賽)","104",
"男B_100M_(初賽)","208",
"男B_100M_(決賽)","251",
"男B_100M_(複賽)","221",
"男B_100米欄_(決賽)","243",
"男B_1500M_(決賽)","239",
"男B_200M_(初賽)","216",
"男B_200M_(決賽)","231",
"男B_3000M_(決賽)","258",
"男B_400M_(決賽)","264",
"男B_800M_(決賽)","202",
"男C_100M_(初賽)","306",
"男C_100M_(決賽)","349",
"男C_100M_(複賽)","325",
"男C_1500M_(決賽)","337",
"男C_200M_(決賽)","329",
"男C_80米欄_(決賽)","342",
"男D_1000M_1(決賽)","455",
"男D_4x50M_(初賽)","462",
"男D_4x50M_(決賽)","468",
"男D_60M_(初賽)","412",
"男D_60M_(複賽)","435",
"男D_60M_(決賽)","447",
"女子A組--跳遠(決賽)","513",
"女子A組--跳高(決賽)","501",
"女子A組--鉛球(決賽)","530",
"女子B組--跳遠(決賽)","658",
"女子B組--跳高(決賽)","628",
"女子B組--鉛球(決賽)","614",
"女子C組--跳遠(決賽)","746",
"女子C組--跳高(決賽)","739",
"女子C組--鉛球(決賽)","761",
"女子D組--壘球(決賽)","837",
"女子D組--跳遠(決賽)","863",
"女子D組--跳高(決賽)","819",
"男子A組--三級跳遠(決賽)","119",
"男子A組--跳遠(決賽)","128",
"男子A組--跳高(決賽)","161",
"男子A組--鉛球(決賽)","103",
"男子B組--三級跳遠(決賽)","236",
"男子B組--跳遠(決賽)","201",
"男子B組--跳高(決賽)","213",
"男子B組--鉛球(決賽)","248",
"男子C組--跳遠(決賽)","338",
"男子C組--跳高(決賽)","354",
"男子C組--鉛球(決賽)","359",
"男子D組--壘球(決賽)","419",
"男子D組--跳遠(決賽)","453",
"男子D組--跳高(決賽)","458",
"NGR","901"
                            };
            List<string> log = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                if (i % 2 == 1) { }
                else
                {
                    Console.Write(items[i]);
                    Console.WriteLine(items[i + 1]);
                    String f  = @"c:\temptemp\" + items[i] + "賽果.htm";
                    String f0 = @"c:\temptemp\" + items[i] + "名單.htm";
                    if (posturl(items[i + 1], items[i], localhost + items[i] + "賽果.htm",log))
                    {
                        log.Add(f);
                    }
                    else if (posturl(items[i + 1], items[i], localhost + items[i] + "名單.htm",log))
                    {
                        log.Add(f0);
                    }
                    Console.WriteLine("--");
                }

            }
            Console.WriteLine("log:");
            Console.ReadLine();
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }
        public static bool posturl(string ds_n, string item, string get_url,List<string> log)
        {
            Console.WriteLine(get_url);           
            string post_context = "";
            WebRequest request = WebRequest.Create(get_url);
            request.Method = "GET";
            try
            {
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream, Encoding.Default);
                string readerstream = reader.ReadToEnd();
                char[] splitchar={'\n','\r'};
                string[] line_s = readerstream.Split(splitchar);
                bool flg = false;
                foreach (string line in line_s) 
                {
                    if (flg)
                    {
                        post_context += line.Replace("width=100%", "").Replace("&nbsp;", "").Replace("<td colspan=1>", "<td>").Replace("colspan=1", "").Replace("align=\"right\"", "").Replace("class=x133r", "").Replace("<td   >", "<td>").Replace("<font color=White>.</font>", "").Replace("<font color=Silver>.</font>", "").Replace("<td  >", "<td>").Replace("<td >", "<td>");
                    }
                    if (line.ToUpper().Contains("BODY")) { flg = true; }
                }
                Console.WriteLine(post_context);
                reader.Close();
                dataStream.Close();
                response.Close();
                Console.WriteLine(postgae(ds_n, item, post_context,log));
            }
           catch(Exception exp){
               return false;
           }
            return true;

        }
        static string postgae(string ds_n,string item,string post_context,List<string>log)
        {
            try
            {
                String post_url = remothost;

                byte[] data = System.Text.Encoding.UTF8.GetBytes("n=" + ds_n + "&ITEM=" + item + "&CONTENT=" + post_context);
                WebRequest post_request = WebRequest.Create(post_url);
                post_request.Method = "POST";
                post_request.ContentType = "application/x-www-form-urlencoded";// "text/html";

                post_request.ContentLength = data.Length;
                Stream dataStream = post_request.GetRequestStream();

                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse response = post_request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                String responseformserver = reader.ReadToEnd();
                if (responseformserver.Contains("succ")) log.Add("succ");
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseformserver;
            }
            catch (Exception ep)
            {
                Console.WriteLine("post to google app spot websit error");
                Console.ReadLine();
                return "error";
            }
          }
        public static string postfile(string ds_n, string item, string filename)
        {

            string post_context = "";
            
            // post_context = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(post_context));
            string[] lines = System.IO.File.ReadAllLines(filename, Encoding.Default);

            // Display the file contents by using a foreach loop.
            bool flg = false;
            foreach (string line in lines)
            {

                if (flg)
                {
                    post_context += line.Replace("width=100%", "").Replace("&nbsp;", "").Replace("<td colspan=1>", "<td>").Replace("colspan=1", "").Replace("align=\"right\"", "").Replace("class=x133r", "").Replace("<td   >", "<td>"); ;
                }
                if (line.ToUpper().Contains("BODY")) { flg = true; }
            }



            String post_url = remothost;
            // post_url = "http://www.contoso.com/PostAccepter.aspx ";
            // ?n=ds_n&ITEM=" + item + "&CONTENT=" + post_context.Substring(0,1000)+"";

            byte[] data = System.Text.Encoding.UTF8.GetBytes("n=" + ds_n + "&ITEM=" + item + "&CONTENT=" + post_context);
            WebRequest request = WebRequest.Create(post_url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";// "text/html";

            request.ContentLength = data.Length;
            Stream dataStream = request.GetRequestStream();

            dataStream.Write(data, 0, data.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            String responseformserver = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();




            return responseformserver;



        }
    }
}
