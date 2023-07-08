using Consulta_medica.Interfaces;
using Consulta_medica.Models;
using Consulta_medica.Repository;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Consulta_medica.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Consulta_medica.RealTemp;
using Hangfire;
using Consulta_medica.Extensions.Hangfire;
using Consulta_medica.Extensions;

namespace Consulta_medica
{
    public class Startup
    {
        private readonly string _MyCors = "MyCors";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
          
            services.AddDbContext<consulta_medicaContext>(options =>
           options.UseSqlServer(Configuration.GetConnectionString("consulta_medica"))
           );

            services.AddCors(options =>
            {
                options.AddPolicy(name: _MyCors, builder =>
                {
                    builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:44301");
                });
            });

            //Agregar serivico signalR
            services.ConfigureHangFire(Configuration);
            services.AddSignalR().AddMessagePackProtocol();

            //Configuracion del servicio para definir la inyeccion
            services.AddTransient<IMedicosRepository, MedicosRepository>();
            services.AddTransient<IPacienteRepository, PacienteRepository>();
            services.AddTransient<ICitasMedicasRepository, CitasMedicasRepository>();
            services.AddTransient<IDiagnosticoRepository, DiagnosticoRepository>();
            services.AddTransient<ICitasMedicasReporteRepository, CitasMedicasReporteRepository>();
            services.AddTransient<IPagosRepository, PagosRepository>();
            services.AddTransient<IConfiguracionesRepository, ConfiguracionesRepository>();
            services.AddTransient<PermisosRepository>();

            services.AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic));
            });

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            //JWT JSON WEB TOKEN

            var appSettings = appSettingsSection.Get<AppSettings>();
            var llave = Encoding.ASCII.GetBytes(appSettings.Secreto);
            services.AddAuthentication(d =>
            {
                d.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                d.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

                .AddJwtBearer(d =>
                {
                    d.RequireHttpsMetadata = false;
                    d.SaveToken = true;
                    d.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(llave),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.AddScoped<IUserServiceRepository, UserServiceRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {

            //Dashboard view progress job
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "Sample Jobs",
                Authorization = new[]
                {
                    new  HangfireAuthorizationFilter("admin")
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_MyCors);

            app.UseAuthentication();

            app.UseAuthorization();

            //Extensión para ejecutar jobs
            serviceProvider.HangfireExecuteJob(recurringJobManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<HubAlert>("/PacienteHub");

            });

            


            //app.UseEndpoints(endpoints =>
            //{

            //    endpoints.MapHub<Paciente>("/PacienteHub");
            //});

        }
    }
}
