﻿using System.Collections.Generic;

namespace SFA.DAS.Activities.Application.Configurations
{
    public interface IProvideSettings
    {
        IDictionary<string, object> Settings { get; }
    }
}