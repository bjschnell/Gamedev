namespace Server.UI
{
	partial class PlayerScore
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerScore));
			this.panelPlayers = new System.Windows.Forms.Panel();
			this.dataGridViewScores = new System.Windows.Forms.DataGridView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.dataGridViewStock = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewScores)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewStock)).BeginInit();
			this.SuspendLayout();
			// 
			// panelPlayers
			// 
			this.panelPlayers.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelPlayers.Location = new System.Drawing.Point(0, 0);
			this.panelPlayers.Name = "panelPlayers";
			this.panelPlayers.Size = new System.Drawing.Size(682, 423);
			this.panelPlayers.TabIndex = 1;
			this.panelPlayers.SizeChanged += new System.EventHandler(this.panelPlayers_SizeChanged);
			// 
			// dataGridViewScores
			// 
			this.dataGridViewScores.AllowUserToAddRows = false;
			this.dataGridViewScores.AllowUserToDeleteRows = false;
			this.dataGridViewScores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewScores.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewScores.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewScores.Name = "dataGridViewScores";
			this.dataGridViewScores.ReadOnly = true;
			this.dataGridViewScores.Size = new System.Drawing.Size(682, 172);
			this.dataGridViewScores.TabIndex = 4;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 423);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.dataGridViewScores);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dataGridViewStock);
			this.splitContainer1.Size = new System.Drawing.Size(682, 367);
			this.splitContainer1.SplitterDistance = 172;
			this.splitContainer1.TabIndex = 5;
			// 
			// dataGridViewStock
			// 
			this.dataGridViewStock.AllowUserToAddRows = false;
			this.dataGridViewStock.AllowUserToDeleteRows = false;
			this.dataGridViewStock.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewStock.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewStock.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewStock.Name = "dataGridViewStock";
			this.dataGridViewStock.ReadOnly = true;
			this.dataGridViewStock.Size = new System.Drawing.Size(682, 191);
			this.dataGridViewStock.TabIndex = 0;
			// 
			// PlayerScore
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(682, 790);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.panelPlayers);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PlayerScore";
			this.Text = "Player Scoreboard";
			this.Load += new System.EventHandler(this.PlayerScore_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewScores)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewStock)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelPlayers;
		private System.Windows.Forms.DataGridView dataGridViewScores;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataGridView dataGridViewStock;
	}
}