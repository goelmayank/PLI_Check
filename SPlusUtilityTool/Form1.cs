using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public class FigureDimension
        {
            public int Nval;
            public double Xpos;
            public double Ypos;
            public double width;
            public double height;
            public string shape;
            public bool isHorizontal;
        }
        private static string path;
        private static string targetPath;
        private string testFile;
        private string linesXmlPath;
        private string rectanglesXmlPath;
        private XDocument LinesDoc;
        private XDocument RectanglesDoc;
        
        private static List<FigureDimension> LineDimensions = null;
        private static List<FigureDimension> RectangleDimensions = null;
        private int Nval;
        private double width;
        private double height;
        private int LineCount= 0;
        private int RectangleCount = 0;
        private int StartingNValueForCircle;
        private int EndingNValueForCircle;
        private bool stopAddingToBuffer=false;
        private double Xdiff;
        private double Ydiff;
        private double XPlusWidth;
        private double YPlusHeight;
        private double XPlusWidthdiff;
        private double YPlusHeightdiff;
        private bool isXintersecting;
        private bool isYintersecting;
        private bool isXPlusWidthIntersecting;
        private bool isYPlusHeightIntersecting;
        private bool isXInBetween;
        private bool isYInBetween;
        private bool isXintersectingFromClose;
        private bool isYintersectingFromClose;
        private bool isXPlusWidthintersectingFromClose;
        private bool isYPlusHeightintersectingFromClose;
        private bool hasClickedCheck = false;
        private string specifiedDimension;
        public Form1()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This application is developed by Mayank Goel, Intern, ABB Pvt Ltd. Please read the README.htm for more instructions.");
        }

        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(specifiedDimension = PipeDimension.Text))
                MessageBox.Show("Please specify the pipe dimension used in the project");
            else
            {
                hasClickedCheck = true;
                string[] files = Directory.GetFiles(parentDirectoryInput.Text, "*.ucbg");
                linesXmlPath = Path.Combine(targetDirectoryInput.Text, "IntersectingLines.xml");
                rectanglesXmlPath = Path.Combine(targetDirectoryInput.Text, "IntersectingRectangles.xml");
                LinesDoc = new XDocument(new XElement("Lines"));
                RectanglesDoc = new XDocument(new XElement("Rectangles"));
                foreach (var file in files)
                {
                    testFile = Path.GetFileName(file);
                    path = Path.Combine(parentDirectoryInput.Text, testFile);
                    targetPath = Path.Combine(targetDirectoryInput.Text, testFile);
                    File.Copy(file, targetPath, true);
                    findObjects();
                    findIntersectingLines();
                    findIntersectingRectangles();
                    EndingNValueForCircle = Nval;
                    MessageBox.Show("Found " + LineCount + " intersecting lines in "+ testFile);
                    MessageBox.Show("Found " + RectangleCount + " intersecting rectangles in "+ testFile);
                }

            }
        }
        private void findObjects()
        {
            FigureDimension fd = null;
            LineDimensions = new List<FigureDimension>();
            RectangleDimensions = new List<FigureDimension>();

            using (var mappedFile1 = MemoryMappedFile.CreateFromFile(path))
            {
                using (Stream mmStream = mappedFile1.CreateViewStream())
                {
                    using (StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var lineWords = line.Split(' ');
                            if (lineWords[0] == "N")
                            {
                                if (fd != null)
                                {
                                    if (fd.shape == "line") LineDimensions.Add(fd);
                                    else if (fd.shape == "rectangle") RectangleDimensions.Add(fd);
                                }
                                fd = new FigureDimension();
                                fd.Nval = Nval = int.Parse(lineWords[1]);

                            }
                            if (lineWords[0] == "P")
                            {
                                fd.Xpos = double.Parse(lineWords[1]);
                                fd.Ypos = double.Parse(lineWords[2]);
                            }
                            try
                            {
                                //format of string for line recognition:"   0 451"
                                if (
                                    lineWords.Length == 2 &&
                                    lineWords[0].StartsWith("\t") &&
                                    Regex.IsMatch(lineWords[0].Trim(), @"^\d+$") &&
                                    Regex.IsMatch(lineWords[1], @"^\d+$") &&
                                    (width = double.Parse(lineWords[0].Trim())) != (height = double.Parse(lineWords[1])) &&
                                    (width == 0 || height == 0) &&
                                    (
                                     (width > 5 && height < 5) ||
                                     (width < 5 && height > 5)
                                    )
                                   )
                                {
                                    fd.width = width;
                                    fd.height = height;
                                    fd.shape = "line";
                                    if (height == 0)
                                        fd.isHorizontal = true;
                                    else
                                        fd.isHorizontal = false;
                                }

                                //format of string for rectangle recognition:"  0 0 7 451 0 0"
                                if (
                                    lineWords.Length == 6 &&
                                    lineWords[0].StartsWith("\t") &&
                                    Regex.IsMatch(lineWords[0].Trim(), @"^\d+$") &&
                                    Regex.IsMatch(lineWords[1], @"^\d+$") &&
                                    Regex.IsMatch(lineWords[2], @"^\d+$") &&
                                    Regex.IsMatch(lineWords[3], @"^\d+$") &&
                                    Regex.IsMatch(lineWords[4], @"^\d+$") &&
                                    Regex.IsMatch(lineWords[5], @"^\d+$") &&
                                    (lineWords[0].Trim() == "0" && lineWords[1] == "0" && lineWords[4] == "0" && lineWords[5] == "0") &&
                                    (lineWords[2] == specifiedDimension || lineWords[3] == specifiedDimension)
                                   )
                                {
                                    fd.width = double.Parse(lineWords[2]);
                                    fd.height = double.Parse(lineWords[3]);
                                    fd.shape = "rectangle";
                                    if (lineWords[3] == specifiedDimension)
                                        fd.isHorizontal = true;
                                    else
                                        fd.isHorizontal = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        if (fd != null)
                        {
                            if (fd.shape == "line") LineDimensions.Add(fd);
                            else if (fd.shape == "rectangle") RectangleDimensions.Add(fd);
                        }
                    }
                }
            }
        }
        private void findIntersectingLines()
        {
            LinesDoc.Descendants("Lines").FirstOrDefault().RemoveAll();
            LinesDoc.Save(linesXmlPath);

            //INTERSECTION LOGIC FOR LINES
            foreach (var f1 in LineDimensions.FindAll(i => i.isHorizontal == true))
            {
                XPlusWidth = f1.Xpos + f1.width;
                foreach (var f2 in LineDimensions.FindAll(i => i.isHorizontal == false))
                {
                    Xdiff = f1.Xpos - f2.Xpos;
                    Ydiff = f1.Ypos - f2.Ypos;
                    XPlusWidthdiff = XPlusWidth - f2.Xpos;
                    YPlusHeightdiff = YPlusHeight - f2.Xpos;
                    isXintersecting = Math.Abs(Xdiff) > 1 && Math.Abs(Xdiff) < 5;
                    isXPlusWidthIntersecting = Math.Abs(XPlusWidthdiff) > 1 && Math.Abs(XPlusWidthdiff) < 5;
                    isYintersecting = Math.Abs(Ydiff) > 1 && Math.Abs(Ydiff) < 5;
                    isYPlusHeightIntersecting = Math.Abs(YPlusHeightdiff) > 1 && Math.Abs(YPlusHeightdiff) < 5;
                    isXInBetween = f2.Xpos > f1.Xpos && f2.Xpos < XPlusWidth;
                    isYInBetween = f2.Ypos > f1.Ypos && f2.Ypos < YPlusHeight;
                    isXintersectingFromClose = Math.Abs(Xdiff) < 5;
                    isYintersectingFromClose = Math.Abs(Ydiff) < 5;
                    isXPlusWidthintersectingFromClose = Math.Abs(XPlusWidthdiff) < 5;
                    isYPlusHeightintersectingFromClose = Math.Abs(YPlusHeightdiff) < 5;
                    if (isXintersecting && isYintersectingFromClose || isXintersectingFromClose && isYintersecting)
                    {
                        foundIntersectingLines("Top left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYintersectingFromClose || isXPlusWidthintersectingFromClose && isYintersecting)
                    {
                        foundIntersectingLines("Top Right", f1, f2);
                    }
                    else if (isXintersecting && isYPlusHeightintersectingFromClose || isXintersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingLines("Bottom left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYPlusHeightintersectingFromClose || isXPlusWidthintersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingLines("Bottom Right", f1, f2);
                    }
                    else if (isXInBetween && isYintersecting)
                    {
                        foundIntersectingLines("T type", f1, f2);
                    }
                    else if (isXInBetween && isYPlusHeightIntersecting)
                    {
                        foundIntersectingLines("Inverted T type", f1, f2);
                    }
                    else if (isXintersecting && isYInBetween)
                    {
                        foundIntersectingLines("Left Side T type", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYInBetween)
                    {
                        foundIntersectingLines("Right Side T type", f1, f2);
                    }
                }
            }
        }

        private void findIntersectingRectangles()
        {
            RectanglesDoc.Descendants("Rectangles").FirstOrDefault().RemoveAll();
            RectanglesDoc.Save(rectanglesXmlPath);

            //INTERSECTION LOGIC FOR RECTANGLES
            foreach (var f1 in RectangleDimensions.FindAll(i => i.isHorizontal == true))
            {
                XPlusWidth = f1.Xpos + f1.width;
                YPlusHeight = f1.Xpos + f1.width;
                foreach (var f2 in RectangleDimensions.FindAll(i => i.isHorizontal == false))
                {
                    Xdiff = f1.Xpos - f2.Xpos;
                    Ydiff = f1.Ypos - f2.Ypos;
                    XPlusWidthdiff = XPlusWidth - f2.Xpos;
                    YPlusHeightdiff = YPlusHeight - f2.Xpos;
                    isXintersecting = Math.Abs(Xdiff) > 1 && Math.Abs(Xdiff) < 5;
                    isXPlusWidthIntersecting = Math.Abs(XPlusWidthdiff) > 1 && Math.Abs(XPlusWidthdiff) < 5;
                    isYintersecting = Math.Abs(Ydiff) > 1 && Math.Abs(Ydiff) < 5;
                    isYPlusHeightIntersecting = Math.Abs(YPlusHeightdiff) > 1 && Math.Abs(YPlusHeightdiff) < 5;
                    isXInBetween = f2.Xpos > f1.Xpos && f2.Xpos < XPlusWidth;
                    isYInBetween = f2.Ypos > f1.Ypos && f2.Ypos < YPlusHeight;
                    isXintersectingFromClose = Math.Abs(Xdiff) < 5;
                    isYintersectingFromClose = Math.Abs(Ydiff) < 5;
                    isXPlusWidthintersectingFromClose = Math.Abs(XPlusWidthdiff) < 5;
                    isYPlusHeightintersectingFromClose = Math.Abs(YPlusHeightdiff) < 5;
                    if (isXintersecting && isYintersectingFromClose || isXintersectingFromClose && isYintersecting)
                    {
                        foundIntersectingRectangles("Top left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYintersectingFromClose || isXPlusWidthintersectingFromClose && isYintersecting)
                    {
                        foundIntersectingRectangles("Top Right", f1, f2);
                    }
                    else if (isXintersecting && isYPlusHeightintersectingFromClose || isXintersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingRectangles("Bottom left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYPlusHeightintersectingFromClose || isXPlusWidthintersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingRectangles("Bottom Right", f1, f2);
                    }
                    else if (isXInBetween && isYintersecting)
                    {
                        foundIntersectingRectangles("T type", f1, f2);
                    }
                    else if (isXInBetween && isYPlusHeightIntersecting)
                    {
                        foundIntersectingRectangles("Inverted T type", f1, f2);
                    }
                    else if (isXintersecting && isYInBetween)
                    {
                        foundIntersectingRectangles("Left Side T type", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYInBetween)
                    {
                        foundIntersectingRectangles("Right Side T type", f1, f2);
                    }
                }
            }
        }

        private void foundIntersectingLines(string IntersectionType, FigureDimension Hfd, FigureDimension Vfd)
        {
            LinesDoc.Descendants("Lines").FirstOrDefault().Add(new XElement("IntersectingLines",
                new XAttribute("IntersectionType", IntersectionType),
                new XElement("HorizontalLine",
                    new XAttribute("Nval", Hfd.Nval),
                    new XAttribute("Xpos", Hfd.Xpos),
                    new XAttribute("Ypos", Hfd.Ypos),
                    new XAttribute("Width", Hfd.width),
                    new XAttribute("Height", Hfd.height),
                    new XAttribute("shape", Hfd.shape),
                    new XAttribute("isHorizontal", Hfd.isHorizontal)
                ),
                new XElement("VerticalLine",
                    new XAttribute("Nval", Vfd.Nval),
                    new XAttribute("Xpos", Vfd.Xpos),
                    new XAttribute("Ypos", Vfd.Ypos),
                    new XAttribute("Width", Vfd.width),
                    new XAttribute("Height", Vfd.height),
                    new XAttribute("shape", Vfd.shape),
                    new XAttribute("isHorizontal", Vfd.isHorizontal)
                )
            ));

            LinesDoc.Save(linesXmlPath);
            Nval += 2; LineCount++;
            if (LineCount == 1 && LineCount > RectangleCount)
                StartingNValueForCircle = Nval;
            MarkWithACircle(Vfd.Xpos, Vfd.Ypos, Vfd.shape);
        }

        private void foundIntersectingRectangles(string IntersectionType, FigureDimension Hfd, FigureDimension Vfd)
        {
            RectanglesDoc.Descendants("Rectangles").FirstOrDefault().Add(new XElement("IntersectingRectangles",
                new XAttribute("IntersectionType", IntersectionType),
                new XElement("HorizontalRectangle",
                    new XAttribute("Nval", Hfd.Nval),
                    new XAttribute("Xpos", Hfd.Xpos),
                    new XAttribute("Ypos", Hfd.Ypos),
                    new XAttribute("Width", Hfd.width),
                    new XAttribute("Height", Hfd.height),
                    new XAttribute("shape", Hfd.shape),
                    new XAttribute("isHorizontal", Hfd.isHorizontal)
                ),
                new XElement("VerticalRectangle",
                    new XAttribute("Nval", Vfd.Nval),
                    new XAttribute("Xpos", Vfd.Xpos),
                    new XAttribute("Ypos", Vfd.Ypos),
                    new XAttribute("Width", Vfd.width),
                    new XAttribute("Height", Vfd.height),
                    new XAttribute("shape", Vfd.shape),
                    new XAttribute("isHorizontal", Vfd.isHorizontal)
                )
            ));

            RectanglesDoc.Save(rectanglesXmlPath);
            Nval += 2; RectangleCount++;
            if (RectangleCount == 1 && RectangleCount > LineCount)
                StartingNValueForCircle = Nval;
            MarkWithACircle(Vfd.Xpos, Vfd.Ypos, Vfd.shape);
        }
        private void MarkWithACircle(double xpos, double ypos , string shape)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamWriter w = File.AppendText(targetPath))
            {
                sb.AppendLine("N " + Nval);
                sb.AppendLine("P " + (xpos - 13) + " " + (ypos - 13));
                sb.AppendLine("T - 1");
                sb.AppendLine("R 0 0");
                sb.AppendLine("0");
                sb.AppendLine("\t0 5 7 0");
                sb.AppendLine("\t Name C"+((shape=="line")?"L"+LineCount:"R"+RectangleCount));
                sb.AppendLine("\t0 1 1");
                sb.AppendLine("!");
                sb.AppendLine("2fe");
                sb.AppendLine("-10000");
                sb.AppendLine("283e3e");
                sb.AppendLine("0");
                sb.AppendLine("0");
                sb.AppendLine("0");
                sb.AppendLine("0 0");
                sb.AppendLine("1");
                sb.AppendLine("\t0 0 26 26");
                sb.AppendLine("0 23040");
                w.WriteLine(sb.ToString());
            }
        }

        private void rectifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(hasClickedCheck == true)
            {
                StringBuilder sb = new StringBuilder();
                using (var mappedFile1 = MemoryMappedFile.CreateFromFile(targetPath))
                {
                    using (Stream mmStream = mappedFile1.CreateViewStream())
                    {
                        using (StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                        {
                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine();
                                var lineWords = line.Split(' ');
                                if (lineWords[0] == "N" && Regex.IsMatch(lineWords[1], @"^\d+$") && int.Parse(lineWords[1]) >= StartingNValueForCircle)
                                {
                                    stopAddingToBuffer = true;
                                }
                                if (!stopAddingToBuffer)
                                {
                                    sb.Append(line + "\r\n");
                                }
                            }
                        }
                    }
                }

                File.WriteAllText(targetPath, sb.ToString());
                MessageBox.Show("Removed all circles");
            }
            else
            {
                MessageBox.Show("No operation performed. Files not yet checked");
            }
        }

        private void UpdatePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            // This is what will execute if the user selects a folder and hits OK (File if you change to FileBrowserDialog)
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                parentDirectoryInput.Text = dlg.SelectedPath;
            }
            else
            {
                // This prevents a crash when you close out of the window with nothing
            }
        }

        private void UpdateTargetPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            // This is what will execute if the user selects a folder and hits OK (File if you change to FileBrowserDialog)
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                targetDirectoryInput.Text = dlg.SelectedPath;
            }
            else
            {
                // This prevents a crash when you close out of the window with nothing
            }
        }
    }
}
