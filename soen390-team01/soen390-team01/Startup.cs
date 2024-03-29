using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using soen390_team01.Data;
using soen390_team01.Models;
using soen390_team01.Services;
using System;
using System.IO;
using System.Reactive.Linq;
using soen390_team01.Controllers;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;

namespace soen390_team01
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<EmailClient>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ProductionClient>();
            services.AddTransient<Random>();
            services.AddTransient<ProductionInventoryValidator>();
            services.AddTransient<IProductionProcessor, ProductionController>();
            services.AddTransient<IProductionReportGenerator, CsvProductionReportGenerator>();
            services.AddTransient<IProductionReportGenerator, WebProductionReportGenerator>();

            
            services.AddSingleton<AuthenticationFirebaseService>();
            services.AddSingleton<IProductionService,ProductionService>();
            services.AddSingleton<IInventoryService, InventoryModel>();
            services.AddSingleton<IAccountingService, AccountingModel>();
            services.AddSingleton<IUserManagementService, UserManagementModel>();
            services.AddSingleton<ITransferService, TransfersModel>();
            services.AddSingleton<IAssemblyService, AssemblyModel>();
            services.AddSingleton(s => new CsvProductionProcessor("productions", new ProductionController(s.GetService<IAssemblyService>(), s.GetService<ILogger<ProductionController>>(), s.GetService<IEmailService>())));
            services.AddSingleton(s => new EncryptionService(
                Environment.GetEnvironmentVariable("ENCRYPTED_KEY")
            ));
            services.AddDataProtection();
            services.AddControllers()
                .AddRazorRuntimeCompilation();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizePage("/Home/Privacy");
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Authentication/index";
                });
            services.AddDbContext<ErpDbContext>(options =>
                options.UseNpgsql(
                    Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!
                )
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/Log.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Authentication}/{action=Index}/{id?}");
            });

            if (!Directory.Exists("productions"))
            {
                Directory.CreateDirectory("productions");
            }

            var timer = Observable.Interval(TimeSpan.FromSeconds(5));
            timer.Subscribe(tick => { app.ApplicationServices.GetService<CsvProductionProcessor>()!.Process(); });
        }
    }
}
