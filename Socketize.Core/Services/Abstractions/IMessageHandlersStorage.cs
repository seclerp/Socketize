namespace Socketize.Core.Services
{
    public interface IMessageHandlersStorage
    {
        void Invoke(string route, ConnectionContext context, byte[] dtoRaw);

        bool HasRoute(string route);
    }
}