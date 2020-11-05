using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using MySql.Data.MySqlClient;

namespace Himitsu
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
            services.Configure<CookiePolicyOptions>(options => {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedMemoryCache();
            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true;
            });
            services.AddMvc();
            var _sql = new MySqlConnection("");
            _sql.Open();
            services.AddSingleton(_sql);
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            error.Run(async ctx => {
                ctx.Response.Clear();
                await ctx.Response.WriteAsync(File.ReadAllText("Views/Shared/error500.cshtml"));
            }));

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("switcher", "switcher", new { controller = "main", Action = "Switcher" }); // done
                endpoints.MapControllerRoute("leaderboard", "leaderboard", new { controller = "main", Action = "Leaderboard" }); // done
                endpoints.MapControllerRoute("verify", "register/verify", new { controller = "register", Action = "Verify" });
                endpoints.MapControllerRoute("register", "register", new { controller = "register", Action = "Register" });
                endpoints.MapControllerRoute("login", "login", new { controller = "main", Action = "Login" });
                endpoints.MapControllerRoute("beatmap", "b/{id?}", new { controller = "beatmap", Action = "Beatmap" }); // done
                endpoints.MapControllerRoute("profile", "u/{id?}", new { controller = "profile", Action = "Profile" }); // done
                endpoints.MapControllerRoute("main", "", new { controller = "main", Action = "Main" });
                endpoints.MapControllerRoute("error404", "{*.}", new { controller = "main", Action = "Error404" }); // done
            });
        }
    }
}
