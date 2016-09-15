using System.Drawing;
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
         this.TypeOfSceneLabel = new System.Windows.Forms.Label();
         this.DaySceneRadioButton = new System.Windows.Forms.RadioButton();
         this.NightSceneRadioButton = new System.Windows.Forms.RadioButton();
         this.ShowTmpImageCheckBox = new System.Windows.Forms.CheckBox();
         this.StopButton = new System.Windows.Forms.Button();
         this.PersonalLabel = new System.Windows.Forms.Label();
         this.LoriesLabel = new System.Windows.Forms.Label();
         this.TogetherLabel = new System.Windows.Forms.Label();
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
         ((System.ComponentModel.ISupportInitialize)(this.mainFrameViewer)).BeginInit();
         this.SuspendLayout();
         // 
         // OpenFileDialog
         // 
         this.OpenFileDialog.FileName = "OpenFileDialog";
         // 
         // OpenFileButton
         // 
         this.OpenFileButton.Location = new System.Drawing.Point(13, 178);
         this.OpenFileButton.Margin = new System.Windows.Forms.Padding(4);
         this.OpenFileButton.Name = "OpenFileButton";
         this.OpenFileButton.Size = new System.Drawing.Size(125, 34);
         this.OpenFileButton.TabIndex = 0;
         this.OpenFileButton.Text = global::IDS.IDS.Resources.LoadVideo;
         this.OpenFileButton.UseVisualStyleBackColor = true;
         this.OpenFileButton.Click += new System.EventHandler(this._LoadVideoButton_Click);
         // 
         // TypeOfSceneLabel
         // 
         this.TypeOfSceneLabel.AutoSize = true;
         this.TypeOfSceneLabel.Location = new System.Drawing.Point(16, 42);
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
         this.NightSceneRadioButton.Location = new System.Drawing.Point(20, 90);
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
         this.ShowTmpImageCheckBox.Location = new System.Drawing.Point(16, 149);
         this.ShowTmpImageCheckBox.Margin = new System.Windows.Forms.Padding(4);
         this.ShowTmpImageCheckBox.Name = "ShowTmpImageCheckBox";
         this.ShowTmpImageCheckBox.Size = new System.Drawing.Size(109, 21);
         this.ShowTmpImageCheckBox.TabIndex = 6;
         this.ShowTmpImageCheckBox.Text = global::IDS.IDS.Resources.ShowDetails;
         this.ShowTmpImageCheckBox.UseVisualStyleBackColor = true;
         // 
         // StopButton
         // 
         this.StopButton.Location = new System.Drawing.Point(13, 220);
         this.StopButton.Margin = new System.Windows.Forms.Padding(4);
         this.StopButton.Name = "StopButton";
         this.StopButton.Size = new System.Drawing.Size(125, 34);
         this.StopButton.TabIndex = 7;
         this.StopButton.Text = global::IDS.IDS.Resources.ButtonStop_Stop;
         this.StopButton.UseVisualStyleBackColor = true;
         this.StopButton.Click += new System.EventHandler(this._StopButton_Click);
         // 
         // PersonalLabel
         // 
         this.PersonalLabel.AutoSize = true;
         this.PersonalLabel.Location = new System.Drawing.Point(803, 42);
         this.PersonalLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.PersonalLabel.Name = "PersonalLabel";
         this.PersonalLabel.Size = new System.Drawing.Size(64, 17);
         this.PersonalLabel.TabIndex = 9;
         this.PersonalLabel.Text = "Personal";
         // 
         // LoriesLabel
         // 
         this.LoriesLabel.AutoSize = true;
         this.LoriesLabel.Location = new System.Drawing.Point(875, 42);
         this.LoriesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.LoriesLabel.Name = "LoriesLabel";
         this.LoriesLabel.Size = new System.Drawing.Size(52, 17);
         this.LoriesLabel.TabIndex = 10;
         this.LoriesLabel.Text = "Lorries";
         // 
         // TogetherLabel
         // 
         this.TogetherLabel.AutoSize = true;
         this.TogetherLabel.Location = new System.Drawing.Point(935, 42);
         this.TogetherLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
         this.TogetherLabel.Name = "TogetherLabel";
         this.TogetherLabel.Size = new System.Drawing.Size(66, 17);
         this.TogetherLabel.TabIndex = 11;
         this.TogetherLabel.Text = "Together";
         // 
         // FpsLabel
         // 
         this.FpsLabel.AutoSize = true;
         this.FpsLabel.Location = new System.Drawing.Point(1031, 11);
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
         this.CarInfoView.Location = new System.Drawing.Point(170, 42);
         this.CarInfoView.Name = "CarInfoView";
         this.CarInfoView.Size = new System.Drawing.Size(626, 561);
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
         // CarTypeOfVehicleHeader
         // 
         this.CarTypeOfVehicleHeader.Text = global::IDS.IDS.Resources.CarType;
         this.CarTypeOfVehicleHeader.Width = 100;
         // 
         // CarIdOfVehicleHeader
         // 
         this.CarIdOfVehicleHeader.Text = global::IDS.IDS.Resources.CarId;
         this.CarIdOfVehicleHeader.Width = 70;
         // 
         // TypeOfVehicleHeader
         // 
         this.TypeOfVehicleHeader.Text = global::IDS.IDS.Resources.TypeOf;
         this.TypeOfVehicleHeader.Width = 100;
         // 
         // SpeedOfVehicleHeader
         // 
         this.SpeedOfVehicleHeader.Text = global::IDS.IDS.Resources.Speed;
         this.SpeedOfVehicleHeader.Width = 70;
         // 
         // CarImageOfVehicleHeader
         // 
         this.CarImageOfVehicleHeader.Text = global::IDS.IDS.Resources.CarBrand;
         this.CarImageOfVehicleHeader.Width = 100;
         // 
         // CarInfoViewImageList
         // 
         this.CarInfoViewImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
         this.CarInfoViewImageList.ImageSize = new System.Drawing.Size(128, 128);
         this.CarInfoViewImageList.TransparentColor = System.Drawing.Color.Transparent;
         // 
         // pictureBox1
         // 
         this.mainFrameViewer.Location = new System.Drawing.Point(806, 72);
         this.mainFrameViewer.Name = "mainFrameViewer";
         this.mainFrameViewer.Size = new System.Drawing.Size(358, 227);
         this.mainFrameViewer.TabIndex = 13;
         this.mainFrameViewer.TabStop = false;
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1176, 613);
         this.Controls.Add(this.mainFrameViewer);
         this.Controls.Add(this.CarInfoView);
         this.Controls.Add(this.FpsLabel);
         this.Controls.Add(this.TogetherLabel);
         this.Controls.Add(this.LoriesLabel);
         this.Controls.Add(this.PersonalLabel);
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
      private System.Windows.Forms.Label TypeOfSceneLabel;
      private System.Windows.Forms.RadioButton DaySceneRadioButton;
      private System.Windows.Forms.RadioButton NightSceneRadioButton;
      private System.Windows.Forms.CheckBox ShowTmpImageCheckBox;
      private System.Windows.Forms.Button StopButton;
      private System.Windows.Forms.Label PersonalLabel;
      private System.Windows.Forms.Label LoriesLabel;
      private System.Windows.Forms.Label TogetherLabel;
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
   }
}

