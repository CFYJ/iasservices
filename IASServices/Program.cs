using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;

namespace IASServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
            //.UseUrls("http://localhost:5001","https://localhost:5001")
            .UseUrls("http://localhost:5001")
            .UseContentRoot(pathToContentRoot)
            .UseIISIntegration()
            .UseStartup<Startup>()
            //.UseApplicationInsights()
            //.UseKestrel(cfg => cfg.UseHttps(cert))
            .Build();

            if (isService)
            {
                host.RunAsService();
            }
            else
            {
                host.Run();
            }
        }
    }
}
