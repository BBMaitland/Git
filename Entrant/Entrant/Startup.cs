namespace Entrant
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Dals;
    using log4net.Config;
    using Models;
    using System.Collections.Concurrent;

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
            services.AddControllers();

            var entrants = new ConcurrentDictionary<int, Entrant>
            {
                [1] = new Entrant {Id = 1, FirstName = "First1", LastName = "Last1"},
                [2] = new Entrant {Id = 2, FirstName = "First2", LastName = "Last2"},
            };

            services.AddSingleton<IEntrantDal>(new EntrantDal(entrants));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var repository = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            XmlConfigurator.Configure(repository, new FileInfo(Path.Combine(basePath, "log4net.xml")));


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
