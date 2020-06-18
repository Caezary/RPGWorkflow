using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WorkflowCore.Models;
using WorkflowCore.Testing;
using Xunit;

namespace RPGWorkflow.Flows.Tests
{
    public class SimpleQuestWithEventSpecs : WorkflowTest<SimpleQuestWithEventFlow, PlayerData>
    {
        private const string ExpectedIncomingEventName = "PlayerEntered";
        private const string ExpectedIncomingEventKey = "SimpleQuest";
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(500);
        private readonly Mock<ISendResponse> _responder;

        public SimpleQuestWithEventSpecs()
        {
            _responder = new Mock<ISendResponse>();
            Setup();
        }
        
        [Fact]
        public void FlowExecuted_PlayerHasNoXP_ReceivesXP()
        {
            var player = new PlayerData
            {
                Id = Guid.NewGuid(),
                ExperiencePoints = 0
            };
            var expectedXP = 100;
            var workflowId = StartWorkflow(new PlayerData());
            
            RunWorkflow(player, workflowId);

            VerifyWorkflowCompleted(workflowId);
            GetData(workflowId).ExperiencePoints.Should().Be(expectedXP);
            VerifyResponderCalledWithExpectedData(player, expectedXP);
        }

        [Fact]
        public void FlowExecuted_PlayerHasXP_DoesNotReceiveXP()
        {
            var expectedXP = 10;
            var player = new PlayerData
            {
                Id = Guid.NewGuid(),
                ExperiencePoints = expectedXP
            };
            var workflowId = StartWorkflow(new PlayerData());
            
            RunWorkflow(player, workflowId);

            VerifyWorkflowCompleted(workflowId);
            GetData(workflowId).ExperiencePoints.Should().Be(expectedXP);
            VerifyResponderCalledWithExpectedData(player, expectedXP);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => _responder.Object);
            services.AddTransient<RespondToPlayer>();
            base.ConfigureServices(services);
        }

        private void RunWorkflow(PlayerData player, string workflowId)
        {
            WaitForEventSubscription(ExpectedIncomingEventName, ExpectedIncomingEventKey, DefaultTimeout);

            Host.PublishEvent(ExpectedIncomingEventName, ExpectedIncomingEventKey, player);

            WaitForWorkflowToComplete(workflowId, DefaultTimeout);
        }

        private void VerifyWorkflowCompleted(string workflowId)
        {
            UnhandledStepErrors.Should().BeEmpty();
            GetStatus(workflowId).Should().Be(WorkflowStatus.Complete);
        }

        private void VerifyResponderCalledWithExpectedData(PlayerData player, int expectedXP)
        {
            _responder.Verify(r => r.Send(It.Is<PlayerData>(data => VerifyPlayerData(data, player, expectedXP))));
        }

        private static bool VerifyPlayerData(PlayerData data, PlayerData player, int expectedXP)
        {
            return data.Id == player.Id && data.ExperiencePoints == expectedXP;
        }
    }
}
