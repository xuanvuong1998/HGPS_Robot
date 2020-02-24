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
        public List<string> Submission { get; set; } = new List<string>(); // time-result

        public int GetSubmissionCount() => Submission.Count();
        
        public int GetLeftChancesNumber()
        {
            var members = TablePositionHelper.GetMembersByGroupNumber(GroupNumber);

            int leftChances = members.Count - Submission.Count;

            return leftChances;
        }
        
        public string GetLatestSubmission()
        {
            var subs = Submission.OrderByDescending(x => int.Parse(x.Split('-')[0])).ToList();
            if (subs == null || subs.Count == 0) return null;
            return subs[0];
        }

        public int GetNumberOfCorrectSub()
        {
            int cnt = 0;
            foreach (var sub in Submission)
            {
                cnt += IsCorrectAnswer(sub);
            }
            return cnt;
        }

        public int GetNumberOfIncorrectSub()
        {
            return Submission.Count - GetNumberOfCorrectSub();
        }

        public string GetFinalResult()
        {
            var latestSub = GetLatestSubmission();
            if (latestSub == null) return null;
            var isCorrect = latestSub.Split('-')[1] == "1";

            if (isCorrect) return GetFirstCorrectSubmission();

            return latestSub;
        }

        public int GetFinalSubTime()
        {
            var finalRes = GetFinalResult();
            if (finalRes == null) return -1;
            return int.Parse(finalRes.Split('-')[0]);
        }

        public bool GetFinalSubResultInBool()
        {
            var finalRes = GetFinalResult();
            if (finalRes == null) return false;
            return GetFinalResult().Split('-')[1] == "1";
        }

        private int IsCorrectAnswer(string sub)
        {
            if (sub == null || sub == "") return 0;
            return sub.Split('-')[1] == "1" ? 1 : 0;
        }
              
        public string GetFirstCorrectSubmission()
        {
            var subs = Submission.OrderBy(x => int.Parse(x.Split('-')[0])).ToList();

            foreach (var sub in subs)
            {
                if (sub.Split('-')[1] == "1") return sub;
            }

            return null; // It never happens, because this function called
            // when checked exist at least 1 correct sub
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>GroupNumber: subTime-result, correctSubCnt</returns>
        public string ToStringFormat()
        {
            var finalRes = GetFinalResult();

            string output = $"Group {GroupNumber}: {finalRes}, {GetNumberOfCorrectSub()}";

            return output;
        }

        public int CompareTo(object obj)
        {
            var another = obj as GroupChallengeRecord;

            // Compare Challenge Number first, if it different, don't compare another things

            int c1, c2;

            c1 = this.ChallengeNumber;
            c2 = another.ChallengeNumber;

            if (c1 != c2) return c1 - c2;

            string f1 = this.GetFinalResult();
            string f2 = another.GetFinalResult();

            c1 = IsCorrectAnswer(f1); // 1: true, 0: false
            c2 = IsCorrectAnswer(f2);

            if (c1 != c2)
                return c2 - c1; // Compare the final result first

            // If both are correct, compare the first submitted time

            if (c1 == 1) // c2 of course = 1 also
            {
                c1 = int.Parse(f1.Split('-')[0]);
                c2 = int.Parse(f2.Split('-')[0]);

                return c1 - c2;
            }
            
            // Wow, both are correct and also same submitted time
            // Or both are not correct => We check by number of correct times
            // If still the same, :((( never mind, too enough, don't compare any more

            c1 = GetNumberOfCorrectSub();
            c2 = another.GetNumberOfCorrectSub();

            if (c1 != c2) return c2 - c1; // If the last answer is the same

            return -1; // Let it be, don't swap
            
        }
    }

}
