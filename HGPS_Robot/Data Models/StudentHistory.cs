using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class StudentHistory
    {
        public string StudentId { get; set; }
        public int QuestionId { get; set; }
        public string StudentAnswer { get; set; }
        public int PointAwarded { get; set; }
        public int LessonHistoryId { get; set; }
    }
}
