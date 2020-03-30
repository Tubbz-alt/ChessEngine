namespace ChessEngine
{
    partial class BoardForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BoardForm));
            this.boardDisp = new System.Windows.Forms.PictureBox();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.boardDisp)).BeginInit();
            this.SuspendLayout();
            // 
            // boardDisp
            // 
            this.boardDisp.Location = new System.Drawing.Point(12, 12);
            this.boardDisp.Name = "boardDisp";
            this.boardDisp.Size = new System.Drawing.Size(800, 800);
            this.boardDisp.TabIndex = 0;
            this.boardDisp.TabStop = false;
            this.boardDisp.Click += new System.EventHandler(this.BoardClick);
            this.boardDisp.Paint += new System.Windows.Forms.PaintEventHandler(this.BoardPaint);
            // 
            // gameTimer
            // 
            this.gameTimer.Enabled = true;
            this.gameTimer.Tick += new System.EventHandler(this.BoardRefresh);
            // 
            // BoardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 820);
            this.Controls.Add(this.boardDisp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BoardForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.boardDisp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox boardDisp;
        private System.Windows.Forms.Timer gameTimer;
    }
}

