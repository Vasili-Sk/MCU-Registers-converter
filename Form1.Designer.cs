namespace EmbeddedRegsConverter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            openFileDialog1 = new OpenFileDialog();
            checkVolatile = new CheckBox();
            richHfile = new RichTextBox();
            richLinkerFile = new RichTextBox();
            splitContainer1 = new SplitContainer();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 13);
            button1.Name = "button1";
            button1.Size = new Size(196, 23);
            button1.TabIndex = 0;
            button1.Text = "Open sytem .h file";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Header files|*.h";
            // 
            // checkVolatile
            // 
            checkVolatile.AutoSize = true;
            checkVolatile.Location = new Point(214, 16);
            checkVolatile.Name = "checkVolatile";
            checkVolatile.Size = new Size(89, 19);
            checkVolatile.TabIndex = 2;
            checkVolatile.Text = "Add volatile";
            checkVolatile.UseVisualStyleBackColor = true;
            // 
            // richHfile
            // 
            richHfile.Dock = DockStyle.Fill;
            richHfile.Location = new Point(0, 0);
            richHfile.Name = "richHfile";
            richHfile.Size = new Size(388, 382);
            richHfile.TabIndex = 4;
            richHfile.Text = "";
            // 
            // richLinkerFile
            // 
            richLinkerFile.Dock = DockStyle.Fill;
            richLinkerFile.Location = new Point(0, 0);
            richLinkerFile.Name = "richLinkerFile";
            richLinkerFile.Size = new Size(408, 382);
            richLinkerFile.TabIndex = 5;
            richLinkerFile.Text = "";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Bottom;
            splitContainer1.Location = new Point(0, 68);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(richHfile);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(richLinkerFile);
            splitContainer1.Size = new Size(800, 382);
            splitContainer1.SplitterDistance = 388;
            splitContainer1.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 50);
            label1.Name = "label1";
            label1.Size = new Size(86, 15);
            label1.TabIndex = 7;
            label1.Text = "Modified H file";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(523, 50);
            label2.Name = "label2";
            label2.Size = new Size(117, 15);
            label2.TabIndex = 7;
            label2.Text = "Linker sections script";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(splitContainer1);
            Controls.Add(checkVolatile);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Embedded registers converter";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private OpenFileDialog openFileDialog1;
        private CheckBox checkVolatile;
        private RichTextBox richHfile;
        private RichTextBox richLinkerFile;
        private SplitContainer splitContainer1;
        private Label label1;
        private Label label2;
    }
}