using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;



public class WebScraper {
    public async Task<List<string>> ScrapeWebsite(string url, string searchString) {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var pinkDivs = document.DocumentNode.Descendants("div")
            .Where(d => d.Attributes["class"]?.Value.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true);

        var divContents = new List<string>();
        foreach (var pinkDiv in pinkDivs) {
            string divContent = pinkDiv.OuterHtml;
            int indexOfDayFlexStart = divContent.IndexOf("<div class=\"day-flex\">");

            try {
                string str_splitted = divContent.Split(@" <div class=""day-flex"">")[0];
                divContents.Add(str_splitted);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        return divContents;
    }

    public WebScraper() {

    }

    public async Task<DayItem[]> ScrapeWebsiteAndProcess(string url) {

        if (!(url.ToLower().Contains("/timetable/public/actual/") && url.Contains("bakalar"))) {
            throw new ArgumentException("The URL must be a valid Bakalari timetable URL.");
        }
        string searchString = "pink ";
        var divContents = await ScrapeWebsite(url, searchString);
        url.Replace("Actual", "Next");
        var divContents_2 = await ScrapeWebsite(url, searchString);
        divContents.AddRange(divContents_2);

        DayItem[] dayItems = new DayItem[divContents.Count];
        int counter = 0;
        foreach (string idk in divContents) {
            string help_str = ConvertTrashToReadableHtml(idk);
            List<string> xd = help_str.Split(",NL").ToList();

            foreach (string xdd in xd) {
                if (xdd.Contains("subjecttext")) {
                    List<string> help_list = xdd.Split("|").ToList();
                    if (help_list.Count == 2) {//" ""subjecttext"":""út 5.12. "

                        dayItems[counter].date = WebUtility.HtmlDecode(help_list[0]).Replace("\"subjecttext\":\"", "");
                        dayItems[counter].hour = WebUtility.HtmlDecode(help_list[1]).Replace("\"end", "");
                    } else {
                        dayItems[counter].Subject = WebUtility.HtmlDecode(GetValue(help_list[0], "subjecttext"));
                        dayItems[counter].date = WebUtility.HtmlDecode(help_list[1]);
                        dayItems[counter].hour = WebUtility.HtmlDecode(help_list[2]).Replace("\"end", "");


                    }
                }
                if (xdd.Contains("teacher")) {
                    dayItems[counter].Teacher = WebUtility.HtmlDecode(GetValue(xdd, "teacher"));
                }
                if (xdd.Contains("room")) {
                    dayItems[counter].Room = WebUtility.HtmlDecode(GetValue(xdd, "room"));
                }
                if (xdd.Contains("changeinfo")) {
                    dayItems[counter].ChangeInfo = WebUtility.HtmlDecode(GetValue(xdd, "changeinfo"));
                }
                if (xdd.Contains("removedinfo")) {
                    dayItems[counter].ChangeInfo = WebUtility.HtmlDecode(GetValue(xdd, "removedinfo"));
                }
                if (xdd.Contains("group")) {
                    dayItems[counter].Group = WebUtility.HtmlDecode(GetValue(xdd, "group"));
                }
            }
            counter++;
        }

        return dayItems;
    }

    static string DecodeUnicodeEscapeSequences(string input) {
        return Regex.Unescape(input);
    }

    static string ConvertTrashToReadableHtml(string trashString) {
        // Replace escaped double quotes with actual double quotes
        string unescapedString = trashString.Replace("&quot;", "\"");

        // Use a regular expression to format the JSON-like content
        string formattedJson = Regex.Replace(unescapedString, @"data-detail=""\{(.+?)\}""", m => {
            string jsonContent = m.Groups[1].Value;
            return $"data-detail='{jsonContent}'";
        });

        // Insert new lines for better readability
        string indentedHtml = formattedJson.Replace(",\"", "end,NL \n\"");
        indentedHtml = formattedJson.Replace(",\"", "end,NL \n\"");
        indentedHtml = indentedHtml.Replace("false", "\"false\"").Replace("null", "\"null\"").Replace("true", "\"true\"").Replace(":\"\"", ":\"null\"");
        indentedHtml = indentedHtml.Replace("<div class=\"day-item-hover  pink \" data-detail='{", "");
        indentedHtml = indentedHtml.Replace("}'>", "");
        indentedHtml = indentedHtml.Replace("\"null\"", "\"null\"end");
        indentedHtml = indentedHtml.Replace("<div class=\"day-item-hover  pink h-100\" data-detail='{", "");
        // Add indentation to the div for better structure

        return indentedHtml;
    }

    static string GetValue(string input, string key) {
        string pattern = $@"""{key}"":""([^""]*)""";
        Match match = Regex.Match(input, pattern);

        return match.Success ? match.Groups[1].Value : null;
    }


    // "subjecttext":"Operačn&#237; syst&#233;my a hardware | po 4.12. | 5 (11:50 - 12:35)"end
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
