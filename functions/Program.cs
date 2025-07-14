using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Azure.Functions.Worker.ApplicationInsights;
 using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;

namespace Kadense.RPG;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args)
        .ConfigureFunctionsWebApplication()
        .Build();
        
        builder.Run();
    }
}