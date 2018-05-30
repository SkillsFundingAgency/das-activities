namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    public class ZipperItem<T>
    {
        public ZipperItem(T item, bool isInA, bool isInB)
        {
            Item = item;
            IsInA = isInA;
            IsInB = isInB;
        }

        public T Item { get; }
        public bool IsInA { get; }
        public bool IsInB { get; }
        public bool IsMissing => !IsInA || !IsInB;
        public bool IsMissingInA => !IsInA;
        public bool IsMissingInB => !IsInB;
    }
}