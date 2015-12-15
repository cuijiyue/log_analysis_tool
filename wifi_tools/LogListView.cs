using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UTILS;

namespace log_analysis_tool
{
    public partial class LogListView : Form
    {
        List<Log> logList;
        int beginIndex;
        int endIndex;
        public LogListView(String name, List<Log> logList)
        {
            if (logList == null)
            {
                return;
            }
            this.Text = name;
            InitializeComponent();
            this.logList = logList;
            beginIndex = 0;
            endIndex = logList.Count;
            InitListView();
        }

        public LogListView(String name, List<Log> logList, int begin, int end)
        {
            if (logList == null)
            {
                return;
            }
            this.Text = name;
            InitializeComponent();
            this.logList = logList;
            beginIndex = begin;
            endIndex = end;
            InitListView();
        }

        public void InitListView()
        {
            //-1按照内容定义宽度，-2按照标题定义宽度
            this.listView1.Columns.Add("LINE", -1, HorizontalAlignment.Left);
            this.listView1.Columns.Add("TIME", -1, HorizontalAlignment.Left);
            this.listView1.Columns.Add("TAG", -2, HorizontalAlignment.Left);
            this.listView1.Columns.Add("CONTENTS", -2, HorizontalAlignment.Left);

            this.listView1.BeginUpdate();
            for (int i = beginIndex; i <= endIndex && i < logList.Count; i++)
            {
                //Console.WriteLine(logList[i].date + logList[i].time + logList[i].filterOut + logList[i].pen.Color);
                ListViewItem lvi = new ListViewItem();
                lvi.BackColor = logList[i].pen.Color;
                lvi.Text = "" + logList[i].line;
                lvi.SubItems.Add(logList[i].time);
                lvi.SubItems.Add(logList[i].logTag);
                lvi.SubItems.Add(logList[i].filterOut);
                this.listView1.Items.Add(lvi);
            }
            this.listView1.EndUpdate();

            //根据form的list的宽度改变窗口的宽度，高度固定
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            this.Width = 650;
            this.Height = 500;
        }

        //listview 点击事件
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.FocusedItem != null)//这个if必须的，不然会得到值但会报错  
            {
                //MessageBox.Show(this.listView1.FocusedItem.SubItems[0].Text);  
                String tmp = this.listView1.FocusedItem.SubItems[0].Text;//获得的listView的值显示在文本框里
                //Console.WriteLine("打开 " + Global.logcatName + " 行：" + tmp);
                CMD.openFileOnLine(Global.editOpenLineCmd, Global.editPath, Global.logFilePath, tmp);
            }
        }
    }
}
