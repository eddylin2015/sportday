using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SportDay
{
    public class webpostds
    {
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
    public class webpost
    {
        public static bool post(String post_context)
        {
         /*
            // String post_url="http://192.168.101.250/a/dbbucket/upd.php?db=001&tb=tba";
            String post_url = "http://202.175.185.186/mbcapp/sp/upd.php?db=001&tb=tba";
            byte[] data = System.Text.Encoding.UTF8.GetBytes("t=" + post_context);// (new ASCIIEncoding())
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(post_url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";// "text/html";
            
            request.ContentLength = data.Length;
            
            */

/*            if (_cookie == null) { _cookie = new CookieContainer(); }
            request.CookieContainer = _cookie;
            Uri u = new Uri(Urihost);
            request.CookieContainer.SetCookies(u, "PHPSESSID=" + PHPSESSID);
            */
            /*
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
            }*/
            return true;
        }
    }
}
