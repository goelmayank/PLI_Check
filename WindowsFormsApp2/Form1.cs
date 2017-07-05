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
            public int nVal;
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
        private XDocument linesDoc;
        private XDocument rectanglesDoc;
        
        private static List<FigureDimension> LineDimensions = null;
        private static List<FigureDimension> RectangleDimensions = null;
        private int nVal;
        private double width;
        private double height;
        private int intersectingLinesCount;
        private int intersectingRectanglesCount;
        private int startingNValueForCircle;
        private int endingNValueForCircle;
        private bool stopAddingToBuffer;
        private double xDiff;
        private double yDiff;
        private double xPlusWidth;
        private double yPlusHeight;
        private double xPlusWidthdiff;
        private double yPlusHeightdiff;
        private bool isXIntersecting;
        private bool isYIntersecting;
        private bool isXPlusWidthIntersecting;
        private bool isYPlusHeightIntersecting;
        private bool isXInBetween;
        private bool isYInBetween;
        private bool isXIntersectingFromClose;
        private bool isYIntersectingFromClose;
        private bool isXPlusWidthIntersectingFromClose;
        private bool isYPlusHeightIntersectingFromClose;
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
                InitializeClassVariables();
                string[] files = Directory.GetFiles(parentDirectoryInput.Text, "*.ucbg");
                foreach (var file in files)
                {
                    testFile = Path.GetFileName(file);
                    path = Path.Combine(parentDirectoryInput.Text, testFile);
                    targetPath = Path.Combine(targetDirectoryInput.Text, testFile);
                    File.Copy(file, targetPath, true);
                    findObjects();
                    findIntersectingLines();
                    findIntersectingRectangles();
                    endingNValueForCircle = nVal;
                    MessageBox.Show("Found " + intersectingLinesCount + " intersecting lines in "+ testFile);
                    MessageBox.Show("Found " + intersectingRectanglesCount + " intersecting rectangles in "+ testFile);
                }

            }
        }
        private void InitializeClassVariables()
        {
            intersectingLinesCount = 0;
            intersectingRectanglesCount = 0;
            stopAddingToBuffer = false;
            hasClickedCheck = true;
            linesXmlPath = Path.Combine(targetDirectoryInput.Text, "IntersectingLines.xml");
            rectanglesXmlPath = Path.Combine(targetDirectoryInput.Text, "IntersectingRectangles.xml");
            linesDoc = new XDocument(new XElement("Lines"));
            rectanglesDoc = new XDocument(new XElement("Rectangles"));
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
                                fd.nVal = nVal = int.Parse(lineWords[1]);

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
            linesDoc.Descendants("Lines").FirstOrDefault().RemoveAll();
            linesDoc.Save(linesXmlPath);

            //INTERSECTION LOGIC FOR LINES
            foreach (var f1 in LineDimensions.FindAll(i => i.isHorizontal == true))
            {
                xPlusWidth = f1.Xpos + f1.width;
                foreach (var f2 in LineDimensions.FindAll(i => i.isHorizontal == false))
                {
                    xDiff = f1.Xpos - f2.Xpos;
                    yDiff = f1.Ypos - f2.Ypos;
                    yPlusHeight = f2.Ypos + f2.height;
                    xPlusWidthdiff = xPlusWidth - f2.Xpos;
                    yPlusHeightdiff = yPlusHeight - f1.Ypos;
                    isXIntersecting = Math.Abs(xDiff) > 1 && Math.Abs(xDiff) < 5;
                    isXPlusWidthIntersecting = Math.Abs(xPlusWidthdiff) > 1 && Math.Abs(xPlusWidthdiff) < 5;
                    isYIntersecting = Math.Abs(yDiff) > 1 && Math.Abs(yDiff) < 5;
                    isYPlusHeightIntersecting = Math.Abs(yPlusHeightdiff) > 1 && Math.Abs(yPlusHeightdiff) < 5;
                    isXInBetween = f2.Xpos > f1.Xpos && f2.Xpos < xPlusWidth;
                    isYInBetween = f2.Ypos > f1.Ypos && f2.Ypos < yPlusHeight;
                    isXIntersectingFromClose = Math.Abs(xDiff) < 5;
                    isYIntersectingFromClose = Math.Abs(yDiff) < 5;
                    isXPlusWidthIntersectingFromClose = Math.Abs(xPlusWidthdiff) < 5;
                    isYPlusHeightIntersectingFromClose = Math.Abs(yPlusHeightdiff) < 5;
                    if (isXIntersecting && isYIntersectingFromClose || isXIntersectingFromClose && isYIntersecting)
                    {
                        foundIntersectingLines("Top left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYIntersectingFromClose || isXPlusWidthIntersectingFromClose && isYIntersecting)
                    {
                        foundIntersectingLines("Top Right", f1, f2);
                    }
                    else if (isXIntersecting && isYPlusHeightIntersectingFromClose || isXIntersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingLines("Bottom left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYPlusHeightIntersectingFromClose || isXPlusWidthIntersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingLines("Bottom Right", f1, f2);
                    }
                    else if (isXInBetween && isYIntersecting)
                    {
                        foundIntersectingLines("T type", f1, f2);
                    }
                    else if (isXInBetween && isYPlusHeightIntersecting)
                    {
                        foundIntersectingLines("Inverted T type", f1, f2);
                    }
                    else if (isXIntersecting && isYInBetween)
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
            rectanglesDoc.Descendants("Rectangles").FirstOrDefault().RemoveAll();
            rectanglesDoc.Save(rectanglesXmlPath);

            //INTERSECTION LOGIC FOR RECTANGLES
            foreach (var f1 in RectangleDimensions.FindAll(i => i.isHorizontal == true))
            {
                xPlusWidth = f1.Xpos + f1.width;
                foreach (var f2 in RectangleDimensions.FindAll(i => i.isHorizontal == false))
                {
                    xDiff = f1.Xpos - f2.Xpos;
                    yDiff = f1.Ypos - f2.Ypos;
                    yPlusHeight = f2.Ypos + f2.height;
                    xPlusWidthdiff = xPlusWidth - f2.Xpos;
                    yPlusHeightdiff = yPlusHeight - f1.Ypos;
                    isXIntersecting = Math.Abs(xDiff) > 1 && Math.Abs(xDiff) < 5;
                    isXPlusWidthIntersecting = Math.Abs(xPlusWidthdiff) > 1 && Math.Abs(xPlusWidthdiff) < 5;
                    isYIntersecting = Math.Abs(yDiff) > 1 && Math.Abs(yDiff) < 5;
                    isYPlusHeightIntersecting = Math.Abs(yPlusHeightdiff) > 1 && Math.Abs(yPlusHeightdiff) < 5;
                    isXInBetween = f2.Xpos > f1.Xpos && f2.Xpos < xPlusWidth;
                    isYInBetween = f2.Ypos > f1.Ypos && f2.Ypos < yPlusHeight;
                    isXIntersectingFromClose = Math.Abs(xDiff) < 5;
                    isYIntersectingFromClose = Math.Abs(yDiff) < 5;
                    isXPlusWidthIntersectingFromClose = Math.Abs(xPlusWidthdiff) < 5;
                    isYPlusHeightIntersectingFromClose = Math.Abs(yPlusHeightdiff) < 5;
                    if (isXIntersecting && isYIntersectingFromClose || isXIntersectingFromClose && isYIntersecting)
                    {
                        foundIntersectingRectangles("Top left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYIntersectingFromClose || isXPlusWidthIntersectingFromClose && isYIntersecting)
                    {
                        foundIntersectingRectangles("Top Right", f1, f2);
                    }
                    else if (isXIntersecting && isYPlusHeightIntersectingFromClose || isXIntersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingRectangles("Bottom left", f1, f2);
                    }
                    else if (isXPlusWidthIntersecting && isYPlusHeightIntersectingFromClose || isXPlusWidthIntersectingFromClose && isYPlusHeightIntersecting)
                    {
                        foundIntersectingRectangles("Bottom Right", f1, f2);
                    }
                    else if (isXInBetween && isYIntersecting)
                    {
                        foundIntersectingRectangles("T type", f1, f2);
                    }
                    else if (isXInBetween && isYPlusHeightIntersecting)
                    {
                        foundIntersectingRectangles("Inverted T type", f1, f2);
                    }
                    else if (isXIntersecting && isYInBetween)
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
            linesDoc.Descendants("Lines").FirstOrDefault().Add(new XElement("IntersectingLines",
                new XAttribute("IntersectionType", IntersectionType),
                new XElement("HorizontalLine",
                    new XAttribute("nVal", Hfd.nVal),
                    new XAttribute("Xpos", Hfd.Xpos),
                    new XAttribute("Ypos", Hfd.Ypos),
                    new XAttribute("Width", Hfd.width),
                    new XAttribute("Height", Hfd.height),
                    new XAttribute("shape", Hfd.shape),
                    new XAttribute("isHorizontal", Hfd.isHorizontal)
                ),
                new XElement("VerticalLine",
                    new XAttribute("nVal", Vfd.nVal),
                    new XAttribute("Xpos", Vfd.Xpos),
                    new XAttribute("Ypos", Vfd.Ypos),
                    new XAttribute("Width", Vfd.width),
                    new XAttribute("Height", Vfd.height),
                    new XAttribute("shape", Vfd.shape),
                    new XAttribute("isHorizontal", Vfd.isHorizontal)
                )
            ));

            linesDoc.Save(linesXmlPath);
            nVal += 2; intersectingLinesCount++;
            if (intersectingLinesCount == 1 && intersectingLinesCount > intersectingRectanglesCount)
                startingNValueForCircle = nVal;
            MarkWithACircle(Vfd.Xpos, Vfd.Ypos, Vfd.shape);
        }

        private void foundIntersectingRectangles(string IntersectionType, FigureDimension Hfd, FigureDimension Vfd)
        {
            rectanglesDoc.Descendants("Rectangles").FirstOrDefault().Add(new XElement("IntersectingRectangles",
                new XAttribute("IntersectionType", IntersectionType),
                new XElement("HorizontalRectangle",
                    new XAttribute("nVal", Hfd.nVal),
                    new XAttribute("Xpos", Hfd.Xpos),
                    new XAttribute("Ypos", Hfd.Ypos),
                    new XAttribute("Width", Hfd.width),
                    new XAttribute("Height", Hfd.height),
                    new XAttribute("shape", Hfd.shape),
                    new XAttribute("isHorizontal", Hfd.isHorizontal)
                ),
                new XElement("VerticalRectangle",
                    new XAttribute("nVal", Vfd.nVal),
                    new XAttribute("Xpos", Vfd.Xpos),
                    new XAttribute("Ypos", Vfd.Ypos),
                    new XAttribute("Width", Vfd.width),
                    new XAttribute("Height", Vfd.height),
                    new XAttribute("shape", Vfd.shape),
                    new XAttribute("isHorizontal", Vfd.isHorizontal)
                )
            ));

            rectanglesDoc.Save(rectanglesXmlPath);
            nVal += 2; intersectingRectanglesCount++;
            if (intersectingRectanglesCount == 1 && intersectingRectanglesCount > intersectingLinesCount)
                startingNValueForCircle = nVal;
            MarkWithACircle(Vfd.Xpos, Vfd.Ypos, Vfd.shape);
        }
        private void MarkWithACircle(double xpos, double ypos , string shape)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamWriter w = File.AppendText(targetPath))
            {
                sb.AppendLine("N " + nVal);
                sb.AppendLine("P " + (xpos - 13) + " " + (ypos - 13));
                sb.AppendLine("T - 1");
                sb.AppendLine("R 0 0");
                sb.AppendLine("0");
                sb.AppendLine("\t0 5 7 0");
                sb.AppendLine("\t Name C"+((shape=="line")?"L"+intersectingLinesCount:"R"+intersectingRectanglesCount));
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
                                if (lineWords[0] == "N" && Regex.IsMatch(lineWords[1], @"^\d+$") && int.Parse(lineWords[1]) >= startingNValueForCircle)
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
