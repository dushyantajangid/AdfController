using AdfController.Model;
using Microsoft.Extensions.Configuration;

namespace AdfController.Interface
{
    public interface IConfigurationSections
    {
        public ApplicationInsights GetApplicationInsightsConfiguration();
        public AzureOptions GetAzureConfiguration();
        public IConfigurationSection GetConfigurationSection(string sectionName);
    }
}
