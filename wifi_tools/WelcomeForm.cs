using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using UTILS;

namespace log_analysis_tool
{
    public partial class WelcomeForm : Form
    {
        public WelcomeForm()
        {
            InitializeComponent();
            initGolbal();
            getLogType();

            //test
            //String test = INIHelper.Read("cui", "test", Global.iniPath).Trim();
            //String[] tests = test.Split(' ');
            //for (int i = 0; i < tests.Length; i++)
            //{
            //    Console.WriteLine("-" + tests[i] + "-");
            //}
            
        }

        private void initGolbal()
        {
            Global.editPath = INIHelper.Read("Global", "editPath", Global.iniPath).Trim();
            Global.filesName = INIHelper.Read("Global", "filesName", Global.iniPath).Trim();
            Global.editOpenCmd = INIHelper.Read("Global", "editOpenCmd", Global.iniPath).Trim();
            if (Global.editOpenCmd != "")
            {
                Global.editOpenCmd = "\"" + Global.editOpenLineCmd + "\"";
            }
            Global.editOpenLineCmd = INIHelper.Read("Global", "editOpenLineCmd", Global.iniPath).Trim();
            if (Global.editOpenLineCmd != "")
            {
                Global.editOpenLineCmd = "\"" + Global.editOpenLineCmd + "\"";
            }
            Global.editFindCmd = INIHelper.Read("Global", "editFindCmd", Global.iniPath).Trim();
            if (Global.editFindCmd != "")
            {
                Global.editFindCmd = "\"" + Global.editOpenLineCmd + "\"";
            }
            Global.logcatName = INIHelper.Read("Global", "logcatName", Global.iniPath).Trim();
            Global.kernelLogName = INIHelper.Read("Global", "kernelLogName", Global.iniPath).Trim();
            Global.logTypeNum = int.Parse(INIHelper.Read("Global", "logTypeNum", Global.iniPath).Trim());
            Global.dumpSection = INIHelper.Read("Global", "dumpSection", Global.iniPath).Trim();
            Global.statisticsRegex = INIHelper.Read("Global", "statisticsRegex", Global.iniPath).Trim();
            //Global.matchTags = INIHelper.Read("Global", "matchTags", Global.iniPath).Trim();
            Global.markColor = ColorTranslator.FromHtml(INIHelper.Read("Global", "markColor", Global.iniPath).Trim());
            Global.markPenWidth = int.Parse(INIHelper.Read("Global", "markPenWidth", Global.iniPath).Trim());

            Global.logTypeList = new List<logType>();
        }

        private void getLogType()
        {
            for (int i = 0; i < Global.logTypeNum; i++)
            {
                String sectionName = "LOG" + i;
                int type = int.Parse(INIHelper.Read(sectionName, "showType", Global.iniPath).Trim());
                //0--two state, this will show two state in on line
                //1--more than one state, like wpas state, state1 -> state2
                //2--more than one state, to state1
                //3--event and cmd, this will show by button
                switch(type)
                {
                    case Global.onOffType:
                        Global.logTypeList.Add(new OnOffType(sectionName, Global.iniPath));
                        break;
                    case Global.stateMachineTwo:
                        Global.logTypeList.Add(new StateMachineTypeTwo(sectionName, Global.iniPath));
                        break;
                    case Global.stateMachineOne:
                        Global.logTypeList.Add(new StateMachineTypeOne(sectionName, Global.iniPath));
                        break;
                    case Global.eventCmdType:
                        Global.logTypeList.Add(new EventCmdType(sectionName, Global.iniPath));
                        break;
                    default :
                        Error.show("unknow showStype in " + sectionName);
                        break;
                }                
            }
        }


        //dragEnbale
        private void textBoxDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
                this.logPathBox.Cursor = System.Windows.Forms.Cursors.Arrow;  //指定鼠标形状（更好看）  
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }


        }
        private void textBoxDragDrop(object sender, DragEventArgs e)
        {
            logPathBox.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (IOHelper.isDir(logPathBox.Text))
            {
                Global.logDirPath = logPathBox.Text;
                Global.logFilePath = Global.logDirPath + "\\LOGANALYSIS";
                if (Directory.GetFiles(Global.logDirPath, Global.logcatName + "*", SearchOption.AllDirectories).Length == 0)
                {
                    Error.show("do not find logcat in this floder");
                }
                else
                {
                    Form logInfoForm = new LogcatFileShowForm(Global.logDirPath, Global.logcatName);
                    logInfoForm.Show();
                }
            }
            else if (IOHelper.isFile(logPathBox.Text))
            {
                Global.logFilePath = logPathBox.Text;
                Global.logDirPath = IOHelper.getDirectoryName(Global.logFilePath);
                if (Global.logFilePath == null)
                {
                    Error.show("no file");
                }
                else
                {                    
                    //this.Hide();
                    Form logAnalysisForm = new AnalysisForm(Global.logFilePath);
                    logAnalysisForm.Show();
                }
            }
            else
            {
                Error.show("nothing");
            }
        }
    }

    static class Global
    {
        public static String iniPath = "./LogAnalysis.ini";
        public static String filesName;

        public static String editPath;
        public static String editOpenCmd;
        public static String editOpenLineCmd;
        public static String editFindCmd;

        //draw Pen
        public static Color markColor;
        public static int markPenWidth;
        public static int logPenWidth = 2;
        public static int controlWidth = 150;

        //showType
        public const int onOffType = 0;
        public const int stateMachineTwo = 1;
        public const int stateMachineOne = 2;
        public const int eventCmdType = 3;

        public static int logTypeNum;
        public static String dumpSection;
        public static String logcatName;
        public static String kernelLogName;
        //public static String matchTags;

        public static String logFilePath;
        public static String logDirPath;
        public static String statisticsRegex;

        public static List<logType> logTypeList;
    }

    public class Log
    {
        public String date;
        public String time;
        public int pId;
        public int uId;
        public String logLeval;
        public String logTag;
        public String logText;
        public int line;
        public String filterOut;
        public Pen pen;

        //example>>11-27 13:22:18.983  1068 12667 V ActivityManager: Waiting for pause to complete...
        public Log(String input)
        {
            if(input.Length > 33)
            {
                String[] logHeads = System.Text.RegularExpressions.Regex.Split(input.Substring(0, 32).Trim(), @"\s+");
                if(logHeads.Length == 5)
                {
                    
                    date = logHeads[0].Trim();
                    time = logHeads[1].Trim();
                    pId = int.Parse(logHeads[2].Trim());
                    uId = int.Parse(logHeads[3].Trim());
                    logLeval = logHeads[4].Trim();
                    logTag = getTagFromLog(input);
                    String temp = input.Substring(33, input.Length - 33).Trim();
                    logText = temp.Substring(temp.IndexOf(':')+1).Trim();
                }
            }
        }

        public static String getTagFromLog(String log)
        {
            String temp = log.Substring(33, log.Length - 33).Trim();
            return temp.Substring(0, temp.IndexOf(':'));
        }
    }

    public abstract class logType
    {
        public List<Log> logList = new List<Log>();
        protected int showIndex;
        //regex.IsMatch(source))
        //regex.Replace(source, "XXXX"));

        public String sectionName;
        //0--two state, this will show two state in on line
        //1--more than one state, like wpas state, state1 -> state2
        //2--more than one state, to state1
        //3--event and cmd, this will show by button
        public int showType;
        public String name;
        public String tag;

        protected List<Regex> matchRegex;
        protected int matchIndex = 10000;
        protected List<Filter> matchFilter;
        protected List<Pen> matchPen;



        protected class Filter
        {
            //0----regex list
            //1----cmd scripts
            public int filterType;
            public List<Regex> filterRegex;
            public List<String> filterReplace;
            public String filterCmd;
        }
             
        public void setType(String sectionName, String iniPath)
        {
            this.sectionName = sectionName;
            this.showType = int.Parse(INIHelper.Read(sectionName, "showType", iniPath).Trim());
            this.name = INIHelper.Read(sectionName, "name", iniPath).Trim();
            this.tag = INIHelper.Read(sectionName, "tag", iniPath).Trim();
            addMatch(iniPath);
        }

        
        
        private Filter getFilter(int filterIndex, String iniPath)
        {
            Filter filter = new Filter();
            filter.filterType = int.Parse(INIHelper.Read(sectionName, "match" + filterIndex + "filterType", iniPath).Trim());
            switch (filter.filterType)
            {
                case 0:
                    int num = int.Parse(INIHelper.Read(sectionName, "match" + filterIndex + "filterNum", iniPath).Trim());
                    filter.filterRegex = new List<Regex>();
                    filter.filterReplace = new List<string>();
                    for (int i = 0; i < num; i++)
                    {
                        filter.filterRegex.Add(new Regex(INIHelper.Read(sectionName, "match" + filterIndex + "filterRegex" + i, iniPath).Trim()));
                        filter.filterReplace.Add(INIHelper.Read(sectionName, "match" + filterIndex + "filterRegex" + i + "Replace", iniPath).Trim());
                        Console.WriteLine("match" + filterIndex + "filterRegex" + i + "=" + INIHelper.Read(sectionName, "match" + filterIndex + "filterRegex" + i, iniPath).Trim());
                        Console.WriteLine("match" + filterIndex + "filterRegex" + i + "Replace" + "=" + INIHelper.Read(sectionName, "match" + filterIndex + "filterRegex" + i + "Replace", iniPath).Trim());
                    }
                    break;
                case 1:
                    filter.filterCmd = INIHelper.Read(sectionName, "match" + filterIndex + "filterCmd", iniPath).Trim();
                    break;
                default:
                    break;
            }
            return filter;
        }

        private void addMatch(String iniPath)
        {
            int num = int.Parse(INIHelper.Read(sectionName, "matchNum", iniPath).Trim());
            matchRegex = new List<Regex>();
            matchPen = new List<Pen>();
            for (int i = 0; i < num; i++)
            {
                Console.WriteLine("matchRegex:" + i + ":" + INIHelper.Read(sectionName, "match" + i + "Regex", iniPath).Trim());
                matchRegex.Add(new Regex(INIHelper.Read(sectionName, "match" + i + "Regex", iniPath).Trim()));
                Pen pen = new Pen(ColorTranslator.FromHtml(INIHelper.Read(sectionName, "match" + i + "Line", Global.iniPath).Trim()));
                pen.Width = Global.logPenWidth;
                matchPen.Add(pen);
            }
            matchFilter = new List<Filter>();
            for (int i = 0; i < num; i++)
            {
                matchFilter.Add( getFilter(i, iniPath) );
            }
        }



        public Boolean isMatch(Log log)
        {
            if (matchRegex == null || log == null || log.logText == null || !log.logTag.Equals(this.tag))
            {
                return false;
            }
            else
            {
                for (int i = 0; i < matchRegex.Count; i++)
                {
                    if (matchRegex[i].IsMatch(log.logText))
                    {
                        matchIndex = i;
                        return true;
                    }
                }
                return false;
            }
        }

        protected String getFilterOut(String source)
        {
            String output = null;
            if (matchIndex < 10000)
            {                
                switch (matchFilter[matchIndex].filterType)
                {
                    case 0:
                        output = source;
                        for (int i = 0; i < matchFilter[matchIndex].filterRegex.Count; i++)
                        {
                            output = matchFilter[matchIndex].filterRegex[i].Replace(output, matchFilter[matchIndex].filterReplace[i]).Trim();
                        }
                        break;
                    case 1:
                        Error.print("use cmd get output");
                        break;
                }
            }
            return output;
        }

        protected Pen getPen(String source)
        {
            Pen pen = null;
            if (matchIndex < 10000)
            {
                pen = matchPen[matchIndex];
            }
            return pen;
        }

        protected Point prePoint;
        public int preIndex = -1;
        public String getNextTime()
        {
            if (logList == null || logList.Count == 0)
            {
                return null;
            }
            if (preIndex < logList.Count - 1)
            {
                return logList[preIndex + 1].date + logList[preIndex + 1].time;
            }
            else
            {
                return "12-31" + "23:59:59";
            }
        }

        //self use button, for logList show
        protected class eventCmdButton : Button
        {
            public int beginIndex;
            public int endIndex;
        }
        protected void showLogListView(object sender, EventArgs e)
        {
            int beginIndex = ((eventCmdButton)sender).beginIndex;
            int endIndex = ((eventCmdButton)sender).endIndex;
            Error.print("total:" + logList.Count + " beginIndex:" + beginIndex + " endIndex:" + endIndex);
            if (logList == null || logList.Count == 0 || endIndex < beginIndex)
	        {
		        return;
	        }
            new LogListView(name, logList, beginIndex, endIndex).Show();
        }

        //CMD.openFileOnLine(Global.editOpenLineCmd, Global.editPath, Global.logFilePath, tmp);
        protected void openLogCatFile(object sender, EventArgs e)
        {
            int line = (int)((Label)sender).Tag;
            //Error.print("total:" + logList.Count + " beginIndex:" + beginIndex + " endIndex:" + endIndex);
            CMD.openFileOnLine(Global.editOpenLineCmd, Global.editPath, Global.logFilePath, "" + line);
        }
        abstract public void setPrePoint(List<AnalysisForm.nameLable> nameLableList);
        abstract public int getNextPointY(List<AnalysisForm.nameLable> nameLableList);
        abstract public void Show(Point toPoint, Graphics gph, Control control, String toTime);
        abstract public void addLog(Log log);
        abstract public void removeUselessLog();
    }


    public class OnOffType : logType
    {
        public override void Show(Point toPoint, Graphics gph, Control control, String toTime)
        {
            //Error.print("OnOffType, " + log.time + " " + log.logText + ">>" + outPut);
            if (logList.Count == 0 || prePoint == null)
            {
                return;
            }
            Pen pen;
            if (preIndex == -1)
            {
                pen = matchPen[0] == logList[0].pen ? matchPen[1] : matchPen[0];
            }
            else
            {
                pen = logList[preIndex].pen;
            }

            gph.DrawLine(pen, prePoint, toPoint);

            preIndex++;
            prePoint = toPoint;

            if (preIndex < logList.Count)
            {
                Label label = new Label();
                label.Text = logList[preIndex].time + "\n" + logList[preIndex].filterOut;
                label.BackColor = Color.Transparent;
                label.Location = new Point(toPoint.X - label.Width / 2, toPoint.Y - label.Height / 2);
                label.Tag = logList[preIndex].line;
                label.Click += new System.EventHandler(this.openLogCatFile);
                control.Controls.Add(label);
                //Error.print(this.name + ": label text:" + label.Text);
            }            
        }

        public override void setPrePoint(List<AnalysisForm.nameLable> nameLableList)
        {
            preIndex = -1;
            for (int i = 0; i < nameLableList.Count; i++)
            {
                if (nameLableList[i].name.Equals(this.name))
                {
                    prePoint = new Point(0, nameLableList[i].getCenterPosition().Y);
                    return;
                }
            }
        }

        public override int getNextPointY(List<AnalysisForm.nameLable> nameLableList)
        {
            return prePoint.Y;
        }


        public OnOffType(String sectionName, String iniPath)
        {
            setType(sectionName, iniPath);
        }

        public override void addLog(Log log)
        {
            log.filterOut = getFilterOut(log.logText);
            log.pen = getPen(log.logText);
            this.logList.Add(log);
        }
        public override void removeUselessLog()
        {

        }
    }

    public class StateMachineTypeTwo : logType
    {
        public override void Show(Point toPoint, Graphics gph, Control control, String toTime)
        {
            //Error.print("OnOffType, " + log.time + " " + log.logText + ">>" + outPut);
            if (logList.Count == 0 || prePoint == null)
            {
                return;
            }
            Pen pen = matchPen[0];

            gph.DrawLine(pen, prePoint.X, prePoint.Y, toPoint.X, prePoint.Y);
            gph.DrawLine(pen, toPoint.X, prePoint.Y, toPoint.X, toPoint.Y);

            preIndex++;
            prePoint = toPoint;

            if (preIndex < logList.Count)
            {
                Label label = new Label();
                label.Text = logList[preIndex].time + "\n" + logList[preIndex].filterOut.Split('|')[1];
                label.BackColor = Color.Transparent;
                label.Location = new Point(toPoint.X - label.Width / 2, toPoint.Y - label.Height / 2);
                label.Tag = logList[preIndex].line;
                label.Click += new System.EventHandler(this.openLogCatFile);
                control.Controls.Add(label);
                //Error.print(this.name + ": label text:" + label.Text);
            }
        }

        public override void setPrePoint(List<AnalysisForm.nameLable> nameLableList)
        {
            preIndex = -1;
            if (logList == null || logList.Count == 0)
            {
                return;
            }
            String temp = logList[0].filterOut.Split('|')[0];
            for (int i = 0; i < nameLableList.Count; i++)
            {
                if (nameLableList[i].name.Equals(temp))
                {
                    prePoint = new Point(0, nameLableList[i].getCenterPosition().Y);
                    Error.print("setPrePoint: name :" + nameLableList[i].name);
                    return;
                }
            }
        }

        public override int getNextPointY(List<AnalysisForm.nameLable> nameLableList)
        {
            if (logList == null || logList.Count == 0)
            {
                return 0;
            }
            String temp;
            if (preIndex < logList.Count - 1)
            {
                temp = logList[preIndex + 1].filterOut.Split('|')[1];
            }
            else
            {
                temp = logList[preIndex].filterOut.Split('|')[1];
            }
            //Error.print("getNextPointY: log :" + logList[preIndex + 1].filterOut + " stateName:" + temp);
            for (int i = 0; i < nameLableList.Count; i++)
            {
                if (nameLableList[i].name.Equals(temp))
                {
                    //Error.print("getNextPointY: name :" + nameLableList[i].name);
                    return nameLableList[i].getCenterPosition().Y;
                }
            }
            return 0;
        }

        public StateMachineTypeTwo(String sectionName, String iniPath)
        {
            setType(sectionName, iniPath);
        }

        public override void addLog(Log log)
        {
            log.filterOut = getFilterOut(log.logText);
            log.pen = getPen(log.logText);
            this.logList.Add(log);
        }
        public override void removeUselessLog()
        {
            if (logList == null || logList.Count == 0)
            {
                return;
            }
            for (int i = logList.Count - 1; i >= 0; i--)
            {
                if (!checkLog(logList[i]))
                {
                    logList.RemoveAt(i);
                }
            }
        }

        private Boolean checkLog(Log log)
        {
            String[] names = name.Split(' ');
            String[] temps = log.filterOut.Split('|');
            Boolean flag1 = false;
            Boolean flag2 = false;
            for (int i = 0; i < names.Length; i++)
            {
                if (temps[0].Equals(names[i]))
                {
                    flag1 = true;
                    break;
                }
            }
            for (int i = 0; i < names.Length; i++)
            {
                if (temps[1].Equals(names[i]))
                {
                    flag2 = true;
                    break;
                }
            }
            return flag1 && flag2;
        }
    }

    public class StateMachineTypeOne : logType
    {
        public override void Show(Point toPoint, Graphics gph, Control control, String toTime)
        {
            //Error.print("StateMachineType, " + log.time + " " + log.logText + ">>" + outPut);
            if (logList.Count == 0 || prePoint == null)
            {
                return;
            }
            Pen pen = matchPen[0];

            gph.DrawLine(pen, prePoint.X, prePoint.Y, toPoint.X, prePoint.Y);
            gph.DrawLine(pen, toPoint.X, prePoint.Y, toPoint.X, toPoint.Y);

            preIndex++;
            prePoint = toPoint;

            if (preIndex < logList.Count)
            {
                Label label = new Label();
                label.Text = logList[preIndex].time + "\n" + logList[preIndex].filterOut;
                label.BackColor = Color.Transparent;
                label.Location = new Point(toPoint.X - label.Width / 2, toPoint.Y - label.Height / 2);
                label.Tag = logList[preIndex].line;
                label.Click += new System.EventHandler(this.openLogCatFile);
                control.Controls.Add(label);
                //Error.print(this.name + ": label text:" + label.Text);
            }
        }

        public override void setPrePoint(List<AnalysisForm.nameLable> nameLableList)
        {
            preIndex = -1;
            if (logList == null || logList.Count == 0)
            {
                return;
            }
            String temp = logList[0].filterOut;
            for (int i = 0; i < nameLableList.Count; i++)
            {
                if (nameLableList[i].name.Equals(temp))
                {
                    prePoint = new Point(0, nameLableList[i].getCenterPosition().Y);
                    return;
                }
            }
        }

        public override int getNextPointY(List<AnalysisForm.nameLable> nameLableList)
        {
            if (logList == null || logList.Count == 0)
            {
                return 0;
            }
            String temp;
            if (preIndex < logList.Count - 1)
            {
                temp = logList[preIndex + 1].filterOut;
            }
            else
            {
                temp = logList[preIndex].filterOut;
            }
            //Error.print("getNextPointY: log :" + logList[preIndex + 1].filterOut + " stateName:" + temp);
            for (int i = 0; i < nameLableList.Count; i++)
            {
                if (nameLableList[i].name.Equals(temp))
                {
                    //Error.print("getNextPointY: name :" + nameLableList[i].name);
                    return nameLableList[i].getCenterPosition().Y;
                }
            }
            return 0;
        }

        public StateMachineTypeOne(String sectionName, String iniPath)
        {
            setType(sectionName, iniPath);
        }

        public override void addLog(Log log)
        {
            log.filterOut = getFilterOut(log.logText);
            log.pen = getPen(log.logText);
            this.logList.Add(log);
        }
        public override void removeUselessLog()
        {
            if (logList == null || logList.Count == 0)
            {
                return;
            }
            for (int i = logList.Count - 1; i >= 0; i--)
            {
                if (!checkLog(logList[i]))
                {
                    logList.RemoveAt(i);
                }
            }
        }

        private Boolean checkLog(Log log)
        {
            String[] names = name.Split(' ');
            String temp = log.filterOut;
            for (int i = 0; i < names.Length; i++)
            {
                if (temp.Equals(names[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class EventCmdType : logType
    {
        public override void Show(Point toPoint, Graphics gph, Control control, String toTime)
        {
            // only use toPiont.X
            //Error.print("EventCmdType, " + log.time + " " + log.logText + ">>" + outPut);
            int count = 0;
            for (int i = preIndex + 1; i < logList.Count; i++)
            {
                if (String.Compare(logList[i].date + logList[i].time, toTime) <= 0)
                {
                    count++ ;
                }
            }
            

            eventCmdButton button = new eventCmdButton();
            button.Text = this.name + " " + count;
            button.Width = toPoint.X - prePoint.X - 4;
            button.Location = new Point(prePoint.X + 2, prePoint.Y - button.Height / 2);
            button.beginIndex = preIndex + 1;
            button.endIndex = preIndex + count;
            button.Click += new System.EventHandler(this.showLogListView);
            control.Controls.Add(button);

            prePoint = new Point(toPoint.X, prePoint.Y);
            preIndex += count;
        }
        public override void setPrePoint(List<AnalysisForm.nameLable> nameLableList)
        {
            preIndex = -1;
            for (int i = 0; i < nameLableList.Count; i++)
            {
                if (nameLableList[i].name.Equals(this.name))
                {
                    prePoint = new Point(0, nameLableList[i].getCenterPosition().Y);                    
                    return;
                }
            }
        }

        public override int getNextPointY(List<AnalysisForm.nameLable> nameLableList)
        {
            return prePoint.Y;
        }
        public EventCmdType(String sectionName, String iniPath)
        {
            setType(sectionName, iniPath);
        }

        public override void addLog(Log log)
        {
            log.filterOut = getFilterOut(log.logText);
            log.pen = getPen(log.logText);
            this.logList.Add(log);
        }
        public override void removeUselessLog()
        {

        }
    }

}
