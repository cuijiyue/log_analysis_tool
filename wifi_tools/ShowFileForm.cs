using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UTILS;

namespace log_analysis_tool
{
    public partial class ShowFileForm : Form
    {
        String timeTobeSertch = null;
        String filePath;
        public ShowFileForm(String filePath, String time)
        {
            InitializeComponent();
            this.filePath = filePath;
            timeTobeSertch = time;
        }

        public ShowFileForm(String filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
            showFile();
        }

        protected void showFile()
        {
            if (filePath == null)
            {
                this.kernelLogTextBox.Text = "file not find！！";
            }
            else
            {
                Error.print("filePath:" + filePath);
                String[] temp = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);
                for (int i = 0; i < temp.Length; i++)
                {
                    this.kernelLogTextBox.Text += temp[i] + System.Environment.NewLine;
                    //Console.WriteLine(temp[i]);
                }
                this.kernelLogTextBox.Font = new Font(kernelLogTextBox.Font.FontFamily, 12, kernelLogTextBox.Font.Style);
            }
        }

        private void KernelLogForm_Load(object sender, EventArgs e)
        {
            //textbox显示数据
            //定位光标到指定行
            
        }

        private void showKernelLogButton_Click(object sender, EventArgs e)
        {
            //EventListView.Execute("\"" + WelcomeForm.uePath + "\"", "\"" + kernelLogPath + " -f" + wlanLog[lineIndex] + "\"");
            CMD.openFile(Global.editOpenCmd, Global.editPath, this.filePath);

        }

        int searchIndex = 0;
        private void searchButton_Click(object sender, EventArgs e)
        {
            searchIndex = this.kernelLogTextBox.Text.IndexOf(this.searchTextBox.Text, searchIndex);
            if (searchIndex < 0)
            {
                searchIndex = 0;
                this.kernelLogTextBox.SelectionStart = 0;
                this.kernelLogTextBox.SelectionLength = 0;
                MessageBox.Show("已到结尾");
                return;
            }
            this.kernelLogTextBox.SelectionStart = searchIndex;
            this.kernelLogTextBox.SelectionLength = this.searchTextBox.Text.Length;
            searchIndex = searchIndex + this.searchTextBox.Text.Length;
            this.kernelLogTextBox.Focus();
            this.kernelLogTextBox.ScrollToCaret();
        }

    }
}
