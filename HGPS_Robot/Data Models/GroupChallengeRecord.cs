using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class GroupChallengeRecord : IComparable
    {
        public int ChallengeNumber { get; set; }
        public int GroupNumber { get; set; }    

        // Submission results each step 
        // Format: result(0/1)-submittedCnt-submittedTime-groupAns
        public List<string> StepSubmissions { get; set; } = new List<string>();

        public int GetNumberOfCorrectSteps()
        {
            int cnt = 0;
            foreach (var step in StepSubmissions)
            {
                cnt += step[0] - '0';
            }
            return cnt;
        }

        public int GetNumberOfSubmissions()
        {
            int cnt = 0;
            foreach (var step in StepSubmissions)
            {
                cnt += step[2] - '0'; // ex: 0-2
            }
            return cnt;
        }

        public int GetTotalSubTime()
        {
            int cnt = 0;
            foreach (var step in StepSubmissions)
            {
                cnt += int.Parse(step.Split('-')[2]);
            }
            return cnt;
        }

        public int CompareTo(object obj)
        {
            var another = obj as GroupChallengeRecord;

            int x = this.GetNumberOfCorrectSteps();
            int y = another.GetNumberOfCorrectSteps();

            if (x != y) return y - x; // who has more passed steps will have smaller order index

            x = this.GetNumberOfSubmissions();
            y = another.GetNumberOfSubmissions();

            if (x != y) return x - y; // prioritize who has less submissions

            x = this.GetTotalSubTime();
            y = another.GetTotalSubTime();

            return x - y; // Last factor             
        }

       
    }

}
