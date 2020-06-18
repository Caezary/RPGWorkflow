using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Primitives;

namespace RPGWorkflow.Flows
{
    public class SimpleQuestWithEventFlow : IWorkflow<PlayerData>
    {
        public string Id => nameof(SimpleQuestWithEventFlow);
        public int Version => 1;

        public void Build(IWorkflowBuilder<PlayerData> builder)
        {
            builder
                .StartWith(ctx => ExecutionResult.Next())
                .WaitFor("PlayerEntered", data => "SimpleQuest")
                    .Output(data => data.Id, step => GetPlayerData(step).Id)
                    .Output(data => data.ExperiencePoints, step => GetPlayerData(step).ExperiencePoints)
                .If(data => data.ExperiencePoints == 0)
                .Do(then => then
                    .StartWith<IncreaseExperiencePoints>()
                        .Input(step => step.IncreaseBy, data => 100)
                        .Input(step => step.StartValue, data => data.ExperiencePoints)
                        .Output(data => data.ExperiencePoints, step => step.ResultValue))
                .Then<RespondToPlayer>()
                    .Input(step => step.ToSend, data => data);
        }

        private static PlayerData GetPlayerData(WaitFor step)
        {
            return step.EventData as PlayerData;
        }
    }
}
