namespace Captchas
{
    partial class FrmCaptcha
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
            this.CaptchaPic = new System.Windows.Forms.PictureBox();
            this.txtCaptcha = new System.Windows.Forms.TextBox();
            this.btnCreateCaptcha = new System.Windows.Forms.Button();
            this.webHotmail = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.CaptchaPic)).BeginInit();
            this.SuspendLayout();
            // 
            // CaptchaPic
            // 
            this.CaptchaPic.Location = new System.Drawing.Point(55, 342);
            this.CaptchaPic.Name = "CaptchaPic";
            this.CaptchaPic.Size = new System.Drawing.Size(229, 77);
            this.CaptchaPic.TabIndex = 0;
            this.CaptchaPic.TabStop = false;
            // 
            // txtCaptcha
            // 
            this.txtCaptcha.Location = new System.Drawing.Point(392, 388);
            this.txtCaptcha.Name = "txtCaptcha";
            this.txtCaptcha.Size = new System.Drawing.Size(328, 20);
            this.txtCaptcha.TabIndex = 1;
            // 
            // btnCreateCaptcha
            // 
            this.btnCreateCaptcha.Location = new System.Drawing.Point(464, 341);
            this.btnCreateCaptcha.Name = "btnCreateCaptcha";
            this.btnCreateCaptcha.Size = new System.Drawing.Size(157, 23);
            this.btnCreateCaptcha.TabIndex = 2;
            this.btnCreateCaptcha.Text = "Create Email";
            this.btnCreateCaptcha.UseVisualStyleBackColor = true;
            this.btnCreateCaptcha.Click += new System.EventHandler(this.btnCreateCaptcha_Click);
            // 
            // webHotmail
            // 
            this.webHotmail.Location = new System.Drawing.Point(39, 22);
            this.webHotmail.MinimumSize = new System.Drawing.Size(20, 20);
            this.webHotmail.Name = "webHotmail";
            this.webHotmail.Size = new System.Drawing.Size(855, 308);
            this.webHotmail.TabIndex = 8;
            this.webHotmail.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webHotmail_DocumentCompleted);
            // 
            // FrmCaptcha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 431);
            this.Controls.Add(this.webHotmail);
            this.Controls.Add(this.btnCreateCaptcha);
            this.Controls.Add(this.txtCaptcha);
            this.Controls.Add(this.CaptchaPic);
            this.Name = "FrmCaptcha";
            this.Text = "Captcha";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCaptcha_FormClosed);
            this.Load += new System.EventHandler(this.FrmCaptcha_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CaptchaPic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox CaptchaPic;
        private System.Windows.Forms.TextBox txtCaptcha;
        private System.Windows.Forms.Button btnCreateCaptcha;
        private System.Windows.Forms.WebBrowser webHotmail;
    }
}