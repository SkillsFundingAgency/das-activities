namespace SFA.DAS.Acitivities.Core.Configuration
{
    public interface IOptions<T>
    {
        T Value { get; }
    }

    public class Options<T> : IOptions<T>
    {
        public T Value { get; set; }
    }
}