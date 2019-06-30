namespace Socketize
{
  public abstract class Options
  {
    public string AppId { get; }

    protected Options(string appId)
    {
      AppId = appId;
    }
  }
}