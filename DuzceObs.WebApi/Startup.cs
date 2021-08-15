using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using DuzceObs.Core.Model.Entities;
using DuzceObs.Core.Services.Interfaces;
using DuzceObs.Infrastructure.Data;
using DuzceObs.Infrastructure.Services;
using DuzceObs.WebApi.Helpers;
using DuzceObs.WebApi.Services.DataServices;
using DuzceObs.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuzceObs.WebApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "DuzceObs Swagger Api",
                        Description = "DuzceObs Demo Swagger",
                        Version = "v1"
                    });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 5;
            }).AddEntityFrameworkStores<DuzceObsDbContext>().AddSignInManager<SignInManager<User>>();
            services.Configure<IdentityOptions>(
                options =>
                {
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                }
                );
            services.TryAddScoped<UserManager<User>>();
            services.TryAddScoped<SignInManager<User>>();
            services.AddDbContext<DuzceObsDbContext>(options =>
                options.UseSqlServer("***"));
            
            services.TryAddSingleton<ISystemClock, SystemClock>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret ilker key")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            var builder = new ContainerBuilder();
            builder.RegisterType<AuthHelper>().As<IAuthHelper>();
            builder.RegisterType<NotlarService>().As<INotlarService>();
            builder.RegisterType<DersService>().As<IDersService>();
            builder.RegisterType<DersDegerlendirmeService>().As<IDersDegerlendirmeService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<StudentService>().As<IStudentService>();
            builder.RegisterType<DuzceObsDataService>().As<IDuzceObsDataService>();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
            builder.Populate(services);

            var appContainer = builder.Build();

            return new AutofacServiceProvider(appContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Demo Api");
            });

        }
    }
}
