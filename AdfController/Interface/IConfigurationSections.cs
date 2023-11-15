using AdfController.Model;

namespace AdfController.Interface
{
    public interface IConfigurationSections
    {
        public ApplicationInsights GetApplicationInsightsConfiguration();
        public Logging GetLoggingConfiguration();
        public AzureOptions GetAzureConfiguration();
    }
}
