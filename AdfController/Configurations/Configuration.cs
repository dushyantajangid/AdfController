using AdfController.Interface;
using AdfController.Model;
using Microsoft.Extensions.Configuration;

namespace AdfController.Configurations
{
    public class Configuration : IConfigurationSections
    {
        IConfiguration _configuration;

        public Configuration()
        {
            _configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile($"Configurations/appsettings.json", optional: true, reloadOnChange: true)
              .Build();
        }
        public ApplicationInsights GetApplicationInsightsConfiguration()
        {
            var applicationInsightsConfig = _configuration.GetSection("ApplicationInsights");

            return new ApplicationInsights()
            {
                ConnectionString = applicationInsightsConfig.GetValue<string>("ConnectionString")
            };
        }
        public AzureOptions GetAzureConfiguration()
        {
            var azureOptionsConfig = _configuration.GetSection("AzureOptions");

            return new AzureOptions()
            {
                ResourceGroup = azureOptionsConfig.GetValue<string>("ResourceGroup"),
                DataFactoryName = azureOptionsConfig.GetValue<string>("DataFactoryName"),
                TenantId = azureOptionsConfig.GetValue<string>("TenantId"),
                ApplicationId = azureOptionsConfig.GetValue<string>("ApplicationId"),
                AuthenticationKey = azureOptionsConfig.GetValue<string>("AuthenticationKey"),
                SubscriptionId = azureOptionsConfig.GetValue<string>("SubscriptionId"),
                ActiveDirectoryAuthority = azureOptionsConfig.GetValue<string>("ActiveDirectoryAuthority"),
                ResourceManagerUrl = azureOptionsConfig.GetValue<string>("ResourceManagerUrl"),
            };
        }
        public IConfigurationSection GetConfigurationSection(string sectionName)
        {
            return _configuration.GetSection(sectionName);
        }
    }
}