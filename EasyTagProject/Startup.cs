using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using EasyTagProject.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EasyTagProject
{
    public class Startup
    {
        // Get configuration to read information in appsettings.json file
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            // Connection to EasyTag DataBase
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration["Data:EasyTagDB:ConnectionString"]));

            #region IdentityFramework
            // Connection to EasyTag Identity DataBase
            services.AddDbContext<ETIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["Data:EasyTagIdentity:ConnectionString"]));

            // Services for Identity Framework
            services.AddIdentity<EasyTagUser, IdentityRole>(options => {
                // Options required for password validation
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                $"abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789!#$%&'()*+,-.:;<=>?@[]^_`|~";
            })
                .AddEntityFrameworkStores<ETIdentityDbContext>()
                //.AddDefaultUI()
                .AddDefaultTokenProviders();

            #endregion

            // Services for dependency injection
            services.AddTransient<IRoomRepository, EFRoomRepository>();
            services.AddTransient<IAppointmentRepository, EFAppointmentRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Required services for application functionality
            services.AddRazorPages();
            services.AddMemoryCache();
            services.AddSession();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Error page returned by environment variable
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            // User CSS and JS provided
            app.UseStaticFiles();
            
            app.UseCookiePolicy();
            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Default routing system
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });


            //Creates EasyTagDB if it does not exist and adds default information
            SeedData.EnsurePopulated(app);
            //Creates IdentityUsersEasyTag database if it does not exist and adds default users and roles
            SeedDataIdentiy.EnsurePopulated(app);            
        }
    }
}
