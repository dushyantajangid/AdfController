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

        public ServiceBuilder(IConfigurationSections config)
        {
            _config = config;
            _services = new ServiceCollection();
        }
        public IServiceProvider ServiceProvider()
        {
            CreateLoggingService();

            CreateApplicationInsights();

            CreateDataFactoryClient();

            CreatePrintOutput();

            // Build ServiceProvider.
            return _services.BuildServiceProvider();
        }

        private void CreateApplicationInsights()
        {
            ApplicationInsights applicationInsights = _config.GetApplicationInsightsConfiguration();
            ApplicationInsightsServiceOptions applicationInsightsServiceOptions = new ApplicationInsightsServiceOptions() { ConnectionString = applicationInsights.ConnectionString };

            _services.AddApplicationInsightsTelemetryWorkerService(applicationInsightsServiceOptions);
        }

        private void CreateLoggingService()
        {
            Logging logging = _config.GetLoggingConfiguration();
            _services.AddLogging(loggingBuilder => loggingBuilder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Category", Microsoft.Extensions.Logging.LogLevel.Information));
        }

        private void CreateDataFactoryClient()
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

        private void CreatePrintOutput()
        {
            _services.AddSingleton(new PrintOutput(_config));
        }
    }
}