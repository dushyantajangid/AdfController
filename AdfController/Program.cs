using ADFServices;
using AdfController.Interface;
using AdfController.ServiceCollections;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AdfController.Configurations;
using Microsoft.Azure.Management.DataFactory.Models;

namespace AdfController
{
    internal class Program
    {
        static IServiceProvider _serviceProvider;
        static ILoggerFactory _loggerFactory;
        static ILogger<Program> _programLogger;
        static IConfigurationSections _config;
        static DataFactoryManagementClient _dfClient;
        static TelemetryClient _telemetryClient;
        static IPrintOutput _printOutput;
        static ADFService _adfService;
        static async Task Main()
        {
            string pipelineName = string.Empty;
            string pipelineRunId = string.Empty;

            StartupInitialization();
            _programLogger.LogInformation("Program:Main Started");

            ServiceInitialization();

            if (_adfService is not null)
            {
                _printOutput?.WelcomeMessage();

                string choice = Console.ReadLine().Trim();

                if (choice is not null)
                {
                    switch (choice)
                    {
                        case "1":
                            var pipelineList = await _adfService.GetAllPipelineInFactoryAsync();

                            _printOutput.AllPipelineName(pipelineList);

                            pipelineName = Console.ReadLine().Trim();

                            _programLogger.LogDebug("Program: Main User provided " + pipelineName + " pipeline name");

                            if (isPipelineExist(pipelineName, pipelineList))
                            {
                                _programLogger.LogDebug("Program: Main Running " + pipelineName + " pipeline");

                                var newRun = await _adfService.RunPipelineAsync(pipelineName, new Dictionary<string, object>());

                                _programLogger.LogDebug("Program: Main Pipeline run id " + newRun.RunId);
                                _printOutput.NewPipelineRun(newRun.RunId);
                            }
                            else
                            {
                                _programLogger.LogDebug("Program: Main Incorrect pipeline name provided " + pipelineName);
                                _printOutput.IncorrectPipelineName();
                            }
                            break;
                        case "2":
                            _printOutput.ExistingPipelineMessage();

                            pipelineRunId = Console.ReadLine().Trim();

                            if (pipelineRunId != string.Empty)
                            {
                                _programLogger.LogDebug("Program: Main Getting pipeline run status for run id " + pipelineRunId);
                                var run = await _adfService.GetPipelineStatusAsync(pipelineRunId);

                                _printOutput.PipelineRunStatus(pipelineRunId, run);
                            }
                            else
                                _printOutput.ExistingPipelineMessage();

                            break;
                        default:
                            _printOutput?.IncorrectOptionMessage();
                            break;
                    }
                }
            }
            _printOutput?.EndMessage();
            _telemetryClient?.Flush();
            _programLogger.LogInformation("Program:Main Completed");
        }

        private static bool isPipelineExist(string pipelineName, IEnumerable<PipelineResource> pipelineList)
        {
            return pipelineList.Select(a => a.Name.ToLower() == pipelineName.ToLower()).FirstOrDefault();
        }

        private static void ServiceInitialization()
        {
            ILogger<ServiceBuilder> serviceBuilderLogger;
            ILogger<ADFService> adfLogger;

            try
            {
                _programLogger.LogInformation("Program:ServiceInitialization Started");

                serviceBuilderLogger = _loggerFactory.CreateLogger<ServiceBuilder>();
                adfLogger = _loggerFactory.CreateLogger<ADFService>();

                _serviceProvider = new ServiceBuilder(_config, serviceBuilderLogger).ServiceProvider();
                _printOutput = _serviceProvider.GetRequiredService<PrintOutput>();
                _telemetryClient = _serviceProvider.GetRequiredService<TelemetryClient>();
                _dfClient = _serviceProvider.GetRequiredService<DataFactoryManagementClient>();

                _adfService = new ADFService(_dfClient, _config, _telemetryClient, adfLogger);
            }
            catch (Exception ex)
            {
                _programLogger.LogDebug(ex, "Program:ServiceInitialization Couldn't initiate services");
                _programLogger.LogCritical("Program:ServiceInitialization Couldn't initiate services");
            }
            _programLogger.LogInformation("Program:ServiceInitialization Completed");
        }
        private static void StartupInitialization()
        {
            _config = new Configuration();
            _loggerFactory = new LoggingBuilder(_config).BuildLogger();

            _programLogger = _loggerFactory.CreateLogger<Program>();
        }
    }
}