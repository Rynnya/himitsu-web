using System.IO;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Himitsu.Variable;
using System.Linq;
using System.Threading.Tasks;
using SqlKata.Compilers;
using SqlKata.Execution;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Himitsu
{
    public class Startup
    {
        public Variables var;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public bool locked = false;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var = new Variables();
            services.AddSingleton(var);
            services.Configure<CookiePolicyOptions>(options => {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedMemoryCache();
            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true;
            });
            services.AddScoped(x => {
                return new QueryFactory(new MySqlConnection(var.Connection), new MySqlCompiler());
            });
            services.AddMvc();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            error.Run(async ctx => {
                ctx.Response.Clear();
                ctx.Response.StatusCode = 500;
                await ctx.Response.WriteAsync(File.ReadAllText("Views/Shared/error500.cshtml"));
            }));

            app.UseSession();

            app.Use(async (context, done) =>
            {
                await Task.Run(() => AutoLogin(context));
                await done.Invoke();
                if (Utility.EscapeDirectories(context.Request))
                    Console.WriteLine($"{context.Request.Method} | {context.Response.StatusCode} | {context.Request.Path}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("verify", "register/verify", new { controller = "register", Action = "Verify" });      // done
                endpoints.MapControllerRoute("register", "register", new { controller = "register", Action = "Register" });         // done
                endpoints.MapControllerRoute("beatmap", "b/{id?}", new { controller = "beatmap", Action = "Beatmap" });             // done
                endpoints.MapControllerRoute("profile", "u/{id?}", new { controller = "profile", Action = "Profile" });             // done
            });
        }

        private void AutoLogin(HttpContext context)
        {
            if (!context.Session.Keys.Contains("userid") && context.Request.Cookies["rt"] != "" && !locked && Utility.EscapeDirectories(context.Request) && !context.Session.Keys.Contains("login"))
            {
                var db = new QueryFactory(new MySqlConnection(var.Connection), new MySqlCompiler());
                locked = true;
                Console.WriteLine($"LOG | Trying autologin by using cookies");
                var data = db.Select("SELECT u.id, u.password_md5, t.token FROM users u LEFT JOIN users_stats s ON s.id = u.id LEFT JOIN tokens t on t.user = u.id WHERE t.token = @token LIMIT 1", new { token = context.Request.Cookies["rt"] }).First();
                if (data != null)
                {
                    int id = data.id;
                    Console.WriteLine($"LOG | Successful login for user {id}");
                    Utility.setCookie(db, context, id);
                    context.Session.SetInt32("userid", id);
                    context.Session.SetString("pw", Utility.CreateMD5((string)data.password_md5));
                }
                locked = false;
            }
        }
    }
}
