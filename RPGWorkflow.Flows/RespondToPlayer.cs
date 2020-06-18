using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace RPGWorkflow.Flows
{
    public class RespondToPlayer : StepBody
    {
        private readonly ISendResponse _responder;
        
        public PlayerData ToSend { get; set; }

        public RespondToPlayer(ISendResponse responder)
        {
            _responder = responder;
        }
        
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _responder.Send(ToSend);
            return ExecutionResult.Next();
        }
    }
}
