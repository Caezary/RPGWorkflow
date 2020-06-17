using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace RPGWorkflow.Flows
{
    public class IncreaseExperiencePoints : StepBody
    {
        public int IncreaseBy { get; set; }
        public int StartValue { get; set; }
        public int ResultValue { get; set; }
        
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            ResultValue = StartValue + IncreaseBy;
            return ExecutionResult.Next();
        }
    }
}
