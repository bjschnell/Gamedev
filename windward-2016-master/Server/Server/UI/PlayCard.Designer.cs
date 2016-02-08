namespace Server.UI
{
	partial class PlayCard
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
			this.labelPlayer = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbTake5Tiles = new System.Windows.Forms.RadioButton();
			this.rbPlace4Tiles = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rbNoneBefore = new System.Windows.Forms.RadioButton();
			this.rb3Free = new System.Windows.Forms.RadioButton();
			this.rbBuy5 = new System.Windows.Forms.RadioButton();
			this.rbTrade2 = new System.Windows.Forms.RadioButton();
			this.rbNoneAfter = new System.Windows.Forms.RadioButton();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelPlayer
			// 
			this.labelPlayer.AutoSize = true;
			this.labelPlayer.Location = new System.Drawing.Point(13, 13);
			this.labelPlayer.Name = "labelPlayer";
			this.labelPlayer.Size = new System.Drawing.Size(43, 13);
			this.labelPlayer.TabIndex = 0;
			this.labelPlayer.Text = "{player}";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbNoneBefore);
			this.groupBox1.Controls.Add(this.rbPlace4Tiles);
			this.groupBox1.Controls.Add(this.rbTake5Tiles);
			this.groupBox1.Location = new System.Drawing.Point(16, 30);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(156, 95);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Before Place Tile";
			// 
			// rbTake5Tiles
			// 
			this.rbTake5Tiles.AutoSize = true;
			this.rbTake5Tiles.Location = new System.Drawing.Point(7, 20);
			this.rbTake5Tiles.Name = "rbTake5Tiles";
			this.rbTake5Tiles.Size = new System.Drawing.Size(84, 17);
			this.rbTake5Tiles.TabIndex = 0;
			this.rbTake5Tiles.TabStop = true;
			this.rbTake5Tiles.Text = "Take 5 Tiles";
			this.rbTake5Tiles.UseVisualStyleBackColor = true;
			// 
			// rbPlace4Tiles
			// 
			this.rbPlace4Tiles.AutoSize = true;
			this.rbPlace4Tiles.Location = new System.Drawing.Point(7, 44);
			this.rbPlace4Tiles.Name = "rbPlace4Tiles";
			this.rbPlace4Tiles.Size = new System.Drawing.Size(86, 17);
			this.rbPlace4Tiles.TabIndex = 1;
			this.rbPlace4Tiles.Text = "Place 4 Tiles";
			this.rbPlace4Tiles.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rbNoneAfter);
			this.groupBox2.Controls.Add(this.rbTrade2);
			this.groupBox2.Controls.Add(this.rbBuy5);
			this.groupBox2.Controls.Add(this.rb3Free);
			this.groupBox2.Location = new System.Drawing.Point(16, 131);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(156, 123);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "After Place Tile";
			// 
			// rbNoneBefore
			// 
			this.rbNoneBefore.AutoSize = true;
			this.rbNoneBefore.Checked = true;
			this.rbNoneBefore.Location = new System.Drawing.Point(7, 68);
			this.rbNoneBefore.Name = "rbNoneBefore";
			this.rbNoneBefore.Size = new System.Drawing.Size(49, 17);
			this.rbNoneBefore.TabIndex = 2;
			this.rbNoneBefore.TabStop = true;
			this.rbNoneBefore.Text = "none";
			this.rbNoneBefore.UseVisualStyleBackColor = true;
			// 
			// rb3Free
			// 
			this.rb3Free.AutoSize = true;
			this.rb3Free.Location = new System.Drawing.Point(7, 20);
			this.rb3Free.Name = "rb3Free";
			this.rb3Free.Size = new System.Drawing.Size(95, 17);
			this.rb3Free.TabIndex = 0;
			this.rb3Free.TabStop = true;
			this.rb3Free.Text = "3 Free (shares)";
			this.rb3Free.UseVisualStyleBackColor = true;
			// 
			// rbBuy5
			// 
			this.rbBuy5.AutoSize = true;
			this.rbBuy5.Location = new System.Drawing.Point(7, 44);
			this.rbBuy5.Name = "rbBuy5";
			this.rbBuy5.Size = new System.Drawing.Size(92, 17);
			this.rbBuy5.TabIndex = 1;
			this.rbBuy5.Text = "Buy 5 (shares)";
			this.rbBuy5.UseVisualStyleBackColor = true;
			// 
			// rbTrade2
			// 
			this.rbTrade2.AutoSize = true;
			this.rbTrade2.Location = new System.Drawing.Point(7, 68);
			this.rbTrade2.Name = "rbTrade2";
			this.rbTrade2.Size = new System.Drawing.Size(102, 17);
			this.rbTrade2.TabIndex = 2;
			this.rbTrade2.Text = "Trade 2 (shares)";
			this.rbTrade2.UseVisualStyleBackColor = true;
			// 
			// rbNoneAfter
			// 
			this.rbNoneAfter.AutoSize = true;
			this.rbNoneAfter.Checked = true;
			this.rbNoneAfter.Location = new System.Drawing.Point(7, 92);
			this.rbNoneAfter.Name = "rbNoneAfter";
			this.rbNoneAfter.Size = new System.Drawing.Size(49, 17);
			this.rbNoneAfter.TabIndex = 3;
			this.rbNoneAfter.TabStop = true;
			this.rbNoneAfter.Text = "none";
			this.rbNoneAfter.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(16, 261);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(97, 260);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// PlayCard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(186, 295);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelPlayer);
			this.Name = "PlayCard";
			this.Text = "PlayCard";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label labelPlayer;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbNoneBefore;
		public System.Windows.Forms.RadioButton rbPlace4Tiles;
		public System.Windows.Forms.RadioButton rbTake5Tiles;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rbNoneAfter;
		public System.Windows.Forms.RadioButton rbTrade2;
		public System.Windows.Forms.RadioButton rbBuy5;
		public System.Windows.Forms.RadioButton rb3Free;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}