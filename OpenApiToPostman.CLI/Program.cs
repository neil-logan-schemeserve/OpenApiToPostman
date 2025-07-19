using OpenApiToPostman.Core.Managers;
using OpenApiToPostman.Core.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace OpenApiToPostman.CLI
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            var manager = host.Services.GetRequiredService<OpenApiToPostmanManager>();

            if (args.Length != 2)
            {
                Console.Error.WriteLine($"Error: please provide a directory path");
                return 1;
            }

            string inputDirectoryPath = args[0];
            string outputDirectoryPath = args[1];

            try
            {
                await manager.ConvertSelectedFileAsync(inputDirectoryPath, outputDirectoryPath);
                return 0; // success
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1; // failure
            }
            finally
            {
                await host.StopAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddCoreServices();
                });
    }
}