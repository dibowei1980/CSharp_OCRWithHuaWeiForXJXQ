namespace OCRForXJXQ
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.process = new System.Windows.Forms.Button();
            this.txt_PdfFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbn_SelectFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // process
            // 
            this.process.Location = new System.Drawing.Point(525, 182);
            this.process.Name = "process";
            this.process.Size = new System.Drawing.Size(106, 41);
            this.process.TabIndex = 0;
            this.process.Text = "开始处理(&P)";
            this.process.UseVisualStyleBackColor = true;
            this.process.Click += new System.EventHandler(this.process_Click);
            // 
            // txt_PdfFolder
            // 
            this.txt_PdfFolder.Location = new System.Drawing.Point(14, 35);
            this.txt_PdfFolder.Multiline = true;
            this.txt_PdfFolder.Name = "txt_PdfFolder";
            this.txt_PdfFolder.Size = new System.Drawing.Size(629, 62);
            this.txt_PdfFolder.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "需要处理的PDF所在目录";
            // 
            // tbn_SelectFolder
            // 
            this.tbn_SelectFolder.Location = new System.Drawing.Point(525, 115);
            this.tbn_SelectFolder.Name = "tbn_SelectFolder";
            this.tbn_SelectFolder.Size = new System.Drawing.Size(106, 41);
            this.tbn_SelectFolder.TabIndex = 3;
            this.tbn_SelectFolder.Text = "选择目录(&S)";
            this.tbn_SelectFolder.UseVisualStyleBackColor = true;
            this.tbn_SelectFolder.Click += new System.EventHandler(this.tbn_SelectFolder_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 235);
            this.Controls.Add(this.tbn_SelectFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_PdfFolder);
            this.Controls.Add(this.process);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button process;
        private System.Windows.Forms.TextBox txt_PdfFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button tbn_SelectFolder;
    }
}

