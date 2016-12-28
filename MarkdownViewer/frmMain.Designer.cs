namespace MarkdownViewer
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlFind = new System.Windows.Forms.Panel();
            this.btnNextSearch = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.pnlFind.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFind
            // 
            this.pnlFind.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlFind.Controls.Add(this.btnNextSearch);
            this.pnlFind.Controls.Add(this.button1);
            this.pnlFind.Controls.Add(this.txtFind);
            this.pnlFind.Location = new System.Drawing.Point(902, 57);
            this.pnlFind.Name = "pnlFind";
            this.pnlFind.Size = new System.Drawing.Size(302, 38);
            this.pnlFind.TabIndex = 0;
            this.pnlFind.Visible = false;
            // 
            // btnNextSearch
            // 
            this.btnNextSearch.FlatAppearance.BorderSize = 0;
            this.btnNextSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextSearch.Image = global::MarkdownViewer.Properties.Resources.fa_arrow_circle_right;
            this.btnNextSearch.Location = new System.Drawing.Point(269, 6);
            this.btnNextSearch.Name = "btnNextSearch";
            this.btnNextSearch.Size = new System.Drawing.Size(28, 24);
            this.btnNextSearch.TabIndex = 2;
            this.btnNextSearch.UseVisualStyleBackColor = true;
            this.btnNextSearch.Click += new System.EventHandler(this.btnNextSearch_Click);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::MarkdownViewer.Properties.Resources.fa_search;
            this.button1.Location = new System.Drawing.Point(3, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 24);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtFind
            // 
            this.txtFind.BackColor = System.Drawing.Color.White;
            this.txtFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFind.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFind.ForeColor = System.Drawing.Color.Gray;
            this.txtFind.Location = new System.Drawing.Point(31, 7);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(232, 23);
            this.txtFind.TabIndex = 0;
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 730);
            this.Controls.Add(this.pnlFind);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1061, 47);
            this.Name = "frmMain";
            this.Text = "Markdown Viewer";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            this.pnlFind.ResumeLayout(false);
            this.pnlFind.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFind;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnNextSearch;
    }
}