namespace RPGWorkflow.Flows
{
    public interface ISendResponse
    {
        void Send<TMessage>(TMessage message);
    }
}
