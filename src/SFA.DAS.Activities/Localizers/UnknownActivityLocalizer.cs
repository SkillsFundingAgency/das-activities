using System;

namespace SFA.DAS.Activities.Localizers
{
    public class UnknownActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            throw new NotImplementedException();
        }

        public string GetSingularText(Activity activity)
        {
            throw new NotImplementedException();
        }
    }
}