using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VisitorLog.Server.Models;

namespace VisitorLog.Server
{
  public class Startup
  {
    
    public Startup(IConfiguration configuration)
    {
      IoCContainer.Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors();
      services.AddControllers();
      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseSqlServer(IoCContainer.Configuration.GetConnectionString("Default"));
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors(
        options => options
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()
       //.AllowCredentials()
       );

      app.UseStaticFiles();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGet("/", async context =>
        {
          await context.Response.WriteAsync("Hello World!");
        });

        endpoints.MapControllers();

      });
    }
  }
}
