namespace Groups
{
    partial class FrmGroupCampaignManagerSetting
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpcampaign = new System.Windows.Forms.DateTimePicker();
            this.btn_SaveSettings = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_NoOfMessages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpcampaign);
            this.groupBox1.Controls.Add(this.btn_SaveSettings);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_NoOfMessages);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(44, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 220);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Campaign Setting";
            // 
            // dtpcampaign
            // 
            this.dtpcampaign.Location = new System.Drawing.Point(192, 114);
            this.dtpcampaign.Name = "dtpcampaign";
            this.dtpcampaign.Size = new System.Drawing.Size(224, 21);
            this.dtpcampaign.TabIndex = 5;
            // 
            // btn_SaveSettings
            // 
            this.btn_SaveSettings.Location = new System.Drawing.Point(149, 170);
            this.btn_SaveSettings.Name = "btn_SaveSettings";
            this.btn_SaveSettings.Size = new System.Drawing.Size(120, 23);
            this.btn_SaveSettings.TabIndex = 4;
            this.btn_SaveSettings.Text = "Save Settings";
            this.btn_SaveSettings.UseVisualStyleBackColor = true;
            this.btn_SaveSettings.Click += new System.EventHandler(this.btn_SaveSettings_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Schedule Time";
            // 
            // txt_NoOfMessages
            // 
            this.txt_NoOfMessages.Location = new System.Drawing.Point(192, 44);
            this.txt_NoOfMessages.Name = "txt_NoOfMessages";
            this.txt_NoOfMessages.Size = new System.Drawing.Size(100, 21);
            this.txt_NoOfMessages.TabIndex = 1;
            this.txt_NoOfMessages.Text = "100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number Of Messages per Hour";
            // 
            // FrmGroupCampaignManagerSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 269);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmGroupCampaignManagerSetting";
            this.Text = "Group Campaign Manager Setting";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpcampaign;
        private System.Windows.Forms.Button btn_SaveSettings;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_NoOfMessages;
        private System.Windows.Forms.Label label1;
    }
}