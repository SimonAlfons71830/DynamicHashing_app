namespace Dynamic_Hash
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
            noRemove = new NumericUpDown();
            label4 = new Label();
            blockFactor_no = new NumericUpDown();
            label5 = new Label();
            bfOverflow_no = new NumericUpDown();
            label6 = new Label();
            hashCount_no = new NumericUpDown();
            label7 = new Label();
            ((System.ComponentModel.ISupportInitialize)insert_no).BeginInit();
            ((System.ComponentModel.ISupportInitialize)find_no).BeginInit();
            ((System.ComponentModel.ISupportInitialize)noRemove).BeginInit();
            ((System.ComponentModel.ISupportInitialize)blockFactor_no).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bfOverflow_no).BeginInit();
            ((System.ComponentModel.ISupportInitialize)hashCount_no).BeginInit();
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
            insert_no.Location = new Point(127, 49);
            insert_no.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            insert_no.Name = "insert_no";
            insert_no.Size = new Size(150, 27);
            insert_no.TabIndex = 2;
            insert_no.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // find_no
            // 
            find_no.Location = new Point(127, 93);
            find_no.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            find_no.Name = "find_no";
            find_no.Size = new Size(150, 27);
            find_no.TabIndex = 3;
            find_no.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 56);
            label2.Name = "label2";
            label2.Size = new Size(56, 20);
            label2.TabIndex = 4;
            label2.Text = "INSERT";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(22, 100);
            label3.Name = "label3";
            label3.Size = new Size(42, 20);
            label3.TabIndex = 5;
            label3.Text = "FIND";
            // 
            // test_button
            // 
            test_button.Location = new Point(258, 186);
            test_button.Name = "test_button";
            test_button.Size = new Size(138, 33);
            test_button.TabIndex = 6;
            test_button.Text = "TEST";
            test_button.UseVisualStyleBackColor = true;
            test_button.Click += test_button_Click;
            // 
            // noRemove
            // 
            noRemove.Location = new Point(127, 137);
            noRemove.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            noRemove.Name = "noRemove";
            noRemove.Size = new Size(150, 27);
            noRemove.TabIndex = 7;
            noRemove.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(22, 144);
            label4.Name = "label4";
            label4.Size = new Size(67, 20);
            label4.TabIndex = 8;
            label4.Text = "REMOVE";
            // 
            // blockFactor_no
            // 
            blockFactor_no.Location = new Point(504, 49);
            blockFactor_no.Name = "blockFactor_no";
            blockFactor_no.Size = new Size(150, 27);
            blockFactor_no.TabIndex = 9;
            blockFactor_no.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(373, 56);
            label5.Name = "label5";
            label5.Size = new Size(87, 20);
            label5.TabIndex = 10;
            label5.Text = "BF main file";
            // 
            // bfOverflow_no
            // 
            bfOverflow_no.Location = new Point(504, 93);
            bfOverflow_no.Name = "bfOverflow_no";
            bfOverflow_no.Size = new Size(150, 27);
            bfOverflow_no.TabIndex = 11;
            bfOverflow_no.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(373, 100);
            label6.Name = "label6";
            label6.Size = new Size(112, 20);
            label6.TabIndex = 12;
            label6.Text = "BF overflow file";
            // 
            // hashCount_no
            // 
            hashCount_no.Location = new Point(504, 137);
            hashCount_no.Name = "hashCount_no";
            hashCount_no.Size = new Size(150, 27);
            hashCount_no.TabIndex = 13;
            hashCount_no.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(373, 144);
            label7.Name = "label7";
            label7.Size = new Size(83, 20);
            label7.TabIndex = 14;
            label7.Text = "Hash count";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(688, 518);
            Controls.Add(label7);
            Controls.Add(hashCount_no);
            Controls.Add(label6);
            Controls.Add(bfOverflow_no);
            Controls.Add(label5);
            Controls.Add(blockFactor_no);
            Controls.Add(label4);
            Controls.Add(noRemove);
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
            ((System.ComponentModel.ISupportInitialize)noRemove).EndInit();
            ((System.ComponentModel.ISupportInitialize)blockFactor_no).EndInit();
            ((System.ComponentModel.ISupportInitialize)bfOverflow_no).EndInit();
            ((System.ComponentModel.ISupportInitialize)hashCount_no).EndInit();
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
        private NumericUpDown noRemove;
        private Label label4;
        private NumericUpDown blockFactor_no;
        private Label label5;
        private NumericUpDown bfOverflow_no;
        private Label label6;
        private NumericUpDown hashCount_no;
        private Label label7;
    }
}