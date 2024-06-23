namespace topin
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
            this.overlayButton = new System.Windows.Forms.Button();
            this.anotherButton = new System.Windows.Forms.Button();
            this.clickLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // overlayButton
            // 
            this.overlayButton.Location = new System.Drawing.Point(1010, 550); // Adjust coordinates based on label's printed coordinates
            this.overlayButton.Name = "overlayButton";
            this.overlayButton.Size = new System.Drawing.Size(75, 23);
            this.overlayButton.TabIndex = 0;
            this.overlayButton.Text = "Close";
            this.overlayButton.UseVisualStyleBackColor = true;
            this.overlayButton.Click += new System.EventHandler(this.OverlayButton_Click);
            this.overlayButton.BringToFront();
            // 
            // anotherButton
            // 
            this.anotherButton.Location = new System.Drawing.Point(1085, 550); // Adjust coordinates based on label's printed coordinates
            this.anotherButton.Name = "anotherButton";
            this.anotherButton.Size = new System.Drawing.Size(75, 23);
            this.anotherButton.TabIndex = 1;
            this.anotherButton.Text = "Action";
            this.anotherButton.UseVisualStyleBackColor = true;
            this.anotherButton.Click += new System.EventHandler(this.AnotherButton_Click);
            this.anotherButton.BringToFront();
            // 
            // clickLabel
            // 
            this.clickLabel.AutoSize = true;
            this.clickLabel.Location = new System.Drawing.Point(11, 800); // Adjust coordinates if necessary
            this.clickLabel.Name = "clickLabel";
            this.clickLabel.Size = new System.Drawing.Size(0, 13);
            this.clickLabel.TabIndex = 2;
            this.clickLabel.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clickLabel.ForeColor = System.Drawing.Color.Red;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.clickLabel);
            this.Controls.Add(this.anotherButton);
            this.Controls.Add(this.overlayButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CLICK";
            this.Text = "CLICK";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.BackColor = System.Drawing.Color.Lime;
            this.TransparencyKey = System.Drawing.Color.Lime;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion


    }
}
