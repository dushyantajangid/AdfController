using AdfController.Interface;
using AdfController.Model;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Extensions.Logging;

namespace ADFServices
{
    public class ADFService : IADFService
    {
        private readonly AzureOptions _azureOptions;
        private readonly DataFactoryManagementClient _dataFactoryManagementClient;
        private readonly ILogger<ADFService> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly IConfigurationSections _config;

        public ADFService(DataFactoryManagementClient dataFactoryManagementClient, IConfigurationSections config, TelemetryClient telemetryClient, ILogger<ADFService> logger)
        {
            _dataFactoryManagementClient = dataFactoryManagementClient;
            _config = config;
            _telemetryClient = telemetryClient;
            _logger = logger;
            _azureOptions = _config.GetAzureConfiguration();
        }

        public async Task<IEnumerable<PipelineResource>> GetAllPipelineInFactoryAsync()
        {
            IEnumerable<PipelineResource> pipelineList;

            using (_telemetryClient.StartOperation<RequestTelemetry>("ADFService_operation"))
            {
                _telemetryClient.TrackEvent("ADFService:GetAllPipelineInFactoryAsync started");

                try
                {
                    _logger.LogDebug("Calling Pipelines.ListByFactoryAsync");
                    pipelineList = await _dataFactoryManagementClient.Pipelines.ListByFactoryAsync(_azureOptions.ResourceGroup, _azureOptions.DataFactoryName);
                    return pipelineList.ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ADFService:GetAllPipelineInFactoryAsync: Error occured");
                    return new List<PipelineResource>();
                }
                finally
                {
                    _telemetryClient.TrackEvent("ADFService:GetAllPipelineInFactoryAsync completed");
                }
            }
        }

        public async Task<PipelineRun> GetPipelineStatusAsync(string runId, CancellationToken cancellationToken = default)
        {
            PipelineRun pipelineRun = new PipelineRun();
            try
            {
                _logger.LogDebug("Calling Pipelines.GetAsync");
                pipelineRun = await _dataFactoryManagementClient.PipelineRuns.GetAsync(_azureOptions.ResourceGroup, _azureOptions.DataFactoryName, runId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ADFService:GetPipelineStatusAsync: Error occured");
            }

            return pipelineRun;
        }

        public async Task<CreateRunResponse> RunPipelineAsync(string pipelineName, IDictionary<string, object> parameters)
        {
            CreateRunResponse runResponse = new CreateRunResponse();
            try
            {
                _logger.LogDebug("Calling Pipelines.CreateRunAsync");
                runResponse = await _dataFactoryManagementClient.Pipelines.CreateRunAsync(
               _azureOptions.ResourceGroup,
               _azureOptions.DataFactoryName,
               pipelineName,
               parameters: parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ADFService:RunPipelineAsync: Error occured");
            }
            return runResponse;
        }
    }
}