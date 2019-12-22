using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HGPS_Robot
{
    public static class LessonStatusHelper
    {
        public static LessonStatus LessonStatus {
            get
            {
                return _lessonStatus;
            }
            set
            {
                _lessonStatus = value;
            }
        }
        private static LessonStatus _lessonStatus;
        static LessonStatusHelper()
        {
            _lessonStatus = new LessonStatus();
        }

        public static void Initialise(string lessonId)
        {
            _lessonStatus.LessonId = lessonId;
        }

        public static void Update(string lessonName = null, Nullable<int> lessonSlide = null, string lessonState = null, 
                                  string mediaPath = null, string mediaCompleted = null, Nullable<int> askQuestionNum = null)
        {
            if (_lessonStatus.LessonId != null)
            {
                _lessonStatus.LessonName = lessonName;
                _lessonStatus.LessonSlide = lessonSlide;
                _lessonStatus.LessonState = lessonState;
                _lessonStatus.MediaPath = mediaPath;
                _lessonStatus.MediaCompleted = mediaCompleted;
                _lessonStatus.AskQuestionNumber = askQuestionNum;

                WebHelper.UpdateStatus(_lessonStatus);
            }
        }
    }
}
