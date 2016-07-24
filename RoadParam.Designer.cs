namespace IDS
{
    partial class RoadParamForm
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
            this.framePictureBox = new System.Windows.Forms.PictureBox();
            this.saveRoadPointsButton = new System.Windows.Forms.Button();
            this.saveRoadDistancePointsButton = new System.Windows.Forms.Button();
            this.realDistanceTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numberOfLanesTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.framePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // framePictureBox
            // 
            this.framePictureBox.Location = new System.Drawing.Point(38, 12);
            this.framePictureBox.Name = "framePictureBox";
            this.framePictureBox.Size = new System.Drawing.Size(425, 271);
            this.framePictureBox.TabIndex = 0;
            this.framePictureBox.TabStop = false;
            this.framePictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.framePictureBox_Paint);
            this.framePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.framePictureBox_MouseDown);
            this.framePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.framePictureBox_MouseMove);
            this.framePictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.framePictureBox_MouseUp);
            // 
            // saveRoadPointsButton
            // 
            this.saveRoadPointsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveRoadPointsButton.Location = new System.Drawing.Point(528, 82);
            this.saveRoadPointsButton.Name = "saveRoadPointsButton";
            this.saveRoadPointsButton.Size = new System.Drawing.Size(100, 38);
            this.saveRoadPointsButton.TabIndex = 1;
            this.saveRoadPointsButton.Text = "Uložiť nastavenia cesty";
            this.saveRoadPointsButton.UseVisualStyleBackColor = true;
            this.saveRoadPointsButton.Click += new System.EventHandler(this.saveRoadPointsButton_Click);
            // 
            // saveRoadDistancePointsButton
            // 
            this.saveRoadDistancePointsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveRoadDistancePointsButton.Location = new System.Drawing.Point(528, 196);
            this.saveRoadDistancePointsButton.Name = "saveRoadDistancePointsButton";
            this.saveRoadDistancePointsButton.Size = new System.Drawing.Size(100, 39);
            this.saveRoadDistancePointsButton.TabIndex = 2;
            this.saveRoadDistancePointsButton.Text = "Uložiť referenčnú vzdialenosť";
            this.saveRoadDistancePointsButton.UseVisualStyleBackColor = true;
            this.saveRoadDistancePointsButton.Click += new System.EventHandler(this.saveRoadDistancePointsButton_Click);
            // 
            // realDistanceTextBox
            // 
            this.realDistanceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.realDistanceTextBox.Location = new System.Drawing.Point(528, 170);
            this.realDistanceTextBox.Name = "realDistanceTextBox";
            this.realDistanceTextBox.Size = new System.Drawing.Size(55, 20);
            this.realDistanceTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(525, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Reálna vzdialenosť:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(525, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Počet pruhov";
            // 
            // numberOfLanesTextBox
            // 
            this.numberOfLanesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numberOfLanesTextBox.Location = new System.Drawing.Point(530, 42);
            this.numberOfLanesTextBox.Name = "numberOfLanesTextBox";
            this.numberOfLanesTextBox.Size = new System.Drawing.Size(53, 20);
            this.numberOfLanesTextBox.TabIndex = 6;
            this.numberOfLanesTextBox.Text = "2";
            // 
            // RoadParamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 321);
            this.Controls.Add(this.numberOfLanesTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.realDistanceTextBox);
            this.Controls.Add(this.saveRoadDistancePointsButton);
            this.Controls.Add(this.saveRoadPointsButton);
            this.Controls.Add(this.framePictureBox);
            this.Name = "RoadParamForm";
            this.Text = "Nastavenie vlastnosti cesty";
            ((System.ComponentModel.ISupportInitialize)(this.framePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox framePictureBox;
        private System.Windows.Forms.Button saveRoadPointsButton;
        private System.Windows.Forms.Button saveRoadDistancePointsButton;
        private System.Windows.Forms.TextBox realDistanceTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox numberOfLanesTextBox;
    }
}