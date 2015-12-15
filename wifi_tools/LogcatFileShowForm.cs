using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using UTILS;

namespace log_analysis_tool
{
    public partial class LogcatFileShowForm : Form
    {
        //存储log文件信息的链表
        public List<logcatInfo> logcatList = new List<logcatInfo>();

        public LogcatFileShowForm(String dirPath, String fileNameTag)
        {
            InitializeComponent();
            findLogInfo(dirPath, fileNameTag);
            InitListView();
        }

        public void InitListView()
        {
            //-1按照内容定义宽度，-2按照标题定义宽度
            this.listView1.Columns.Add("log name", -1, HorizontalAlignment.Left);
            this.listView1.Columns.Add("log begin time", -2, HorizontalAlignment.Left);
            this.listView1.Columns.Add("statisticsTimes", -2, HorizontalAlignment.Left);

            this.listView1.BeginUpdate();
            for (int i = 0; i < logcatList.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = logcatList[i].name;
                lvi.SubItems.Add(logcatList[i].startTime);
                lvi.SubItems.Add("" + logcatList[i].statisticsTimes);
                this.listView1.Items.Add(lvi);
                //Console.WriteLine(logcatList[i].name + logcatList[i].startTime);
            }
            this.listView1.EndUpdate();

            //根据form的list的宽度改变窗口的宽度，高度固定
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
        }


        public void findLogInfo(String dirPath, String fileNameTag)
        {

            String[] files = Directory.GetFiles(dirPath, fileNameTag + "*", SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                //当前目录中并没有log文件，退出
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                logcatList.Add(new logcatInfo(files[i]));
            }
            
            //排序
            logcatList.Sort(Compare);
        }

        //自定义排序函数
        public static int Compare(logcatInfo r1, logcatInfo r2)
        {
            if (r2.startTime == null)
            {
                return 0;
            }
            else if (r1.startTime == null)
            {
                return 1;
            }
            return r1.startTime.CompareTo(r2.startTime);
        }

        public class logcatInfo
        {
            public String name = null;
            public String startTime = null;
            public int statisticsTimes = 0;
            public String path = null;

            public logcatInfo(String  fileName)
            {
                name = IOHelper.getFileName(fileName);
                path = IOHelper.getFullPath(fileName); ;
                findInfo(fileName);
            }

            public void findInfo(String fileName)
            {
                if (name == null)
                    return;

                try
                {
                    FileStream aFile = new FileStream(fileName, FileMode.Open);
                    StreamReader sr = new StreamReader(aFile);
                    String strLine = sr.ReadLine();
                    Regex regex = new Regex(Global.statisticsRegex);
                    while (strLine != null)
                    {
                        //获取log最开头的时间
                        if (startTime == null)
                        {
                            startTime = new Log(strLine).time;
                        }
                        
                        if (Global.statisticsRegex != null && regex.IsMatch(strLine))
                        {
                            statisticsTimes++;
                            //Error.print(strLine);
                        }
                        
                        strLine = sr.ReadLine();
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
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            List<String> selectLogNames = new List<string>();
            
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                if (this.listView1.Items[i].Checked)
                {
                    selectLogNames.Add(logcatList[i].path);
                }
            }

            IOHelper.CombineFile(selectLogNames, Global.logFilePath);

            //分析log，打开新窗口
            Form logAnalysisForm = new AnalysisForm(Global.logFilePath);           
            logAnalysisForm.Show();
            this.Close();


            //处理kernel log
            //String kernelLogPath = Utils.IOHelper.FindFileInDir(WelcomeForm.logDirPath, "dmesglog");
            //EventListView.Execute(Application.StartupPath + @"\dmesgtime.exe " + "\"" + kernelLogPath + "\"" + " 1", 1);
            //EventListView.Execute("dmesgtime.exe", "\"" + kernelLogPath + "\" 1");
            
        }
        

        

        private void allSelectCheckBox_CheckedChanged(object sender, EventArgs e)
        {            
            if (this.allSelectCheckBox.Checked)
            {
                //Console.WriteLine("allSelectCheckBox_CheckedChanged");
                for (int i = 0; i < this.listView1.Items.Count; i++)
                {
                    this.listView1.Items[i].Checked = true;
                }
            }
            else
            {
                for (int i = 0; i < this.listView1.Items.Count; i++)
                {
                    this.listView1.Items[i].Checked = false;
                }
            }
        }
    }
}
