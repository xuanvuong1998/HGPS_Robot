using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Robot;

namespace HGPS_Robot
{
    public class UpperBodyHelper
    {
        static int[] minPos = { 2048, 2000, 2730, 1200, 2816, 1870, 2200, 1930 };
        //static int[] maxPos = { 2840, 1400, 2150, 2048, 2030, 1430, 2048, 2240 };
        static int[] maxPos = { 2640, 1600, 2350, 1848, 2230, 1530, 2048, 2240 };
        public static class Head
        {
            const string HEAD_UP = "HEAD_UP";
            const string HEAD_DOWN = "HEAD_DOWN";
            const string HEAD_LEFT = "HEAD_LEFT";
            const string HEAD_RIGHT = "HEAD_RIGHT";
            const string HEAD_CENTER = "HEAD_CENTER";
            const string HEAD_ID = "7";
            const string NECK_ID = "8";

            public static void Up()
            {
                upperBody.Move(HEAD_UP);
            }

            public static void Down()
            {
                upperBody.Move(HEAD_DOWN);
            }
            public static void Right()
            {
                upperBody.Move(HEAD_RIGHT);
            }
            public static void Left()
            {
                upperBody.Move(HEAD_LEFT);
            }

            public static void Reset()
            {
                upperBody.Move(HEAD_CENTER);
            }
        }

        public static class Shoulder
        {
            const string UD_LEFT_ID = "4";
            const string UD_RIGHT_ID = "1";
            const string LR_LEFT_ID = "5";
            const string LR_RIGHT_ID = "2";

            public class LeftS
            {
                const string LEFT_SHOULDER_UP = "LEFT_SHOULDER_UP";
                const string LEFT_SHOULDER_DOWN = "LEFT_SHOULDER_DOWN";
                const string LEFT_SHOULDER_LEFT = "LEFT_SHOULDER_LEFT";
                const string LEFT_SHOULDER_RIGHT = "LEFT_SHOULDER_RIGHT";
                const string LEFT_SHOULDER_CENTER = "LEFT_SHOULDER_CENTER";

                public static void Up()
                {
                    upperBody.Move(LEFT_SHOULDER_UP);
                }

                public static void Down()
                {
                    upperBody.Move(LEFT_SHOULDER_DOWN);
                }
                public static void Right()
                {
                    upperBody.Move(LEFT_SHOULDER_RIGHT);
                }
                public static void Left()
                {
                    upperBody.Move(LEFT_SHOULDER_LEFT);
                }

                public static void Center()
                {
                    upperBody.Move(LEFT_SHOULDER_CENTER);
                }
                public static void Reset()
                {
                    Right();
                    Down();
                }
            }
            public class RightS
            {
                const string RIGHT_SHOULDER_UP = "RIGHT_SHOULDER_UP";
                const string RIGHT_SHOULDER_DOWN = "RIGHT_SHOULDER_DOWN";
                const string RIGHT_SHOULDER_LEFT = "RIGHT_SHOULDER_LEFT";
                const string RIGHT_SHOULDER_RIGHT = "RIGHT_SHOULDER_RIGHT";
                const string RIGHT_SHOULDER_CENTER = "RIGHT_SHOULDER_CENTER";
                public static void Up()
                {
                    upperBody.Move(RIGHT_SHOULDER_UP);
                }

                public static void Down()
                {
                    upperBody.Move(RIGHT_SHOULDER_DOWN);

                }
                public static void Right()
                {
                    upperBody.Move(RIGHT_SHOULDER_RIGHT);

                }
                public static void Left()
                {
                    upperBody.Move(RIGHT_SHOULDER_LEFT);

                }
                public static void Center()
                {
                    upperBody.Move(RIGHT_SHOULDER_CENTER);
                }

                public static void Reset()
                {
                    Left();
                    Down();
                }
            }
        }

        public static class Elbow
        {
            const string LEFT_ID = "6";
            const string RIGHT_ID = "3";
            public class LeftS
            {
                const string LEFT_ELBOW_UP = "LEFT_ELBOW_UP";
                const string LEFT_ELBOW_DOWN = "LEFT_ELBOW_DOWN";
                const string LEFT_ELBOW_CENTER = "LEFT_ELBOW_CENTER";
                public static void Up()
                {
                    upperBody.Move(LEFT_ELBOW_UP);
                }

                public static void Down()
                {
                    upperBody.Move(LEFT_ELBOW_DOWN);
                }

                public static void Center()
                {
                    Reset();
                }
                public static void Reset()
                {
                    upperBody.Move(LEFT_ELBOW_CENTER);
                }
            }

            public class RightS
            {
                const string RIGHT_ELBOW_UP = "RIGHT_ELBOW_UP";
                const string RIGHT_ELBOW_DOWN = "RIGHT_ELBOW_DOWN";
                const string RIGHT_ELBOW_CENTER = "RIGHT_ELBOW_CENTER";
                public static void Up()
                {
                    upperBody.Move(RIGHT_ELBOW_UP);
                }

                public static void Down()
                {
                    upperBody.Move(RIGHT_ELBOW_DOWN);

                }

                public static void Center()
                {
                    Reset();
                }

                public static void Reset()
                {
                    upperBody.Move(RIGHT_ELBOW_CENTER);
                }

            }
        }

        private static List<int> motorIds = new List<int>(
                        new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        private static UpperBody upperBody;

        public static void Init()
        {
            return;
            upperBody = new UpperBody(motorIds);
            ResetAll();
        }

        public static List<int> ReadPresentLocations()
        {
            return upperBody.GetPresentPositions();
        }

        public static void SavePosturesTODB(List<List<int>> postures, string postureName)
        {
            upperBody.SavePosturesTODB(postures, postureName);
        }

        public static void LockMotors()
        {
            upperBody.LockMotors();
        }

        public static void UnLockMotors()
        {
            upperBody.UnlockMotors();
        }

        public static void MoveTo(int motorId, int goalPos)
        {
            upperBody.MoveTo(motorId, goalPos);
        }

        public static void MoveSpecialPosture(string posture)
        {
            Task.Factory.StartNew(() =>
            {
                List<List<int>> list = upperBody.GetSpecialPostures(posture);

                MoveAllMotors(list, 1500);

                ResetAll();
            });
        }

        public static void MoveWhileMoving()
        {

        }

        public static void MoveWhileGreeting()
        {

        }


        public static void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public static void MoveAllMotors(List<List<int>> list, int delay)
        {
            foreach (var posture in list)
            {
                for (int i = 1; i <= 8; i++)
                {
                    MoveTo(i, posture[i - 1]);
                }
                Wait(delay);
            }
        }

        public static void MoveRandomlyAllMotorsOneTime()
        {
            for (int i = 1; i <= 8; i++)
            {
                MoveRandomly(i, 1);
            }
        }

        /// <summary>
        /// Move during lesson and chatbot
        /// </summary>
        public static void MoveRandomlyAllMotors()
        {
            return;
            //System.Windows.Forms.MessageBox.Show("DANCE"); return;
            Task.Factory.StartNew(() =>
            {
                //GlobalFlowControl.UpperBody.MovingRandomly = true;
                do
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        MoveRandomly(i, 0.7);
                    }

                    //if (GlobalFlowControl.Navigation.Moving) break;

                    Wait(4000);

                } while (GlobalFlowControl.Lesson.Starting
                 || GlobalFlowControl.ChatBot.Talking);
                ResetAll();
                //GlobalFlowControl.UpperBody.MovingRandomly = false;
            });

        }
        public static void MoveRandomly(int motorId, double delta)
        {
            int x = minPos[motorId - 1];
            int y = maxPos[motorId - 1];

            double randomV;

            if (x < y)
            {
                randomV = new Random().Next(y - x + 1) * delta + x;
            }
            else
            {
                randomV = new Random().Next(x - y + 1) * delta + y;
            }

            MoveTo(motorId, (int)randomV);
        }


        #region GestureCommand

        public static void DoGestures(string gesKind)
        {
            gesKind = gesKind.ToUpper();
            switch (gesKind)
            {
                case "RANDOM_ONE_TIME":
                    MoveRandomlyAllMotorsOneTime();
                    break;
                case "RANDOM_UNTIL":
                    MoveRandomlyAllMotors();
                    break;
                case "HEAD_UP":
                    Head.Up();
                    break;
                case "HEAD_DOWN":
                    Head.Down();
                    break;
                case "HEAD_LEFT":
                    Head.Left();
                    break;
                case "HEAD_RIGHT":
                    Head.Right();
                    break;
                case "HEAD_CENTER":
                    Head.Reset();
                    break;

                case "LEFT_SHOULDER_UP":
                    Shoulder.LeftS.Up();
                    break;
                case "LEFT_SHOULDER_DOWN":
                    Shoulder.LeftS.Down();
                    break;
                case "LEFT_SHOULDER_LEFT":
                    Shoulder.LeftS.Left();
                    break;
                case "LEFT_SHOULDER_RIGHT":
                    Shoulder.LeftS.Right();
                    break;
                case "LEFT_SHOULDER_CENTER":
                    Shoulder.LeftS.Reset();
                    break;

                case "RIGHT_SHOULDER_UP":
                    Shoulder.RightS.Up();
                    break;
                case "RIGHT_SHOULDER_DOWN":
                    Shoulder.RightS.Down();
                    break;
                case "RIGHT_SHOULDER_LEFT":
                    Shoulder.RightS.Left();
                    break;
                case "RIGHT_SHOULDER_RIGHT":
                    Shoulder.RightS.Right();
                    break;
                case "RIGHT_SHOULDER_CENTER":
                    Shoulder.RightS.Reset();
                    break;

                case "RIGHT_ELBOW_UP":
                    Elbow.RightS.Up();
                    break;
                case "RIGHT_ELBOW_DOWN":
                    Elbow.RightS.Down();
                    break;

                case "LEFT_ELBOW_UP":
                    Elbow.LeftS.Up();
                    break;
                case "LEFT_ELBOW_DOWN":
                    Elbow.LeftS.Down();
                    break;

                case "RESET_ALL":
                    ResetAll();
                    break;
            }
        }

        public static void ResetAll()
        {
            Head.Reset();
            Shoulder.LeftS.Reset();
            Shoulder.RightS.Reset();
            Elbow.LeftS.Reset();
            Elbow.RightS.Reset();
        }

        public static void ResetHead()
        {
            Head.Reset();
        }

        public static void ResetArms()
        {
            Shoulder.LeftS.Reset();
            Shoulder.RightS.Reset();
        }

        #endregion

    }
}
