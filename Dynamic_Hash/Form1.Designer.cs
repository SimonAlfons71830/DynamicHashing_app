﻿namespace Dynamic_Hash
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
            richTextBox1 = new RichTextBox();
            label1 = new Label();
            insert_no = new NumericUpDown();
            find_no = new NumericUpDown();
            label2 = new Label();
            label3 = new Label();
            test_button = new Button();
            ((System.ComponentModel.ISupportInitialize)insert_no).BeginInit();
            ((System.ComponentModel.ISupportInitialize)find_no).BeginInit();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(12, 225);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(659, 281);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(275, 9);
            label1.Name = "label1";
            label1.Size = new Size(134, 20);
            label1.TabIndex = 1;
            label1.Text = "Dynamic Hash Test";
            // 
            // insert_no
            // 
            insert_no.Location = new Point(128, 73);
            insert_no.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            insert_no.Name = "insert_no";
            insert_no.Size = new Size(150, 27);
            insert_no.TabIndex = 2;
            // 
            // find_no
            // 
            find_no.Location = new Point(128, 117);
            find_no.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            find_no.Name = "find_no";
            find_no.Size = new Size(150, 27);
            find_no.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 80);
            label2.Name = "label2";
            label2.Size = new Size(56, 20);
            label2.TabIndex = 4;
            label2.Text = "INSERT";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(23, 124);
            label3.Name = "label3";
            label3.Size = new Size(42, 20);
            label3.TabIndex = 5;
            label3.Text = "FIND";
            // 
            // test_button
            // 
            test_button.Location = new Point(380, 111);
            test_button.Name = "test_button";
            test_button.Size = new Size(138, 33);
            test_button.TabIndex = 6;
            test_button.Text = "TEST";
            test_button.UseVisualStyleBackColor = true;
            test_button.Click += test_button_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(688, 518);
            Controls.Add(test_button);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(find_no);
            Controls.Add(insert_no);
            Controls.Add(label1);
            Controls.Add(richTextBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)insert_no).EndInit();
            ((System.ComponentModel.ISupportInitialize)find_no).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox1;
        private Label label1;
        private NumericUpDown insert_no;
        private NumericUpDown find_no;
        private Label label2;
        private Label label3;
        private Button test_button;
    }
}