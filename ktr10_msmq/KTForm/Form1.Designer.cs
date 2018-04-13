namespace KTForm
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
            this.bOpenCDM = new System.Windows.Forms.Button();
            this.bOpenCIM = new System.Windows.Forms.Button();
            this.bExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bOpenCDM
            // 
            this.bOpenCDM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.bOpenCDM.Location = new System.Drawing.Point(18, 15);
            this.bOpenCDM.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bOpenCDM.Name = "bOpenCDM";
            this.bOpenCDM.Size = new System.Drawing.Size(330, 35);
            this.bOpenCDM.TabIndex = 0;
            this.bOpenCDM.Text = "CDM";
            this.bOpenCDM.UseVisualStyleBackColor = true;
            this.bOpenCDM.Click += new System.EventHandler(this.bOpenCDM_Click);
            // 
            // bOpenCIM
            // 
            this.bOpenCIM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.bOpenCIM.Location = new System.Drawing.Point(18, 60);
            this.bOpenCIM.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bOpenCIM.Name = "bOpenCIM";
            this.bOpenCIM.Size = new System.Drawing.Size(330, 35);
            this.bOpenCIM.TabIndex = 1;
            this.bOpenCIM.Text = "CIM";
            this.bOpenCIM.UseVisualStyleBackColor = true;
            this.bOpenCIM.Click += new System.EventHandler(this.bOpenCIM_Click);
            // 
            // bExit
            // 
            this.bExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.bExit.Location = new System.Drawing.Point(18, 105);
            this.bExit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(330, 35);
            this.bExit.TabIndex = 2;
            this.bExit.Text = "EXIT";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.bExit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 154);
            this.Controls.Add(this.bExit);
            this.Controls.Add(this.bOpenCIM);
            this.Controls.Add(this.bOpenCDM);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SmartTellerMachine - CRM";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bOpenCDM;
        private System.Windows.Forms.Button bOpenCIM;
        private System.Windows.Forms.Button bExit;
    }
}

