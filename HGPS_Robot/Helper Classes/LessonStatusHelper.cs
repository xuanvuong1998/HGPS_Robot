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
        public static LessonStatus LessonStatus { get; set; }

        static LessonStatusHelper()
        {
            LessonStatus = new LessonStatus();
        }

        public static void Initialise(string lessonId)
        {
            LessonStatus.LessonId = lessonId;
        }

        public static void Update(string lessonName = null, Nullable<int> lessonSlide = null, string lessonState = null,
                                  string mediaPath = null, string mediaCompleted = null, Nullable<int> askQuestionNum = null)
        {

            if (LessonStatus.LessonId == null)
            {
                LessonStatus.LessonId = LessonHelper.LessonId;
            }
            
            LessonStatus.LessonName = lessonName;
            LessonStatus.LessonSlide = lessonSlide;
            LessonStatus.LessonState = lessonState;
            LessonStatus.MediaPath = mediaPath;
            LessonStatus.MediaCompleted = mediaCompleted;
            LessonStatus.AskQuestionNumber = askQuestionNum;

            WebHelper.UpdateStatus(LessonStatus);

        }
    }
}
