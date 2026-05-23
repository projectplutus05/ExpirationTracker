using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpirationTracker.Helpers;

public static class HtmlHelpers
{
    public static IHtmlContent DateBadge(this IHtmlHelper html, DateTime? date)
    {
        if (!date.HasValue)
            return HtmlString.Empty;

        var today = DateTime.Today;
        var days = (int)(date.Value - today).TotalDays;

        string cssClass;
        string label = date.Value.ToString("MM/dd/yyyy");

        if (days < 0)
            cssClass = "badge bg-danger";
        else if (days <= 30)
            cssClass = "badge bg-warning text-dark";
        else
            cssClass = "badge bg-success";

        return new HtmlString($"<span class=\"{cssClass}\">{label}</span>");
    }
}
