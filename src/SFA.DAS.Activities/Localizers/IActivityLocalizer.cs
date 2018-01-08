namespace SFA.DAS.Activities.Localizers
{
    public interface IActivityLocalizer
    {
        string GetPluralText(Activity activity, long count);
        string GetSingularText(Activity activity);
    }
}