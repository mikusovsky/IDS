﻿using System.Drawing;
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
         this.components = new System.ComponentModel.Container();
         this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
         this.OpenFileButton = new System.Windows.Forms.Button();
         this.OpenFilesButon = new System.Windows.Forms.Button();
         this.TypeOfSceneLabel = new System.Windows.Forms.Label();
         this.DaySceneRadioButton = new System.Windows.Forms.RadioButton();
         this.NightSceneRadioButton = new System.Windows.Forms.RadioButton();
         this.ShowTmpImageCheckBox = new System.Windows.Forms.CheckBox();
         this.StopButton = new System.Windows.Forms.Button();
         this.FpsLabel = new System.Windows.Forms.Label();
         this.CarInfoView = new System.Windows.Forms.ListView();
         this.CarBrandOfVehicleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.CarTypeOfVehicleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.CarIdOfVehicleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.TypeOfVehicleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.SpeedOfVehicleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.CarImageOfVehicleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.CarInfoViewImageList = new System.Windows.Forms.ImageList(this.components);
         this.mainFrameViewer = new System.Windows.Forms.PictureBox();
         this.ButtonLoadDb = new System.Windows.Forms.Button();
         this.ProgresBarThis = new System.Windows.Forms.ProgressBar();
         this.ButtonMatch = new System.Windows.Forms.Button();
         this.ButtonNormalizeDb = new System.Windows.Forms.Button();
         this.button2 = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.mainFrameViewer)).BeginInit();
         this.SuspendLayout();
         // 
         // OpenFileDialog
         // 
         this.OpenFileDialog.FileName = "OpenFileDialog";
         // 
         // OpenFileButton
         // 
         this.OpenFileButton.Location = new System.Drawing.Point(23, 178);
         this.OpenFileButton.Margin = new System.Windows.Forms.Padding(4);
         this.OpenFileButton.Name = "OpenFileButton";
         this.OpenFileButton.Size = new System.Drawing.Size(125, 34);
         this.OpenFileButton.TabIndex = 0;
         this.OpenFileButton.Text = global::IDS.IDS.Resources.LoadVideo;
         this.OpenFileButton.UseVisualStyleBackColor = true;
         this.OpenFileButton.Click += new System.EventHandler(this._LoadVideoButton_Click);
         // 
         // OpenFilesButon
         // 
         this.OpenFilesButon.Location = new System.Drawing.Point(23, 222);
         this.OpenFilesButon.Margin = new System.Windows.Forms.Padding(4);
         this.OpenFilesButon.Name = "OpenFilesButon";
         this.OpenFilesButon.Size = new System.Drawing.Size(123, 32);
         this.OpenFilesButon.TabIndex = 20;
         this.OpenFilesButon.Text = global::IDS.IDS.Resources.LoadVideos;
         this.OpenFilesButon.UseVisualStyleBackColor = true;
         this.OpenFilesButon.Click += new System.EventHandler(this._LoadVideosButtonClick);
         // 
         // TypeOfSceneLabel
         // 
         this.TypeOfSceneLabel.AutoSize = true;
         this.TypeOfSceneLabel.Location = new System.Drawing.Point(19, 42);
         this.TypeOfSceneLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.TypeOfSceneLabel.Name = "TypeOfSceneLabel";
         this.TypeOfSceneLabel.Size = new System.Drawing.Size(98, 17);
         this.TypeOfSceneLabel.TabIndex = 2;
         this.TypeOfSceneLabel.Text = "Type of scene";
         // 
         // DaySceneRadioButton
         // 
         this.DaySceneRadioButton.AutoSize = true;
         this.DaySceneRadioButton.Checked = true;
         this.DaySceneRadioButton.Location = new System.Drawing.Point(20, 62);
         this.DaySceneRadioButton.Margin = new System.Windows.Forms.Padding(4);
         this.DaySceneRadioButton.Name = "DaySceneRadioButton";
         this.DaySceneRadioButton.Size = new System.Drawing.Size(98, 21);
         this.DaySceneRadioButton.TabIndex = 3;
         this.DaySceneRadioButton.TabStop = true;
         this.DaySceneRadioButton.Text = global::IDS.IDS.Resources.DayScene;
         this.DaySceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // NightSceneRadioButton
         // 
         this.NightSceneRadioButton.AutoSize = true;
         this.NightSceneRadioButton.Location = new System.Drawing.Point(21, 92);
         this.NightSceneRadioButton.Margin = new System.Windows.Forms.Padding(4);
         this.NightSceneRadioButton.Name = "NightSceneRadioButton";
         this.NightSceneRadioButton.Size = new System.Drawing.Size(106, 21);
         this.NightSceneRadioButton.TabIndex = 4;
         this.NightSceneRadioButton.Text = global::IDS.IDS.Resources.NightScene;
         this.NightSceneRadioButton.UseVisualStyleBackColor = true;
         // 
         // ShowTmpImageCheckBox
         // 
         this.ShowTmpImageCheckBox.AutoSize = true;
         this.ShowTmpImageCheckBox.Location = new System.Drawing.Point(23, 148);
         this.ShowTmpImageCheckBox.Margin = new System.Windows.Forms.Padding(4);
         this.ShowTmpImageCheckBox.Name = "ShowTmpImageCheckBox";
         this.ShowTmpImageCheckBox.Size = new System.Drawing.Size(109, 21);
         this.ShowTmpImageCheckBox.TabIndex = 6;
         this.ShowTmpImageCheckBox.Text = global::IDS.IDS.Resources.ShowDetails;
         this.ShowTmpImageCheckBox.UseVisualStyleBackColor = true;
         // 
         // StopButton
         // 
         this.StopButton.Location = new System.Drawing.Point(20, 261);
         this.StopButton.Margin = new System.Windows.Forms.Padding(4);
         this.StopButton.Name = "StopButton";
         this.StopButton.Size = new System.Drawing.Size(125, 34);
         this.StopButton.TabIndex = 7;
         this.StopButton.Text = global::IDS.IDS.Resources.ButtonStop_Stop;
         this.StopButton.UseVisualStyleBackColor = true;
         this.StopButton.Click += new System.EventHandler(this._StopButton_Click);
         // 
         // FpsLabel
         // 
         this.FpsLabel.AutoSize = true;
         this.FpsLabel.Location = new System.Drawing.Point(1435, 11);
         this.FpsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.FpsLabel.Name = "FpsLabel";
         this.FpsLabel.Size = new System.Drawing.Size(39, 17);
         this.FpsLabel.TabIndex = 12;
         this.FpsLabel.Text = "0 fps";
         // 
         // CarInfoView
         // 
         this.CarInfoView.AllowColumnReorder = true;
         this.CarInfoView.CheckBoxes = true;
         this.CarInfoView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CarBrandOfVehicleHeader,
            this.CarTypeOfVehicleHeader,
            this.CarIdOfVehicleHeader,
            this.TypeOfVehicleHeader,
            this.SpeedOfVehicleHeader,
            this.CarImageOfVehicleHeader});
         this.CarInfoView.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.CarInfoView.ForeColor = System.Drawing.SystemColors.HotTrack;
         this.CarInfoView.FullRowSelect = true;
         this.CarInfoView.LargeImageList = this.CarInfoViewImageList;
         this.CarInfoView.Location = new System.Drawing.Point(171, 42);
         this.CarInfoView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.CarInfoView.Name = "CarInfoView";
         this.CarInfoView.Size = new System.Drawing.Size(944, 562);
         this.CarInfoView.SmallImageList = this.CarInfoViewImageList;
         this.CarInfoView.TabIndex = 0;
         this.CarInfoView.UseCompatibleStateImageBehavior = false;
         this.CarInfoView.View = System.Windows.Forms.View.Details;
         // 
         // CarBrandOfVehicleHeader
         // 
         this.CarBrandOfVehicleHeader.Text = global::IDS.IDS.Resources.Photo;
         this.CarBrandOfVehicleHeader.Width = 150;
         // 
         // CarBrandOfVehicel
         // 
         this.CarTypeOfVehicleHeader.Text = global::IDS.IDS.Resources.CarBrand;
         this.CarTypeOfVehicleHeader.Width = 100;
         // 
         // CarModelOfVehicel
         // 
         this.CarIdOfVehicleHeader.Text = global::IDS.IDS.Resources.CarModel;
         this.CarIdOfVehicleHeader.Width = 70;
         // 
         // TypeGenerationHeader
         // 
         this.TypeOfVehicleHeader.Text = global::IDS.IDS.Resources.CarGeneration;
         this.TypeOfVehicleHeader.Width = 100;
         // 
         // CarCarYears
         // 
         this.CarImageOfVehicleHeader.Text = global::IDS.IDS.Resources.CarIntervalBuild;
         this.CarImageOfVehicleHeader.Width = 100;
         // 
         // SpeedOfVehicleHeader
         // 
         this.SpeedOfVehicleHeader.Text = global::IDS.IDS.Resources.Speed;
         this.SpeedOfVehicleHeader.Width = 70;
         // 
         // CarInfoViewImageList
         // 
         this.CarInfoViewImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
         this.CarInfoViewImageList.ImageSize = new System.Drawing.Size(128, 128);
         this.CarInfoViewImageList.TransparentColor = System.Drawing.Color.Transparent;
         // 
         // mainFrameViewer
         // 
         this.mainFrameViewer.Location = new System.Drawing.Point(1121, 42);
         this.mainFrameViewer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.mainFrameViewer.Name = "mainFrameViewer";
         this.mainFrameViewer.Size = new System.Drawing.Size(357, 226);
         this.mainFrameViewer.TabIndex = 13;
         this.mainFrameViewer.TabStop = false;
         // 
         // ButtonLoadDb
         // 
         this.ButtonLoadDb.Location = new System.Drawing.Point(23, 569);
         this.ButtonLoadDb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.ButtonLoadDb.Name = "ButtonLoadDb";
         this.ButtonLoadDb.Size = new System.Drawing.Size(125, 33);
         this.ButtonLoadDb.TabIndex = 14;
         this.ButtonLoadDb.Text = global::IDS.IDS.Resources.LoadDb;
         this.ButtonLoadDb.UseVisualStyleBackColor = true;
         this.ButtonLoadDb.Click += new System.EventHandler(this.ButtonLoadDb_Click);
         // 
         // ProgresBarThis
         // 
         this.ProgresBarThis.Location = new System.Drawing.Point(1121, 575);
         this.ProgresBarThis.Margin = new System.Windows.Forms.Padding(4);
         this.ProgresBarThis.Name = "ProgresBarThis";
         this.ProgresBarThis.Size = new System.Drawing.Size(357, 28);
         this.ProgresBarThis.TabIndex = 15;
         this.ProgresBarThis.Visible = false;
         // 
         // ButtonMatch
         // 
         this.ButtonMatch.Location = new System.Drawing.Point(23, 494);
         this.ButtonMatch.Margin = new System.Windows.Forms.Padding(4);
         this.ButtonMatch.Name = "ButtonMatch";
         this.ButtonMatch.Size = new System.Drawing.Size(125, 33);
         this.ButtonMatch.TabIndex = 16;
         this.ButtonMatch.Text = global::IDS.IDS.Resources.Match;
         this.ButtonMatch.UseVisualStyleBackColor = true;
         this.ButtonMatch.Click += new System.EventHandler(this.ButtonMatch_Click);
         // 
         // ButtonNormalizeDb
         // 
         this.ButtonNormalizeDb.Location = new System.Drawing.Point(23, 389);
         this.ButtonNormalizeDb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.ButtonNormalizeDb.Name = "ButtonNormalizeDb";
         this.ButtonNormalizeDb.Size = new System.Drawing.Size(125, 34);
         this.ButtonNormalizeDb.TabIndex = 17;
         this.ButtonNormalizeDb.Text = global::IDS.IDS.Resources.NormalizeDb;
         this.ButtonNormalizeDb.UseVisualStyleBackColor = true;
         this.ButtonNormalizeDb.Click += new System.EventHandler(this.ButtonNormalizeDb_Click);
         // 
         // button2
         // 
         this.button2.Location = new System.Drawing.Point(23, 534);
         this.button2.Margin = new System.Windows.Forms.Padding(4);
         this.button2.Name = "button2";
         this.button2.Size = new System.Drawing.Size(123, 28);
         this.button2.TabIndex = 19;
         this.button2.Text = global::IDS.IDS.Resources.Test;
         this.button2.UseVisualStyleBackColor = true;
         this.button2.Click += new System.EventHandler(this.ButtonTest_Click);
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1488, 613);
         this.Controls.Add(this.OpenFilesButon);
         this.Controls.Add(this.button2);
         this.Controls.Add(this.ButtonNormalizeDb);
         this.Controls.Add(this.ButtonMatch);
         this.Controls.Add(this.ProgresBarThis);
         this.Controls.Add(this.ButtonLoadDb);
         this.Controls.Add(this.mainFrameViewer);
         this.Controls.Add(this.CarInfoView);
         this.Controls.Add(this.FpsLabel);
         this.Controls.Add(this.StopButton);
         this.Controls.Add(this.ShowTmpImageCheckBox);
         this.Controls.Add(this.NightSceneRadioButton);
         this.Controls.Add(this.DaySceneRadioButton);
         this.Controls.Add(this.TypeOfSceneLabel);
         this.Controls.Add(this.OpenFileButton);
         this.Margin = new System.Windows.Forms.Padding(4);
         this.Name = "MainForm";
         this.Text = "IDS";
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
         ((System.ComponentModel.ISupportInitialize)(this.mainFrameViewer)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.OpenFileDialog OpenFileDialog;
      private System.Windows.Forms.Button OpenFileButton;
      private System.Windows.Forms.Button OpenFilesButon;
      private System.Windows.Forms.Label TypeOfSceneLabel;
      private System.Windows.Forms.RadioButton DaySceneRadioButton;
      private System.Windows.Forms.RadioButton NightSceneRadioButton;
      private System.Windows.Forms.CheckBox ShowTmpImageCheckBox;
      private System.Windows.Forms.Button StopButton;
      private System.Windows.Forms.Label FpsLabel;
      private System.Windows.Forms.ListView CarInfoView;
      private System.Windows.Forms.ImageList CarInfoViewImageList;
      private System.Windows.Forms.ColumnHeader CarBrandOfVehicleHeader;
      private System.Windows.Forms.ColumnHeader CarTypeOfVehicleHeader;
      private System.Windows.Forms.ColumnHeader CarIdOfVehicleHeader;
      private System.Windows.Forms.ColumnHeader TypeOfVehicleHeader;
      private System.Windows.Forms.ColumnHeader SpeedOfVehicleHeader;
      private System.Windows.Forms.ColumnHeader CarImageOfVehicleHeader;
      private System.Windows.Forms.PictureBox mainFrameViewer;
      private System.Windows.Forms.Button ButtonLoadDb;
      private System.Windows.Forms.Button ButtonMatch;
      private System.Windows.Forms.ProgressBar ProgresBarThis;
      private System.Windows.Forms.Button ButtonNormalizeDb;
      private System.Windows.Forms.Button button2;
   }
}

