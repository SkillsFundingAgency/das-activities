using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HtmlTags;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.UnitTests.Client
{
    public static class HtmlHelperExtensionsTests
    {
        public class When_getting_activities_html : WebTest
        {
            private HtmlHelper _htmlHelper;
            private HtmlTag _htmlTag;

            private readonly ActivitiesResult _result = new ActivitiesResult
            {
                Activities = new List<Activity>
                {
                    PayeSchemeCreated2,
                    PayeSchemeDeleted,
                    PayeSchemeCreated1,
                    AccountCreated
                },
                Total = 4
            };

            protected override void Given()
            {
                _htmlHelper = new HtmlHelper(ViewContext, ViewDataContainer, Routes);
            }

            protected override void When()
            {
                _htmlTag = _htmlHelper.Activities(_result);
            }

            [Test]
            public void Then_should_return_correct_html()
            {
                Assert.That(_htmlTag, Is.Not.Null);

                Assert.That(_htmlTag.ToString(), Is.EqualTo(
                    "<ol class=\"timeline timeline--complete\">" +
                        "<li class=\"first\">" +
                            "<h4 title=\"" + PayeSchemeCreated2.At.ToGmtStandardTime().ToString("U") + "\">Today</h4>" +
                            "<p class=\"activity\">" +
                                $"PAYE scheme {PayeSchemeCreated2.Data["PayeScheme"]} added by {PayeSchemeCreated2.Data["CreatorName"]}" +
                            "</p>" +
                            "<p class=\"meta\">" +
                                "At " +
                                "<time>" + PayeSchemeCreated2.At.ToGmtStandardTime().ToString("h:mm tt") + "</time> " +
                                "<a href=\"/EmployerAccountPaye\">See details</a>" +
                            "</p>" +
                        "</li>" +
                        "<li>" +
                            "<h4 title=\"" + PayeSchemeDeleted.At.ToGmtStandardTime().ToString("U") + "\">Today</h4>" +
                            "<p class=\"activity\">" +
                                $"PAYE scheme {PayeSchemeDeleted.Data["PayeScheme"]} removed by {PayeSchemeDeleted.Data["CreatorName"]}" +
                            "</p>" +
                            "<p class=\"meta\">" +
                                "At " +
                                "<time>" + PayeSchemeDeleted.At.ToGmtStandardTime().ToString("h:mm tt") + "</time> " +
                                "<a href=\"/EmployerAccountPaye\">See details</a>" +
                            "</p>" +
                        "</li>" +
                        "<li>" +
                            "<h4 title=\"" + PayeSchemeCreated1.At.ToGmtStandardTime().ToString("U") + "\">Today</h4>" +
                            "<p class=\"activity\">" +
                                $"PAYE scheme {PayeSchemeCreated1.Data["PayeScheme"]} added by {PayeSchemeCreated1.Data["CreatorName"]}" +
                            "</p>" +
                            "<p class=\"meta\">" + 
                                "At " + 
                                "<time>" + PayeSchemeCreated1.At.ToGmtStandardTime().ToString("h:mm tt") + "</time> " +
                                "<a href=\"/EmployerAccountPaye\">See details</a>" +
                            "</p>" +
                        "</li>" +
                        "<li class=\"first last\">" +
                            "<h4 title=\"" + AccountCreated.At.ToGmtStandardTime().ToString("U") + "\">Yesterday</h4>" +
                            "<p class=\"activity\">" +
                                $"Account created by {AccountCreated.Data["CreatorName"]}" +
                            "</p>" +
                            "<p class=\"meta\">" +
                                "At " +
                                "<time>" + AccountCreated.At.ToGmtStandardTime().ToString("h:mm tt") + "</time> " +
                            "</p>" +
                        "</li>" +
                    "</ol>"));
            }
        }

        public class When_getting_latest_activities_html : WebTest
        {
            private HtmlHelper _htmlHelper;
            private HtmlTag _htmlTag;

            private readonly AggregatedActivitiesResult _result = new AggregatedActivitiesResult
            {
                Aggregates = new List<AggregatedActivityResult>
                {
                    new AggregatedActivityResult
                    {
                        TopHit = PayeSchemeCreated2,
                        Count = 2
                    },
                    new AggregatedActivityResult
                    {
                        TopHit = PayeSchemeDeleted,
                        Count = 1
                    },
                    new AggregatedActivityResult
                    {
                        TopHit = AccountCreated,
                        Count = 1
                    },
                },
                Total = 4
            };

            protected override void Given()
            {
                _htmlHelper = new HtmlHelper(ViewContext, ViewDataContainer, Routes);
            }

            protected override void When()
            {
                _htmlTag = _htmlHelper.LatestActivities(_result);
            }

            [Test]
            public void Then_should_return_correct_html()
            {
                Assert.That(_htmlTag, Is.Not.Null);

                Assert.That(_htmlTag.ToString(), Is.EqualTo(
                    "<ol id=\"item-list\">" +
                        "<li class=\"item\">" +
                            "<div class=\"item-label\">Today</div>" +
                            "<div class=\"item-description\">" +
                                "2 PAYE schemes added" +
                            "</div>" +
                        "</li>" +
                        "<li class=\"item\">" +
                            "<div class=\"item-label\">Today</div>" +
                            "<div class=\"item-description\">" +
                                $"PAYE scheme {PayeSchemeDeleted.Data["PayeScheme"]} removed by {PayeSchemeDeleted.Data["CreatorName"]}" +
                            "</div>" +
                        "</li>" +
                        "<li class=\"item\">" +
                            "<div class=\"item-label\">Yesterday</div>" +
                            "<div class=\"item-description\">" +
                                $"Account created by {AccountCreated.Data["CreatorName"]}" +
                            "</div>" +
                        "</li>" +
                        "<li class=\"item all-activity\">" +
                            "<div class=\"item-label\">" +
                                "<a href=\"/EmployerTeam/Activity\">See all activity</a>" +
                            "</div>" +
                        "</li>" +
                    "</ol>"));
            }
        }

        private static readonly Activity AccountCreated = new Activity
        {
            AccountId = 5,
            At = DateTime.UtcNow.AddDays(-1),
            Created = DateTime.UtcNow,
            Data = new Dictionary<string, string>
            {
                ["CreatorName"] = "John Doe",
                ["CreatorUserRef"] = "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047"
            },
            Description = ActivityType.AccountCreated.GetDescription(),
            Type = ActivityType.AccountCreated
        };

        private static readonly Activity PayeSchemeCreated1 = new Activity
        {
            AccountId = 5,
            At = DateTime.UtcNow.AddMinutes(-2),
            Created = DateTime.UtcNow,
            Data = new Dictionary<string, string>
            {
                ["CreatorName"] = "John Doe",
                ["CreatorUserRef"] = "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047",
                ["PayeScheme"] = "333/AA00001"
            },
            Description = ActivityType.PayeSchemeAdded.GetDescription(),
            Type = ActivityType.PayeSchemeAdded
        };

        private static readonly Activity PayeSchemeDeleted = new Activity
        {
            AccountId = 5,
            At = DateTime.UtcNow.AddMinutes(-1),
            Created = DateTime.UtcNow,
            Data = new Dictionary<string, string>
            {
                ["CreatorName"] = "Jane Doe",
                ["CreatorUserRef"] = "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047",
                ["PayeScheme"] = "333/AA00001"
            },
            Description = ActivityType.PayeSchemeRemoved.GetDescription(),
            Type = ActivityType.PayeSchemeRemoved
        };

        private static readonly Activity PayeSchemeCreated2 = new Activity
        {
            AccountId = 5,
            At = DateTime.UtcNow,
            Created = DateTime.UtcNow,
            Data = new Dictionary<string, string>
            {
                ["CreatorName"] = "Jane Doe",
                ["CreatorUserRef"] = "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047",
                ["PayeScheme"] = "333/AA00002"
            },
            Description = ActivityType.PayeSchemeAdded.GetDescription(),
            Type = ActivityType.PayeSchemeAdded
        };
    }
}