using System;

namespace SFA.DAS.Activities.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ActionAttribute : Attribute
    {
        public string Action { get; }
        public string Controller { get; }

        public ActionAttribute(string action, string controller)
        {
            Action = action;
            Controller = controller;
        }
    }
}