using Microsoft.Azure.Management.DataFactory.Models;

namespace AdfController.Interface
{
    public interface IADFService
    {
        Task<CreateRunResponse> RunPipelineAsync(string pipelineName, IDictionary<string, object> parameters);
        Task<PipelineRun> GetPipelineStatusAsync(string runId, CancellationToken cancellationToken);
    }
}