﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayerAccount.BusinessLogic;
using PayerAccount.Common;
using PayerAccount.Dal.Local;

namespace PayerAccount
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
            services.AddMvc();

            services.AddDbContext<PayerAccountDbContext>(options =>
                options.UseSqlite("Data Source=PayerAccount.db"));

            services.AddTransient<IPayerAccountContext, PayerAccountContext>(
                serviceProvider => new PayerAccountContext(serviceProvider.GetService<PayerAccountDbContext>()));

            services.AddDistributedMemoryCache();
            services.AddSession();

            Config.Provider = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("dc7c1e72-414d-4264-8987-ac34edfca3c8")
                .Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
