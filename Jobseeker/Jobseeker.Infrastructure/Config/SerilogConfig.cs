using Serilog;
using Serilog.Events;

namespace Jobseeker.Infrastructure.Config;

public static class SerilogConfig
{
    public static void ConfigureLogging() => Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
}
