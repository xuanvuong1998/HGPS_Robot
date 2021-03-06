﻿using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static string _baseAddress = ApplicationSettings.BASE_ADDRESS;

        public static event EventHandler<StatusEventArgs> StatusChanged;
        public static event EventHandler<RobotCommandEventArgs> RobotCommandChanged;
        
        static SyncHelper()
        {
            _hubConnection = new HubConnection(_baseAddress);
            _syncHub = _hubConnection.CreateHubProxy("SyncHub");
            _myHub = _hubConnection.CreateHubProxy("MyHub");

            _myHub.On("displayRankings", (rankingsType)
                => OnDisplayResult(rankingsType));
            _myHub.On<List<List<string>>>("updateGroupResult", (results) =>
                    OnUpdateGroupResult(results));
            _myHub.On<string>("SendGroupResultToRobot", (groupSub)
                 => OnUpdateGroupSubmission(groupSub));
            _myHub.On("RequireGroupChallengeFromRobot", () =>
                    OnRequireGroupChallengeResults());

            _syncHub.On<string>("statusChanged", (status) => OnStatusChanged(status));
            _syncHub.On<RobotCmd>("haveRobotCommands", (command) => OnRobotCommand(command));            
            
            _hubConnection.Start().Wait();

            _syncHub.Invoke("Notify", CLIENT_NAME, _hubConnection.ConnectionId);
        }

        private static void OnRequireGroupChallengeResults()
        {
            List<int> top3 = GroupChallengeHelper.GetTop3();

            List<List<string>> members = new List<List<string>>();

            foreach (var topGroup in top3)
            {
                List<string> memberInGroup =
                        TablePositionHelper.GetMembersByGroupNumber(topGroup);

                members.Add(memberInGroup);
            }

            string top3InStringFormat = "";
            foreach (var item in top3)
            {
                top3InStringFormat += "Group " + item + "-";
            }

            // remove last "-"
            string order = top3InStringFormat.Remove(top3InStringFormat.Length - 1);

            _myHub.Invoke("ReceiveTop3GroupChallenge", order, members);
        }

        private static void OnUpdateGroupSubmission(string groupSub)
        {
            GroupChallengeHelper.ReceiveNewSubmission(groupSub);
        }

        private static void OnUpdateGroupResult(List<List<string>> results)
        {
            Debug.WriteLine("Received Group results"); 
            GlobalFlowControl.GroupChallenge.GroupResults = results; 
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

        public static void SendGroupChallengStepsToServer()
        {
            _myHub.Invoke<List<Quiz>>("InitGroupChallengeSteps",
           GroupChallengeHelper.GroupChallengeSteps.Values.ToList());
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
