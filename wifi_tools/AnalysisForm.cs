using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using UTILS;
using System.Text.RegularExpressions;

namespace log_analysis_tool
{
    public partial class AnalysisForm : Form
    {
        public class nameLable
        {
            public String name;
            public Label label;

            public nameLable(String name)
            {
                this.name = name;
                this.label = new Label();
                //占满
                //this.label.AutoSize = false;
                this.label.Dock = DockStyle.Fill;
                //文字居右
                this.label.TextAlign = ContentAlignment.MiddleRight;
                this.label.Text = this.name;
            }

            public Point getCenterPosition()
            {
                int Y = label.Location.Y + label.Height / 2;
                int X = label.Location.X + label.Width / 2;
                return new Point(X, Y);
            }
        }


        int showingLog = 1;
        int showingFile = 2;
        int showing;
        public AnalysisForm(String filePath)
        {
            InitializeComponent();
            readLogToList(filePath, LogListSingleton.GetInstance());
            checkLog(Global.logTypeList);
            this.logButton.Tag = showingLog;
            this.fileButton.Tag = showingFile;
        }

        private void AnalysisForm_Load(object sender, EventArgs e)
        {
            showing = showingLog;
            logButtonbehavior();
        }
        private void AnalysisForm_Resize(object sender, EventArgs e)
        {
            if (this.Width > 0 && this.Height > 0)
            {
                if (showing == showingLog)
                {
                    logButtonbehavior();
                }
                else if (showing == showingFile)
                {
                    fileButtonbehavior();
                }
                
            }            
        }

        private void checkLog(List<logType> logTypeList)
        {
            //Console.WriteLine("logTypeList length :" + logTypeList.Count);
            for (int i = 0; i < logTypeList.Count; i++)
            {
                Console.WriteLine("logTypeList:" + i + ":" + logTypeList[i].name);
                List<Log> logList = LogListSingleton.getList();
                for (int j = 0; j < logList.Count; j++)
                {
                    //Console.WriteLine("loglist:" + j + ":" + logList[j].logTag + ":" + logList[j].logText);
                    //Regex regex = new Regex("^>>>\\sSCREEN_ON\\s<<<$");
                    //if (regex.IsMatch(logList[j].logText))
                    if (logTypeList[i].isMatch(logList[j]))
                    {
                        logTypeList[i].addLog(logList[j]);
                        //Console.WriteLine(logTypeList[i].name + ": add : time :" + logList[j].time + logList[j].logText);
                    }
                }
            }

            for (int i = 0; i < logTypeList.Count; i++)
            {
                logTypeList[i].removeUselessLog();
            }
        }

        //log button

        private void logButtonbehavior()
        {
            this.showPanel.Controls.Clear();
            this.showPanel.Controls.Add(this.logPictureBox);

            List<nameLable> nameLableList = showName(Global.logTypeList);
            
            //getMapWidth;
            int mapWidth = 0;
            for (int i = 0; i < Global.logTypeList.Count; i++)
            {
                if (Global.logTypeList[i].showType != Global.eventCmdType && Global.logTypeList[i].logList != null && Global.logTypeList[i].logList.Count > 0)
                    mapWidth += Global.logTypeList[i].logList.Count;
            }
            mapWidth = (mapWidth + 1) * Global.controlWidth;
            
            int mapHeight = this.nameTablePanel.Height;
            Bitmap map = new Bitmap(mapWidth, mapHeight);

            Pen markPen = new Pen(Global.markColor);
            markPen.Width = Global.markPenWidth;
            drawMarkLine(markPen, map, nameLableList);

            //showLog(Global.logTypeList);
            showLog(Global.logTypeList, map, nameLableList);

            this.logPictureBox.Image = map;
        }
        private List<nameLable> showName(List<logType> logTypeList)
        {
            this.nameTablePanel.Controls.Clear();
            this.nameTablePanel.RowCount = 1;
            List<nameLable> nameList = new List<nameLable>();
            for (int i = 0; i < logTypeList.Count; i++)
            {
                switch(logTypeList[i].showType)
                {
                    case Global.onOffType:
                        nameList.Add(new nameLable(logTypeList[i].name));
                        break;
                    case Global.stateMachineTwo:
                    case Global.stateMachineOne:
                        String[] names = logTypeList[i].name.Split(' ');
                        for (int j = 0; j < names.Length; j++)
                        {
                            nameList.Add(new nameLable(names[j]));
                        }                        
                        break;
                    case Global.eventCmdType:
                        nameList.Add(new nameLable(logTypeList[i].name));
                        break;

                }                
            }
            this.nameTablePanel.RowCount = nameList.Count;
            int maxWidth = 0;
            for (int i = 0; i < this.nameTablePanel.RowCount; i++)
            {
                //nameList[i].label.Text += i;
                if (maxWidth < nameList[i].label.Width)
                {
                    maxWidth = nameList[i].label.Width;
                }
                this.nameTablePanel.Controls.Add(nameList[i].label, 0, i);
            }
            this.nameTablePanel.Width = maxWidth;
            this.authorNamePanel.Width = maxWidth;
            this.showPanel.Location = new Point(maxWidth, this.nameTablePanel.Location.Y);
            this.showPanel.Width = this.Width - maxWidth - 20;
            //this.showPanel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            return nameList;            
        }
        private void drawMarkLine(Pen pen, Bitmap map, List<nameLable> nameLableList)
        {
            Graphics gph = Graphics.FromImage(map);
            gph.Clear(Color.White);
            for (int i = 0; i < nameLableList.Count; i++)
            {
                gph.DrawLine(pen, 0, nameLableList[i].getCenterPosition().Y, map.Width, nameLableList[i].getCenterPosition().Y);
            }

            //test
            //gph.DrawLine(pen, 0, 0, map.Width, map.Height);
        }
        

        private void showLog(List<logType> logTypeList, Bitmap map, List<nameLable> nameLableList)
        {
            this.logPictureBox.Controls.Clear();
            int X = Global.controlWidth;
            for (int i = 0; i < logTypeList.Count; i++)
			{
			    //if (logTypeList[i].showType != Global.eventCmdType)
                //{
                    logTypeList[i].setPrePoint(nameLableList);
                //}
			}
            Graphics gph = Graphics.FromImage(map);
            while (!logListToTail(logTypeList))
            {
                int index = getNextIndex(logTypeList);
                int Y = logTypeList[index].getNextPointY(nameLableList);
                String nextTime = logTypeList[index].getNextTime();
                logTypeList[index].Show(new Point(X, Y), gph, this.logPictureBox, "");                

                //show eventCmd button
                if (logTypeList[index].sectionName.Equals(Global.dumpSection))
                {
                    for (int i = 0; i < logTypeList.Count; i++)
                    {
                        if (logTypeList[i].showType == Global.eventCmdType)
                        {
                            logTypeList[i].Show(new Point(X, Y), gph, this.logPictureBox, nextTime);
                        }
                    }                    
                }

                X += Global.controlWidth;
            }

            //draw last line
            for (int i = 0; i < logTypeList.Count; i++)
            {
                //if (logTypeList[i].showType != Global.eventCmdType)
                //{
                    int x = map.Width;
                    int y = logTypeList[i].getNextPointY(nameLableList);
                    logTypeList[i].Show(new Point(x, y), gph, this.logPictureBox, "12-3123:59:59.999");
                //}
            }

        }

        private Boolean logListToTail(List<logType> logTypeList)
        {
            Boolean flag = true;
            for (int i = 0; i < logTypeList.Count; i++)
            {
                if (logTypeList[i].showType == Global.eventCmdType)
                {
                    continue;
                }
                if (logTypeList[i].preIndex == logTypeList[i].logList.Count - 1)
                {
                    flag = flag && true;
                }
                else
                {
                    flag = flag && false;
                }
            }
            return flag;
        }

        private int getNextIndex(List<logType> logTypeList)
        {
            int minTimeIndex = 0;
            for (int i = 0; i < logTypeList.Count; i++)
            {
                if (gettoBeShowLog(logTypeList[i]) != null){
                    minTimeIndex = i;
                    break;
                }                    
            }
            for (int i = 0; i < logTypeList.Count; i++)
            {
                Log nextLog = gettoBeShowLog(logTypeList[i]);
                Log minTimeLog = gettoBeShowLog(logTypeList[minTimeIndex]);

                if (nextLog != null && minTimeLog != null)
                {
                    if (String.Compare(nextLog.date + nextLog.time, minTimeLog.date + minTimeLog.time) < 0)
                    {
                        minTimeIndex = i;
                    }
                }
            }
            return minTimeIndex;
        }
        private Log gettoBeShowLog(logType logType)
        {
            if (logType.showType == Global.eventCmdType)
            {
                return null;
            }
            if (logType.logList == null || logType.logList.Count == 0)
            {
                return null;
            }
            if (logType.preIndex + 1 == logType.logList.Count)
            {
                return null;
            }
            return logType.logList[logType.preIndex + 1];
        }


        private void readLogToList(String filePath, List<Log> logList)
        {
            if (!IOHelper.Exists(filePath))
            {
                return;
            }
            try
            {
                FileStream aFile = new FileStream(filePath, FileMode.Open);
                StreamReader sr = new StreamReader(aFile);
                String strLine = sr.ReadLine();
                Log log;
                int line = 1;
                while (strLine != null)
                {
                    log = new Log(strLine);
                    log.line = line;
                    if (log.logText != null)
                    {
                            logList.Add(log);                        
                    }
                    strLine = sr.ReadLine();
                    line++;
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return;
            }
        }

        private void logButton_Click(object sender, EventArgs e)
        {
            if (showing == showingLog)
            {
                return;
            }
            else
            {
                showing = showingLog;
                logButtonbehavior();
            }
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            if (showing == showingFile)
            {
                return;
            }
            else
            {
                showing = showingFile;
                fileButtonbehavior();
            }
        }

        public void fileButtonbehavior()
        {
            String[] fileList = Global.filesName.Split(' ');
            if (fileList.Length > 0)
            {
                this.nameTablePanel.Controls.Clear();
                this.nameTablePanel.RowCount = 1;
                List<nameLable> nameList = new List<nameLable>();
                for (int i = 0; i < fileList.Length; i++)
                {
                    nameList.Add(new nameLable(fileList[i]));
                    nameList[i].label.Tag = i;
                    nameList[i].label.Click += new System.EventHandler(this.fileLable_Click);
                }
                this.nameTablePanel.RowCount = nameList.Count;
                int maxWidth = 0;
                for (int i = 0; i < this.nameTablePanel.RowCount; i++)
                {
                    //nameList[i].label.Text += i;
                    if (maxWidth < nameList[i].label.Width)
                    {
                        maxWidth = nameList[i].label.Width;
                    }
                    this.nameTablePanel.Controls.Add(nameList[i].label, 0, i);
                }
                this.nameTablePanel.Width = maxWidth;
                this.authorNamePanel.Width = maxWidth;
                this.showPanel.Location = new Point(maxWidth, this.nameTablePanel.Location.Y);
                this.showPanel.Width = this.Width - maxWidth - 20;

                //show file
                this.showPanel.Controls.Clear();
                addFileFormToLable(Directory.GetFiles(Global.logDirPath, fileList[0], SearchOption.AllDirectories)[0]);
            }
        }

        private void fileLable_Click(object sender, EventArgs e)
        {
            this.showPanel.Controls.Clear();
            String[] fileList = Global.filesName.Split(' ');
            int index = (int)((Label)sender).Tag;
            String filePath = Directory.GetFiles(Global.logDirPath, fileList[index], SearchOption.AllDirectories)[0];
            addFileFormToLable(filePath);
        }

        private void addFileFormToLable(String filePath)
        {
            ShowFileForm fileForm = new ShowFileForm(filePath);
            //fileForm.FormBorderStyle = FormBorderStyle.None;
            fileForm.TopLevel = false;
            fileForm.WindowState = FormWindowState.Maximized;
            fileForm.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.showPanel.Controls.Add(fileForm);
            fileForm.Show();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Author: Cui Jiyue\nEmail: cuijy2@lenovo.com\nSource: https://github.com/cuijiyue/log_analysis_tool", "ABOUT", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
       
    }

    public partial class LogListSingleton
    {
        static volatile LogListSingleton _instance = null;
        static readonly object Padlock = new object();
        static List<Log> logList;

        private LogListSingleton()
        {
            logList = new List<Log>();
        }
        /// <summary>
        /// 获取单一实例
        /// </summary>
        /// <returns></returns>
        public static List<Log> GetInstance()
        {
            if (_instance == null)
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new LogListSingleton();
                    }
                }
            }
            return logList;
        }

        public static List<Log> getList()
        {
            return logList;
        }

        public static Log getLog(int index)
        {
            if (index >= logList.Count)
            {
                return null;
            }
            return logList[index];
        }

        public static void add(Log log)
        {
            lock (Padlock)
            {
                logList.Add(log);
            }            
        }

        public static void remove(Log log)
        {
            lock (Padlock)
            {
                logList.Remove(log);
            }
        }

        public static void removeAt(int index)
        {
            lock (Padlock)
            {
                logList.RemoveAt(index);
            }
        }

        public static void clear(Log log)
        {
            lock (Padlock)
            {
                logList.Clear();
            }
        }


    }

}
