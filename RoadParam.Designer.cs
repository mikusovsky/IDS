namespace IDS
{
   partial class RoadParamForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer _components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (_components != null))
         {
            _components.Dispose();
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
         this._framePictureBox = new System.Windows.Forms.PictureBox();
         this._saveRoadPointsButton = new System.Windows.Forms.Button();
         this._saveRoadDistancePointsButton = new System.Windows.Forms.Button();
         this._realDistanceTextBox = new System.Windows.Forms.TextBox();
         this._realDistinctLabel = new System.Windows.Forms.Label();
         this._countRoutsLabel = new System.Windows.Forms.Label();
         this._numberOfLanesTextBox = new System.Windows.Forms.TextBox();
         ((System.ComponentModel.ISupportInitialize)(this._framePictureBox)).BeginInit();
         this.SuspendLayout();
         // 
         // framePictureBox
         // 
         this._framePictureBox.Location = new System.Drawing.Point(38, 12);
         this._framePictureBox.Name = "_framePictureBox";
         this._framePictureBox.Size = new System.Drawing.Size(425, 271);
         this._framePictureBox.TabIndex = 0;
         this._framePictureBox.TabStop = false;
         this._framePictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this._FramePictureBox_Paint);
         this._framePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FramePictureBox_MouseDown);
         this._framePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this._FramePictureBox_MouseMove);
         this._framePictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FramePictureBox_MouseUp);
         // 
         // saveRoadPointsButton
         // 
         this._saveRoadPointsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this._saveRoadPointsButton.Location = new System.Drawing.Point(528, 82);
         this._saveRoadPointsButton.Name = "_saveRoadPointsButton";
         this._saveRoadPointsButton.Size = new System.Drawing.Size(100, 38);
         this._saveRoadPointsButton.TabIndex = 1;
         this._saveRoadPointsButton.Text = "Uložiť nastavenia cesty";
         this._saveRoadPointsButton.UseVisualStyleBackColor = true;
         this._saveRoadPointsButton.Click += new System.EventHandler(this.SaveRoadPointsButton_Click);
         // 
         // saveRoadDistancePointsButton
         // 
         this._saveRoadDistancePointsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this._saveRoadDistancePointsButton.Location = new System.Drawing.Point(528, 196);
         this._saveRoadDistancePointsButton.Name = "_saveRoadDistancePointsButton";
         this._saveRoadDistancePointsButton.Size = new System.Drawing.Size(100, 39);
         this._saveRoadDistancePointsButton.TabIndex = 2;
         this._saveRoadDistancePointsButton.Text = "Uložiť referenčnú vzdialenosť";
         this._saveRoadDistancePointsButton.UseVisualStyleBackColor = true;
         this._saveRoadDistancePointsButton.Click += new System.EventHandler(this.SaveRoadDistancePointsButton_Click);
         // 
         // realDistanceTextBox
         // 
         this._realDistanceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this._realDistanceTextBox.Location = new System.Drawing.Point(528, 170);
         this._realDistanceTextBox.Name = "_realDistanceTextBox";
         this._realDistanceTextBox.Size = new System.Drawing.Size(55, 20);
         this._realDistanceTextBox.TabIndex = 3;
         // 
         // label1
         // 
         this._realDistinctLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this._realDistinctLabel.AutoSize = true;
         this._realDistinctLabel.Location = new System.Drawing.Point(525, 154);
         this._realDistinctLabel.Name = "_realDistinctLabel";
         this._realDistinctLabel.Size = new System.Drawing.Size(101, 13);
         this._realDistinctLabel.TabIndex = 4;
         this._realDistinctLabel.Text = "Reálna vzdialenosť:";
         // 
         // label2
         // 
         this._countRoutsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this._countRoutsLabel.AutoSize = true;
         this._countRoutsLabel.Location = new System.Drawing.Point(525, 24);
         this._countRoutsLabel.Name = "_countRoutsLabel";
         this._countRoutsLabel.Size = new System.Drawing.Size(71, 13);
         this._countRoutsLabel.TabIndex = 5;
         this._countRoutsLabel.Text = "Počet pruhov";
         // 
         // numberOfLanesTextBox
         // 
         this._numberOfLanesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this._numberOfLanesTextBox.Location = new System.Drawing.Point(530, 42);
         this._numberOfLanesTextBox.Name = "_numberOfLanesTextBox";
         this._numberOfLanesTextBox.Size = new System.Drawing.Size(53, 20);
         this._numberOfLanesTextBox.TabIndex = 6;
         this._numberOfLanesTextBox.Text = "2";
         // 
         // RoadParamForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(645, 321);
         this.Controls.Add(this._numberOfLanesTextBox);
         this.Controls.Add(this._countRoutsLabel);
         this.Controls.Add(this._realDistinctLabel);
         this.Controls.Add(this._realDistanceTextBox);
         this.Controls.Add(this._saveRoadDistancePointsButton);
         this.Controls.Add(this._saveRoadPointsButton);
         this.Controls.Add(this._framePictureBox);
         this.Name = "RoadParamForm";
         this.Text = "Nastavenie vlastnosti cesty";
         ((System.ComponentModel.ISupportInitialize)(this._framePictureBox)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.PictureBox _framePictureBox;
      private System.Windows.Forms.Button _saveRoadPointsButton;
      private System.Windows.Forms.Button _saveRoadDistancePointsButton;
      private System.Windows.Forms.TextBox _realDistanceTextBox;
      private System.Windows.Forms.Label _realDistinctLabel;
      private System.Windows.Forms.Label _countRoutsLabel;
      private System.Windows.Forms.TextBox _numberOfLanesTextBox;
   }
}