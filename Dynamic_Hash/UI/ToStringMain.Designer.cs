namespace Dynamic_Hash.UI
{
    partial class ToStringMain
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
            Menu = new Panel();
            endToString_button = new Button();
            oveflowfile_button = new Button();
            mainfile_button = new Button();
            panel_mainFile = new Panel();
            mainTextBox = new RichTextBox();
            panelOverflowFile = new Panel();
            overflowTextBox = new RichTextBox();
            Menu.SuspendLayout();
            panel_mainFile.SuspendLayout();
            panelOverflowFile.SuspendLayout();
            SuspendLayout();
            // 
            // Menu
            // 
            Menu.Controls.Add(endToString_button);
            Menu.Controls.Add(oveflowfile_button);
            Menu.Controls.Add(mainfile_button);
            Menu.Location = new Point(1, 1);
            Menu.Name = "Menu";
            Menu.Size = new Size(825, 79);
            Menu.TabIndex = 0;
            // 
            // endToString_button
            // 
            endToString_button.BackColor = SystemColors.ActiveCaption;
            endToString_button.FlatAppearance.BorderSize = 0;
            endToString_button.FlatStyle = FlatStyle.Flat;
            endToString_button.Location = new Point(550, 0);
            endToString_button.Name = "endToString_button";
            endToString_button.Size = new Size(275, 77);
            endToString_button.TabIndex = 2;
            endToString_button.Text = "Koniec";
            endToString_button.UseVisualStyleBackColor = false;
            endToString_button.Click += endToString_button_Click;
            // 
            // oveflowfile_button
            // 
            oveflowfile_button.BackColor = SystemColors.ActiveCaption;
            oveflowfile_button.FlatAppearance.BorderSize = 0;
            oveflowfile_button.FlatStyle = FlatStyle.Flat;
            oveflowfile_button.Location = new Point(275, 0);
            oveflowfile_button.Name = "oveflowfile_button";
            oveflowfile_button.Size = new Size(275, 77);
            oveflowfile_button.TabIndex = 1;
            oveflowfile_button.Text = "Overflow File";
            oveflowfile_button.UseVisualStyleBackColor = false;
            oveflowfile_button.Click += oveflowfile_button_Click;
            // 
            // mainfile_button
            // 
            mainfile_button.BackColor = SystemColors.ActiveCaption;
            mainfile_button.FlatAppearance.BorderSize = 0;
            mainfile_button.FlatStyle = FlatStyle.Flat;
            mainfile_button.Location = new Point(0, 0);
            mainfile_button.Name = "mainfile_button";
            mainfile_button.Size = new Size(275, 77);
            mainfile_button.TabIndex = 0;
            mainfile_button.Text = "Main File";
            mainfile_button.UseVisualStyleBackColor = false;
            mainfile_button.Click += mainfile_button_Click;
            // 
            // panel_mainFile
            // 
            panel_mainFile.Controls.Add(mainTextBox);
            panel_mainFile.Location = new Point(1, 80);
            panel_mainFile.Name = "panel_mainFile";
            panel_mainFile.Size = new Size(825, 496);
            panel_mainFile.TabIndex = 1;
            // 
            // mainTextBox
            // 
            mainTextBox.Location = new Point(7, 8);
            mainTextBox.Name = "mainTextBox";
            mainTextBox.Size = new Size(811, 481);
            mainTextBox.TabIndex = 1;
            mainTextBox.Text = "";
            // 
            // panelOverflowFile
            // 
            panelOverflowFile.Controls.Add(overflowTextBox);
            panelOverflowFile.Location = new Point(0, 80);
            panelOverflowFile.Name = "panelOverflowFile";
            panelOverflowFile.Size = new Size(825, 496);
            panelOverflowFile.TabIndex = 2;
            // 
            // overflowTextBox
            // 
            overflowTextBox.Location = new Point(9, 9);
            overflowTextBox.Name = "overflowTextBox";
            overflowTextBox.Size = new Size(811, 481);
            overflowTextBox.TabIndex = 0;
            overflowTextBox.Text = "";
            // 
            // ToStringMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(827, 574);
            Controls.Add(panelOverflowFile);
            Controls.Add(panel_mainFile);
            Controls.Add(Menu);
            Name = "ToStringMain";
            Text = "ToStringMain";
            Load += ToStringMain_Load;
            Menu.ResumeLayout(false);
            panel_mainFile.ResumeLayout(false);
            panelOverflowFile.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel Menu;
        private Button endToString_button;
        private Button oveflowfile_button;
        private Button mainfile_button;
        private Panel panel_mainFile;
        private Panel panelOverflowFile;
        private RichTextBox mainTextBox;
        private RichTextBox overflowTextBox;
    }
}