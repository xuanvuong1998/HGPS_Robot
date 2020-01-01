using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Robot;
using Robot.Data;
using ROS = Robot.Data.ROS;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    public class BaseHelper
    {
        public static double LINEAR_SPEED = 0.3;
        public static double ANGULAR_SPEED = 0.8;
        private static Base rBase = new Base();
        private static int nextRovingLocationIndex = 0;
        private static string[] locationList;
        private static Thread rovingThread;
        private static bool rovingDirection; 
        private static Timer rBaseStopTimer = new Timer();
        private static readonly double METER_PER_ROUND = 1.27484;
        private static readonly string BASE_IP = "192.168.31.200:9090";

        static BaseHelper()
        {                     
            rBaseStopTimer.Interval = 1000;
            rBaseStopTimer.Elapsed += RBaseStopTimer_Elapsed;
            rBaseStopTimer.AutoReset = false;
        }
        
        #region events
        private static void RBaseStopTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            rBase.Stop();
        }


        static void WaitToStop(double time)
        {
            rBaseStopTimer.Interval = time;
            rBaseStopTimer.Start();
        }

        public static void WaitForNavigationReachedOrCanceled()
        {
            /*while (!GlobalFlowControl.NavigationCancelled && !GlobalFlowControl.NavigationReachedGoal
                && GlobalFlowControl.IsRoving) ;

            */
        }

        public static void Wait(int delayTime)
        {
            Thread.Sleep(delayTime);
        }

        #endregion

        #region spinning       

        public static void TurnLeft()
        {
           Move(ROS.BaseDirection.ANTICLOCKWISE);
        }

        public static void TurnRight()
        {
           Move(ROS.BaseDirection.CLOCKWISE);
        }
        public static void TurnLeft(int rounds)
        {
            double totalM = rounds * METER_PER_ROUND;
            double vel = ANGULAR_SPEED;

            double time = totalM / vel;

            TurnLeft();

            if (rounds > 0) WaitToStop(time);
        }

        public static void TurnRight(int rounds)
        {
            double totalM = rounds * METER_PER_ROUND;
            double vel = ANGULAR_SPEED;

            double time = totalM / vel;

            TurnRight();

            if (rounds > 0) WaitToStop(time);
        }

        public static void TurnLeftDuring(double interval)
        {
            interval *= 1000;
            TurnLeft();
            WaitToStop(interval);
        }

        public static void TurnRightDuring(double interval)
        {
            interval *= 1000;
            TurnRight();
            WaitToStop(interval);
        }

        #endregion

        #region Moves
        public static void Forward()
        {
           Move(ROS.BaseDirection.FORWARD);
        }

        public static void ForwardDuring(int interval)
        {
            interval *= 1000;
            Forward();
            WaitToStop(interval);
        }
        public static void BackwardDuring(int interval)
        {
            interval *= 1000;
            Backward();
            WaitToStop(interval);
        }

        public static void Forward(int meters)
        {
            double vel = LINEAR_SPEED;

            double time = meters / vel;

            time *= 1000;

            Forward();

            if (meters > 0) WaitToStop(time);
        }

        public static void Backward(int meters)
        {
            double vel = LINEAR_SPEED;

            double time = meters / vel;

            time *= 1000;

            Backward();

            if (meters > 0) WaitToStop(time);
        }

        public static void Backward()
        {
           Move(ROS.BaseDirection.BACKWARD);
        }
        #endregion

        #region commands
        public static void DoBaseMovements(string direction)
        {
            switch (direction)
            {
                case "forward":
                    Forward();
                    break;
                case "backward":
                    Backward();
                    break;
                case "left-turn":
                    TurnLeft();
                    break;
                case "right-turn":
                    TurnRight();
                    break;
                case "stop":
                    Stop();
                    break;
                default:
                    Stop();
                    break;
            }
        }
        #endregion

        #region Navigation
        public static bool IsCancelledNavigation() => GetNavigationStatus() == "";
        public static bool IsReachedGoal() => GetNavigationStatus() == "Goal reached.";

        public static string GetNavigationStatus()
        {
            return ROS.DataList[ROSTopic.NAVIGATION_STATUS].Data;
        }
        static public void CancelNavigation()
        {
            rBase.Stop();
            rBase.CancelNavigation();
        }
        static public void Go(string location)
        {
            try
            {
                GlobalFlowControl.Navigation.ResetBeforeNavigation();

                rBase.Go(location);
            }
            catch { }
        }

        static public void GoUntilReachedGoalOrCanceled(string location)
        {
            Go(location);
            while (GlobalFlowControl.Navigation.Moving == true) ;
        }

        private static void RBase_NavigationStatusChanged(object o, NavigationStatusEventArgs e)
        {
            if (e.Status == "Goal reached.")
            {
                //SpeechHelper.SpeakAsync("Navigation Reached Goal!");
                GlobalFlowControl.Navigation.ReachedGoal = true;

            }
            else if (e.Status == "")
            {
                GlobalFlowControl.Navigation.Canceled = true;
                //SpeechHelper.SpeakAsync("Navigation Cancelled!");
            }
            else
            {
                GlobalFlowControl.Navigation.Stucked = true;
                //SpeechHelper.SpeakAsync("Sorry! I am stucked");
            }

            //mainForm.NavSTT = e.Status; 
        }
        #endregion

        #region Setup
        static public void Connect()
        {
            try
            {
                rBase.Connect(BASE_IP);
                rBase.Initialise();
                rBase.NavigationStatusChanged += RBase_NavigationStatusChanged;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        static public void Disconnect()
        {
            rBase.Disconnect();
        }
        #endregion

        #region Cores
        static public void Stop()
        {
            rBase.Stop();
        }


        static public void Move(ROS.BaseDirection direction)
        {
            try
            {
                rBase.AngularSpeed = ANGULAR_SPEED;
                rBase.LinearSpeed = LINEAR_SPEED;
                rBase.Move(direction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
