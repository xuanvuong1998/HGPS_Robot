using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class GroupChallengeDTO
    {
        public int GroupNumber { get; set; }
        public int PointAwarded { get; set; }
        public int SubmissionCount { get; set; }
        public List<string> Members { get; set; } = new List<string>();
    }
}
