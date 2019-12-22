using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class Lesson
    {
        public string LessonId { get; set; }
        public string LessonName { get; set; }
        public string TeacherId { get; set; }
        public string Subject { get; set; }
        public int LessonSlides { get; set; }
        public string DateModified { get; set; }
    }
}
