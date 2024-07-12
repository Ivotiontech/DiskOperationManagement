namespace DiskOperationManagementApp
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtAccessURL = new System.Windows.Forms.TextBox();
            this.btnAccess = new System.Windows.Forms.Button();
            this.txtAccessKey = new System.Windows.Forms.TextBox();
            this.lblAccessURL = new System.Windows.Forms.Label();
            this.lblAccessKey = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtAccessURL
            // 
            this.txtAccessURL.Location = new System.Drawing.Point(12, 27);
            this.txtAccessURL.Name = "txtAccessURL";
            this.txtAccessURL.Size = new System.Drawing.Size(209, 20);
            this.txtAccessURL.TabIndex = 2;
            this.txtAccessURL.Visible = false;
            // 
            // btnAccess
            // 
            this.btnAccess.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnAccess.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.btnAccess.FlatAppearance.BorderSize = 0;
            this.btnAccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccess.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAccess.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAccess.Location = new System.Drawing.Point(253, 18);
            this.btnAccess.Name = "btnAccess";
            this.btnAccess.Size = new System.Drawing.Size(125, 35);
            this.btnAccess.TabIndex = 3;
            this.btnAccess.Text = "Access";
            this.btnAccess.UseVisualStyleBackColor = false;
            this.btnAccess.Click += new System.EventHandler(this.btnAccess_Click);
            // 
            // txtAccessKey
            // 
            this.txtAccessKey.Location = new System.Drawing.Point(12, 27);
            this.txtAccessKey.Name = "txtAccessKey";
            this.txtAccessKey.Size = new System.Drawing.Size(209, 20);
            this.txtAccessKey.TabIndex = 4;
            // 
            // lblAccessURL
            // 
            this.lblAccessURL.AutoSize = true;
            this.lblAccessURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessURL.Location = new System.Drawing.Point(13, 8);
            this.lblAccessURL.Name = "lblAccessURL";
            this.lblAccessURL.Size = new System.Drawing.Size(77, 13);
            this.lblAccessURL.TabIndex = 5;
            this.lblAccessURL.Text = "Access URL";
            this.lblAccessURL.Visible = false;
            // 
            // lblAccessKey
            // 
            this.lblAccessKey.AutoSize = true;
            this.lblAccessKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessKey.Location = new System.Drawing.Point(12, 8);
            this.lblAccessKey.Name = "lblAccessKey";
            this.lblAccessKey.Size = new System.Drawing.Size(73, 13);
            this.lblAccessKey.TabIndex = 6;
            this.lblAccessKey.Text = "Access Key";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 66);
            this.Controls.Add(this.lblAccessKey);
            this.Controls.Add(this.lblAccessURL);
            this.Controls.Add(this.txtAccessKey);
            this.Controls.Add(this.btnAccess);
            this.Controls.Add(this.txtAccessURL);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Legallogger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtAccessURL;
        private System.Windows.Forms.Button btnAccess;
        private System.Windows.Forms.TextBox txtAccessKey;
        private System.Windows.Forms.Label lblAccessURL;
        private System.Windows.Forms.Label lblAccessKey;
    }
}

