using AdfController.Interface;
using AdfController.Model;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace AdfController.ServiceCollections
{
    public class ServiceBuilder
    {
        private readonly IConfigurationSections _config;
        private readonly IServiceCollection _services;
        private readonly ILogger<ServiceBuilder> _logger;

        public ServiceBuilder(IConfigurationSections config, ILogger<ServiceBuilder> logger)
        {
            _config = config;
            _logger = logger;
            _services = new ServiceCollection();
        }
        public IServiceProvider ServiceProvider()
        {
            CreatePrintOutput();
            CreateLoggingService();
            CreateApplicationInsights();
            CreateDataFactoryClient();          
            return _services.BuildServiceProvider();
        }

        private void CreateApplicationInsights()
        {
            try
            {
                ApplicationInsights applicationInsights = _config.GetApplicationInsightsConfiguration();
                ApplicationInsightsServiceOptions applicationInsightsServiceOptions = new ApplicationInsightsServiceOptions() { ConnectionString = applicationInsights.ConnectionString };

                _services.AddApplicationInsightsTelemetryWorkerService(applicationInsightsServiceOptions);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "ServiceBuilder:CreateApplicationInsights");
                _logger.LogError("ServiceBuilder:CreateApplicationInsights: Error occured, Couldn't create application insights service");
            }
        }

        private void CreateLoggingService()
        {
            try
            {
                _services.AddLogging(loggingBuilder => loggingBuilder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("AdfController", Microsoft.Extensions.Logging.LogLevel.Information));
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "ServiceBuilder:CreateLoggingService");
                _logger.LogError("ServiceBuilder:CreateLoggingService: Error occured");
            }
        }

        private void CreateDataFactoryClient()
        {
            try
            {
                AzureOptions azureOptions = _config.GetAzureConfiguration();

                var context = new AuthenticationContext($"{azureOptions.ActiveDirectoryAuthority}/{azureOptions.TenantId}");
                var clientCredentials = new ClientCredential(azureOptions.ApplicationId, azureOptions.AuthenticationKey);
                var authResult = context.AcquireTokenAsync(azureOptions.ResourceManagerUrl, clientCredentials).Result;
                ServiceClientCredentials credentials = new TokenCredentials(authResult.AccessToken);

                _services.AddSingleton(new DataFactoryManagementClient(credentials)
                {
                    SubscriptionId = azureOptions.SubscriptionId
                });
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "ServiceBuilder:CreateDataFactoryClient");
                _logger.LogError("ServiceBuilder:CreateDataFactoryClient: Error occured, Couldn't create datafactory client service");
            }
        }

        private void CreatePrintOutput()
        {
            _services.AddSingleton(new PrintOutput(_config));
        }
    }
}