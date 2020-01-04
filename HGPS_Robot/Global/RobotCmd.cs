using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeechLibrary;

namespace HGPS_Robot
{
    /// <summary>
    /// This class to process command from teacher panel (using signalR) 
    /// </summary>
    public class RobotCmd
    {
        public string Navigation { get; set; }
        public string Movement { get; set; }
        public string Gesture { get; set; }
        public string Chatbot { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public string SpecialAction { get; set; }
        public string Praise { get; set; }
        public string LessonStatus { get; set; }

        public List<StudentHistoryDTO> AssessPerformance { get; set; }

        #region Student Performance
        private void AnalyzeStudentPerformance(List<StudentHistoryDTO> list)
        {            
            var rdm = new Random();
            var rdmNum = rdm.Next(1, 4); // generate random number 1-3

            var speech = "";
            
            switch (rdmNum)
            {
                case 1:
                    var performance = StudentsPerformanceHelper.GetSummary(list);
                    speech = "The average score for this class currently at " + performance.AverageScore.ToString();
                    
                    if (performance.AverageScore == 0)
                    {
                        speech += ". No one got any correct answer";
                    }
                    else
                    {
                        speech += ". Well done!";
                    }
                                        
                    break;

                case 2:
                    var numOfFullScore = StudentsPerformanceHelper.GetNumOfFullScore(list);
                    if (numOfFullScore != 0)
                    {
                        speech = $"We have {numOfFullScore.ToString()} students with full score!";
                        speech += " Great job!";
                    }
                    else
                    {
                        speech = GetTopStudentsPraise(list);
                    }
                    break;

                case 3:
                    speech = GetTopStudentsPraise(list);
                    break;
            }

            LessonHelper.InsertPraise(speech);            
        }
        
        private string GetTopStudentsPraise(List<StudentHistoryDTO> list)
        {
            if (list != null)
            {
                var speech = new StringBuilder(); 

                var topStudents = StudentsPerformanceHelper.GetTopStudents(list);
                if (topStudents.Count == 1)
                {
                    speech.Append($"Currently the top students for this class is {topStudents.FirstOrDefault().Key}" +
                             $" with a score of {topStudents.FirstOrDefault().Value.ToString()}. ");
                    speech.Append("The rest of you please try your best!");
                }
                else if (topStudents.Count <= 5)
                {
                    foreach (var stud in topStudents)
                    {
                        speech.Append(stud.Key);
                        speech.Append(", ");
                    }
                    speech.Append(" are currently the top students in this class, ");
                    speech.Append($"with a score of {topStudents.FirstOrDefault().Value.ToString()}. ");
                    speech.Append("Great job everyone!");
                }
                else
                {
                    speech.Append($"Many of you have done well with a score of {topStudents.FirstOrDefault().Value.ToString()}. ");
                    speech.Append("Keep up the good work! ");
                }
                return speech.ToString(); 
            }
            return null;
        }
        private string GetTopStreakPraise(List<StudentHistoryDTO> list)
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

        private void AnalyzePersonalPerformance()
        {
            string studentId = Praise.Split('-')[0];
            string message = Praise.Split('-')[1];
            
            if (message.ToLower() == "auto")
            {
                if (AssessPerformance != null)
                {
                    var stdHis = AssessPerformance
                                .Where(x => x.Student_id == studentId)
                                .FirstOrDefault();                    
                }
            }
            else
            {
                LessonHelper.InsertPraise(message);
            }
        }
        #endregion

        // From Teacher Panel
        public void ProcessCommand()
        {
            if (Navigation != null)
            {
                BaseHelper.Go(Navigation);
            }

            if (Movement != null)
            {
                BaseHelper.DoBaseMovements(Movement);
            }

            if (Gesture != null)
            {
                UpperBodyHelper.DoGestures(Gesture);
            }

            if (Chatbot != null)
            {

            }

            if (Praise != null)
            {
                AnalyzePersonalPerformance();
            }

            if (AssessPerformance != null)
            {
                AnalyzeStudentPerformance(AssessPerformance);
            }
        }
    }
}
