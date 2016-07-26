namespace IDS
{
    partial class MainForm
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileButton = new System.Windows.Forms.Button();
            this.consoleText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.daySceneRadioButton = new System.Windows.Forms.RadioButton();
            this.nightSceneRadioButton = new System.Windows.Forms.RadioButton();
            this.showTmpImageCheckBox = new System.Windows.Forms.CheckBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // openFileButton
            // 
            this.openFileButton.Location = new System.Drawing.Point(12, 174);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(102, 23);
            this.openFileButton.TabIndex = 0;
            this.openFileButton.Text = "Načítať video";
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this._OpenFileButton_Click);
            // 
            // consoleText
            // 
            this.consoleText.Location = new System.Drawing.Point(207, 31);
            this.consoleText.Multiline = true;
            this.consoleText.Name = "consoleText";
            this.consoleText.ReadOnly = true;
            this.consoleText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleText.Size = new System.Drawing.Size(266, 230);
            this.consoleText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Režim snímania";
            // 
            // daySceneRadioButton
            // 
            this.daySceneRadioButton.AutoSize = true;
            this.daySceneRadioButton.Checked = true;
            this.daySceneRadioButton.Location = new System.Drawing.Point(15, 50);
            this.daySceneRadioButton.Name = "daySceneRadioButton";
            this.daySceneRadioButton.Size = new System.Drawing.Size(54, 17);
            this.daySceneRadioButton.TabIndex = 3;
            this.daySceneRadioButton.TabStop = true;
            this.daySceneRadioButton.Text = "denný";
            this.daySceneRadioButton.UseVisualStyleBackColor = true;
            // 
            // nightSceneRadioButton
            // 
            this.nightSceneRadioButton.AutoSize = true;
            this.nightSceneRadioButton.Location = new System.Drawing.Point(15, 73);
            this.nightSceneRadioButton.Name = "nightSceneRadioButton";
            this.nightSceneRadioButton.Size = new System.Drawing.Size(54, 17);
            this.nightSceneRadioButton.TabIndex = 4;
            this.nightSceneRadioButton.Text = "nočný";
            this.nightSceneRadioButton.UseVisualStyleBackColor = true;
            // 
            // showTmpImageCheckBox
            // 
            this.showTmpImageCheckBox.AutoSize = true;
            this.showTmpImageCheckBox.Location = new System.Drawing.Point(12, 121);
            this.showTmpImageCheckBox.Name = "showTmpImageCheckBox";
            this.showTmpImageCheckBox.Size = new System.Drawing.Size(123, 17);
            this.showTmpImageCheckBox.TabIndex = 6;
            this.showTmpImageCheckBox.Text = "Zobraziť podrobnosti";
            this.showTmpImageCheckBox.UseVisualStyleBackColor = true;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(19, 233);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(94, 28);
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this._StopButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(571, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 26);
            this.label3.TabIndex = 9;
            this.label3.Text = "Osobné \r\nvozidlá";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(624, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 26);
            this.label4.TabIndex = 10;
            this.label4.Text = "Nákladné\r\nvozidlá\r\n";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(684, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "SPOLU";
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Location = new System.Drawing.Point(773, 9);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(30, 13);
            this.fpsLabel.TabIndex = 12;
            this.fpsLabel.Text = "0 fps";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 269);
            this.Controls.Add(this.fpsLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.showTmpImageCheckBox);
            this.Controls.Add(this.nightSceneRadioButton);
            this.Controls.Add(this.daySceneRadioButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.consoleText);
            this.Controls.Add(this.openFileButton);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.TextBox consoleText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton daySceneRadioButton;
        private System.Windows.Forms.RadioButton nightSceneRadioButton;
        private System.Windows.Forms.CheckBox showTmpImageCheckBox;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label fpsLabel;

    }
}

