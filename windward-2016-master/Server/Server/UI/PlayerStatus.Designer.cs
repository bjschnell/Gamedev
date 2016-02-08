namespace Server.UI
{
	partial class PlayerStatus
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerStatus));
			this.labelScore = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.pictWinner = new System.Windows.Forms.PictureBox();
			this.pictNoConnection = new System.Windows.Forms.PictureBox();
			this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
			this.labelFirst = new System.Windows.Forms.Label();
			this.labelSecond = new System.Windows.Forms.Label();
			this.labelStock = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictWinner)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictNoConnection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
			this.SuspendLayout();
			// 
			// labelScore
			// 
			this.labelScore.AutoSize = true;
			this.labelScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelScore.Location = new System.Drawing.Point(42, 4);
			this.labelScore.Name = "labelScore";
			this.labelScore.Size = new System.Drawing.Size(48, 26);
			this.labelScore.TabIndex = 1;
			this.labelScore.Text = "000";
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelName.AutoEllipsis = true;
			this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelName.Location = new System.Drawing.Point(4, 39);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(302, 17);
			this.labelName.TabIndex = 3;
			this.labelName.Text = "David Thielen";
			// 
			// pictWinner
			// 
			this.pictWinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictWinner.Image = global::Server.Sprites.StatusSprites.signal_flag_checkered;
			this.pictWinner.Location = new System.Drawing.Point(254, 3);
			this.pictWinner.Name = "pictWinner";
			this.pictWinner.Size = new System.Drawing.Size(48, 48);
			this.pictWinner.TabIndex = 10;
			this.pictWinner.TabStop = false;
			// 
			// pictNoConnection
			// 
			this.pictNoConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictNoConnection.Image = global::Server.Sprites.StatusSprites.flash;
			this.pictNoConnection.Location = new System.Drawing.Point(278, 3);
			this.pictNoConnection.Name = "pictNoConnection";
			this.pictNoConnection.Size = new System.Drawing.Size(24, 24);
			this.pictNoConnection.TabIndex = 9;
			this.pictNoConnection.TabStop = false;
			this.pictNoConnection.Visible = false;
			// 
			// pictureBoxAvatar
			// 
			this.pictureBoxAvatar.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxAvatar.Image")));
			this.pictureBoxAvatar.Location = new System.Drawing.Point(4, 4);
			this.pictureBoxAvatar.Name = "pictureBoxAvatar";
			this.pictureBoxAvatar.Size = new System.Drawing.Size(32, 32);
			this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxAvatar.TabIndex = 0;
			this.pictureBoxAvatar.TabStop = false;
			// 
			// labelFirst
			// 
			this.labelFirst.AutoSize = true;
			this.labelFirst.Location = new System.Drawing.Point(22, 60);
			this.labelFirst.Name = "labelFirst";
			this.labelFirst.Size = new System.Drawing.Size(29, 13);
			this.labelFirst.TabIndex = 11;
			this.labelFirst.Text = "First:";
			// 
			// labelSecond
			// 
			this.labelSecond.AutoSize = true;
			this.labelSecond.Location = new System.Drawing.Point(4, 77);
			this.labelSecond.Name = "labelSecond";
			this.labelSecond.Size = new System.Drawing.Size(47, 13);
			this.labelSecond.TabIndex = 12;
			this.labelSecond.Text = "Second:";
			// 
			// labelStock
			// 
			this.labelStock.AutoSize = true;
			this.labelStock.Location = new System.Drawing.Point(13, 94);
			this.labelStock.Name = "labelStock";
			this.labelStock.Size = new System.Drawing.Size(38, 13);
			this.labelStock.TabIndex = 13;
			this.labelStock.Text = "Stock:";
			// 
			// PlayerStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.labelStock);
			this.Controls.Add(this.labelSecond);
			this.Controls.Add(this.labelFirst);
			this.Controls.Add(this.pictNoConnection);
			this.Controls.Add(this.pictWinner);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.labelScore);
			this.Controls.Add(this.pictureBoxAvatar);
			this.DoubleBuffered = true;
			this.Name = "PlayerStatus";
			this.Size = new System.Drawing.Size(305, 113);
			this.Load += new System.EventHandler(this.PlayerStatus_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PlayerStatus_Paint);
			((System.ComponentModel.ISupportInitialize)(this.pictWinner)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictNoConnection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxAvatar;
		private System.Windows.Forms.Label labelScore;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.PictureBox pictNoConnection;
		private System.Windows.Forms.PictureBox pictWinner;
		private System.Windows.Forms.Label labelFirst;
		private System.Windows.Forms.Label labelSecond;
		private System.Windows.Forms.Label labelStock;
	}
}
