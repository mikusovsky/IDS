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
         OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
         OpenFileButton = new System.Windows.Forms.Button();
         ConsoleText = new System.Windows.Forms.TextBox();
         TypeOfSceneLabel = new System.Windows.Forms.Label();
         DaySceneRadioButton = new System.Windows.Forms.RadioButton();
         NightSceneRadioButton = new System.Windows.Forms.RadioButton();
         ShowTmpImageCheckBox = new System.Windows.Forms.CheckBox();
         StopButton = new System.Windows.Forms.Button();
         PersonalLabel = new System.Windows.Forms.Label();
         LoriesLabel = new System.Windows.Forms.Label();
         TogetherLabel = new System.Windows.Forms.Label();
         FpsLabel = new System.Windows.Forms.Label();
         SuspendLayout();
         // 
         // OpenFileDialog
         // 
         OpenFileDialog.FileName = "OpenFileDialog";
         // 
         // OpenFileButton
         // 
         OpenFileButton.Location = new System.Drawing.Point(20, 375);
         OpenFileButton.Margin = new System.Windows.Forms.Padding(4);
         OpenFileButton.Name = "loadVideoButton";
         OpenFileButton.Size = new System.Drawing.Size(125, 34);
         OpenFileButton.TabIndex = 0;
         OpenFileButton.Text = Resources.LoadVideo;
         OpenFileButton.UseVisualStyleBackColor = true;
         OpenFileButton.Click += new System.EventHandler(_LoadVideoButton_Click);
         // 
         // ConsoleText
         // 
         ConsoleText.Location = new System.Drawing.Point(180, 38);
         ConsoleText.Margin = new System.Windows.Forms.Padding(4);
         ConsoleText.Multiline = true;
         ConsoleText.Name = "ConsoleText";
         ConsoleText.ReadOnly = true;
         ConsoleText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         ConsoleText.Size = new System.Drawing.Size(539, 422);
         ConsoleText.TabIndex = 1;
         // 
         // TypeOfSceneLabel
         // 
         TypeOfSceneLabel.AutoSize = true;
         TypeOfSceneLabel.Location = new System.Drawing.Point(16, 42);
         TypeOfSceneLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         TypeOfSceneLabel.Name = "TypeOfSceneLabel";
         TypeOfSceneLabel.Size = new System.Drawing.Size(98, 17);
         TypeOfSceneLabel.TabIndex = 2;
         TypeOfSceneLabel.Text = "Type of scene";
         // 
         // DaySceneRadioButton
         // 
         DaySceneRadioButton.AutoSize = true;
         DaySceneRadioButton.Checked = true;
         DaySceneRadioButton.Location = new System.Drawing.Point(20, 62);
         DaySceneRadioButton.Margin = new System.Windows.Forms.Padding(4);
         DaySceneRadioButton.Name = "DaySceneRadioButton";
         DaySceneRadioButton.Size = new System.Drawing.Size(98, 21);
         DaySceneRadioButton.TabIndex = 3;
         DaySceneRadioButton.TabStop = true;
         DaySceneRadioButton.Text = Resources.DayScene;
         DaySceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // NightSceneRadioButton
         // 
         NightSceneRadioButton.AutoSize = true;
         NightSceneRadioButton.Location = new System.Drawing.Point(20, 90);
         NightSceneRadioButton.Margin = new System.Windows.Forms.Padding(4);
         NightSceneRadioButton.Name = "NightSceneRadioButton";
         NightSceneRadioButton.Size = new System.Drawing.Size(106, 21);
         NightSceneRadioButton.TabIndex = 4;
         NightSceneRadioButton.Text = Resources.NightScene;
         NightSceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // ShowTmpImageCheckBox
         // 
         ShowTmpImageCheckBox.AutoSize = true;
         ShowTmpImageCheckBox.Location = new System.Drawing.Point(16, 149);
         ShowTmpImageCheckBox.Margin = new System.Windows.Forms.Padding(4);
         ShowTmpImageCheckBox.Name = "ShowTmpImageCheckBox";
         ShowTmpImageCheckBox.Size = new System.Drawing.Size(109, 21);
         ShowTmpImageCheckBox.TabIndex = 6;
         ShowTmpImageCheckBox.Text = Resources.ShowDetails;
         ShowTmpImageCheckBox.UseVisualStyleBackColor = true;
         // 
         // StopButton
         // 
         StopButton.Location = new System.Drawing.Point(20, 426);
         StopButton.Margin = new System.Windows.Forms.Padding(4);
         StopButton.Name = "StopButton";
         StopButton.Size = new System.Drawing.Size(125, 34);
         StopButton.TabIndex = 7;
         StopButton.Text = Resources.ButtonStop_Stop;
         StopButton.UseVisualStyleBackColor = true;
         StopButton.Click += new System.EventHandler(this._StopButton_Click);
         // 
         // PersonalLabel
         // 
         PersonalLabel.AutoSize = true;
         PersonalLabel.Location = new System.Drawing.Point(761, 42);
         PersonalLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         PersonalLabel.Name = "PersonalLabel";
         PersonalLabel.Size = new System.Drawing.Size(64, 17);
         PersonalLabel.TabIndex = 9;
         PersonalLabel.Text = Resources.PersonalVehicles;
         // 
         // label4
         // 
         LoriesLabel.AutoSize = true;
         LoriesLabel.Location = new System.Drawing.Point(832, 42);
         LoriesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         LoriesLabel.Name = "label4";
         LoriesLabel.Size = new System.Drawing.Size(52, 17);
         LoriesLabel.TabIndex = 10;
         LoriesLabel.Text = Resources.Lorries;
         // 
         // label5
         // 
         TogetherLabel.AutoSize = true;
         TogetherLabel.Location = new System.Drawing.Point(912, 42);
         TogetherLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         TogetherLabel.Name = "TogetherLabel";
         TogetherLabel.Size = new System.Drawing.Size(66, 17);
         TogetherLabel.TabIndex = 11;
         TogetherLabel.Text = Resources.Together;
         // 
         // fpsLabel
         // 
         FpsLabel.AutoSize = true;
         FpsLabel.Location = new System.Drawing.Point(1031, 11);
         FpsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         FpsLabel.Name = "FpsLabel";
         FpsLabel.Size = new System.Drawing.Size(39, 17);
         FpsLabel.TabIndex = 12;
         FpsLabel.Text = "0 fps";
         // 
         // MainForm
         // 
         AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         ClientSize = new System.Drawing.Size(1093, 482);
         Controls.Add(FpsLabel);
         Controls.Add(TogetherLabel);
         Controls.Add(LoriesLabel);
         Controls.Add(PersonalLabel);
         Controls.Add(StopButton);
         Controls.Add(ShowTmpImageCheckBox);
         Controls.Add(NightSceneRadioButton);
         Controls.Add(DaySceneRadioButton);
         Controls.Add(TypeOfSceneLabel);
         Controls.Add(ConsoleText);
         Controls.Add(OpenFileButton);
         Margin = new System.Windows.Forms.Padding(4);
         Name = "MainForm";
         Text = "IDS";
         KeyDown += new System.Windows.Forms.KeyEventHandler(MainForm_KeyDown);
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion
      private System.Windows.Forms.OpenFileDialog OpenFileDialog;
      private System.Windows.Forms.Button OpenFileButton;
      private System.Windows.Forms.TextBox ConsoleText;
      private System.Windows.Forms.Label TypeOfSceneLabel;
      private System.Windows.Forms.RadioButton DaySceneRadioButton;
      private System.Windows.Forms.RadioButton NightSceneRadioButton;
      private System.Windows.Forms.CheckBox ShowTmpImageCheckBox;
      private System.Windows.Forms.Button StopButton;
      private System.Windows.Forms.Label PersonalLabel;
      private System.Windows.Forms.Label LoriesLabel;
      private System.Windows.Forms.Label TogetherLabel;
      private System.Windows.Forms.Label FpsLabel;
   }
}

