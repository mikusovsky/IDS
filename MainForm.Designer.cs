using IDS.IDS;

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
         openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
         openFileButton = new System.Windows.Forms.Button();
         consoleText = new System.Windows.Forms.TextBox();
         label1 = new System.Windows.Forms.Label();
         daySceneRadioButton = new System.Windows.Forms.RadioButton();
         nightSceneRadioButton = new System.Windows.Forms.RadioButton();
         showTmpImageCheckBox = new System.Windows.Forms.CheckBox();
         stopButton = new System.Windows.Forms.Button();
         label3 = new System.Windows.Forms.Label();
         label4 = new System.Windows.Forms.Label();
         label5 = new System.Windows.Forms.Label();
         fpsLabel = new System.Windows.Forms.Label();
         SuspendLayout();
         // 
         // openFileDialog1
         // 
         openFileDialog1.FileName = "openFileDialog1";
         // 
         // openFileButton
         // 
         openFileButton.Location = new System.Drawing.Point(12, 174);
         openFileButton.Name = "openFileButton";
         openFileButton.Size = new System.Drawing.Size(102, 23);
         openFileButton.TabIndex = 0;
         openFileButton.Text = Resource.LoadVideo;
         openFileButton.UseVisualStyleBackColor = true;
         openFileButton.Click += new System.EventHandler(this._OpenFileButton_Click);
         // 
         // consoleText
         // 
         consoleText.Location = new System.Drawing.Point(207, 31);
         consoleText.Multiline = true;
         consoleText.Name = "consoleText";
         consoleText.ReadOnly = true;
         consoleText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         consoleText.Size = new System.Drawing.Size(266, 230);
         consoleText.TabIndex = 1;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Location = new System.Drawing.Point(12, 34);
         label1.Name = "label1";
         label1.Size = new System.Drawing.Size(82, 13);
         label1.TabIndex = 2;
         label1.Text = Resource.TypeOfScene;
         // 
         // daySceneRadioButton
         // 
         daySceneRadioButton.AutoSize = true;
         daySceneRadioButton.Checked = true;
         daySceneRadioButton.Location = new System.Drawing.Point(15, 50);
         daySceneRadioButton.Name = "daySceneRadioButton";
         daySceneRadioButton.Size = new System.Drawing.Size(54, 17);
         daySceneRadioButton.TabIndex = 3;
         daySceneRadioButton.TabStop = true;
         daySceneRadioButton.Text = Resource.DayScene;
         daySceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // nightSceneRadioButton
         // 
         nightSceneRadioButton.AutoSize = true;
         nightSceneRadioButton.Location = new System.Drawing.Point(15, 73);
         nightSceneRadioButton.Name = "nightSceneRadioButton";
         nightSceneRadioButton.Size = new System.Drawing.Size(54, 17);
         nightSceneRadioButton.TabIndex = 4;
         nightSceneRadioButton.Text = Resource.NightScene;
         nightSceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // showTmpImageCheckBox
         // 
         showTmpImageCheckBox.AutoSize = true;
         showTmpImageCheckBox.Location = new System.Drawing.Point(12, 121);
         showTmpImageCheckBox.Name = "showTmpImageCheckBox";
         showTmpImageCheckBox.Size = new System.Drawing.Size(123, 17);
         showTmpImageCheckBox.TabIndex = 6;
         showTmpImageCheckBox.Text = Resource.ShowDetails;
         showTmpImageCheckBox.UseVisualStyleBackColor = true;
         // 
         // stopButton
         // 
         stopButton.Location = new System.Drawing.Point(19, 233);
         stopButton.Name = "stopButton";
         stopButton.Size = new System.Drawing.Size(94, 28);
         stopButton.TabIndex = 7;
         stopButton.Text = Resource.ButtonStop_Stop;
         stopButton.UseVisualStyleBackColor = true;
         stopButton.Click += new System.EventHandler(this._StopButton_Click);
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Location = new System.Drawing.Point(571, 34);
         label3.Name = "label3";
         label3.Size = new System.Drawing.Size(47, 26);
         label3.TabIndex = 9;
         label3.Text = Resource.PersonalVehicles;
         // 
         // label4
         // 
         label4.AutoSize = true;
         label4.Location = new System.Drawing.Point(624, 34);
         label4.Name = "label4";
         label4.Size = new System.Drawing.Size(53, 26);
         label4.TabIndex = 10;
         label4.Text = Resource.Lorries;
         // 
         // label5
         // 
         label5.AutoSize = true;
         label5.Location = new System.Drawing.Point(684, 40);
         label5.Name = "label5";
         label5.Size = new System.Drawing.Size(43, 13);
         label5.TabIndex = 11;
         label5.Text = Resource.Together;
         // 
         // fpsLabel
         // 
         fpsLabel.AutoSize = true;
         fpsLabel.Location = new System.Drawing.Point(773, 9);
         fpsLabel.Name = "fpsLabel";
         fpsLabel.Size = new System.Drawing.Size(30, 13);
         fpsLabel.TabIndex = 12;
         fpsLabel.Text = Resource.ZeroFPS;
         // 
         // Resources
         // 
         AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         ClientSize = new System.Drawing.Size(820, 269);
         Controls.Add(fpsLabel);
         Controls.Add(label5);
         Controls.Add(label4);
         Controls.Add(label3);
         Controls.Add(stopButton);
         Controls.Add(showTmpImageCheckBox);
         Controls.Add(nightSceneRadioButton);
         Controls.Add(daySceneRadioButton);
         Controls.Add(label1);
         Controls.Add(consoleText);
         Controls.Add(openFileButton);
         Name = "MainForm";
         Text = Resource.MainFrameName;
         KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
         ResumeLayout(false);
         PerformLayout();

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

