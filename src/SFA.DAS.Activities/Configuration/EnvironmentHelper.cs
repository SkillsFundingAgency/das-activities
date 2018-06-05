using System;
using System.Linq;
using Microsoft.Azure;

namespace SFA.DAS.Activities.Configuration
{
    public static class EnvironmentHelper
    {
        public static string GetEnvironmentName()
        {
            var environmentName = System.Environment.GetEnvironmentVariable("DASENV");

            if (String.IsNullOrEmpty(environmentName))
            {
                environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            return environmentName;
        }

        public static DeployedEnvironment CurrentDeployedEnvironment
        {
            get
            {
                switch (CurrentEnvironmentName)
                {
                    case "LOCAL": return DeployedEnvironment.Local;
                    case "AT": return DeployedEnvironment.AT;
                    case "TEST": return DeployedEnvironment.Test;
                    case "PROD": return DeployedEnvironment.Prod;
                    case "DEMO": return DeployedEnvironment.Demo;
                    default: return DeployedEnvironment.Unknown;
                }
            }
        }

        public static string CurrentEnvironmentName
        {
            get
            {
                var environmentName = System.Environment.GetEnvironmentVariable("DASENV");

                if (String.IsNullOrEmpty(environmentName))
                {
                    environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
                }

                return environmentName.ToUpperInvariant();
            }
        }

        public static bool IsAnyOf(params DeployedEnvironment[] deployedEnvironment)
        {
            return deployedEnvironment.Contains(CurrentDeployedEnvironment);
        }
    }
}