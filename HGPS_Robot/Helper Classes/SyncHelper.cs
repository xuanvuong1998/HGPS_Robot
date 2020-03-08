using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public static class SyncHelper
    {
        private static HubConnection _hubConnection;
        private static IHubProxy _syncHub, _myHub;
        private const string CLIENT_NAME = "Robo-TA";
        private const string _baseAddress = "http://robo-ta.com/";
        //private const string _baseAddress = "https://localhost:44353/";

        public static event EventHandler<StatusEventArgs> StatusChanged;
        public static event EventHandler<RobotCommandEventArgs> RobotCommandChanged;
        
        static SyncHelper()
        {
            _hubConnection = new HubConnection(_baseAddress);
            _syncHub = _hubConnection.CreateHubProxy("SyncHub");
            _myHub = _hubConnection.CreateHubProxy("MyHub");
            _syncHub.On<string>("statusChanged", (status) => OnStatusChanged(status));
            _myHub.On("displayRankings", (rankingsType)
                => OnDisplayResult(rankingsType));
            _syncHub.On<RobotCmd>("haveRobotCommands", (command) => OnRobotCommand(command));            
            _hubConnection.Start().Wait();

            _syncHub.Invoke("Notify", CLIENT_NAME, _hubConnection.ConnectionId);
        }

        // Event handler from myhub
        private static void OnDisplayResult(string message)
        {
            if (message.Contains("group-competition"))
            {
                GroupCompetitionHelper.ProcessResult(message);
            }
            else if (message.Contains("individual"))
            {
                if (message.Contains("improvement"))
                {
                    StudentsPerformanceHelper
                        .AnnouceBestImprovements(message.Split(';')[1]);
                }
                else if (message.Contains("top-students"))
                {
                    StudentsPerformanceHelper
                    .AnnouceBestStudentsOfLesson(message.Split(';')[1]);
                }
            }
        }

        private static void OnRobotCommand(RobotCmd command)
        {
            var e = new RobotCommandEventArgs(command);
            RobotCommandChanged?.Invoke(null, e);
        }

        private static void OnStatusChanged(string status)
        {
            var _status = JsonConvert.DeserializeObject<LessonStatus>(status);
            var e = new StatusEventArgs(_status);
            StatusChanged?.Invoke(null, e);
        }

        public static void InvokeRankingsResult(string type)
        {
            _myHub.Invoke<string>("AcquireRankingResults", type);
        }

        /// <summary>
        /// Call hub method, send signal to slide viewer to open google chrome contains results
        /// </summary>
        /// <param name="type"></param>
        public static void RequestOpeningURL(string type)
        {
            _myHub.Invoke<string>("RequestOpenResultURL", type);
        }
    }

    public class RobotCommandEventArgs
    {
        public RobotCmd Command { get; set; }

        public RobotCommandEventArgs(RobotCmd cmd)
        {
            Command = cmd;
        }
    }

    public class StatusEventArgs
    {
        public LessonStatus Status { get; set; }
        public StatusEventArgs(LessonStatus status)
        {
            Status = status;
        }
    }
}
