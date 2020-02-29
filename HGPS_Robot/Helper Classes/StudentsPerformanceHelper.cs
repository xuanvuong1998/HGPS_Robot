using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public static class StudentsPerformanceHelper
    {
        public static List<StudentHistoryDTO> studentPerformanceOverall
             = new List<StudentHistoryDTO>();

        public static int GetCorrectSubNum(string sub)
        {
            return sub.Count(x => x == '1');
        }

        public static Dictionary<string, int> GetStudentsRanking()
        {
            var orderedList = studentPerformanceOverall.OrderByDescending(
                        x => x.ResultInBinaryString.Count(y => y == '1')).ToList();

            int rank = 0;
            int preLevel = -1;

            Dictionary<string, int> ranks = new Dictionary<string, int>();

            foreach (var std in orderedList)
            {
                var p = GetCorrectSubNum(std.ResultInBinaryString);

                if (p < preLevel)
                {
                    rank++;
                }

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
