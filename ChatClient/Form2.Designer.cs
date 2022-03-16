
namespace ChatClient
{
    partial class Form2
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
            this.btn_Entrance = new System.Windows.Forms.Button();
            this.tb_userName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Entrance
            // 
            this.btn_Entrance.Location = new System.Drawing.Point(107, 129);
            this.btn_Entrance.Name = "btn_Entrance";
            this.btn_Entrance.Size = new System.Drawing.Size(75, 23);
            this.btn_Entrance.TabIndex = 0;
            this.btn_Entrance.Text = "입장";
            this.btn_Entrance.UseVisualStyleBackColor = true;
            this.btn_Entrance.Click += new System.EventHandler(this.btn_Entrance_Click);
            // 
            // tb_userName
            // 
            this.tb_userName.Location = new System.Drawing.Point(77, 72);
            this.tb_userName.Name = "tb_userName";
            this.tb_userName.Size = new System.Drawing.Size(128, 23);
            this.tb_userName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("AppleSDGothicNeoEB00", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(99, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "닉네임 설정";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 193);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_userName);
            this.Controls.Add(this.btn_Entrance);
            this.Name = "Form2";
            this.Text = "채팅 프로그램";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Entrance;
        private System.Windows.Forms.TextBox tb_userName;
        private System.Windows.Forms.Label label1;
    }
}