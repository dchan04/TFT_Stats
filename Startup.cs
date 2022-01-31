using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TFT_Stats.Services;
using Hangfire;
using TFT_Stats.Data;
using Microsoft.EntityFrameworkCore;

namespace TFT_Stats
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
            services.AddControllersWithViews();
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("TFT_Database")));
            services.AddHangfireServer();

            services.AddScoped<ITFT_DataService, TFT_DataService>();

            //DbContext configuration
            services.AddDbContext<TFTDbContext>(
                optionsBuilder =>
                {
                    optionsBuilder.UseSqlServer(Configuration.GetConnectionString("TFT_Database"));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
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

            app.UseHangfireDashboard();

            recurringJobManager.AddOrUpdate("Get RiotAPI Data",
                () => serviceProvider.GetService<ITFT_DataService>().GetApiData(),
                "0 0 12 ? * SAT"
                );

            recurringJobManager.AddOrUpdate("Get Additional information on Companions",
                () => serviceProvider.GetService<ITFT_DataService>().GetAdditionalCompanionInfo(),
                "0 30 12 ? * MON"
                );

            recurringJobManager.AddOrUpdate("Update CompanionVM Database",
                () => serviceProvider.GetService<ITFT_DataService>().UpdateCompanionVmDb(),
                "0 0 13 ? * MON"
                );

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
