using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public static class StudentsPerformanceHelper
    {
        private static string GetTopStreakPraise(List<StudentHistoryDTO> list)
        {
            if (list != null)
            {
                var speech = new StringBuilder();
                var studentsStreak = StudentsPerformanceHelper.GetCorrectStreak(list);
                var numOfQuestions = StudentsPerformanceHelper.GetNumberOfQuestions(list);

                var maxStreak = studentsStreak.Values.Max();
                var studentsFullStreak = new List<string>();
                foreach (var stud in studentsStreak)
                {
                    if (stud.Value == maxStreak)
                        studentsFullStreak.Add(stud.Key);
                }

                if (studentsFullStreak.Count == 1)
                {
                    speech.Append($"Only {studentsFullStreak[0]} has gotten every question correct! ");
                    speech.Append($"A round of applause for {studentsFullStreak[0]}. ");
                }
                else if (studentsFullStreak.Count > 1)
                {
                    if (studentsFullStreak.Count > 1 && studentsFullStreak.Count <= 5)
                    {
                        foreach (var stud in studentsFullStreak)
                        {
                            speech.Append($"{stud}, ");
                        }
                        speech.Append("have gotten every question correct! ");
                    }
                    else if (studentsFullStreak.Count > 5)
                    {
                        speech.Append("I am very happy that many of you gotten all questions correct! ");
                    }
                    speech.Append("Keep up the good work! ");
                }
                else
                {
                    return null;
                }
                return speech.ToString();
            }
            return null;
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

        public static Dictionary<string,int> GetCorrectStreak(List<StudentHistoryDTO> studHistories)
        {
            if (studHistories != null)
            {
                var studentsStreak = new Dictionary<string, int>();

                foreach (var stud in studHistories)
                {
                    var streak = 0;
                    var scores = stud.ResultInBinaryString.Select(digit => int.Parse(digit.ToString())).ToList();
                    foreach (var score in scores)
                    {
                        if (score != 0) streak++;
                        else streak = 0;
                    }
                    studentsStreak.Add(stud.UserAccountFullName, streak);
                }
                return studentsStreak;
            }
            return null;
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
