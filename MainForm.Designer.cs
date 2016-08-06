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
         openFileButton.Location = new System.Drawing.Point(20, 375);
         openFileButton.Margin = new System.Windows.Forms.Padding(4);
         openFileButton.Name = "loadVideoButton";
         openFileButton.Size = new System.Drawing.Size(125, 34);
         openFileButton.TabIndex = 0;
         openFileButton.Text = Resources.LoadVideo;
         openFileButton.UseVisualStyleBackColor = true;
         openFileButton.Click += new System.EventHandler(_LoadVideoButton_Click);
         // 
         // consoleText
         // 
         consoleText.Location = new System.Drawing.Point(180, 38);
         consoleText.Margin = new System.Windows.Forms.Padding(4);
         consoleText.Multiline = true;
         consoleText.Name = "consoleText";
         consoleText.ReadOnly = true;
         consoleText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         consoleText.Size = new System.Drawing.Size(539, 422);
         consoleText.TabIndex = 1;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Location = new System.Drawing.Point(16, 42);
         label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         label1.Name = "label1";
         label1.Size = new System.Drawing.Size(98, 17);
         label1.TabIndex = 2;
         label1.Text = "Type of scene";
         // 
         // daySceneRadioButton
         // 
         daySceneRadioButton.AutoSize = true;
         daySceneRadioButton.Checked = true;
         daySceneRadioButton.Location = new System.Drawing.Point(20, 62);
         daySceneRadioButton.Margin = new System.Windows.Forms.Padding(4);
         daySceneRadioButton.Name = "daySceneRadioButton";
         daySceneRadioButton.Size = new System.Drawing.Size(98, 21);
         daySceneRadioButton.TabIndex = 3;
         daySceneRadioButton.TabStop = true;
         daySceneRadioButton.Text = Resources.DayScene;
         daySceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // nightSceneRadioButton
         // 
         nightSceneRadioButton.AutoSize = true;
         nightSceneRadioButton.Location = new System.Drawing.Point(20, 90);
         nightSceneRadioButton.Margin = new System.Windows.Forms.Padding(4);
         nightSceneRadioButton.Name = "nightSceneRadioButton";
         nightSceneRadioButton.Size = new System.Drawing.Size(106, 21);
         nightSceneRadioButton.TabIndex = 4;
         nightSceneRadioButton.Text = Resources.NightScene;
         nightSceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // showTmpImageCheckBox
         // 
         showTmpImageCheckBox.AutoSize = true;
         showTmpImageCheckBox.Location = new System.Drawing.Point(16, 149);
         showTmpImageCheckBox.Margin = new System.Windows.Forms.Padding(4);
         showTmpImageCheckBox.Name = "showTmpImageCheckBox";
         showTmpImageCheckBox.Size = new System.Drawing.Size(109, 21);
         showTmpImageCheckBox.TabIndex = 6;
         showTmpImageCheckBox.Text = Resources.ShowDetails;
         showTmpImageCheckBox.UseVisualStyleBackColor = true;
         // 
         // stopButton
         // 
         stopButton.Location = new System.Drawing.Point(20, 426);
         stopButton.Margin = new System.Windows.Forms.Padding(4);
         stopButton.Name = "stopButton";
         stopButton.Size = new System.Drawing.Size(125, 34);
         stopButton.TabIndex = 7;
         stopButton.Text = Resources.ButtonStop_Stop;
         stopButton.UseVisualStyleBackColor = true;
         stopButton.Click += new System.EventHandler(this._StopButton_Click);
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Location = new System.Drawing.Point(761, 42);
         label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         label3.Name = "label3";
         label3.Size = new System.Drawing.Size(64, 17);
         label3.TabIndex = 9;
         label3.Text = "Personal";
         // 
         // label4
         // 
         label4.AutoSize = true;
         label4.Location = new System.Drawing.Point(832, 42);
         label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         label4.Name = "label4";
         label4.Size = new System.Drawing.Size(52, 17);
         label4.TabIndex = 10;
         label4.Text = "Lorries";
         // 
         // label5
         // 
         label5.AutoSize = true;
         label5.Location = new System.Drawing.Point(912, 42);
         label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         label5.Name = "label5";
         label5.Size = new System.Drawing.Size(66, 17);
         label5.TabIndex = 11;
         label5.Text = "Together";
         // 
         // fpsLabel
         // 
         fpsLabel.AutoSize = true;
         fpsLabel.Location = new System.Drawing.Point(1031, 11);
         fpsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         fpsLabel.Name = "fpsLabel";
         fpsLabel.Size = new System.Drawing.Size(39, 17);
         fpsLabel.TabIndex = 12;
         fpsLabel.Text = "0 fps";
         // 
         // MainForm
         // 
         AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         ClientSize = new System.Drawing.Size(1093, 482);
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
         Margin = new System.Windows.Forms.Padding(4);
         Name = "MainForm";
         Text = "IDS";
         KeyDown += new System.Windows.Forms.KeyEventHandler(MainForm_KeyDown);
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

