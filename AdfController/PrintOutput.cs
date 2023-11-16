using AdfController.Interface;
using AdfController.Model;
using Microsoft.Azure.Management.DataFactory.Models;

namespace AdfController
{
    public class PrintOutput : IPrintOutput
    {
        readonly IConfigurationSections _config;
        public PrintOutput(IConfigurationSections config)
        {
            _config = config;
        }
        public void WelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine();
            Console.WriteLine("==== Run Azure data factory pipeline and get status of pipeline                ====");
            Console.WriteLine("==== Please make sure all configuration is updated in appsettings.json file    ====");
            Console.WriteLine("==== Please make choice                                                        ====");
            Console.WriteLine("==== 1 -> Run new pipeline, Need to enter pipeline name                        ====");
            Console.WriteLine("==== 2 -> Get pipeline status by run id                                        ====");
            PrintLine();
        }
        public void EndMessage()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine();
            Console.WriteLine("= Please enter any value to exit                                               ====");
            PrintLine();
            PrintLine();
        }
        public void IncorrectOptionMessage()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine("= Wrong options selected, Please select 1 or 2                                 ====");
            PrintLine();
        }
        public void ExistingPipelineMessage()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine();
            Console.WriteLine("= Please enter valid pipeline run id                                           ====");
            PrintLine();
        }
        public void AllPipelineName(IEnumerable<PipelineResource> pipelineList)
        {
            AzureOptions azureOptions = _config.GetAzureConfiguration();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine();
            Console.WriteLine("= Please enter valid pipeline name");
            Console.WriteLine("= Available pipeline in '" + azureOptions.DataFactoryName + "' factory ");

            foreach (var pipeline in pipelineList)
            {
                Console.WriteLine("= " + pipeline.Name);
            }

            PrintLine();
        }
        public void PipelineRunStatus(string pipelineRunId, PipelineRun run)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine();
            if (run.Status is not null)
            {
                Console.WriteLine("= Pipeline run status for run id:- " + pipelineRunId);
                Console.WriteLine("= Pipeline name:- " + run.PipelineName);
                Console.WriteLine("= Pipeline run start:- " + run.RunStart);
                Console.WriteLine("= Pipeline run end:- " + run.RunEnd);
                Console.WriteLine("= Pipeline status:- " + run.Status);
            }
        }
        public void NewPipelineRun(string runId)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine();
            Console.WriteLine("= Pipeline run id:- " + runId);
        }
        public void IncorrectPipelineName()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine();
            Console.WriteLine("= Please provide correct pipeline name");
        }
        private static void PrintLine()
        {
            Console.WriteLine("===================================================================================");
        }
    }
}