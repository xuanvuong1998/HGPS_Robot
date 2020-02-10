using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpeechLibrary;

namespace HGPS_Robot
{
    /// <summary>
    /// This class to process command from teacher panel (using signalR) 
    /// </summary>
    public class RobotCmd
    {
        #region properties
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
        #endregion

        public void AskRandomStudent(List<StudentHistoryDTO> list)
        {

            var rdm = new Random();
            int rdmNum = rdm.Next(1, 11);
            if (GlobalFlowControl.Lesson.ChosenStudent != null 
                || rdmNum <= 10)                
            {
                //ask
                Debug.WriteLine("ASKING STUDENT");

                StudentHistoryDTO student;

                if (GlobalFlowControl.Lesson.ChosenStudent != null)
                {
                    student = list.FirstOrDefault(x => 
                            x.Student_id == GlobalFlowControl.Lesson.ChosenStudent);

                    if (GlobalFlowControl.Lesson.IsStudentChosenBefore(student.Student_id) == false)
                    {
                        GlobalFlowControl.Lesson.ChosenStudentList.Add(student.Student_id);
                    }
                }
                else
                {
                    int randNum;

                    // All students have their all turns to be asked (approached) by robot
                    if (GlobalFlowControl.Lesson.ChosenStudentList.Count
                         == list.Count)
                    {
                        GlobalFlowControl.Lesson.ChosenStudentList.Clear();
                    }
                    do
                    {
                        randNum = rdm.Next(0, list.Count);
                        Debug.WriteLine("Random Student: " + randNum);
                    } while (GlobalFlowControl.Lesson.IsStudentChosenBefore(list[randNum].Student_id));

                    
                    student = list[randNum];

                    GlobalFlowControl.Lesson.ChosenStudentList.Add(student.Student_id); 
                }
                                     
                string speech = student.UserAccountFullName + "! ";
                int num = rdm.Next(0, 5);
                switch (num)
                {
                    case 0:
                        speech += "Can you share what is your answer?";
                        break;
                    case 1:
                        speech += "Did you have your own answer? Is this question easy or difficult?";
                        break;
                    case 2:
                        speech += "What did you choose as your answer for this question?";
                        break;
                    case 3:
                        speech += "What do you think about this question?";
                        break;
                    case 4:
                        speech += "Can you share your answer to the class and explain why did you choose it?";
                        break;
                }
                //Get position of student
                var position = StudentPositionHelper.FindTablePosByStdId(student.Student_id);

                if (position != null)
                {
                    GlobalFlowControl.Lesson.ApproachStudent = position;

                    //Go to student
                    LessonHelper.InsertCommand("gountil", position);
                    LessonHelper.InsertCommand("speak", speech);
                    LessonHelper.InsertCommand("asking", "1");

                    bool studentResult = student.ResultInBinaryString.Last() == '1';
                    string resultSpeech = "";

                    if (studentResult)
                    {
                        switch (num)
                        {
                            case 0: case 1:
                                resultSpeech = "Excellent ";
                                break;
                            case 2: case 3:
                                resultSpeech = "Not bad ";
                                break;
                            case 4:
                                resultSpeech = "Very good ";
                                break;

                        }
                        resultSpeech += student.Student_id + "! Your answer is correct!";  
                    }
                    else
                    {                       
                        resultSpeech = "Sorry " + student.Student_id + ". Your answer is not correct!";
                    }
                    
                    LessonHelper.InsertCommand("speak", resultSpeech);

                }
            }
            

            AnalyzeStudentPerformance(AssessPerformance);

        }

        #region Student Performance
        private void AnalyzeStudentPerformance(List<StudentHistoryDTO> list)
        {
            var rdm = new Random();
            var rdmNum = rdm.Next(1, 4); // generate random number 1-3

            int incorrectCnt = StudentsPerformanceHelper.GetNumberOfInCorrectStudent(list);
            int correctCnt = StudentsPerformanceHelper.GetNumberOfCorrectStudent(list);

            var speech = "";
            if (correctCnt == 0)
            {
                var x = rdm.Next(0, 3);
                switch (x)
                {
                    case 0:
                        speech = "I am very sad now, because no one got any correct answer for this question. ";
                        break;
                    case 1:
                        speech = "No one is correct. Is it because this question too difficult? ";
                        break;
                    case 2:
                        speech = "Sorry to say this, but none of you had a correct answer. ";
                        break;
                }
                speech += "Come on guys, don't give up. ";

                LessonHelper.InsertPraise(speech);


                return;
            }
            else
            {
                var x = rdm.Next(0, 3);
                if (incorrectCnt == 0)
                {
                    switch (x)
                    {
                        case 0:
                            speech = "Brilliant! No one did any mistake for this question. ";
                            break;
                        case 1:
                            speech = "Amazing! Everybody answers are correct. ";
                            break;
                        case 2:
                            speech = "Incredible! All of you got the correct answer. ";
                            break;
                    }

                    speech += "Everyone, please clap your hands";
                    LessonHelper.InsertPraise(speech);
                    return;
                }
                else if (correctCnt >= incorrectCnt)
                {
                    switch (x)
                    {
                        case 0:
                            speech = "Well, I am every happy to see many of you got correct answer. ";
                            break;
                        case 1:
                            speech = "Great! Most of you did a good job! ";
                            break;
                        case 2:
                            speech = "Good job everybody, most of you are correct. ";
                            break;
                    }
                }
            }

            var numOfFullScore = StudentsPerformanceHelper.GetNumOfFullScore(list);
            if (numOfFullScore != 0)
            {
                speech += $"Awesome, we have {numOfFullScore.ToString()} students with full score! ";
            }

            switch (rdmNum)
            {
                case 1:
                    speech += GetTopStudentsPraise(list);
                    break;

                case 2:

                    speech += GetTopStudentsPraise(list);

                    break;

                case 3:
                    speech += GetTopStudentsPraise(list);
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
                var x = new Random().Next(0, 3);

                if (topStudents.Count == 1)
                {
                    switch (x)
                    {
                        case 0:
                            speech.Append($"Currently the top students for this class is {topStudents.FirstOrDefault().Key}" +
                             $" with a score of {topStudents.FirstOrDefault().Value.ToString()}. ");
                            break;
                        case 1:
                            speech.Append($"{topStudents.FirstOrDefault().Key} is having the" +
                                $" highest score in the class, congratulation! ");
                            break;
                        case 2:
                            speech.Append($"With the score of {topStudents.FirstOrDefault().Value.ToString()}. " +
                                $"I wanna say that, now, {topStudents.FirstOrDefault().Key} is the top student" +
                                $", congratulation {topStudents.FirstOrDefault().Key}. ");
                            break;
                    }

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

        private void AnalyzeEmotion(double happyPc, double sadPc, double neutralPc)
        {
            if (sadPc >= 0.3) // roughly 1/3 unhappy, give teacher time to explain
                              // for unhappy students again or engage some activities
            {
                LessonHelper.SendEmotionFeedBackToServer("survey-unhappy");
                LessonHelper.PauseLesson();    
                
                LessonHelper.ResumeSpeak();
                Synthesizer.Speak("Well, since some of you are not sure of this topic, let Mr Nizam explain again. ");
            } // Ok, happy or neutral
            else
            {
                Debug.WriteLine("Before Pause Lesson");
                LessonHelper.PauseLesson();
                Debug.WriteLine("After pause lesson");
                LessonHelper.ResumeSpeak();

                Synthesizer.Speak("Wow, most of you understand the topic! Let us continue with the lesson. ");
                //LessonHelper.SendEmotionFeedBackToServer("survey-happy");
                Debug.WriteLine("After speak the feedback");
                LessonHelper.Wait(2000);

                Debug.WriteLine("After wait 2 seconds");
                LessonHelper.ResumeLesson();

                Debug.WriteLine("After resume");
            }
        }

        // From Teacher Panel
        public void ProcessCommand()
        {         
            if (Navigation != null)
            {
                Debug.WriteLine("CHosen Student" + Navigation);
                GlobalFlowControl.Lesson.ChosenStudent = Navigation;
                //BaseHelper.Go(Navigation);
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

            if (LessonStatus != null)
            {
                Debug.WriteLine(LessonStatus + " Lesson Status");
                if (LessonStatus == "StopQuestion")
                {
                    GlobalFlowControl.Lesson.StartingQuiz = false;
                }
            }

            if (SpecialAction != null)
            {
                Debug.WriteLine("Special Action : " + SpecialAction);

                string thing = SpecialAction.Split('-')[0];
                string info = SpecialAction.Split('-')[1];

                if (thing == "Emotion")
                {

                    GlobalFlowControl.Lesson.StudentFeedbackReceived = true;
                    double sadPc = double.Parse(info.Split(';')[0]);
                    double neutralPc = double.Parse(info.Split(';')[1]);
                    double happyPc = double.Parse(info.Split(';')[2]);

                    AnalyzeEmotion(happyPc, sadPc, neutralPc);
                    
                }
                
            }

            if (Praise != null)
            {
                AnalyzePersonalPerformance();
            }

            if (AssessPerformance != null)
            {
                AskRandomStudent(AssessPerformance);

            }
        }
    }
}
