using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    /// <summary>
    /// Main Form
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
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
        }
        public class UCBG_File
        {
            public string name;
            public int startingNValueForCircle;
        }
        private static string path;
        private static string targetPath;
        private string testFile;

        private static List<FigureDimension> LineDimensions = null;
        private static List<FigureDimension> PipeDimensions = null;
        private static List<UCBG_File> ucbg_files = null;
        private UCBG_File ucbg_file;
        private int nVal;
        private int width;
        private int height;
        private int intersectingLinesCount;
        private int intersectingPipesCount;
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
        private int count;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the aboutToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This sample is developed by IAPG, ABB. Please read the Readme.docx for more details");
        }

        /// <summary>
        /// Handles the Click event of the checkToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(parentDirectoryInput.Text) || !Directory.Exists(parentDirectoryInput.Text))
                MessageBox.Show("Please specify the parent directory used in the project");
            else if (string.IsNullOrEmpty(targetDirectoryInput.Text) || !Directory.Exists(targetDirectoryInput.Text))
                MessageBox.Show("Please specify the target directory used in the project");
            else
            {
                hasClickedCheck = true;

                string[] files = Directory.GetFiles(parentDirectoryInput.Text, "*.ucbg");
                ucbg_files = new List<UCBG_File>();
                foreach (var file in files)
                {
                    ucbg_file = new UCBG_File();
                    ucbg_file.name = file;
                    InitializeClassVariables(file);
                    if (File.Exists(targetPath)) File.Delete(targetPath);
                    identifyLinesAndPipes();
                    findIntersectingLines();
                }
                MessageBox.Show("Checking Complete");
            }
        }

        /// <summary>
        /// Initializes the class variables.
        /// </summary>
        /// <param name="file">The file.</param>
        private void InitializeClassVariables(string file)
        {

            intersectingLinesCount = 0;
            intersectingPipesCount = 0;
            testFile = Path.GetFileName(file);
            path = Path.Combine(parentDirectoryInput.Text, testFile);
            targetPath = Path.Combine(targetDirectoryInput.Text, testFile);
        }

        /// <summary>
        /// Identifies the lines and pipes.
        /// </summary>
        private void identifyLinesAndPipes()
        {
            FigureDimension fd = null;
            LineDimensions = new List<FigureDimension>();
            PipeDimensions = new List<FigureDimension>();
            StringBuilder sb = new StringBuilder();
            string[] strArr;

            using (var mappedFile1 = MemoryMappedFile.CreateFromFile(path))
            {
                using (Stream mmStream = mappedFile1.CreateViewStream())
                {
                    using (StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                    {
                        string line;
                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                sb.AppendLine(line);
                                continue;
                            }
                            var lineWords = line.Split(' ');
                            if (lineWords[0] == "N")
                            {
                                if (fd != null)
                                {
                                    if (fd.shape == "line")
                                    {
                                        strArr = sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                        int len = strArr.Length;
                                        var words = strArr[len - 5].Split(' ');
                                        //check shadow width at line no. 5 from bottom
                                        if (!(
                                            words.Length == 6 &&
                                            words[0].StartsWith("\t") &&
                                            Regex.IsMatch(words[0].Trim(), @"^\d+$") &&
                                            Regex.IsMatch(words[1], @"^\d+$") &&
                                            Regex.IsMatch(words[2], @"^\d+$") &&
                                            Regex.IsMatch(words[3], @"^\d+$") &&
                                            Regex.IsMatch(words[4], @"^\d+$")
                                          ))
                                        {
                                            LineDimensions.Add(fd);
                                        }
                                    }
                                    else if (fd.shape == "pipe") PipeDimensions.Add(fd);
                                }
                                File.AppendAllText(targetPath, sb.ToString());
                                sb = new StringBuilder();
                                fd = new FigureDimension();
                                fd.nVal = nVal = int.Parse(lineWords[1]);

                            }

                            if (lineWords[0] == "P")
                            {
                                fd.Xpos = (int)Math.Round(double.Parse(lineWords[1]), MidpointRounding.AwayFromZero);
                                fd.Ypos = (int)Math.Round(double.Parse(lineWords[2]), MidpointRounding.AwayFromZero);
                            }

                            //format of string for line recognition:"   0 451"
                            if (
                                lineWords.Length == 2 &&
                                lineWords[0].StartsWith("\t") &&
                                (Regex.IsMatch(lineWords[0].Trim(), @"^\d+$") || Regex.IsMatch(lineWords[0].Trim(), @"^(?:\d{1,2})?(?:\.\d{1,6})?$")) &&
                                (Regex.IsMatch(lineWords[1], @"^\d+$") || Regex.IsMatch(lineWords[1], @"^(?:\d{1,2})?(?:\.\d{1,6})?$")) &&
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

                            if (line.StartsWith("\0")) continue;
                            sb.AppendLine(line);
                        }
                        ucbg_file.startingNValueForCircle = nVal + 2;
                        ucbg_files.Add(ucbg_file);
                        if (fd != null)
                        {
                            if (fd.shape == "line")
                            {
                                strArr = sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                int len = strArr.Length;
                                var words = strArr[len - 5].Split(' ');
                                //check shadow width at line no. 5 from bottom
                                if (!(
                                    words.Length == 5 &&
                                    words[0].StartsWith("\t") &&
                                    Regex.IsMatch(words[0].Trim(), @"^\d+$") &&
                                    Regex.IsMatch(words[1], @"^\d+$") &&
                                    Regex.IsMatch(words[2], @"^\d+$") &&
                                    Regex.IsMatch(words[3], @"^\d+$") &&
                                    Regex.IsMatch(words[4], @"^\d+$")
                                  ))
                                {
                                    LineDimensions.Add(fd);
                                }
                            }
                            
                        }
                        File.AppendAllText(targetPath, sb.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Finds the intersecting lines.
        /// </summary>
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

        /// <summary>
        /// Applies the intersection logic.
        /// </summary>
        /// <param name="f1">The f1.</param>
        /// <param name="f2">The f2.</param>
        private void applyIntersectionLogic(FigureDimension f1, FigureDimension f2)
        {
            xDiff = f1.Xpos - f2.Xpos;
            yDiff = f1.Ypos - f2.Ypos;
            yPlusHeight = f2.Ypos + f2.height;
            xPlusWidthdiff = xPlusWidth - f2.Xpos;
            yPlusHeightdiff = yPlusHeight - f1.Ypos;
            isXIntersecting = Math.Abs(xDiff) >= 1 && Math.Abs(xDiff) <= 5;
            isXPlusWidthIntersecting = Math.Abs(xPlusWidthdiff) >= 1 && Math.Abs(xPlusWidthdiff) <= 5;
            isYIntersecting = Math.Abs(yDiff) >= 1 && Math.Abs(yDiff) <= 5;
            isYPlusHeightIntersecting = Math.Abs(yPlusHeightdiff) >= 1 && Math.Abs(yPlusHeightdiff) <= 5;
            isXInBetween = f2.Xpos > f1.Xpos && f2.Xpos < xPlusWidth;
            isYInBetween = f2.Ypos > f1.Ypos && f2.Ypos < yPlusHeight;
            isXIntersectingFromClose = Math.Abs(xDiff) <= 5;
            isYIntersectingFromClose = Math.Abs(yDiff) <= 5;
            isXPlusWidthIntersectingFromClose = Math.Abs(xPlusWidthdiff) <= 5;
            isYPlusHeightIntersectingFromClose = Math.Abs(yPlusHeightdiff) <= 5;
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

        /// <summary>
        /// Marks the with a circle.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <param name="shape">The shape.</param>
        private void MarkWithACircle(double xpos, double ypos, string shape)
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
            using (StreamWriter w = File.AppendText(targetPath))
            {
                sb.AppendLine("N " + (ucbg_file.startingNValueForCircle + ((2 * intersectingLinesCount) - 2)));
                sb.AppendLine("P " + (xpos - 13) + " " + (ypos - 13));
                sb.AppendLine("T - 1");
                sb.AppendLine("R 0 0");
                sb.AppendLine("0");
                sb.AppendLine("\t0 5 7 0");
                sb.AppendLine("\tName TEST_C" + count);
                sb.AppendLine("\t0 1 1");
                sb.AppendLine("!");
                sb.AppendLine("2fe");
                sb.AppendLine("-10000");
                sb.AppendLine("c0c0c0");
                sb.AppendLine("0");
                sb.AppendLine("0");
                sb.AppendLine("0");
                sb.AppendLine("0 0");
                sb.AppendLine("1");
                sb.AppendLine("\t0 0 26 26");
                sb.AppendLine("\t0 23040");

                w.WriteLine(sb.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the rectifyToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void rectifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hasClickedCheck == true)
            {
                foreach (var file in ucbg_files)
                {
                    stopAddingToBuffer = false;
                    testFile = Path.GetFileName(file.name);
                    targetPath = Path.Combine(targetDirectoryInput.Text, testFile);
                    if (intersectingLinesCount == 0)
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
                                        var lineWords = line.Split(' ');
                                        if (lineWords[0] == "N" && lineWords[1] == file.startingNValueForCircle.ToString())
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

        /// <summary>
        /// Handles the Click event of the UpdatePath control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UpdatePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                parentDirectoryInput.Text = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// Handles the Click event of the UpdateTargetPath control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
