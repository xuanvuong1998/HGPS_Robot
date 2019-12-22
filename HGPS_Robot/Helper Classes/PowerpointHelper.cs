using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace HGPS_Robot
{
    public class PowerpointHelper
    {
        public static void SaveSlidesImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please provide a file path");
                return;
            }
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Could not find file: {filePath}");
                return;
            }
            if (!filePath.Contains(".ppt"))
            {
                MessageBox.Show("Please select a valid PowerPoint file (.ppt or .pptx)");
                return;
            }

            var pptApplication = new PowerPoint.Application();
            var pptPresentation = pptApplication.Presentations.Open(filePath);
            var directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath + @"\Images");

            foreach (PowerPoint.Slide pptSlide in pptPresentation.Slides)
            {
                string fileName = directoryPath + $@"\Images\{pptSlide.SlideIndex.ToString()}.png";
                pptSlide.Export(fileName, "PNG", 1366, 768);
            }
            //ClosePpt(pptApplication, pptPresentation);
            ForceClosePpt();
        }
        public static List<Image> GetSlidesImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please provide a file path");
                return null;
            }
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Could not find file: {filePath}");
                return null;
            }
            if (!filePath.Contains(".ppt"))
            {
                MessageBox.Show("Please select a valid PowerPoint file (.ppt or .pptx)");
                return null;
            }

            var pptApplication = new PowerPoint.Application();
            var pptPresentation = pptApplication.Presentations.Open(filePath);
            var images = new List<Image>();

            foreach (PowerPoint.Slide pptSlide in pptPresentation.Slides)
            {
                string pathString = Application.StartupPath + @"\TempData";
                if (!File.Exists(pathString)) Directory.CreateDirectory(pathString);
                //Unable to use the same name repeatedly as object is not released
                string file = pathString + $@"\Slide{pptSlide.SlideIndex.ToString()}.png";
                pptSlide.Export(file, "PNG", 1366, 768);
                images.Add(Image.FromFile(file));
            }
            ClosePpt(pptApplication, pptPresentation);
            return images;
        }
        public static List<string> GetSlidesImageInString(string filePath)
        {
            var imagesInString = new List<string>();
            var images = GetSlidesImage(filePath);

            foreach (var img in images)
            {
                var imgString = ImageToString(img);
                imagesInString.Add(imgString);
            }
            return imagesInString;
        }
        public static List<RobotProgSlide> GetSlidesData(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please provide a file path");
                return null;
            }
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Could not find file: {filePath}");
                return null;
            }
            if (!filePath.Contains(".ppt"))
            {
                MessageBox.Show("Please select a valid PowerPoint file (.ppt or .pptx)");
                return null;
            }

            var robotSlides = new List<RobotProgSlide>();
            var pptApplication = new PowerPoint.Application();
            var pptPresentations = pptApplication.Presentations;
            var pptPresentation = pptPresentations.Open(filePath, MsoTriState.msoTrue,
                                                        MsoTriState.msoFalse, MsoTriState.msoFalse);
            var slides = pptPresentation.Slides;

            if (slides != null)
            {
                var slidesCount = slides.Count;
                if (slidesCount > 0)
                {
                    for (int slideIndex = 1; slideIndex <= slidesCount; slideIndex++)
                    {
                        var slide = slides[slideIndex];
                        var robotSlide = new RobotProgSlide();
                        string robotCode = null;
                        foreach (PowerPoint.Shape textShape in slide.Shapes)
                        {
                            if (textShape.HasTextFrame == MsoTriState.msoTrue &&
                                textShape.TextFrame.HasText == MsoTriState.msoTrue)
                            {
                                PowerPoint.TextRange pptTextRange = textShape.TextFrame.TextRange;
                                if (pptTextRange != null && pptTextRange.Length > 0)
                                {
                                    robotCode += pptTextRange.Text;
                                    Marshal.ReleaseComObject(pptTextRange);
                                }
                            }
                            else if (textShape.Type == MsoShapeType.msoPicture)
                            {
                                string pathString = Application.StartupPath + @"\TempData";
                                if (!File.Exists(pathString)) Directory.CreateDirectory(pathString);
                                string file = pathString + $@"\Image{slideIndex.ToString()}.png";
                                object[] x = new object[] { file, PowerPoint.PpShapeFormat.ppShapeFormatJPG,
                                                                0, 0, PowerPoint.PpExportMode.ppScaleXY };
                                textShape.GetType().InvokeMember("Export", System.Reflection.BindingFlags.InvokeMethod, null, textShape, x);
                                robotSlide.Image = Image.FromFile(file);
                            }
                            Marshal.ReleaseComObject(textShape);
                        }
                        robotCode = robotCode.Replace("\x0B", String.Empty);
                        robotCode = robotCode.Replace("\x11", String.Empty);
                        robotCode = robotCode.Replace("\r", String.Empty);
                        robotCode = robotCode.Replace("\n", String.Empty);
                        robotSlide.Code = robotCode;
                        robotSlides.Add(robotSlide);
                    }
                }
            }
            ClosePpt(pptApplication, pptPresentation);
            return robotSlides;
        }
        public static string ImageToString(Image image)
        { 
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                var bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
        public static Image StringToImage(string imageString)
        {
            var bytes = Convert.FromBase64String(imageString);
            var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }
        public static void ForceClosePpt()
        {
            Process[] pros = Process.GetProcesses();
            for (int i = 0; i < pros.Count(); i++)
            {
                if (pros[i].ProcessName.ToLower().Contains("powerpnt"))
                {
                    pros[i].Kill();
                }
            }
        }
        private static void ClosePpt(PowerPoint.Application application, PowerPoint.Presentation presentation)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(presentation);
            application.Quit();
            Marshal.ReleaseComObject(application);
        }
    }
}
