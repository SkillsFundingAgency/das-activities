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
                var urlHelper = htmlHelper.GetUrlHelper();
                var activitiesUrl = urlHelper.Activities(result.Total);

                foreach (var activity in result.Activities)
                {
                    var at = activity.At.ToGmtStandardTime();
                    var activityUrl = urlHelper.Activity(activity);
                    var activityText = htmlHelper.Activity(activity);

                    ol.Add("li", li => li.AddClass(activity.At.Date != date ? "first" : "")
                        .Append("h4", h4 => h4.Title(at.ToString("U")).Text(at.ToRelativeFormat(now)))
                        .Append("p", p => p.AddClass("activity").Text(activityText))
                        .Append("p", p =>
                        {
                            p.AddClass("meta").AppendText("At").AppendHtml(" ")
                                .Append("time", time => time.Text(at.ToString("h:mm tt"))).AppendHtml(" ");

                            if (!string.IsNullOrEmpty(activityUrl))
                            {
                                p.Add("a", a => a.Attr("href", activityUrl).Text("See details"));
                            }
                        })
                    );

                    date = activity.At.Date;
                }

                if (result.Activities.Count() != result.Total)
                {
                    ol.Add("li", li => li.Add("a", a => a.Attr("href", activitiesUrl).Text("See all activity")));
                }

                return ol;
            }

            return new HtmlTag("p").Text("You have no recent activity");
        }

        public static HtmlTag LatestActivities(this HtmlHelper htmlHelper, AggregatedActivitiesResult result)
        {
            if (result.Aggregates.Any())
            {
                var now = DateTime.UtcNow.ToGmtStandardTime();
                var ol = new HtmlTag("ol").Id("item-list");
                var urlHelper = htmlHelper.GetUrlHelper();
                var activitiesUrl = urlHelper.Activities();

                foreach (var aggregate in result.Aggregates)
                {
                    var at = aggregate.TopHit.At.ToGmtStandardTime();
                    var activityText = htmlHelper.Activity(aggregate.TopHit, aggregate.Count);

                    ol.Add("li", li => li.AddClass("item")
                        .Append("div", div => div.AddClass("item-label").Text(at.ToRelativeFormat(now)))
                        .Append("div", div => div.AddClass("item-description").Text(activityText))
                    );
                }
                
                ol.Add("li", li => li.AddClass("item all-activity")
                    .Append("div", div => div.AddClass("item-label")
                        .Append("a", a => a.Attr("href", activitiesUrl).Text("See all activity"))
                    )
                );

                return ol;
            }

            return new HtmlTag("p").Text("You have no recent activity");
        }

        public static string Activity(this HtmlHelper htmlHelper, Activity activity, long count = 1)
        {
            var localizer = activity.Type.GetLocalizer();
            var text = count > 1 ? localizer.GetPluralText(activity, count) : localizer.GetSingularText(activity);

            return text;
        }

        public static UrlHelper GetUrlHelper(this HtmlHelper htmlHelper)
        {
            return new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
        }
    }
}