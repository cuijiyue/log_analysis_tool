namespace log_analysis_tool
{
    partial class ShowFileForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowFileForm));
            this.showFileButton = new System.Windows.Forms.Button();
            this.kernelLogTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // showFileButton
            // 
            this.showFileButton.Location = new System.Drawing.Point(1, 1);
            this.showFileButton.Margin = new System.Windows.Forms.Padding(1);
            this.showFileButton.Name = "showFileButton";
            this.showFileButton.Size = new System.Drawing.Size(136, 23);
            this.showFileButton.TabIndex = 0;
            this.showFileButton.Text = " OpenFile";
            this.showFileButton.UseVisualStyleBackColor = true;
            this.showFileButton.Click += new System.EventHandler(this.showKernelLogButton_Click);
            // 
            // kernelLogTextBox
            // 
            this.kernelLogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kernelLogTextBox.Location = new System.Drawing.Point(1, 29);
            this.kernelLogTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.kernelLogTextBox.Multiline = true;
            this.kernelLogTextBox.Name = "kernelLogTextBox";
            this.kernelLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.kernelLogTextBox.Size = new System.Drawing.Size(609, 327);
            this.kernelLogTextBox.TabIndex = 1;
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Location = new System.Drawing.Point(523, 2);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 2;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.Location = new System.Drawing.Point(238, 4);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(279, 21);
            this.searchTextBox.TabIndex = 3;
            // 
            // ShowFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 357);
            this.Controls.Add(this.searchTextBox);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.kernelLogTextBox);
            this.Controls.Add(this.showFileButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShowFileForm";
            this.Text = "FileShowForm";
            this.Load += new System.EventHandler(this.KernelLogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button showFileButton;
        private System.Windows.Forms.TextBox kernelLogTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TextBox searchTextBox;
    }
}