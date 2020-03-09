using SpeechLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public static class StudentsPerformanceHelper
    {
        public static List<StudentHistoryDTO> studentPerformanceOverall
             = new List<StudentHistoryDTO>();

        // Indicate whether robot received student performance results from web server ot not yet
        public static bool ResultReceived { get; set; }

        // Incicate whether robot is annoucing top students of the lesson

        public static bool PraisingBestStudents { get; set; }

        public static bool PraisingBestImprovements { get; set; }

        private static Random rand = new Random();

        public static void AnnouceBestStudentsOfLesson(string top)
        {
            // Wait to finish annoucing best improvement first
            // Because signal received from server hub => different thread

            while (PraisingBestImprovements)
            {
                Thread.Sleep(1000);
            }

            Debug.WriteLine("Top students " + top);

            if (top == "")
            {
                PraisingBestStudents = false; // Finish praising

                return;
            }

            SyncHelper.RequestOpeningURL("individual-top-students");
            string[] infos = top.Split('-');

            string[] top1 = infos[0].Split(',');

            int len = top.Split('-').Count();
            string point1 = infos[len / 2];

            string[] top2, top3;

            string point2 = "-1", point3 = "-1";
            if (len < 3)
            {
                top2 = new string[] { };
            }
            else
            {
                top2 = infos[1].Split(',');

                point2 = infos[len / 2 + 1];

            }

            if (len < 5)
            {
                top3 = new string[] { };
            }
            else
            {
                top3 = infos[2].Split(',');
                point3 = infos[len / 2 + 2];
            }

            string speech = "";

            if (top3.Count() > 0)
            {
                foreach (var std in top3)
                {
                    speech += std + ". ";
                }

                speech += $"Based on today's lesson. You are " +
                    "awarded the bronze medal for the best student. " +
                    "Well done. ";

                Synthesizer.Speak(speech);
                AudioHelper.PlayApplauseSound();
                speech = "";
            }

            if (top2.Count() > 0)
            {
                foreach (var std in top2)
                {
                    speech += std + ". ";
                }

                speech += $"You are "
                        + "awarded the silver medal. " +
                        "Very good job!";

                Synthesizer.Speak(speech);
                AudioHelper.PlayApplauseSound();

                speech = "";
            }


            foreach (var std in top1)
            {
                speech += std + ". ";
            }

            speech += speech += $"You are "
                   + "the best student today, and you won the gold medal. " +
                   "Very very good! Congratulation!";

            Synthesizer.Speak(speech);

            AudioHelper.PlayChampionSound();

            SyncHelper.RequestOpeningURL("hide-results");
            PraisingBestStudents = false; // Finish praising
        }

        public static void AnnouceBestImprovements(string top)
        {
            Debug.WriteLine("Best Improvement" + top);

            if (top == "")
            {
                PraisingBestImprovements = false; // Finish praising

                return;
            }

            SyncHelper.RequestOpeningURL("individual-best-improvement");


            Thread.Sleep(2000);

            string[] top1 = top.Split('-')[0].Split(',');
            string point1 = "-1";

            string[] infos = top.Split('-');
            int len = top.Split('-').Count();
            point1 = infos[len / 2];

            string[] top2, top3;

            string point2 = "-1", point3 = "-1";
            if (len < 3)
            {
                top2 = new string[] { };
            }
            else
            {
                top2 = infos[1].Split(',');

                point2 = infos[len / 2 + 1];

            }

            if (len < 5)
            {
                top3 = new string[] { };
            }
            else
            {
                top3 = infos[2].Split(',');
                point3 = infos[len / 2 + 2];
            }

            string speech = "";

            //if (top3.Count() > 0)
            //{
            //    foreach (var std in top3)
            //    {
            //        speech += std + ". ";
            //    }

            //    speech += $"You are awarded a bronze medal for the " +
            //        $"best improvement student. Good job. ";

            //    Synthesizer.Speak(speech);
            //    AudioHelper.PlayApplauseSound();
            //    speech = "";
            //}

            //if (top2.Count() > 0)
            //{
            //    foreach (var std in top2)
            //    {
            //        speech += std + ". ";
            //    }

            //    speech += $"You are awarded the silver medal " +
            //        $"for the best improvement student. Well done! ";

            //    Synthesizer.Speak(speech);
            //    AudioHelper.PlayApplauseSound();

            //    speech = "";
            //}

            //foreach (var std in top1)
            //{
            //    speech += std + ". ";
            //}

            //if (point1.Contains("-")) point1 = "3"; // Fake
            //speech += $"You are awarded the gold medal for the best improvement student. " +
            //    $"Congratulation";

            // don't ranking 3 students, 3 is the same effort
            foreach (var std in top3)
            {
                speech += std + ". ";
            }
            foreach (var std in top2)
            {
                speech += std + ". ";
            }
            foreach (var std in top1)
            {
                speech += std + ". ";
            }

            speech += "I am very happy to" +
                " see your positive results today. You guys deserve to get a medal for the " +
                "best improvement students. Congratulation! ";

            Synthesizer.Speak(speech);

            AudioHelper.PlayChampionSound();

            PraisingBestImprovements = false; // Finish praising best improvements
        }

        public static void AssessStudentAnswer(string res)
        {
            Synthesizer.Resume();
            var rdmStd = TablePositionHelper.LatestChosenStudent;

            int rdmNum = rand.Next(5);
            if (res == "correct")
            {
                switch (rdmNum)
                {
                    case 0: Synthesizer.Speak("I think...Yes exactly ! Well done " + rdmStd); break;
                    case 1:
                        Synthesizer.Speak("It is absolutely correct! Good job "
                    + rdmStd); break;
                    case 2: Synthesizer.Speak("I couldn't agree more. Very good " + rdmStd); break;
                    case 3:
                        Synthesizer.Speak(rdmStd + ". Your answer is definitely accurate. " +
                    "Good job. "); break;
                    case 4:
                        Synthesizer.Speak(rdmStd + ". that's very correct. There is " +
                    "nothing to add on that. Well done"); break;
                }

                BaseHelper.Go("C2");

                AudioHelper.PlayApplauseSound();
            }
            else if (res == "incorrect")
            {
                switch (rdmNum)
                {
                    case 0:
                        Synthesizer.Speak("Oh no, that's not correct "); break;
                    case 1:
                        Synthesizer.Speak("Oh sorry, you just made a mistake"); break;
                    case 2:
                    case 3:
                    case 4:
                        Synthesizer.Speak("Oh no, You've got it wrong"); break;
                }

                BaseHelper.Go("C2");

                AudioHelper.PlaySadSound();
            }

        }

        public static void InitMockData()
        {
            var tablePosition = TablePositionHelper.TablePositions;

            var x1 = new TablePosition { GroupNumber = 1, Student_Id = "andi", TableName = "A1" };
            var x2 = new TablePosition { GroupNumber = 2, Student_Id = "farhan", TableName = "A2" };
            var x3 = new TablePosition { GroupNumber = 3, Student_Id = "einstein", TableName = "A3" };
            var x4 = new TablePosition { GroupNumber = 4, Student_Id = "ariq", TableName = "A4" };
            var x5 = new TablePosition { GroupNumber = 5, Student_Id = "danni", TableName = "A5" };
            var x6 = new TablePosition { GroupNumber = 6, Student_Id = "nathalie", TableName = "A6" };
            var x7 = new TablePosition { GroupNumber = 7, Student_Id = "fiona", TableName = "A7" };

            tablePosition.Add(x1);
            tablePosition.Add(x2);
            tablePosition.Add(x3);
            tablePosition.Add(x4);
            tablePosition.Add(x5);
            tablePosition.Add(x6);
            tablePosition.Add(x7);


            var s1 = new StudentHistoryDTO { Student_id = "andi", ResultInBinaryString = "100000" };
            var s2 = new StudentHistoryDTO { Student_id = "farhan", ResultInBinaryString = "111110" };
            var s3 = new StudentHistoryDTO { Student_id = "einstein", ResultInBinaryString = "100110" };
            var s4 = new StudentHistoryDTO { Student_id = "ariq", ResultInBinaryString = "101110" };
            var s5 = new StudentHistoryDTO { Student_id = "danni", ResultInBinaryString = "101000" };
            var s6 = new StudentHistoryDTO { Student_id = "nathalie", ResultInBinaryString = "111111" };
            var s7 = new StudentHistoryDTO { Student_id = "fiona", ResultInBinaryString = "111100" };

            // nathalie(6), farhan(5), fiona(4), ariq(4), einstein(3), danni(2), andi(1)

            studentPerformanceOverall.Add(s1);
            studentPerformanceOverall.Add(s2);
            studentPerformanceOverall.Add(s3);
            studentPerformanceOverall.Add(s4);
            studentPerformanceOverall.Add(s5);
            studentPerformanceOverall.Add(s6);
            studentPerformanceOverall.Add(s7);
        }

        public static int GetCorrectSubNum(string sub)
        {
            return sub.Count(x => x == '1');
        }

        public static Dictionary<string, int> GetStudentsRanking()
        {
            var orderedList = studentPerformanceOverall.OrderByDescending(
                        x => x.ResultInBinaryString.Count(y => y == '1')).ToList();

            int rank = 0;
            int preLevel = 10000;

            Dictionary<string, int> ranks = new Dictionary<string, int>();

            foreach (var std in orderedList)
            {
                var p = GetCorrectSubNum(std.ResultInBinaryString);

                if (p < preLevel)
                {
                    rank++;
                }

                preLevel = p;

                ranks.Add(std.Student_id, rank);
            }

            return ranks;
        }

        public static int GetNumberOfCorrectStudent(List<StudentHistoryDTO> stdHis)
        {
            int cnt = 0;
            foreach (var item in stdHis)
            {
                cnt += item.ResultInBinaryString.Last() == '1' ? 1 : 0;
            }
            return cnt;
        }

        public static int GetNumberOfInCorrectStudent(List<StudentHistoryDTO> stdHis)
        {
            int cnt = 0;
            foreach (var item in stdHis)
            {
                cnt += item.ResultInBinaryString.Last() == '1' ? 0 : 1;
            }
            return cnt;
        }

        public static Dictionary<string, int> CalculateScore(List<StudentHistoryDTO> studHistories)
        {
            if (studHistories != null)
            {
                var studentsScore = new Dictionary<string, int>();
                foreach (var stud in studHistories)
                {
                    studentsScore.Add(stud.UserAccountFullName, stud.TotalPointAwarded);
                }
                return studentsScore;
            }
            return null;
        }

        public static Dictionary<string, int> GetTopStudents(List<StudentHistoryDTO> studHistories)
        {
            if (studHistories != null)
            {
                var topStudents = new Dictionary<string, int>();
                var studentsScore = CalculateScore(studHistories);
                var highestScore = studentsScore.Values.Max();

                foreach (var stud in studentsScore)
                {
                    if (stud.Value == highestScore)
                    {
                        topStudents.Add(stud.Key, stud.Value);
                    }
                }
                return topStudents;
            }
            return null;
        }

        public static int GetNumOfFullScore(List<StudentHistoryDTO> studHistories)
        {
            if (studHistories != null)
            {
                var numOfFullScore = 0;
                foreach (var stud in studHistories)
                {
                    var fullScore = true;
                    var scores = stud.ResultInBinaryString.Select(digit => int.Parse(digit.ToString())).ToList();
                    foreach (var score in scores)
                    {
                        if (score == 0)
                        {
                            fullScore = false;
                            break;
                        }
                    }
                    if (fullScore) numOfFullScore++;
                }
                return numOfFullScore;
            }
            return 0;
        }

        public static int GetNumberOfQuestions(List<StudentHistoryDTO> studHistories)
        {
            if (studHistories != null)
            {
                var stud = studHistories[0];
                var numOfQuestions = stud.ResultInBinaryString.ToCharArray().Length;
                return numOfQuestions;
            }
            return 0;
        }

        public static StudentsPerformance GetSummary(List<StudentHistoryDTO> studHistories)
        {
            if (studHistories != null)
            {
                var studentsScore = CalculateScore(studHistories);
                var highestScore = studentsScore.Values.Max();
                var lowestScore = studentsScore.Values.Min();
                var numOfQuestions = GetNumberOfQuestions(studHistories);
                var numOfFullScore = GetNumOfFullScore(studHistories);

                var totalScore = 0;
                foreach (var stud in studentsScore) totalScore += stud.Value;
                var averageScore = Math.Round((float)totalScore / (float)studentsScore.Count, 2);

                var studentsPerformance = new StudentsPerformance
                {
                    AverageScore = averageScore,
                    HighestScore = highestScore,
                    LowestScore = lowestScore,
                    NumberOfQuestions = numOfQuestions,
                    NumberOfFullScore = numOfFullScore
                };
                return studentsPerformance;
            }
            return null;
        }
    }
}
