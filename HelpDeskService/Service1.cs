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

                        string body = System.Text.Encoding.Default.GetString(message.RawMessage);

                        string pat = @"<html>(.*)</html>";

                        Regex r = new Regex(pat, RegexOptions.Singleline);

                        Match m = r.Match(body);
                        int count = 0;
                        while (m.Success)
                        {
                            string rez = m.Value;

                            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["helpdesk"].ConnectionString);
                            conn.Open();
                            if (conn.State == ConnectionState.Open)
                            {
                                System.Data.SqlClient.SqlCommand query = new System.Data.SqlClient.SqlCommand(@"insert into HelpDeskInfo (tresc) values(' " + rez + "')", conn);

                                query.ExecuteNonQuery();

                                //pop.DeleteMessage(msg);

                                conn.Close();
                            }

                            m.NextMatch();
                        }

                        var stop = 0;

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

                       
                    }
                    catch (Exception ex) { writeLog(ex.Message); Thread.Sleep(5000); };
                }

                pop.Disconnect();
            }
        }

        private void writeLog(string text)
        {
            System.IO.File.AppendAllText(ConfigurationManager.AppSettings["logstorage"].ToString(), "\r\n" + text);
        }



    }
}
