using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
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
            var topStudents = new Dictionary<string, int>();

            switch (rdmNum)
            {
                case 1:
                    var performance = StudentsPerformanceHelper.GetSummary(list);
                    speech = "The average score for this class currently at " + performance.AverageScore.ToString();
                    speech += " Well done!";
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
                        topStudents = StudentsPerformanceHelper.GetTopStudents(list);
                        if (topStudents.Count == 1)
                        {
                            speech = $"Currently the top students for this class is {topStudents.FirstOrDefault().Key} with" +
                                     $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                            speech += " The rest of you please try your best!"; ;
                        }
                        else if (topStudents.Count <= 5)
                        {
                            foreach (var stud in topStudents)
                            {
                                speech += stud.Key;
                                speech += ", ";
                            }
                            speech += " are currently the top students in this class";
                            speech += $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                            speech += " Great job everyone!";
                        }
                    }
                    break;

                case 3:
                    topStudents = StudentsPerformanceHelper.GetTopStudents(list);
                    if (topStudents.Count == 1)
                    {
                        speech = $"Currently the top students for this class is {topStudents.FirstOrDefault().Key}" +
                                 $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                        speech += " The rest of you please try your best!"; ;
                    }
                    else if (topStudents.Count <= 5)
                    {
                        foreach (var stud in topStudents)
                        {
                            speech += stud.Key;
                            speech += ", ";
                        }
                        speech += " are currently the top students in this class";
                        speech += $" with a score of {topStudents.FirstOrDefault().Value.ToString()}";
                        speech += " Great job everyone!";
                    }
                    break;
            }
            LessonHelper.InsertPraise(speech);
        }
        #endregion

        // From Teacher Panel
        public void ProcessCommand()
        {
            if (Navigation != null)
            {
                
            }

            if (Movement != null)
            {

            }

            if (Gesture != null)
            {

            }

            if (Chatbot != null)
            {

            }

            if (AssessPerformance != null)
            {
                AnalyzeStudentPerformance(AssessPerformance);
            }
        }
    }
}
