namespace GymManager
{
    partial class main_form
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_alunos = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel_principal = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_alunos
            // 
            this.btn_alunos.Location = new System.Drawing.Point(12, 12);
            this.btn_alunos.Name = "btn_alunos";
            this.btn_alunos.Size = new System.Drawing.Size(130, 23);
            this.btn_alunos.TabIndex = 0;
            this.btn_alunos.Text = "Alunos";
            this.btn_alunos.UseVisualStyleBackColor = true;
            this.btn_alunos.Click += new System.EventHandler(this.btn_alunos_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 58);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(130, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Acessos";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel_principal
            // 
            this.panel_principal.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel_principal.Location = new System.Drawing.Point(178, 12);
            this.panel_principal.Name = "panel_principal";
            this.panel_principal.Size = new System.Drawing.Size(377, 398);
            this.panel_principal.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 101);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Planos";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 140);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(130, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Pagamentos";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // main_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(573, 430);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel_principal);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btn_alunos);
            this.Name = "main_form";
            this.Text = "GymManager";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_alunos;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel_principal;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
    }
}

