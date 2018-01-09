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

                    ol.Add("li", li => li
                        .AddClass(activity.At.Date != date ? "first" : "")
                            .Append("h4", h4 => h4
                                .Title(at.ToString("U"))
                                .AppendText(at.ToRelativeFormat(now)))
                            .Append("p", p => p
                                .AddClass("activity")
                                .AppendText(htmlHelper.Activity(activity)))
                            .Append("p", p =>
                            {
                                p.AddClass("meta")
                                    .AppendText("At")
                                    .AppendHtml(" ")
                                        .Append("time", time => time.AppendText(at.ToString("h:mm tt"))).AppendHtml(" ");

                                var url = htmlHelper.GetActivityUrl(activity);

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
                            .Attr("href", htmlHelper.GetActivitiesUrl(result.Total))
                            .AppendText("See all activity")));
                }

                return ol;
            }

            return new HtmlTag("p").AppendText("You have no recent activity");
        }

        public static string Activity(this HtmlHelper htmlHelper, Activity activity, long count = 1)
        {
            var localizer = activity.Type.GetLocalizer();
            var text = count > 1 ? localizer.GetPluralText(activity, count) : localizer.GetSingularText(activity);
            
            return text;
        }

        public static string GetActivitiesUrl(this HtmlHelper htmlHelper, long? total = null)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
            var url = urlHelper.Action("Activity", "EmployerTeam", new { take = total });

            return url;
        }

        public static string GetActivityUrl(this HtmlHelper htmlHelper, Activity activity)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
            var action = activity.Type.GetAction();
            var url = action != null ? urlHelper.Action(action.Item1, action.Item2) : null;

            return url;
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
                                .AppendText(htmlHelper.Activity(aggregate.TopHit, aggregate.Count))));
                }
                
                ol.Add("li", li => li
                    .AddClass("item all-activity")
                    .Append("div", div => div
                        .AddClass("item-label")
                        .Append("a", a => a
                            .Attr("href", htmlHelper.GetActivitiesUrl())
                            .AppendText("See all activity"))));

                return ol;
            }

            return new HtmlTag("p").AppendText("You have no recent activity");
        }
    }
}