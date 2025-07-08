using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Htsoft.Uttt.Edi.Infraestructure.Loging
{

        public static class NLogExtensions
        {
            public static ILoggingBuilder AddNLogConfiguration(this ILoggingBuilder loggingBuilder, string configFile = "nlog.config")
            {
                return loggingBuilder.ClearProviders()
                                     .SetMinimumLevel(LogLevel.Information)
                                     .AddNLog(new NLogProviderOptions
                                     {
                                         CaptureMessageTemplates = true,
                                         CaptureMessageProperties = true
                                     });
            }
        }
    
}
