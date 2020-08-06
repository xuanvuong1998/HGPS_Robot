using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.Diagnostics;

namespace HGPS_Robot
{
    public static class WebHelper
    {
        private static string BASE_ADDRESS = ApplicationSettings.BASE_ADDRESS;
        private const string ACCESS_TOKEN = "1H099XeDsRteM89yy91QonxH3mEd0DoE";


        public static async Task<LessonStatus> GetStatus()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_ADDRESS);
                var response = await client.GetAsync("api/StatusApi/GetLessonStatus");

                if (response.IsSuccessStatusCode)
                {
                    var statusJson = await response.Content.ReadAsStringAsync();
                    if (statusJson != null)
                    {
                        statusJson = Regex.Unescape(statusJson);
                        statusJson = statusJson.Substring(1, statusJson.Length - 2);
                        var status = JsonConvert.DeserializeObject<LessonStatus>(statusJson);
                        return status;
                    }
                }
                return null;
            }
        }
        
        public static async void UpdateStatus(LessonStatus status)
        {
            if (status != null)
            {
                if (status.AccessToken == null || String.IsNullOrWhiteSpace(status.AccessToken))
                    status.AccessToken = ACCESS_TOKEN;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BASE_ADDRESS);

                    using (var req = new HttpRequestMessage(HttpMethod.Post, "api/StatusApi/Update"))
                    {
                        req.Content = new StringContent(JsonConvert.SerializeObject(status), Encoding.UTF8, "application/json");
                        await client.SendAsync(req);
                    }
                }
            }
        }

        public static List<SavedLessons> GetSavedLessons()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BASE_ADDRESS);
            var response = client.GetAsync("api/LessonApi/GetLessonsWithDateModified").Result;
            var resultsJson = response.Content.ReadAsStringAsync().Result;

            if (resultsJson != null)
            {
                resultsJson = Regex.Unescape(resultsJson);
                resultsJson = resultsJson.Substring(1, resultsJson.Length - 2);
                var results = JsonConvert.DeserializeObject<List<SavedLessons>>(resultsJson);
                return results;
            }
            return null;
        }

        public static async void DeleteAllTablePositions()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(BASE_ADDRESS)
            };
            
            await client.GetAsync("api/TablePositionApi/DeleteAll");       
            
        }

        public static List<TablePosition> GetTablePositions()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BASE_ADDRESS); 
            var response = client.GetAsync("api/TablePositionApi/GetPositions").Result;
            var resultsJson = response.Content.ReadAsStringAsync().Result;

            if (resultsJson != null)
            {
                resultsJson = Regex.Unescape(resultsJson);
                resultsJson = resultsJson.Substring(1, resultsJson.Length - 2);
                var results = JsonConvert.DeserializeObject<List<TablePosition>>(resultsJson);
                return results;
            }
            return null;
        }


        public static async Task<string> AddLesson(Lesson lesson)
        {
            if (lesson != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BASE_ADDRESS);
                    using (var req = new HttpRequestMessage(HttpMethod.Post, "api/LessonApi/AddLesson"))
                    {
                        req.Content = new StringContent(JsonConvert.SerializeObject(lesson), Encoding.UTF8, "application/json");
                        var msg = await client.SendAsync(req);
                        var content = msg.Content.ReadAsStringAsync().Result;
                        return content;
                    }
                }
            }
            return null;
        }

        public static async void DeleteLesson(string lessonName)
        {
            if (lessonName != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BASE_ADDRESS);
                    using (var req = new HttpRequestMessage(HttpMethod.Post, "api/LessonApi/DeleteLesson"))
                    {
                        req.Content = new StringContent(JsonConvert.SerializeObject(lessonName), Encoding.UTF8, "application/json");
                        await client.SendAsync(req);
                    }
                }
            }
        }

        public static async void AddQuestion(Question question)
        {
            if (question != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BASE_ADDRESS);
                    using (var req = new HttpRequestMessage(HttpMethod.Post, "api/QuestionApi/AddQuestion"))
                    {
                        req.Content = new StringContent(JsonConvert.SerializeObject(question), Encoding.UTF8, "application/json");
                        await client.SendAsync(req);
                    }
                }
            }   
        }

        public static async void AddLessonHistory(LessonHistory history)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_ADDRESS);
                using (var req = new HttpRequestMessage(HttpMethod.Post, "api/LessonHistoryApi/AddLessonHistory"))
                {
                    req.Content = new StringContent(JsonConvert.SerializeObject(history), Encoding.UTF8, "application/json");
                    await client.SendAsync(req);
                }
            }
        }

        public static async Task<List<RobotProgSlide>> GetLessonCommands(string lessonName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_ADDRESS);
                var response = await client.GetAsync($"LessonManagement/GetLessonCommands?name={lessonName}");
                if (response.IsSuccessStatusCode)
                {
                    var commandsJson = await response.Content.ReadAsStringAsync();

                    var commands = JsonConvert.DeserializeObject<List<string>>(commandsJson);
                    var robotSlides = new List<RobotProgSlide>();

                    foreach (var command in commands)
                    {
                        var robotSlide = new RobotProgSlide();
                        var robotCode = command.Replace("\n", String.Empty)
                                               .Replace("\x0B", String.Empty)
                                               .Replace("\x11", String.Empty)
                                               .Replace("\r", String.Empty);
                        robotSlide.Code = robotCode;
                        robotSlides.Add(robotSlide);

                    }
                    return robotSlides;
                }
                return null;
            }
        }

        public static async Task<List<string>> GetLessonSlidesName()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_ADDRESS);
                var response = await client.GetAsync("LessonManagement/GetAllExistingLessonNames");
                if (response.IsSuccessStatusCode)
                {
                    var lessonSlidesName = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<string>>(lessonSlidesName);
                }
                return null;
            }
        }
    }
}
