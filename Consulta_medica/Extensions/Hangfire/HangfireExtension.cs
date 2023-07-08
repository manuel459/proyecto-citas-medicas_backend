using Consulta_medica.Interfaces;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Consulta_medica.Extensions.Hangfire
{
    public static class HangfireExtension
    {
        public static IServiceProvider HangfireExecuteJob(this IServiceProvider serviceProvider, IRecurringJobManager recurringJobManager) 
        {
            var CitasService = serviceProvider.GetService<ICitasMedicasRepository>();

            recurringJobManager.AddOrUpdate("Noti_Recordatorio_Cita", () => CitasService.RecordatorioNotification(), Cron.Daily(6));

            return serviceProvider;
        }
    }
}
