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
    public partial class MainForm : Form
    {
        public class FigureDimension
        {
            public int nVal;
            public int Xpos;
            public int Ypos;
            public int width;
            public int height;
            public string shape;
            public bool isHorizontal;
            //public bool nameIsSet;
        }
        private static string path;
        private static string targetPath;
        private string testFile;
        
        private static List<FigureDimension> LineDimensions = null;
        private static List<FigureDimension> PipeDimensions = null;
        private int nVal;
        private int width;
        private int height;
        private int intersectingLinesCount;
        private int intersectingPipesCount;
        private int startingNValueForCircle;
        private bool stopAddingToBuffer;
        private int xDiff;
        private int yDiff;
        private int xPlusWidth;
        private int yPlusHeight;
        private int xPlusWidthdiff;
        private int yPlusHeightdiff;
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
        private string pipeDimension;
        private int lineWidth;
        //private bool nextLineShouldBeName = false;
        private int linesCount;
        private int count;
        public MainForm()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This application is developed by Mayank Goel, Intern, ABB Pvt Ltd. Please read the README.htm for more instructions.");
        }

        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pipeDimension = fixedPipeDimension.Text))
                MessageBox.Show("Please specify the pipe dimension used in the project");
            if (string.IsNullOrEmpty(fixedLineWidth.Text))
                MessageBox.Show("Please specify the line width used in the project");
            else if (string.IsNullOrEmpty(parentDirectoryInput.Text))
                MessageBox.Show("Please specify the parent directory used in the project");
            else if (string.IsNullOrEmpty(targetDirectoryInput.Text))
                MessageBox.Show("Please specify the target directory used in the project");
            else
            {
                hasClickedCheck = true;
                lineWidth = int.Parse(fixedLineWidth.Text) - 1;
                string[] files = Directory.GetFiles(parentDirectoryInput.Text, "*.ucbg");
                foreach (var file in files)
                {
                    InitializeClassVariables(file);
                    if (File.Exists(targetPath)) File.Delete(targetPath);
                    identifyLinesAndPipes();
                    findIntersectingLines();
                    findIntersectingPipes();
                }
            }
            MessageBox.Show("Checking Complete");
        }
        
        private void InitializeClassVariables(string file)
        {

            intersectingLinesCount = 0;
            intersectingPipesCount = 0;
            testFile = Path.GetFileName(file);
            path = Path.Combine(parentDirectoryInput.Text, testFile);
            targetPath = Path.Combine(targetDirectoryInput.Text, testFile);
        }

        private void identifyLinesAndPipes()
        {
            FigureDimension fd = null;
            LineDimensions = new List<FigureDimension>();
            PipeDimensions = new List<FigureDimension>();
            StringBuilder sb = new StringBuilder();
            string[] strArr;
            linesCount = 0;
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
                                    if (fd.shape == "line")
                                    {
                                        //check line width(line no. 3 from bottom)
                                        strArr = sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                                        int len = strArr.Length;
                                        if (strArr[len - 4].Trim() == lineWidth.ToString())
                                        {
                                            LineDimensions.Add(fd);
                                            //if (fd.nameIsSet == false)
                                            //{
                                            //    strArr[6] = "	Name LINE" + ++linesCount;
                                            //    fd.nameIsSet = true;
                                            //    sb = new StringBuilder();
                                            //    for (int i = 0; i < strArr.Length; i++)
                                            //        sb.Append(strArr[i] + "\r\n");
                                            //}
                                        }
                                    }
                                    else if (fd.shape == "pipe") PipeDimensions.Add(fd);
                                }
                                File.AppendAllText(targetPath, sb.ToString());
                                if (lineWords[0] == "P")
                                {

                                }
                                sb = new StringBuilder();
                                fd = new FigureDimension();
                                fd.nVal = nVal = int.Parse(lineWords[1]);

                            }

                            if (lineWords[0] == "P")
                            {
                                fd.Xpos = (int)Math.Round(double.Parse(lineWords[1]), MidpointRounding.AwayFromZero);
                                fd.Ypos = (int)Math.Round(double.Parse(lineWords[2]), MidpointRounding.AwayFromZero);
                            }
                            
                            //format of string for line recognition:"	0 0 1567 0"
                            //if (
                            //    lineWords.Length == 4 &&
                            //    lineWords[0].StartsWith("\t") &&
                            //    Regex.IsMatch(lineWords[0].Trim(), @"^\d+$") &&
                            //    Regex.IsMatch(lineWords[1], @"^\d+$") &&
                            //    Regex.IsMatch(lineWords[2], @"^\d+$") &&
                            //    Regex.IsMatch(lineWords[3], @"^\d+$") &&
                            //    (lineWords[0].Trim() == "0" && lineWords[1] == "0" && lineWords[2] != "0" && lineWords[3] == "0")
                            //   )
                            //{
                            //    sb.Append(line + "\r\n");
                            //    //go to the next line
                            //    nextLineShouldBeName = true;
                            //    continue;
                            //}

                            //if(nextLineShouldBeName == true)
                            //{
                            //    //format of string for line recognition:"	Name L1"
                            //    if (lineWords[0].Trim() != "Name")
                            //    {
                            //        fd.nameIsSet = false;
                            //        sb.Append("\r\n");
                            //    }
                            //    else
                            //    {
                            //        fd.nameIsSet = true;
                            //    }
                            //    nextLineShouldBeName = false;
                            //}

                            //format of string for line recognition:"   0 451"
                            if (
                                lineWords.Length == 2 &&
                                lineWords[0].StartsWith("\t") &&
                                Regex.IsMatch(lineWords[0].Trim(), @"^\d+$") &&
                                Regex.IsMatch(lineWords[1], @"^\d+$") &&
                                (width = (int)Math.Round(double.Parse(lineWords[0].Trim()), MidpointRounding.AwayFromZero)) != (height = (int)Math.Round(double.Parse(lineWords[1]), MidpointRounding.AwayFromZero)) &&
                                (
                                    (width > 5 && height == 0) ||
                                    (width == 0 && height > 5)
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

                            //format of string for pipe recognition:"  0 0 7 451 0 0"
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
                                (lineWords[2] == pipeDimension || lineWords[3] == pipeDimension)
                               )
                            {
                                fd.width = (int)Math.Round(double.Parse(lineWords[2]), MidpointRounding.AwayFromZero);
                                fd.height = (int)Math.Round(double.Parse(lineWords[3]), MidpointRounding.AwayFromZero);
                                fd.shape = "pipe";
                                if (lineWords[3] == pipeDimension)
                                    fd.isHorizontal = true;
                                else
                                    fd.isHorizontal = false;
                            }
                            sb.Append(line + "\r\n");
                        }
                        startingNValueForCircle = nVal + 2;
                        if (fd != null)
                        {
                            if (fd.shape == "line")
                            {
                                //iterate backwards to set name(line no. 15 from bottom) and line width(line no. 9 from bottom)
                                strArr = sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                                int len = strArr.Length;
                                if (strArr[len-4].Trim() == lineWidth.ToString())
                                {
                                    LineDimensions.Add(fd);
                                    //if (fd.nameIsSet == false)
                                    //{
                                    //    strArr[6] = "	Name LINE" + ++linesCount;
                                    //    fd.nameIsSet = true;
                                    //    sb = new StringBuilder();
                                    //    for (int i = 0; i < strArr.Length; i++)
                                    //        sb.Append(strArr[i] + "\r\n");
                                    //}
                                }
                            }
                            else if (fd.shape == "pipe") PipeDimensions.Add(fd);
                        }
                        //File.AppendAllText(targetPath, sb.ToString());
                    }
                }
            }
        }
                
        private void findIntersectingLines()
        {
            foreach (var f1 in LineDimensions.FindAll(i => i.isHorizontal == true))
            {
                xPlusWidth = f1.Xpos + f1.width;
                foreach (var f2 in LineDimensions.FindAll(i => i.isHorizontal == false))
                {
                    applyIntersectionLogic(f1, f2);
                }
            }
        }

        private void findIntersectingPipes()
        {
            foreach (var f1 in PipeDimensions.FindAll(i => i.isHorizontal == true))
            {
                xPlusWidth = f1.Xpos + f1.width;
                foreach (var f2 in PipeDimensions.FindAll(i => i.isHorizontal == false))
                {
                    applyIntersectionLogic(f1, f2);
                }
            }
        }

        private void applyIntersectionLogic(FigureDimension f1, FigureDimension f2)
        {
            xDiff = f1.Xpos - f2.Xpos;
            yDiff = f1.Ypos - f2.Ypos;
            yPlusHeight = f2.Ypos + f2.height;
            xPlusWidthdiff = xPlusWidth - f2.Xpos;
            yPlusHeightdiff = yPlusHeight - f1.Ypos;
            isXIntersecting = Math.Abs(xDiff) >= 1 && Math.Abs(xDiff) < 5;
            isXPlusWidthIntersecting = Math.Abs(xPlusWidthdiff) >= 1 && Math.Abs(xPlusWidthdiff) < 5;
            isYIntersecting = Math.Abs(yDiff) >= 1 && Math.Abs(yDiff) < 5;
            isYPlusHeightIntersecting = Math.Abs(yPlusHeightdiff) >= 1 && Math.Abs(yPlusHeightdiff) < 5;
            isXInBetween = f2.Xpos > f1.Xpos && f2.Xpos < xPlusWidth;
            isYInBetween = f2.Ypos > f1.Ypos && f2.Ypos < yPlusHeight;
            isXIntersectingFromClose = Math.Abs(xDiff) < 5;
            isYIntersectingFromClose = Math.Abs(yDiff) < 5;
            isXPlusWidthIntersectingFromClose = Math.Abs(xPlusWidthdiff) < 5;
            isYPlusHeightIntersectingFromClose = Math.Abs(yPlusHeightdiff) < 5;
            if (isXIntersecting && isYIntersectingFromClose || isXIntersectingFromClose && isYIntersecting)
            {
                //Top left
                MarkWithACircle(f1.Xpos, f1.Ypos, f1.shape);
            }
            else if (isXPlusWidthIntersecting && isYIntersectingFromClose || isXPlusWidthIntersectingFromClose && isYIntersecting)
            {
                //Top Right
                MarkWithACircle(xPlusWidth, f1.Ypos, f1.shape);
            }
            else if (isXIntersecting && isYPlusHeightIntersectingFromClose || isXIntersectingFromClose && isYPlusHeightIntersecting)
            {
                //Bottom left
                MarkWithACircle(f2.Xpos, yPlusHeight, f1.shape);
            }
            else if (isXPlusWidthIntersecting && isYPlusHeightIntersectingFromClose || isXPlusWidthIntersectingFromClose && isYPlusHeightIntersecting)
            {
                //Bottom Right
                MarkWithACircle(xPlusWidth, yPlusHeight, f1.shape);
            }
            else if (isXInBetween && isYIntersecting)
            {
                //T type
                MarkWithACircle(f2.Xpos, f2.Ypos, f1.shape);
            }
            else if (isXInBetween && isYPlusHeightIntersecting)
            {
                //Inverted T type
                MarkWithACircle(f2.Xpos, yPlusHeight, f1.shape);
            }
            else if (isXIntersecting && isYInBetween)
            {
                //Left Side T type
                MarkWithACircle(f1.Xpos, f1.Ypos, f1.shape);
            }
            else if (isXPlusWidthIntersecting && isYInBetween)
            {
                //Right Side T type
                MarkWithACircle(xPlusWidth, f1.Ypos, f1.shape);
            }
        }

        private void MarkWithACircle(double xpos, double ypos , string shape)
        {
            if (shape == "line")
            {
                intersectingLinesCount++;
                count = intersectingLinesCount;
            }
            else
            {
                intersectingPipesCount++;
                count = intersectingPipesCount;
            }
            StringBuilder sb = new StringBuilder();
            if (intersectingLinesCount + intersectingPipesCount == 1) sb.AppendLine("N 999999999999999999");
            using (StreamWriter w = File.AppendText(targetPath))
            {
                sb.AppendLine("N " + (startingNValueForCircle + (2*intersectingLinesCount) + (2* intersectingPipesCount) -2));
                sb.AppendLine("P " + (xpos - 13) + " " + (ypos - 13));
                sb.AppendLine("T - 1");
                sb.AppendLine("R 0 0");
                sb.AppendLine("0");
                sb.AppendLine("\t0 5 7 0");
                sb.AppendLine("\t Name TEST_C" + count);
                sb.AppendLine("\t0 1 1");
                sb.AppendLine("!");
                sb.AppendLine("ffffffff");
                sb.AppendLine("0");
                sb.AppendLine("-10000");
                sb.AppendLine("c0c0c0");
                sb.AppendLine("0");
                sb.AppendLine("0");
                sb.AppendLine("0");
                sb.AppendLine("0 0");
                sb.AppendLine("1");
                sb.AppendLine("\tDefault");
                sb.AppendLine("\t0 0 26 26");
                sb.AppendLine("0 23040");

                w.WriteLine(sb.ToString());
            }
        }

        private void rectifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hasClickedCheck == true)
            {
                string[] files = Directory.GetFiles(parentDirectoryInput.Text, "*.ucbg");
                foreach (var file in files)
                {
                    stopAddingToBuffer = false;
                    testFile = Path.GetFileName(file);
                    targetPath = Path.Combine(targetDirectoryInput.Text, testFile);
                    if (intersectingLinesCount == 0 && intersectingPipesCount == 0)
                    {
                        MessageBox.Show("No circles found in " + testFile);
                    }
                    else
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
                                        if (line == "N 999999999999999999")
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
                    }
                }
                MessageBox.Show("Rectification Complete");
            }
            else
            {
                MessageBox.Show("No operation performed. Files not yet checked");
            }
        }

        private void UpdatePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                parentDirectoryInput.Text = dlg.SelectedPath;
            }
        }

        private void UpdateTargetPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                targetDirectoryInput.Text = dlg.SelectedPath;
            }
        }

    }
}
