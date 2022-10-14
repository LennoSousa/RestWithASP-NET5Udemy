using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestWithASPNETUdemy.Model.Context;
using RestWithASPNETUdemy.Business.Implementations;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Business;
using Serilog;
using System;
using System.Collections.Generic;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using RestWithASPNETUdemy.Repository.Generic;

namespace RestWithASPNETUdemy
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddControllers();

            var connection = Configuration["MySQLConnection:MySQLConnectionString"];
            
            //Essa versão do curso, não está funcionando.
            //services.AddDbContext<MySQLContext>(options => options.UseMySql(connection));
            
            //precisou adicionar o trecho final do código para poder estabelecer conexão com o banco.
            services.AddDbContext<MySQLContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)));


            MigrationDatabase(connection);
            if (Environment.IsDevelopment())
            {
                MigrationDatabase(connection);
            }


            //Versionamento da API
            services.AddApiVersioning();


            //Adicionando injeção de dependência da nossa interface criada.
            services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
            //services.AddScoped<IPersonRepository, PersonRepositoryImplementation>();

            services.AddScoped<IBookBusiness, BookBusinessImplementation>();
            //services.AddScoped<IBookRepository, BookRepositoryImplementation>();

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        private void MigrationDatabase(string connection)
        {
            try
            {
                var evolveConnection = new MySql.Data.MySqlClient.MySqlConnection(connection);
                var evolve = new Evolve.Evolve(evolveConnection, msg => Log.Information(msg))
                {
                    Locations = new List<string> { "db/migrations", "db/dataset" },
                    IsEraseDisabled = true,
                };
                evolve.Repair();
                evolve.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error("Database migration failed", ex);
                throw;
            }
        }
    }
}
