using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class RobotActionHelper
    {

        public static void Wait(int miliSec)
        {
            Thread.Sleep(miliSec);
        }

        public static void MoveDuringChatbot()
        {
            var rdm = new Random();
            Task.Factory.StartNew(() =>
            {
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

                } while (GlobalFlowControl.ChatBot.ConversationEnable);

                UpperBodyHelper.ResetAll();
            });
        }

        public static void MoveDuringLesson()
        {
            var rdm = new Random();
            Task.Factory.StartNew(() =>
            {
                do
                {
                    if (GlobalFlowControl.Navigation.Moving)
                    {
                        UpperBodyHelper.ResetAll();
                        Thread.Sleep(1000); // Reduce too-busy-waiting                      
                    }
                    else
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            UpperBodyHelper.MoveRandomly(i, 0.7);
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

                        if (GlobalFlowControl.Lesson.Starting == false) break;

                        if (GlobalFlowControl.Navigation.Moving)
                        {
                            UpperBodyHelper.ResetAll();
                        }
                        else
                        {
                            Wait(4000);
                        }
                    }


                } while (GlobalFlowControl.Lesson.Starting);

                UpperBodyHelper.ResetAll();
            });
        }
    }
}
