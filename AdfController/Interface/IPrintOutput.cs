using Microsoft.Azure.Management.DataFactory.Models;

namespace AdfController.Interface
{
    internal interface IPrintOutput
    {
        public void WelcomeMessage();
        public void EndMessage();
        public void IncorrectOptionMessage();
        public void ExistingPipelineMessage();
        public void AllPipelineName(IEnumerable<PipelineResource> pipelineList);
        public void PipelineRunStatus(string pipelineRunId, PipelineRun run);
        public void NewPipelineRun(string runId);
        public void IncorrectPipelineName();
    }
}