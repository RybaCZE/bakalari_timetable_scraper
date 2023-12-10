WebScraper for Bakalari Timetable
This C# class provides a web scraper for extracting timetable information from Bakalari using the HtmlAgilityPack library. The scraper retrieves div contents based on the provided URL and returns a structured array of DayItem objects.

Usage
To use the WebScraper, follow these steps:

Instantiate the WebScraper:

csharp
Copy code
WebScraper webScraper = new WebScraper();
Scrape the Website:

Use the scrape_website method to retrieve a list of div contents based on the provided URL.
csharp
Copy code
List<string> divContents = await webScraper.scrape_website(url);
Process the Timetable Data:

Use the scrape_website_and_process method to extract and process timetable data from the specified URL.
csharp
Copy code
WebScraper.DayItem[] dayItems = await webScraper.scrape_website_and_process(url);
Access Timetable Information:

Access timetable information from the DayItem struct, which contains properties such as Subject, Teacher, hour, date, Room, Group, and ChangeInfo.
Example
csharp
Copy code
WebScraper webScraper = new WebScraper();
string url = "https://example.com/timetable/public/actual/bakalar";

// Scrape website and process timetable data
WebScraper.DayItem[] dayItems = await webScraper.scrape_website_and_process(url);

// Access timetable information
foreach (var dayItem in dayItems) {
    Console.WriteLine($"Subject: {dayItem.Subject}, Teacher: {dayItem.Teacher}, Room: {dayItem.Room}");
}
Note: Ensure that the provided URL is a valid Bakalari timetable URL.

Dependencies
HtmlAgilityPack
System.Net
System.Text.RegularExpressions
System.Net.Http
