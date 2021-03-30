using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Symbiose.Mail.Config;
using Symbiose.Mail.Data;
using Symbiose.Mail.Email_Delivery;
using Symbiose.Mail.Models;
using Symbiose.Mail.Repositories;
using Symbiose.Mail.Services;

namespace Symbiose.Mail
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
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Symbiose.Mail", Version = "v1" });
            });

            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpClient<IMailgunService, MailgunService>();
            services.AddHttpClient<ISendgridService, SendgridService>();
            services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(Configuration.GetValue<string>("DatabaseName")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Symbiose.Mail v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
