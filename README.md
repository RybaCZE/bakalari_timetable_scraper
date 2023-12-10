# WebScraper for Bakalari Timetable

This C# class provides a web scraper for extracting timetable information from Bakalari using the HtmlAgilityPack library. The scraper retrieves div contents based on the provided URL and returns a structured array of structs.

## Usage

To use the WebScraper, follow these steps:

1. **Instantiate the WebScraper:**

```csharp
WebScraper web_scraper = new WebScraper();
```
2. **Get the array of structs:**

```csharp
string url = "https://example.com/timetable/public/actual/bakalar";
WebScraper.DayItem[] data = await web_scraper.scrape_website(url);
```

Access Timetable Information:
Access timetable information from the DayItem struct, which contains properties such as Subject, Teacher, hour, date, Room, Group, and ChangeInfo.

**Example:**
```csharp
WebScraper web_scraper = new WebScraper();
string url = "https://example.com/timetable/public/actual/bakalar";

// Scrape website and process timetable data
WebScraper.DayItem[] dayItems = await web_scraper.scrape_website(url);

// Access timetable information
foreach (var dayItem in dayItems) {
    Console.WriteLine($"Subject: {dayItem.Subject}");
    Console.WriteLine($"Teacher: {dayItem.Teacher}");
    Console.WriteLine($"Hour: {dayItem.hour}");
    Console.WriteLine($"Date: {dayItem.date}");
    Console.WriteLine($"Room: {dayItem.Room}");
    Console.WriteLine($"Group: {dayItem.Group}");
    Console.WriteLine($"ChangeInfo: {dayItem.ChangeInfo}");
    Console.WriteLine();
}
```
*Note: Ensure that the provided URL is a valid Bakalari timetable URL.*

### Dependencies
1. HtmlAgilityPack
2. System.Net
3. System.Text.RegularExpressions
4. System.Net.Http
