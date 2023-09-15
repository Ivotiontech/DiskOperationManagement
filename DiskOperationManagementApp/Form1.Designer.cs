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
            this.txtAccessKey = new System.Windows.Forms.TextBox();
            this.btnAccess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAccessKey
            // 
            this.txtAccessKey.Location = new System.Drawing.Point(12, 27);
            this.txtAccessKey.Name = "txtAccessKey";
            this.txtAccessKey.Size = new System.Drawing.Size(209, 20);
            this.txtAccessKey.TabIndex = 2;
            // 
            // btnAccess
            // 
            this.btnAccess.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnAccess.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.btnAccess.FlatAppearance.BorderSize = 0;
            this.btnAccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccess.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAccess.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAccess.Location = new System.Drawing.Point(244, 18);
            this.btnAccess.Name = "btnAccess";
            this.btnAccess.Size = new System.Drawing.Size(125, 35);
            this.btnAccess.TabIndex = 3;
            this.btnAccess.Text = "Access";
            this.btnAccess.UseVisualStyleBackColor = false;
            this.btnAccess.Click += new System.EventHandler(this.btnAccess_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 79);
            this.Controls.Add(this.btnAccess);
            this.Controls.Add(this.txtAccessKey);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Disk Operation System";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtAccessKey;
        private System.Windows.Forms.Button btnAccess;
    }
}

