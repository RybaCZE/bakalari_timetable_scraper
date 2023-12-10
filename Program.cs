using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

public class WebScraper {
    public async Task<List<string>> scrape_website(string url, string searchString) {
        // Scrape the website and return a list of div contents
        //the main and only needed method outside of this class
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var pink_divs = document.DocumentNode.Descendants("div")
            .Where(d => d.Attributes["class"]?.Value.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true);

        var divc_ontents = new List<string>();
        foreach (var pinkDiv in pink_divs) {
            string divContent = pinkDiv.OuterHtml;
            int indexOfDayFlexStart = divContent.IndexOf("<div class=\"day-flex\">");
            try {
                // Split and add the content before "<div class=\"day-flex\">"
                string strSplitted = divContent.Split(@" <div class=""day-flex"">")[0];
                divc_ontents.Add(strSplitted);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        return divc_ontents;
    }

    private async Task<DayItem[]> scrape_website_and_process(string url) {
        if (!(url.ToLower().Contains("/timetable/public/actual/") && url.Contains("bakalar"))) {
            throw new ArgumentException("The URL must be a valid Bakalari timetable URL.");
        }
        string searchString = "pink ";
        var divc_ontents = await scrape_website(url, searchString);
        url = url.Replace("Actual", "Next"); // Replace "Actual" with "Next" in the URL
        var divc_ontents2 = await scrape_website(url, searchString);
        divc_ontents.AddRange(divc_ontents2);

        DayItem[] dayItems = new DayItem[divc_ontents.Count];
        int counter = 0;
        foreach (string element in divc_ontents) {
            string helpStr = convertUnreadableStringToReadable(element);
            List<string> items = helpStr.Split(",NL").ToList();

            foreach (string item in items) {
                if (item.Contains("subjecttext")) {
                    List<string> helpList = item.Split("|").ToList();
                    if (helpList.Count == 2) {
                        // Extract subject and hour from the item
                        dayItems[counter].Subject = WebUtility.HtmlDecode(helpList[0]).Replace("\"subjecttext\":\"", "");
                        dayItems[counter].hour = WebUtility.HtmlDecode(helpList[1]).Replace("\"end", "");
                    } else if (helpList.Count == 3) {
                        // Extract subject, date, and hour from the item
                        dayItems[counter].Subject = WebUtility.HtmlDecode(getValue(item, "subjecttext"));
                        dayItems[counter].date = WebUtility.HtmlDecode(helpList[1]);
                        dayItems[counter].hour = WebUtility.HtmlDecode(helpList[2]).Replace("\"end", "");
                    } else {
                        throw new Exception("The timetable is not in the correct format. (internal error)");
                    }
                }
                // Extract other properties from the item
                if (item.Contains("teacher")) {
                    dayItems[counter].Teacher = WebUtility.HtmlDecode(getValue(item, "teacher"));
                }
                if (item.Contains("room")) {
                    dayItems[counter].Room = WebUtility.HtmlDecode(getValue(item, "room"));
                }
                if (item.Contains("changeinfo")) {
                    dayItems[counter].ChangeInfo = WebUtility.HtmlDecode(getValue(item, "changeinfo"));
                }
                if (item.Contains("removedinfo")) {
                    dayItems[counter].ChangeInfo = WebUtility.HtmlDecode(getValue(item, "removedinfo"));
                }
                if (item.Contains("group")) {
                    dayItems[counter].Group = WebUtility.HtmlDecode(getValue(item, "group"));
                }
            }
            counter++;
        }
        return dayItems;
    }

    private static string formatString(string input) {
        // Format the input string
        string indentedHtml = input.Replace(",\"", "end,NL \n\"");
        indentedHtml = indentedHtml.Replace("false", "\"false\"").Replace("null", "\"null\"").Replace("true", "\"true\"").Replace(":\"\"", ":\"null\"");
        indentedHtml = indentedHtml.Replace("<div class=\"day-item-hover  pink \" data-detail='{", "");
        indentedHtml = indentedHtml.Replace("}'>", "");
        indentedHtml = indentedHtml.Replace("\"null\"", "\"null\"end");
        indentedHtml = indentedHtml.Replace("<div class=\"day-item-hover  pink h-100\" data-detail='{", "");
        return indentedHtml;
    }

    private static string convertUnreadableStringToReadable(string unreadableString) {
        // Replace escaped double quotes with actual double quotes
        string unescapedString = unreadableString.Replace("&quot;", "\"");
        // Format the JSON-like content
        string formattedJson = Regex.Replace(unescapedString, @"data-detail=""\{(.+?)\}""", m => {
            string jsonContent = m.Groups[1].Value;
            return $"data-detail='{jsonContent}'";
        });

        // Clear unnecessary characters for readability
        string returnString = formatString(formattedJson);
        return returnString;
    }

    private static string getValue(string input, string key) {
        // Extract the value corresponding to the given key
        string pattern = $@"""{key}"":""([^""]*)""";
        Match match = Regex.Match(input, pattern);
        return match.Success ? match.Groups[1].Value : null;
    }

    public struct DayItem {
        public string Subject { get; set; }
        public string Teacher { get; set; }
        public string hour { get; set; }
        public string date { get; set; }
        public string Room { get; set; }
        public string Group { get; set; }
        public string ChangeInfo { get; set; }
    }
}
