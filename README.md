# WebScraper for Bakalari Timetable

This C# class provides a web scraper for extracting timetable information from Bakalari using the HtmlAgilityPack library. The scraper retrieves div contents based on the provided URL and returns a structured array of `DayItem` objects.

## Usage

To use the WebScraper, follow these steps:

1. **Instantiate the WebScraper:**

```csharp
WebScraper webScraper = new WebScraper();
```
2. **Scrape the Website**

Use the `scrape_website` method to retrieve a list of div contents based on the provided URL.

```csharp
List<string> divContents = await webScraper.scrape_website(url);
```

3. **Process the Timetable Data:**
4. 
Use the scrape_website_and_process method to extract and process timetable data from the specified URL.
```csharp
WebScraper.DayItem[] dayItems = await webScraper.scrape_website_and_process(url);
```
Access Timetable Information:
Access timetable information from the DayItem struct, which contains properties such as Subject, Teacher, hour, date, Room, Group, and ChangeInfo.

**Example:**
```csharp
WebScraper webScraper = new WebScraper();
string url = "https://example.com/timetable/public/actual/bakalar";

// Scrape website and process timetable data
WebScraper.DayItem[] dayItems = await webScraper.scrape_website_and_process(url);

// Access timetable information
foreach (var dayItem in dayItems) {
    Console.WriteLine($"Subject: {dayItem.Subject}, Teacher: {dayItem.Teacher}, Room: {dayItem.Room}");
}
```
*Note: Ensure that the provided URL is a valid Bakalari timetable URL.*

### Dependencies
2. HtmlAgilityPack
3. System.Net
4. System.Text.RegularExpressions
50 System.Net.Http
