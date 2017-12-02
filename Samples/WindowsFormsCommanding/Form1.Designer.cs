namespace WindowsFormsCommanding
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
            this._textbox = new System.Windows.Forms.TextBox();
            this._runCommandButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _textbox
            // 
            this._textbox.Location = new System.Drawing.Point(12, 63);
            this._textbox.Multiline = true;
            this._textbox.Name = "_textbox";
            this._textbox.Size = new System.Drawing.Size(730, 286);
            this._textbox.TabIndex = 0;
            // 
            // _runCommandButton
            // 
            this._runCommandButton.Location = new System.Drawing.Point(12, 12);
            this._runCommandButton.Name = "_runCommandButton";
            this._runCommandButton.Size = new System.Drawing.Size(730, 45);
            this._runCommandButton.TabIndex = 1;
            this._runCommandButton.Text = "Run Command";
            this._runCommandButton.UseVisualStyleBackColor = true;
            this._runCommandButton.Click += new System.EventHandler(this._runCommandButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 361);
            this.Controls.Add(this._runCommandButton);
            this.Controls.Add(this._textbox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _textbox;
        private System.Windows.Forms.Button _runCommandButton;
    }
}

