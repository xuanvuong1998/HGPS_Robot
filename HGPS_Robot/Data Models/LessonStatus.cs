using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class LessonStatus
    {
        public string LessonId { get; set; }
        public string LessonState { get; set; }
        public string LessonName { get; set; }
        public Nullable<int> LessonSlide { get; set; }
        public string MediaPath { get; set; }
        public string MediaCompleted { get; set; }
        public string AskQuestion { get; set; }
        public string AccessToken { get; set; }
    }
}
