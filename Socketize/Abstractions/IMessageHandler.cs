namespace Socketize.Abstractions
{
  public interface IMessageHandler<TMessage>
  {
    void Handle(Context context, TMessage message);
  }

  public interface IMessageHandler
  {
    void Handle(Context context);
  }
}