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

            using (_telemetryClient.StartOperation<RequestTelemetry>("ADFService_GetAllPipelineInFactoryAsync_operation"))
            {
                _telemetryClient.TrackEvent("ADFService:GetAllPipelineInFactoryAsync started");

                try
                {
                    _logger.LogDebug("Calling Pipelines.ListByFactoryAsync for resource group " + _azureOptions.ResourceGroup + " DataFactory " + _azureOptions.DataFactoryName);
                    pipelineList = await _dataFactoryManagementClient.Pipelines.ListByFactoryAsync(_azureOptions.ResourceGroup, _azureOptions.DataFactoryName);
                    _logger.LogDebug("Calling Pipelines.ListByFactoryAsync for resource group " + _azureOptions.ResourceGroup + " DataFactory " + _azureOptions.DataFactoryName + " Completed successfully");
                    return pipelineList.ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "ADFService:GetAllPipelineInFactoryAsync");
                    _logger.LogError("ADFService:GetAllPipelineInFactoryAsync: Error occured while getting all pipeline");
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
            using (_telemetryClient.StartOperation<RequestTelemetry>("ADFService_GetPipelineStatusAsync_operation"))
            {
                try
                {
                    _logger.LogDebug("Calling Pipelines.GetAsync for run id " + runId);
                    pipelineRun = await _dataFactoryManagementClient.PipelineRuns.GetAsync(_azureOptions.ResourceGroup, _azureOptions.DataFactoryName, runId);
                    _logger.LogDebug("Calling Pipelines.GetAsync for run id " + runId + " Completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "ADFService:GetPipelineStatusAsync");
                    _logger.LogError("ADFService:GetPipelineStatusAsync: Error occured while getting pipeline status");
                }
                finally
                {
                    _telemetryClient.TrackEvent("ADFService:GetPipelineStatusAsync completed");
                }

                return pipelineRun;
            }
        }

        public async Task<CreateRunResponse> RunPipelineAsync(string pipelineName, IDictionary<string, object> parameters)
        {
            CreateRunResponse runResponse = new CreateRunResponse();
            using (_telemetryClient.StartOperation<RequestTelemetry>("ADFService_RunPipelineAsync_operation"))
            {
                try
                {
                    _logger.LogDebug("Calling Pipelines.CreateRunAsync for resource group " + _azureOptions.ResourceGroup + " data factory name " + _azureOptions.DataFactoryName
                        + " pipeline " + pipelineName);
                    runResponse = await _dataFactoryManagementClient.Pipelines.CreateRunAsync(
                   _azureOptions.ResourceGroup,
                   _azureOptions.DataFactoryName,
                   pipelineName,
                   parameters: parameters);
                    _logger.LogDebug("Calling Pipelines.CreateRunAsync for resource group " + _azureOptions.ResourceGroup + " data factory name " + _azureOptions.DataFactoryName
                        + " pipeline " + pipelineName + " Completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "ADFService:RunPipelineAsync");
                    _logger.LogError(ex, "ADFService:RunPipelineAsync: Error occured while creating new pipeline run");
                }
                finally
                {
                    _telemetryClient.TrackEvent("ADFService:RunPipelineAsync completed");
                }
                return runResponse;
            }
        }
    }
}