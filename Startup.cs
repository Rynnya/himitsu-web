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
                    Console.WriteLine($"{context.Request.Method,-4} | {context.Response.StatusCode} | {context.Request.Path}");
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("verify", "register/verify", new { controller = "register", Action = "Verify" });
                endpoints.MapControllerRoute("register", "register", new { controller = "register", Action = "Register" });
                endpoints.MapControllerRoute("beatmap", "b/{id?}", new { controller = "beatmap", Action = "Beatmap" });
                endpoints.MapControllerRoute("profile", "u/{id?}", new { controller = "profile", Action = "Profile" });
                endpoints.MapControllerRoute("set",     "s/{id?}", new { controller = "beatmap", Action = "Set" });
            });
        }
        private void AutoLogin(HttpContext context)
        {
            try
            {
                if (!context.Session.Keys.Contains("userid") && !string.IsNullOrEmpty(context.Request.Cookies["rt"]) && Utility.EscapeDirectories(context.Request) && !context.Session.Keys.Contains("login"))
                {
                    QueryFactory db = new QueryFactory(new MySqlConnection(var.Connection), new MySqlCompiler());
                    Console.WriteLine($"LOG  | XXX | Trying autologin by using cookies");
                    dynamic data = db.Select("SELECT u.id, u.password_md5, t.token FROM users u LEFT JOIN users_stats s ON s.id = u.id LEFT JOIN tokens t on t.user = u.id WHERE t.token = @token LIMIT 1", new { token = context.Request.Cookies["rt"] }).First();
                    int id = data.id;
                    Console.WriteLine($"LOG  | XXX | Successful login for user {id}");
                    Utility.setCookie(db, context, id);
                    context.Session.SetInt32("userid", id);
                    context.Session.SetString("pw", (string)data.password_md5);
                    context.Session.CommitAsync();
                }
            }
            catch { 
                Console.WriteLine("WARN | ERR | Something bad happend when trying to get cookie, aborting.");
                context.Session.SetInt32("login", 0);
                context.Session.CommitAsync();
            }
        }
    }
}
