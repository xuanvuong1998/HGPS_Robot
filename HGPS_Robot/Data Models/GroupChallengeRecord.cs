using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class GroupChallengeRecord
    {
        public int ChallengeNumber { get; set; }
        public int GroupNumber { get; set; }
        public List<string> Submission { get; set; } = new List<string>();
        
        public string GetFinalSubmission()
        {
            var subs = Submission.OrderByDescending(x => int.Parse(x.Split('-')[0])).ToList();
            return subs[0];
        }

                
        public string FirstCorrectSubmission()
        {
            var subs = Submission.OrderBy(x => int.Parse(x.Split('-')[0])).ToList();

            foreach (var sub in subs)
            {
                if (sub.Split('-')[1] == "1") return sub;
            }

            return null;
        }
    }

}
