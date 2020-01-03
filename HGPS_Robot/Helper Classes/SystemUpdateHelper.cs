
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
        public static void Start()
        {
            ThreadStart starter = Update;
            starter += () =>
            {
                MessageBox.Show("System updated");
            };
            Thread thread = new Thread(starter) { IsBackground = true };
            thread.Start();
        }

        private static void Update()
        {
            ExportSlides();
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
            var savedLessons = WebHelper.GetSavedLessons();

            string lessonsPath = FileHelper.BasePath;

            foreach (var folder in Directory.GetDirectories(lessonsPath))
            {
                string lessonName = Path.GetFileName(folder);
                string codePath = folder + @"\code.pptx";
                if (string.IsNullOrEmpty(codePath)) continue;
                if (!File.Exists(codePath)) continue;

                FileInfo fileInfo = new FileInfo(codePath);
                var lastModified = fileInfo.LastWriteTime.ToString("dd/MM/yyyy hh:mm:ss tt");

                if (savedLessons != null)
                {
                    int index = savedLessons.FindIndex(s => s.Name == lessonName);
                    if (index >= 0) //saved
                    {
                        if (savedLessons[index].DateModified != lastModified)
                        {
                            //update - delete and save
                            WebHelper.DeleteLesson(lessonName);
                            Save(folder, lastModified);
                        }
                    }
                    else //not saved
                    {
                        Save(folder, lastModified);
                    }
                }
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

            if (lessonSaved)
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
    }
}
