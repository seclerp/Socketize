namespace Socketize.Core.Routing.Abstractions
{
    /// <summary>
    /// Abstraction for builder that creates some TResult.
    /// </summary>
    /// <typeparam name="TResult">Type of result that will be.</typeparam>
    public interface IBuilder<out TResult>
    {
        /// <summary>
        /// Creates resulting object of type TResult.
        /// </summary>
        /// <returns>Configured object of type TResult.</returns>
        TResult Build();
    }
}