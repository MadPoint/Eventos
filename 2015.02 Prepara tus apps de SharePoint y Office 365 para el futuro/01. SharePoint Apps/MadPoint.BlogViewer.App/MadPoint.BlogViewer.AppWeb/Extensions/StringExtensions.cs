using System.Text.RegularExpressions;

namespace MadPoint.BlogViewer.AppWeb.Extensions
{
    public static class StringExtensions
    {
        public static string ToTextWithoutHtmlTags(this string htmlString)
        {
            if (string.IsNullOrWhiteSpace(htmlString))
            {
                return string.Empty;
            }

            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }
    }
}