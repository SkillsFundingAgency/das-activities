// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using SFA.DAS.Activities.Application.Configurations;

namespace SFA.DAS.Activities.WebJob.DependencyResolution {
    using StructureMap;
	
    public static class IoC {
        public static IContainer Initialize() {
            return new Container(c =>
            {
                    //c.Policies.Add(new SFA.DAS.Activities.WebJob.Configuration.Policies.ConfigurationPolicy<ActivitiesConfiguration>(ConfigName));
                    //c.Policies.Add(new SFA.DAS.Activities.Infrastructure.DependencyResolution.Configuration.Policies.MessagePublisherPolicy<ActivitiesConfiguration>(ConfigName));
                    //c.Policies.Add(new MessageSubscriberPolicy());
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<MediatrRegistry>();
                c.AddRegistry<InfrastructureRegistry>();
            });
        }
    }
}