using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace RPGWorkflow.Flows
{
    public class SimpleQuestFlow : IWorkflow<PlayerData>
    {
        public string Id => nameof(SimpleQuestFlow);
        public int Version => 1;

        public void Build(IWorkflowBuilder<PlayerData> builder)
        {
            builder
                .StartWith(ctx => ExecutionResult.Next())
                .If(data => data.ExperiencePoints == 0)
                .Do(then => then
                    .StartWith<IncreaseExperiencePoints>()
                    .Input(step => step.IncreaseBy, data => 100)
                    .Input(step => step.StartValue, data => data.ExperiencePoints)
                    .Output(data => data.ExperiencePoints, step => step.ResultValue));
        }
    }
}