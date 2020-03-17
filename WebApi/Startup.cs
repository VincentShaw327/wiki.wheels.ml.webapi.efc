using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text;
using AutoMapper;
using System.IO;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http;


using WebApi.DataAccess.Base;
using WebApi.DataAccess.Implement;
using WebApi.DataAccess.Interface;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Entities;
using WebApi.Services;
using WebApi.Authorize;

namespace WebApi
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
            services.AddAutoMapper();

            // DI
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            //注册数据库服务
            services.AddDbContext<SqlContext>(options => options.UseMySQL(Configuration.GetConnectionString("AlanConnection")));
            services.AddScoped<IUserService, UserService>();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);


            //configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(30), //Zero,
                RequireExpirationTime = true,

            };
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            //这个集合模拟用户权限表,可从数据库中查询出来
            var permission = new List<Permission> {
                              new Permission {  Url="/", Name="admin"},
                              new Permission {  Url="/api/values", Name="admin"},
                              new Permission {  Url="/api/topic", Name="admin"},
                              new Permission {  Url="/api/wiki", Name="admin"},
                              new Permission {  Url="/api/wiki/item", Name="admin"},
                              new Permission {  Url="/", Name="system"},
                              new Permission {  Url="/api/values1", Name="system"}
                          };
            //如果第三个参数，是ClaimTypes.Role，上面集合的每个元素的Name为角色名称，如果ClaimTypes.Name，即上面集合的每个元素的Name为用户名
            var permissionRequirement = new PermissionRequirement(
                "/api/denied", permission,
                ClaimTypes.Name,
                ClaimTypes.Expiration,
                audienceConfig["Issuer"],
                audienceConfig["Audience"],
                signingCredentials,
                expiration: TimeSpan.FromSeconds(30)
                );

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.LoginPath = new PathString("/login");
                   options.AccessDeniedPath = new PathString("/denied");
               }
             );

            services.AddAuthorization(options =>
            {

                options.AddPolicy("Permission",
                          policy => policy.Requirements.Add(permissionRequirement));

            }).AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    //不使用https
                    //x.RequireHttpsMetadata = true;
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            if (context.Request.Path.Value.ToString() == "/api/logout")
                            {
                                var token = ((context as TokenValidatedContext).SecurityToken as JwtSecurityToken).RawData;
                            }
                            return Task.CompletedTask;
                        }
                    };

                    //x.Events = new JwtBearerEvents
                    //{
                    //    OnTokenValidated = context =>
                    //    {
                    //        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                    //        var userId = context.Principal.Identity.Name;
                    //        var user = userService.GetById(userId);
                    //        if (user == null)
                    //        {
                    //            // return unauthorized if user no longer exists
                    //            context.Fail("Unauthorized");
                    //        }
                    //        return Task.CompletedTask;
                    //    }
                    //};
                    //x.TokenValidationParameters = new TokenValidationParameters
                    //{
                    //    ValidateIssuerSigningKey = true,
                    //    IssuerSigningKey = new SymmetricSecurityKey(key),
                    //    ValidateIssuer = false,
                    //    ValidateAudience = false
                    //};
                });

            services.AddIdentity<ApplicationUser, ApplicationRole>()  //ApplicationUser  IdentityRole  AccountRole
                .AddEntityFrameworkStores<SqlContext>()
                .AddDefaultTokenProviders();

            //services.AddIdentity<User, IdentityRole>()
            //    .AddEntityFrameworkStores<VueContext>()
            //    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login.
                options.LoginPath = "/api/Account/Login";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = "yourdomain.com",
            //            ValidAudience = "yourdomain.com"
            //        };
            //    });


            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            //注入授权Handler
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc(option => option.EnableEndpointRouting = true).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            // 启用跨域
            //services.AddCors();
            //services.AddCors(options =>
            //options.AddPolicy("MyDomain",
            //builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();//用于生成接口文档

            //app.UseIdentity();

            //app.Run(async (context) =>
            //{
            //    context.Response.ContentType = "text/html";
            //    //await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            //    await SendFileResponseExtensions.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            //});

            // set up whatever routes you use with UseMvc()// you may not need to set up any routes here// if you only use attribute routes!


            //handle client side routes
            //app.Run(async (context) => {
            //    context.Response.ContentType = "text/html";
            //    await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            //});


            //var rewrite = new RewriteOptions()
            //    .AddRedirect("films", "/").AddRedirect("/test/*", "/")
            //    .AddRewrite("actors", "stars", true);

            //app.UseRewriter(rewrite);

            //app.Run(async (context) =>
            //{
            //    var path = context.Request.Path;
            //    var query = context.Request.QueryString;
            //    await context.Response.WriteAsync($"New URL: {path}{query}");
            //});

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            // 配置跨域
            app.UseCors(builder =>
                   builder
                       .AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()

                    .WithOrigins("http://localhost:8888")
            );

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            //app.UseMvc();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
