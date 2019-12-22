using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Helpers;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // This configuration is passed from appsettings.json
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // services here will be injected to each other
        /* 
        public void ConfigureServices(IServiceCollection services)
        // {
        //     // AddDbContext is used to setup database connection.
        //     services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

        //     services.AddControllers();
        //     services.AddCors(); // added to enable cors as service and add to pipeline
        //     services.AddAutoMapper(typeof(Startup));

        //     services.AddScoped<IAuthRepository, AuthRepository>();  // Adding Interface and Repository
        //     services.AddScoped<IDatingRepository, DatingRepository>();  // AddScoped creates new instance per request

        //     services.AddTransient<Seed>();

        //     services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings")); // to bind Values in Cloudinarysettings.cs to Ones in appsettings.json

            
        //     services.AddMvc(option => option.EnableEndpointRouting = false)
        //         .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
        //         .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);



        //     // Middleware added for authentication
        //     services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
        //             ValidateIssuer = false,
        //             ValidateAudience = false
        //         };
        //     });

        //     services.AddScoped<LogUserActivity>();
        // }
        */


        // Mvc automatically runs services based on convention. eg. This function has Development as convention
        // To start development, do not use the environment_variable in terminal and change settings in launchSettings.json
        public void ConfigureServices(IServiceCollection services)
        {
            // AddDbContext is used to setup database connection.
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // AUTHENTICATION CONFIGURATION
            // Used for JWT token bearer system instead of cookie based auth
            // configuraion for user
            IdentityBuilder builder = services.AddIdentityCore<User>(opt => {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();    // use our DataContext for tables
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            // Middleware added for authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization(options => {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
                options.AddPolicy("VipOnly", policy => policy.RequireRole("VIP"));
            });

            services.AddControllers();
            services.AddCors(); // added to enable cors as service and add to pipeline
            services.AddAutoMapper(typeof(Startup));

            // Since the project now uses IdentityClasses and UserManager for login and signup
            // services.AddScoped<IAuthRepository, AuthRepository>();  // Adding Interface and Repository
            
            services.AddScoped<IDatingRepository, DatingRepository>();  // AddScoped creates new instance per request

            services.AddTransient<Seed>();

            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings")); // to bind Values in Cloudinarysettings.cs to Ones in appsettings.json

            
            services.AddMvc(option => {
                option.EnableEndpointRouting = false;

                // This defines Auth policies
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser() 
                    .Build();

                option.Filters.Add(new AuthorizeFilter(policy));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                // This acts as global exception Handler that throws error if any part of api throw error.
                app.UseDeveloperExceptionPage();
            }
            else 
            {
                // handling exceptions means using try catch block to show messages for production
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {  // context is http request and response
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });  // midlleware for global exception handling
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            // seeder is added as service and then used in Configure
            // seeder.SeedUsers();  // Uncomment and restart to seed the data
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();  // look for default files eg. index.html
            app.UseStaticFiles(); // looks inside wwwroot folder to serve those files
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
