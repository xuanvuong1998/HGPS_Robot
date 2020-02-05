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

        public static void MoveDuringLesson()
        {
            var rdm = new Random();
            Task.Factory.StartNew(() =>
            {
                do
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        UpperBodyHelper.MoveRandomly(i, 0.7);
                    }

                    if (GlobalFlowControl.Lesson.Starting == false) break;

                    int x = rdm.Next(8);
                    
                    if (x <= 4)
                    {
                        Debug.WriteLine("Go C" + x);
                        BaseHelper.Go("C" + x);
                    }

                    Wait(4000);

                } while (GlobalFlowControl.Lesson.Starting);

            });
        }
    }
}
