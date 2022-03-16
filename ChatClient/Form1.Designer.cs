
namespace ChatClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtb_Message = new System.Windows.Forms.RichTextBox();
            this.rtb_chatBox = new System.Windows.Forms.RichTextBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.tb_myName = new System.Windows.Forms.TextBox();
            this.label_NickName = new System.Windows.Forms.Label();
            this.tb_userNames = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // rtb_Message
            // 
            this.rtb_Message.Location = new System.Drawing.Point(12, 42);
            this.rtb_Message.Name = "rtb_Message";
            this.rtb_Message.ReadOnly = true;
            this.rtb_Message.Size = new System.Drawing.Size(472, 400);
            this.rtb_Message.TabIndex = 0;
            this.rtb_Message.Text = "";
            // 
            // rtb_chatBox
            // 
            this.rtb_chatBox.Location = new System.Drawing.Point(13, 449);
            this.rtb_chatBox.Name = "rtb_chatBox";
            this.rtb_chatBox.Size = new System.Drawing.Size(402, 74);
            this.rtb_chatBox.TabIndex = 1;
            this.rtb_chatBox.Text = "";
            // 
            // btn_Send
            // 
            this.btn_Send.Location = new System.Drawing.Point(422, 449);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(62, 74);
            this.btn_Send.TabIndex = 2;
            this.btn_Send.Text = "전송";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // tb_myName
            // 
            this.tb_myName.Location = new System.Drawing.Point(383, 13);
            this.tb_myName.Name = "tb_myName";
            this.tb_myName.Size = new System.Drawing.Size(100, 23);
            this.tb_myName.TabIndex = 3;
            // 
            // label_NickName
            // 
            this.label_NickName.AutoSize = true;
            this.label_NickName.Font = new System.Drawing.Font("AppleSDGothicNeoB00", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_NickName.Location = new System.Drawing.Point(326, 15);
            this.label_NickName.Name = "label_NickName";
            this.label_NickName.Size = new System.Drawing.Size(51, 17);
            this.label_NickName.TabIndex = 4;
            this.label_NickName.Text = "내 이름";
            // 
            // tb_userNames
            // 
            this.tb_userNames.Location = new System.Drawing.Point(13, 13);
            this.tb_userNames.Name = "tb_userNames";
            this.tb_userNames.ReadOnly = true;
            this.tb_userNames.Size = new System.Drawing.Size(307, 23);
            this.tb_userNames.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 535);
            this.Controls.Add(this.tb_userNames);
            this.Controls.Add(this.label_NickName);
            this.Controls.Add(this.tb_myName);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.rtb_chatBox);
            this.Controls.Add(this.rtb_Message);
            this.Name = "Form1";
            this.Text = "채팅 프로그램";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb_Message;
        private System.Windows.Forms.RichTextBox rtb_chatBox;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.TextBox tb_myName;
        private System.Windows.Forms.Label label_NickName;
        private System.Windows.Forms.TextBox tb_userNames;
    }
}

