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
using Microsoft.Net.Http.Headers;
using RestWithASPNETUdemy.Hypermedia.Filters;
using RestWithASPNETUdemy.Hypermedia.Enricher;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Rewrite;
using RestWithASPNETUdemy.Services;
using RestWithASPNETUdemy.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using RestWithASPNETUdemy.Services.Implementations;
using RestWithASPNETUdemy.Repository.Implementations;
using restwithaspnetudemy.repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;

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
            var tokenConfigurations = new TokenConfiguration();

            new ConfigureFromConfigurationOptions<TokenConfiguration>(
                Configuration.GetSection("TokenConfigurations")
                )
                .Configure(tokenConfigurations);

            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = tokenConfigurations.Issuer,
                     ValidAudience = tokenConfigurations.Audience,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
                 };
             });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });


            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

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

            //permitindo que a API suporte tanto JSON como XML para entrada e saída das informações.
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
            })
                .AddXmlSerializerFormatters();


            var filterOptions = new HyperMediaFilterOptions();
            filterOptions.ContentResponseEnricherList.Add(new PersonEnricher());
            filterOptions.ContentResponseEnricherList.Add(new BookEnricher());

            services.AddSingleton(filterOptions);


            //Versionamento da API
            services.AddApiVersioning();

            //Adicionando suporte ao Swagger
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "REST API's from 0 to Azure with ASP.NET Core 5 and Docker",
                        Version = "v1",
                        Description = "API RESTful developed in course 'REST API's from 0 to Azure with ASP.NET Core 5 and Docker'",
                        Contact = new OpenApiContact
                        {
                            Name = "Lenno Sousa",
                            Url = new Uri("https://google.com")
                        }
                    });
            });

            #region Dependency Injection

            //dependencia para capturar o path da requisição.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Adicionando injeção de dependência da nossa interface criada.
            services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
            //services.AddScoped<IPersonRepository, PersonRepositoryImplementation>();

            services.AddScoped<IBookBusiness, BookBusinessImplementation>();
            //services.AddScoped<IBookRepository, BookRepositoryImplementation>();

            services.AddScoped<ILoginBusiness, LoginBusinessImplementation>();


            services.AddScoped<IFileBusiness, FileBusinessImplementation>();



            services.AddTransient<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IPersonRepository, PersonRepositoryImplementation>();

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            #endregion

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

            app.UseCors();

            //responsável por gerar o json, com a documentação.
            app.UseSwagger();

            //responsável por gerar uma página html para acessar e visualizar a documentação.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "REST API's from 0 to Azure with ASP.NET Core 5 and Docker - v1");
            });

            //configurando a nossa swagger page
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("DefaultApi", "{controller=values}/{id?}");
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
