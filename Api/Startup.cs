using System.IO;
using System.Text;
using Api.Hubs;
using Api.MIddlewares;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmsGateway;
using SmsGateway.Models;

namespace Api
{
    public class Startup
    {
        private IConfiguration configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            this.configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.configuration);
            services.AddCors(opts =>
            {
                opts.AddPolicy(
                    "allowAll",
                    builder => builder
                        .WithOrigins("*", "http://localhost:4200")
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddCookie()
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateActor = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration["Authentication:jwt:issuer"],
                            ValidAudience = configuration["Authentication:jwt:audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:jwt:key"]))
                        };
                    });
            services.AddDataProtection().UseCryptographicAlgorithms(new Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.EncryptionAlgorithm.AES_256_GCM
            });
            //services.AddDataProtection()
            //        .UseCryptographicAlgorithms(new AuthenticatedEncryptionSettings());

            services.AddDbContext<bcareContext>(options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<SMSServerContext>(options => options.UseSqlServer(this.configuration.GetConnectionString("SmsServerConnection")));

            services.AddSingleton<SmsSender>(ctx => 
            {
                //var smsSender = default(SmsSender);

                //using (var scope = ctx.CreateScope())
                //{
                //    smsSender = new SmsSender(scope.ServiceProvider.GetService<SMSServerContext>());
                //}

                var dbOptions = new DbContextOptionsBuilder<SMSServerContext>();
                    dbOptions.UseSqlServer(this.configuration.GetConnectionString("SmsServerConnection"));

                return new SmsSender(new SMSServerContext(dbOptions.Options));
            });

            services
                .AddMvc()
                .AddJsonOptions(opt => 
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            //services
            //    .AddCors(o => o.AddPolicy("CorsPolicy", builder => 
            //    {
            //        builder.AllowAnyMethod();
            //        builder.AllowAnyHeader();
            //        builder.AllowAnyOrigin();
            //    }));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication().UseClaimsValidationMiddleware();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseCors("allowAll");
            app.UseCors(builder => {
                builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });
            app.UseMvc();
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/api/hubs/notification");
            });
        } 
    }
}