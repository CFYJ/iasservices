using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SyncWebIAS.Model;
using IASServices.Repositories;
using System.Web.Http.Cors;
using IASServices.Models;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Rewrite;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Http.Features;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IASServices
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DelegacjaContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DelegacjeConnection")));
            services.AddDbContext<KontaktyContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DelegacjeConnection")));
            services.AddDbContext<UpowaznieniaContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DelegacjeConnection")));
            services.AddDbContext<IasSecurityContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DelegacjeConnection")));
            services.AddDbContext<GrafyContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DelegacjeConnection")));
            services.AddDbContext<InterpretacjeContext>(o => o.UseSqlServer(Configuration.GetConnectionString("InterpretacjeConnection")));
            services.AddDbContext<HelpDeskContext>(o => o.UseSqlServer(Configuration.GetConnectionString("LubelskaIasConnection")));

            services.AddScoped(typeof(IData<Delegacja, long>), typeof(DelegacjaRepository));
            // Add framework services.
            //services.AddRouting().AddMvcCore();

            //var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            ////var pathToExe = @"c:\tmp\IASServices\IASServices.exe";
            //var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            //var cert = new X509Certificate2(Path.Combine(pathToContentRoot, "localhost1.pfx"), "123456!");
            //services.Configure<KestrelServerOptions>(opt =>
            //{
            //    opt.UseHttps(cert);
            //});

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                                     .AllowAnyMethod()
                                                                     .AllowAnyHeader()));

            
         
            services.AddMvc();
          

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAll"));
               
            });



            //services.AddAuthorization(options =>
            //{

            //    options.AddPolicy(
            //        "HelpDeskModule",
            //        policy => policy.RequireRole("helpdesk"));

            //    //options.AddPolicy("HelpDeskModule",
            //    //    policy =>
            //    //    {
            //    //        policy.RequireClaim("helpdesk");
            //    //    });
            //});

            // services.AddAuthentication();

            //services.AddMvc(config =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //                     .RequireAuthenticatedUser()
            //                     .Build();
            //    config.Filters.Add(new AuthorizeFilter(policy));
            //});

            //wasl zwiekszenie limitow uploadu plikow
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 10; //default 1024
                options.ValueLengthLimit = int.MaxValue; //not recommended value
                options.MultipartBodyLengthLimit = long.MaxValue; //not recommended value
            });

        

            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAll"));
            //});
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});

            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {     

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSession();
            app.UseCors("AllowAll");
            //app.UseCors(builder => builder.AllowAnyOrigin()//.WithOrigins("http://192.168.1.101:81")
            //                              .AllowAnyMethod()
            //                              .AllowAnyHeader());
            app.UseResponseCompression();

           

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                //AutomaticChallenge = true,
                //TokenValidationParameters = tokenValidationParameters
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("VeryCompl!c@teSecretKey")),
                    ValidateActor = false
                },
        });

            app.UseMvc();
         



            //var options = new RewriteOptions()
            //    .AddRedirectToHttps();
            //app.UseRewriter(options);
        }
    }
}
