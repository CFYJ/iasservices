using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using OpenPop;
using System.Configuration;
using System.Text.RegularExpressions;

using System.Data;
using System.Web;
using System.Net.Mail;

namespace HelpDeskService
{
    public partial class Service1 : ServiceBase
    {
        System.Threading.Thread apka;

        public Service1()
        {
            InitializeComponent();
        }

        public void Start()
        {
            this.OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            writeLog("start");
            isRunning = true;
            apka = new System.Threading.Thread(delegate () { mainApk(); }); //new System.Threading.Thread(mainApk);
            apka.Start();
        }

        protected override void OnStop()
        {
            isRunning = false;
            apka.Abort();
        }


        bool isRunning = false;
        private void mainApk()
        {
            while (isRunning)
            {
                if (ncountMessages() > 0)
                    getMessages();

                //isRunning = false;
                System.Threading.Thread.Sleep(10000);
            }
        }



        private OpenPop.Pop3.Pop3Client connect()
        {
            string server = ConfigurationManager.AppSettings["server"].ToString();
            string port = ConfigurationManager.AppSettings["port"].ToString();
            string ssl = ConfigurationManager.AppSettings["ssl"].ToString();
            string user = ConfigurationManager.AppSettings["login"].ToString();
            string password = ConfigurationManager.AppSettings["password"].ToString();

            try
            {

                OpenPop.Pop3.Pop3Client pop = new OpenPop.Pop3.Pop3Client();
                pop.Connect(server, int.Parse(port), bool.Parse(ssl));
                pop.Authenticate(user, password, OpenPop.Pop3.AuthenticationMethod.Auto);

                if (pop.Connected)
                {
                    return pop;
                }
            }
            catch (Exception ex) { writeLog(ex.Message); }

            return null;
        }

        private int ncountMessages()
        {
            var pop = connect();
            if (pop != null)
            {
                int ile = pop.GetMessageCount();
                pop.Disconnect();
                return ile;
            }
            return 0;
        }


        private static string Decode(string input, string charSet)
        {
            if (string.IsNullOrEmpty(charSet))
            {
                var charSetOccurences = new Regex(@"=\?.*\?Q\?", RegexOptions.IgnoreCase);
                var charSetMatches = charSetOccurences.Matches(input);
                foreach (Match match in charSetMatches)
                {
                    charSet = match.Groups[0].Value.Replace("=?", "").Replace("?Q?", "");
                    input = input.Replace(match.Groups[0].Value, "").Replace("?=", "");
                }
            }

            Encoding enc = new ASCIIEncoding();
            if (!string.IsNullOrEmpty(charSet))
            {
                try
                {
                    enc = Encoding.GetEncoding(charSet);
                }
                catch
                {
                    enc = new ASCIIEncoding();
                }
            }

            //decode iso-8859-[0-9]
            var occurences = new Regex(@"=[0-9a-zA-Z]{2}", RegexOptions.Multiline);
            var matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                try
                {
                    byte[] b = new byte[] { byte.Parse(match.Groups[0].Value.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier) };
                    char[] hexChar = enc.GetChars(b);
                    input = input.Replace(match.Groups[0].Value, hexChar[0].ToString());
                }
                catch
                {; }
            }

            //decode base64String (utf-8?B?)
            occurences = new Regex(@"\?utf-8\?B\?.*\?", RegexOptions.IgnoreCase);
            matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                byte[] b = Convert.FromBase64String(match.Groups[0].Value.Replace("?utf-8?B?", "").Replace("?UTF-8?B?", "").Replace("?", ""));
                string temp = Encoding.UTF8.GetString(b);            
                input = input.Replace(match.Groups[0].Value, temp);
            }

            input = input.Replace("=\r\n", "").Replace("š","ą").Replace("Ľ","Ą");

            return input;
        }

        private void getMessages()
        {
            var pop = connect();

            int msg = 0;
            if (pop.Connected)
            {

                while ((msg = pop.GetMessageCount()) > 0)
                {
                    try
                    {

                        OpenPop.Mime.Message message = pop.GetMessage(msg);

                        if (message.MessagePart.MessageParts != null)
                            foreach (OpenPop.Mime.MessagePart msgpart in message.MessagePart.MessageParts)
                            {
                                if (msgpart.Body != null)
                                {
                                    if (msgpart.FileName.Contains("Forwarded") || msgpart.ContentType.MediaType.Contains("html"))
                                        parseMessage(msgpart.Body);
                                }
                            }


                        pop.DeleteMessage(msg);
                    }
                    catch (Exception ex) { writeLog(ex.Message); Thread.Sleep(5000); };
                }

                pop.Disconnect();
            }
        }

        private void parseMessage(byte[] msgbody)
        {
            string body = System.Text.Encoding.Default.GetString(msgbody);


            string text = Decode(body, "ISO-8859-2");
            body = text;

            //string opisPat= "(<td colspan=\"4\" align=\"left\">OPIS)(.*?)</td>";
            string zgloszeniePat = "(<td colspan=\"4\" align=\"left\">OPIS)(.*?)(<p>(.*?)</p>)(.*?)</td>";
            string zgloszenie = (new Regex(zgloszeniePat, RegexOptions.Singleline)).Match(body).Groups[3].Value;

            //string dataZgPat = @"(?<1>(<td[^>]*>ZAREJESTROWANO.*?</td>)(\s*)(<td.*?</td>))";
            string dataZgPat = @"(?<1>(<td[^>]*>ZAREJESTROWANO.*?</td>)(\s*)(<td[^>]*>(.*?)</td>))";
            string dataZg = (new Regex(dataZgPat, RegexOptions.Singleline)).Match(body).Groups[4].Value;


            string nrZgPat = @"(?<1>(<td[^>]*>NR.*?</td>)(\s*)(<td[^>]*>(.*?)</td>))";
            string nrZg = (new Regex(nrZgPat, RegexOptions.Singleline)).Match(body).Groups[4].Value;

            string tematPat = @"(?<1>(<td[^>]*>TEMAT.*?</td>)(\s*)(<td[^>]*>(.*?)</td>))";
            string tematZg = (new Regex(tematPat, RegexOptions.Singleline)).Match(body).Groups[4].Value;

            string statusPat = @"(?<1>(<td[^>]*>STATUS.*?</td>)(\s*)(<td[^>]*>(.*?)</td>))";
            string statusZg = (new Regex(statusPat, RegexOptions.Singleline)).Match(body).Groups[4].Value;

            string zglaszajacyPat = @"(?<1>(<td[^>]*>ZGŁASZAJĄCY.*?</td>)(\s*)(<td[^>]*>(.*?)</td>))";
            string zglaszajacyZg = (new Regex(zglaszajacyPat, RegexOptions.Singleline)).Match(body).Groups[4].Value;


            //string pat = @"<html><table(.*)</table></html>";                 
            //string pat = @"<body[^>]*>(.*)</body>";
            //string trescPat = @"<body.*?(<table.*</table>).*?</body>";
            //string tresc = (new Regex(trescPat, RegexOptions.Singleline)).Match(body).Groups[1].Value;
            string trescPat = "<td[^>]*>Zgłoszenie.*?<table.*</table>";
            string tresc = (new Regex(trescPat, RegexOptions.Singleline)).Match(body).Value;

            trescPat = "<table.*</table>";
            tresc = (new Regex(trescPat, RegexOptions.Singleline)).Match(tresc).Value;


            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["helpdesk"].ConnectionString);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                string querystring = @"insert into HelpDeskInfo (tresc, zgloszenie, data, nr, temat, zglaszajacy,datarejestracji,status) " +
                    "values('" + tresc.Replace("'","\"") + "', '" + zgloszenie.Replace("'", "\"") + "' ,getdate(), '" + nrZg.Replace("'", "\"") + "','" + tematZg.Replace("'", "\"") + "','" + zglaszajacyZg.Replace("'", "\"") + "','" + dataZg.Replace("'", "\"") + "','" + statusZg.Replace("'", "\"") + "')";
                System.Data.SqlClient.SqlCommand query = new System.Data.SqlClient.SqlCommand(querystring, conn);

                query.ExecuteNonQuery();

                conn.Close();
            }

        }

   

        private void writeLog(string text)
        {
            System.IO.File.AppendAllText(ConfigurationManager.AppSettings["logstorage"].ToString(), "\r\n" + text);
        }


        #region odczyt pliku
        //char[] charArray = message.Headers.Subject.ToCharArray();
        //Array.Reverse(charArray);
        //string plik = new string(charArray);

        //plik = message.Headers.Subject.Substring(plik.Length - plik.IndexOf(' '));
        //plik = plik.Replace(@"\", "_");
        //plik = plik.Replace(@"/", "_");

        //int at = 0;
        //foreach (OpenPop.Mime.MessagePart mp in pop.GetMessage(msg).FindAllAttachments())
        //{
        //    if (mp.IsAttachment && !mp.ContentType.MediaType.Contains("image"))
        //    {
        //        mp.SaveToFile(new System.IO.FileInfo(filestorage + plik + "_" + at.ToString() + ".pdf"));
        //        writeLog(message.Headers.Subject + "  -   " + mp.FileName);
        //    }

        //    at++;
        //}

        #endregion

    }
}
