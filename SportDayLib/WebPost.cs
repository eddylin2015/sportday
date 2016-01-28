using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Specialized;

namespace SportDayLib
{
    class WebPost
    {

        private static Stream GetPostStream(string filePath, NameValueCollection formData, string boundary)
        {
            Stream postDataStream = new System.IO.MemoryStream();

            //adding form data
            string formDataHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine +
            "Content-Disposition: form-data; name=\"{0}\";" + Environment.NewLine + Environment.NewLine + "{1}";

            foreach (string key in formData.Keys)
            {
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(formDataHeaderTemplate,
                key, formData[key]));
                postDataStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            //adding file data
            /*
            FileInfo fileInfo = new FileInfo(filePath);

            string fileHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine +
            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
            Environment.NewLine + "Content-Type: application/vnd.ms-excel" + Environment.NewLine + Environment.NewLine;

            byte[] fileHeaderBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate,
            "UploadCSVFile", fileInfo.FullName));

            postDataStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

            FileStream fileStream = fileInfo.OpenRead();

            byte[] buffer = new byte[1024];

            int bytesRead = 0;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                postDataStream.Write(buffer, 0, bytesRead);
            }

            fileStream.Close();
            */
            byte[] endBoundaryBytes = System.Text.Encoding.UTF8.GetBytes("--" + boundary + "--");
            postDataStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);

            return postDataStream;
        }
        public static string  postfile(string ds_n, string item, string filename)
        {
            
            string post_context = "";
           // post_context = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(post_context));
            string[] lines = System.IO.File.ReadAllLines(filename,Encoding.Default);

            // Display the file contents by using a foreach loop.
            bool flg = false;
            foreach (string line in lines)
            {

          
                if (flg)
                {
                    post_context += line.Replace("width=100%", "").Replace("&nbsp;", "").Replace("<td colspan=1>", "<td>").Replace("colspan=1", "").Replace("align=\"right\"", "").Replace("class=x133r", "").Replace("<td   >", "<td>").Replace("<font color=White>.</font>", "").Replace("<font color=Silver>.</font>", ""); ;
                }
                if (line.ToUpper().Contains("BODY")) { flg = true; }
            }


    
            
            String post_url = "http://mbcsportday2015.appspot.com/postrc";
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
        public static bool post(String post_context)
        {
            // String post_url="http://192.168.101.250/a/dbbucket/upd.php?db=001&tb=tba";
            String post_url = "http://mbcsportday2015.appspot.com/postrc";
            byte[] data = System.Text.Encoding.UTF8.GetBytes("t=" + post_context);// (new ASCIIEncoding())
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(post_url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";// "text/html";

            request.ContentLength = data.Length;



            /*            if (_cookie == null) { _cookie = new CookieContainer(); }
                        request.CookieContainer = _cookie;
                        Uri u = new Uri(Urihost);
                        request.CookieContainer.SetCookies(u, "PHPSESSID=" + PHPSESSID);
                        */
            Stream reqStream = request.GetRequestStream();

            reqStream.Write(data, 0, data.Length);

            string b;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        b = reader.ReadToEnd();
                        System.Windows.Forms.MessageBox.Show(b);

                    }
                }
            }
            catch (Exception excep)
            {
                System.Windows.Forms.MessageBox.Show(excep.Message);
                return false;
            }
            return true;
        }

    }
}
