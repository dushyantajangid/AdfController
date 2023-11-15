using ADFServices;
using AdfController.Interface;
using AdfController.ServiceCollections;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AdfController.Configurations;

namespace AdfController
{
    internal class Program
    {
        static IServiceProvider _serviceProvider;
        static TelemetryClient _telemetryClient;
        static ILogger<ADFService> _logger;
        static DataFactoryManagementClient _dfClient;
        static IConfigurationSections _config;
        static IPrintOutput _printOutput;

        static async Task Main()
        {
            string pipelineName = string.Empty;
            string pipelineRunId = string.Empty;

            PrepareService();

            var adfService = new ADFService(_dfClient, _config, _telemetryClient, _logger);

            _printOutput.WelcomeMessage();

            string choice = Console.ReadLine();

            if (choice is not null)
            {
                switch (choice)
                {
                    case "1":
                        var pipelineList = await adfService.GetAllPipelineInFactoryAsync();

                        _printOutput.AllPipelineName(pipelineList);

                        pipelineName = Console.ReadLine();
                        var isPipelineExists = pipelineList.Select(a => a.Name.ToLower() == pipelineName.ToLower()).FirstOrDefault();

                        if (isPipelineExists)
                        {
                            var newRun = await adfService.RunPipelineAsync(pipelineName, new Dictionary<string, object>());
                            _printOutput.NewPipelineRun(newRun.RunId);
                        }
                        else
                            _printOutput.IncorrectPipelineName();
                        break;
                    case "2":
                        _printOutput.ExistingPipelineMessage();

                        pipelineRunId = Console.ReadLine();
                        var run = await adfService.GetPipelineStatusAsync(pipelineRunId);

                        _printOutput.PipelineRunStatus(pipelineRunId, run);
                        break;
                    default:
                        _printOutput.IncorrectOptionMessage();
                        break;
                }
            }

            _telemetryClient.Flush();
            _printOutput.EndMessage();
        }

        private static void PrepareService()
        {
            _config = new Configuration();

            _serviceProvider = new ServiceBuilder(_config).ServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<ADFService>>();
            _telemetryClient = _serviceProvider.GetRequiredService<TelemetryClient>();
            _dfClient = _serviceProvider.GetRequiredService<DataFactoryManagementClient>();
            _printOutput = _serviceProvider.GetRequiredService<PrintOutput>();
        }
    }
}