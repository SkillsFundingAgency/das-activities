using System;
using System.Linq;
using System.Web.Mvc;
using HtmlTags;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Client
{
    public static class HtmlHelperExtensions
    {
        public static HtmlTag Activities(this HtmlHelper htmlHelper, ActivitiesResult result)
        {
            if (result.Activities.Any())
            {
                var date = DateTime.MinValue;
                var now = DateTime.UtcNow.ToGmtStandardTime();
                var ol = new HtmlTag("ol").AddClass("timeline").AddClass("timeline--complete");

                foreach (var activity in result.Activities)
                {
                    var at = activity.At.ToGmtStandardTime();
                    var url = htmlHelper.ActivityUrl(activity);

                    ol.Add("li", li => li
                        .AddClass(activity.At.Date != date ? "first" : "")
                            .Append("h4", h4 => h4
                                .Title(at.ToString("U"))
                                .AppendText(at.ToRelativeFormat(now)))
                            .Append("p", p => p
                                .AddClass("activity")
                                .Append(htmlHelper.Activity(activity)))
                            .Append("p", p =>
                            {
                                p.AddClass("meta")
                                    .AppendText("At")
                                    .AppendHtml(" ")
                                        .Append("time", time => time.AppendText(at.ToString("h:mm tt"))).AppendHtml(" ");

                                if (!string.IsNullOrEmpty(url))
                                {
                                    p.Append("a", a => a.Attr("href", url)
                                        .AppendText("See details"));
                                }
                            }));

                    date = activity.At.Date;
                }

                if (result.Activities.Count() != result.Total)
                {
                    ol.Add("li", li => li
                        .Append("a", a => a
                            .Attr("href", htmlHelper.ActivitiesUrl(result.Total))
                            .AppendText("See all activity")));
                }

                return ol;
            }

            return new HtmlTag("p").AppendText("You have no recent activity");
        }

        public static string ActivitiesUrl(this HtmlHelper htmlHelper, long? total = null)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
            var url = urlHelper.Action("Activity", "EmployerTeam", new { take = total });

            return url;
        }

        public static HtmlTag Activity(this HtmlHelper htmlHelper, Activity activity, long count = 1)
        {
            var localizer = activity.Type.GetLocalizer();
            var text = count > 1 ? localizer.GetPluralText(activity, count) : localizer.GetSingularText(activity);
            
            return HtmlTag.Placeholder().AppendText(text);
        }

        public static string ActivityUrl(this HtmlHelper htmlHelper, Activity activity, long count = 1)
        {
            var action = activity.Type.GetAction();

            if (action != null)
            {
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
                var url = urlHelper.Action(action.Item1, action.Item2);

                return url;
            }

            return null;
        }

        public static HtmlTag LatestActivities(this HtmlHelper htmlHelper, AggregatedActivitiesResult result)
        {
            if (result.Aggregates.Any())
            {
                var now = DateTime.UtcNow.ToGmtStandardTime();
                var ol = new HtmlTag("ol").Id("item-list");

                foreach (var aggregate in result.Aggregates)
                {
                    var at = aggregate.TopHit.At.ToGmtStandardTime();

                    ol.Add("li", li => li
                        .AddClass("item")
                            .Append("div", div => div
                                .AddClass("item-label")
                                .AppendText(at.ToRelativeFormat(now)))
                            .Append("div", div => div
                                .AddClass("item-description")
                                .Append(htmlHelper.Activity(aggregate.TopHit, aggregate.Count))));
                }
                
                ol.Add("li", li => li
                    .AddClass("item all-activity")
                    .Append("div", div => div
                        .AddClass("item-label")
                        .Append("a", a => a
                            .Attr("href", htmlHelper.ActivitiesUrl())
                            .AppendText("See all activity"))));

                return ol;
            }

            return new HtmlTag("p").AppendText("You have no recent activity");
        }
    }
}