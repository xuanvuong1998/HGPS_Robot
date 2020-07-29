
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HGPS_Robot
{
    public static class SystemUpdateHelper
    {
        public static event EventHandler SystemUpdated;
        public static void Start()
        {
            ThreadStart starter = Update;
            starter += () =>
            {
                OnSystemUpdated();
            };
            Thread thread = new Thread(starter) { IsBackground = true };
            thread.Start();
        }

        private static void Update()
        {
            //ExportSlides();
            UpdateToServer();
        }

        private static void ExportSlides()
        {
            string lessonsPath = FileHelper.BasePath;

            foreach (var folder in Directory.GetDirectories(lessonsPath))
            {
                string imagesPath = folder + @"\Images";
                if (!Directory.Exists(imagesPath))
                {
                    PowerpointHelper.SaveSlidesImage(folder + @"\slides.pptx");
                    Thread.Sleep(1000); //buffer
                }
                else
                {
                    //if empty folder
                    var images = Directory.GetFiles(imagesPath, "*.png");

                    if (images == null || images.Length == 0)
                    {
                        PowerpointHelper.SaveSlidesImage(folder + @"\slides.pptx");
                        Thread.Sleep(1000); //buffer
                    }
                }
            }
        }

        public static void UpdateToServer()
        {
            var savedLessons = WebHelper.GetSavedLessons().Select(x => x.Name).ToList();
            var savedLessonSlides = WebHelper.GetLessonSlidesName().Result;

            if (savedLessons != null)
            {
                foreach (var lessonSlide in savedLessonSlides)
                {
                    if (!savedLessons.Contains(lessonSlide))
                    {
                        Save(lessonSlide);
                    }
                }
            }
        }
        private static async void Save(string lessonName)
        {
            var progData = await WebHelper.GetLessonCommands(lessonName);

            var lesson = new Lesson();
            lesson.Id = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            lesson.Name = lessonName;
            lesson.DateModified = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            lesson.Slides = progData.Count;
            lesson.Teacher_Id = progData[0].TeacherId;
            lesson.Subject = progData[0].Subject;

            var lessonSaved = await WebHelper.AddLesson(lesson);

            if (lessonSaved == "true")
            {
                int _questionNumber = 0;
                for (int i = 0; i < progData.Count; i++)
                {
                    var question = progData[i].Question;
                    if (question != null)
                    {
                        question.Id = 1;
                        question.Lesson_Id = lesson.Id;
                        _questionNumber += 1;
                        question.Number = _questionNumber;
                        WebHelper.AddQuestion(question);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Lesson {lesson.Name} not saved!");
            }
        }


        private static async void Save(string lessonFolder, string dateModified)
        {
            string codePath = lessonFolder + @"\code.pptx";
            var lesson = new Lesson();
            lesson.Id = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            lesson.Name = Path.GetFileName(lessonFolder);
            lesson.DateModified = dateModified;
            lesson.Slides = FileHelper.GetLessonSlidesNumber(lesson.Name);

            var slidesData = PowerpointHelper.GetSlidesData(codePath);
            lesson.Teacher_Id = slidesData[0].TeacherId;
            lesson.Subject = slidesData[0].Subject;

            var lessonSaved = await WebHelper.AddLesson(lesson);

            if (lessonSaved == "true")
            {
                int _questionNumber = 0;
                for (int i = 0; i < slidesData.Count; i++)
                {
                    var question = slidesData[i].Question;
                    if (question != null)
                    {
                        question.Id = 1;
                        question.Lesson_Id = lesson.Id;
                        _questionNumber += 1;
                        question.Number = _questionNumber;
                        WebHelper.AddQuestion(question);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Lesson {lesson.Name} not saved!");
            }
        }
        private static void OnSystemUpdated()
        {
            SystemUpdated?.Invoke(null, EventArgs.Empty);
        }
    }
}
