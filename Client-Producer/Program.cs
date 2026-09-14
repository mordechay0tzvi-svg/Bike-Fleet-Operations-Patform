using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// URLs לבחירה:
// var url = "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_status.json";
// var url = "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_information.json";
var url = "https://gbfs.lyft.com/gbfs/2.3/bkn/en/vehicle_types.json";

// 1. יצירת אובייקט ה-HttpClient שבאמצעותו מנהלים את התקשורת
using var client = new HttpClient();

Console.WriteLine($"Sending GET request to: {url}\n");

try
{
    // 2. שליחת בקשת HTTP GET לקבלת תשובת השרת
    HttpResponseMessage response = await client.GetAsync(url);

    // 3. בדיקה אם הבקשה הצליחה (קוד תשובה בטווח 200-299)
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"[STATUS] {response.StatusCode} OK\n");

        // 4. חילוץ התוכן (Body) של התשובה כמחרוזת JSON
        string jsonContent = await response.Content.ReadAsStringAsync();

        // הדפסת 300 התווים הראשונים בלבד כדי לא להציף את המסך
        Console.WriteLine("--- Preview of JSON Response ---");
        Console.WriteLine(jsonContent.Substring(0, Math.Min(300, jsonContent.Length)) + "...\n");

        // 5. פירסור דינמי ללא DTO - קריאת שדות מתוך ה-JSON
        using JsonDocument doc = JsonDocument.Parse(jsonContent);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("last_updated", out JsonElement lastUpdated))
        {
            Console.WriteLine($"[PARSED DATA] Last Updated: {lastUpdated.GetInt64()}");
        }

        if (root.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("stations", out JsonElement stations))
        {
            Console.WriteLine($"[PARSED DATA] Total stations received: {stations.GetArrayLength()}");
        }
    }
    else
    {
        Console.WriteLine($"[ERROR] Request failed with Status Code: {response.StatusCode}");
    }
}
catch (HttpRequestException ex)
{
    // טיפול בשגיאות תקשורת (למשל: אין חיבור לאינטרנט או ה-URL שגוי)
    Console.WriteLine($"[NETWORK ERROR] {ex.Message}");
}