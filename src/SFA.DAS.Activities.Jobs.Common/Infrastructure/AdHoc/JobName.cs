namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    internal class JobName
    {
        public JobName(string jobName)
        {
            var jobNameParts = jobName.Split(new char[] { '.' });

            if (jobNameParts.Length == 2)
            {
                ClassName = jobNameParts[0];
                MethodName = jobNameParts[1];
                FullName = string.Concat(ClassName, ".", MethodName);
            }
            else
            {
                FullName = jobName;
            }
        }

        public string ClassName { get; }
        public string MethodName { get; }
        public string FullName { get; }
        public bool HasClassName => !string.IsNullOrWhiteSpace(ClassName);
        public bool HasMethodName => !string.IsNullOrWhiteSpace(MethodName);
    }
}