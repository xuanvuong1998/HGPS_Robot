using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class SearchPreference
    {
        public int CurrentLevel { get; set; }
        public string CurrentClass { get; set; }
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string Subject { get; set; }
        public string LessonName { get; set; }
        public int LessonHistoryId { get; set; }
        public int QuestionNumber { get; set; }
    }
}
