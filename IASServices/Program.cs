using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System;

namespace IASServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //try {

            //bool isService = true;
            bool isService = ConfigurationManager.AppSettings.Get("runasservice") == "true" ? true : false;
                if (Debugger.IsAttached || args.Contains("--console"))
                {
                    isService = false;
                }

                var pathToContentRoot = Directory.GetCurrentDirectory();
                if (isService)
                {
                    var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                    pathToContentRoot = Path.GetDirectoryName(pathToExe);
                }


                //var cert = new X509Certificate2(Path.Combine(pathToContentRoot, "10.10.1.95.pfx"), "123456!");


                var host = new WebHostBuilder()
                .UseKestrel()
                //.UseKestrel(cfg => cfg.UseHttps(cert))
                //.UseUrls("http://localhost:5000","https://localhost:5001")
                .UseUrls("http://localhost:5000")
                .UseContentRoot(pathToContentRoot)
                .UseIISIntegration()
                .UseStartup<Startup>()
                //.UseApplicationInsights()
                //.UseKestrel(cfg => cfg.UseHttps(cert))
                .Build();

                if (isService)
                {
                    //System.IO.File.AppendAllText("C:\\tmp\\iaslog.txt", "run us service\r\n");
                    host.RunAsService();
                }
                else
                {
                    //       System.IO.File.AppendAllText("C:\\tmp\\iaslog.txt", "run us console\r\n");
                    host.Run();
                }

            
        //}catch(Exception ex) { System.IO.File.AppendAllText("C:\\tmp\\iaslog.txt", "message:\r\n" + ex.Message + "\r\n\r\ninnerexception:\r\n" + ex.InnerException + "\r\n\r\nstacktrace\r\n" + ex.StackTrace+"\r\n"); }
        }
    }
}
