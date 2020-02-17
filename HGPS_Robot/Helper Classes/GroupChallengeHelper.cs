using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class GroupChallengeHelper
    {
        public List<GroupChallengeRecord> SortGroupByRanks(int challengeNumber)
        {
            var records = GlobalFlowControl.Lesson.GroupRecords;

            records = records.Where(x => x.ChallengeNumber == challengeNumber).ToList();

            
            
        }
    }
}
