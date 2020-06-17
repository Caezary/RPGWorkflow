using System;
using FluentAssertions;
using WorkflowCore.Models;
using WorkflowCore.Testing;
using Xunit;

namespace RPGWorkflow.Flows.Tests
{
    public class SimpleQuestSpecs : WorkflowTest<SimpleQuestFlow, PlayerData>
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(10);

        public SimpleQuestSpecs()
        {
            Setup();
        }
        
        [Fact]
        public void FlowExecuted_PlayerHasNoXP_ReceivesXP()
        {
            var player = new PlayerData();
            var workflowId = StartWorkflow(player);
            
            WaitForWorkflowToComplete(workflowId, DefaultTimeout);

            GetStatus(workflowId).Should().Be(WorkflowStatus.Complete);
            GetData(workflowId).ExperiencePoints.Should().Be(100);
        }
        
        [Fact]
        public void FlowExecuted_PlayerHasXP_DoesNotReceiveXP()
        {
            var player = new PlayerData {ExperiencePoints = 10};
            var workflowId = StartWorkflow(player);
            
            WaitForWorkflowToComplete(workflowId, DefaultTimeout);

            GetStatus(workflowId).Should().Be(WorkflowStatus.Complete);
            GetData(workflowId).ExperiencePoints.Should().Be(10);
        }
    }
}
