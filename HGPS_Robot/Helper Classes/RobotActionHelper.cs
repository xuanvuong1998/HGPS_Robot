using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    class RobotActionHelper
    {
        private static bool _isMoving = false;
        private static Timer timer = new Timer();
        public static void Wait(int miliSec)
        {
            Thread.Sleep(miliSec);
        }

        public static void StopAll()
        {
            _isMoving = false;
        }

        static RobotActionHelper()
        {
            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _isMoving = true;
        }

        public static void MoveDuringChatbot()
        {
            var rdm = new Random();
            Task.Factory.StartNew(() =>
            {
                _isMoving = true;
                do
                {
                    int headMovementRdm = rdm.Next(2);

                    if (headMovementRdm == 0)
                    {
                        UpperBodyHelper.Head.Left();
                    }
                    else
                    {
                        UpperBodyHelper.Head.Right();
                    }

                } while (GlobalFlowControl.ChatBot.ConversationEnable
                        && _isMoving);
                _isMoving = false;

                UpperBodyHelper.ResetAll();
            });
        }

        public static void WaitToMove(int delay) // milisecond
        {
            timer.Interval = delay;

            _isMoving = false;

            timer.Start();

            while (_isMoving == false
                && GlobalFlowControl.Navigation.Moving == false
                && LessonHelper.PauseRequested == false) ;

            timer.Stop();
        }

        public static void MoveDuringLesson()
        {
            if (ApplicationSettings.RobotGestureEnable == false) return;
            var rdm = new Random();
            bool resetArd = false;
            Task.Factory.StartNew(() =>
            {
                do
                {
                    // RObot stop doing gestures when
                    //:moving, group challenge, teacher taking over (lesson is pausing)

                    if (GlobalFlowControl.Navigation.Moving
                       || LessonHelper.PauseRequested)
                    {
                        if (resetArd == false) // Prevent calling motors func to much
                        {
                            UpperBodyHelper.ResetAll();
                            resetArd = true;
                        }
                        Thread.Sleep(1000); // Reduce too-busy-waiting                      
                    }
                    else
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            UpperBodyHelper.MoveRandomly(i, 1);
                        }

                        int headMovementRdm = rdm.Next(2);

                        if (headMovementRdm == 0)
                        {
                            UpperBodyHelper.Head.Left();
                        }
                        else
                        {
                            UpperBodyHelper.Head.Right();
                        }

                        WaitToMove(4000);
                    }

                } while (_isMoving);

                UpperBodyHelper.ResetAll();
            });
        }
    }
}
