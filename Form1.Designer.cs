namespace ProjecteCobolDavid
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
            btnGuardar = new Button();
            txtNom = new TextBox();
            numCost = new NumericUpDown();
            dtpData = new DateTimePicker();
            cmbTipus = new ComboBox();
            dgvDespeses = new DataGridView();
            btnActualitzar = new Button();
            btnBorrarDat = new Button();
            btnMostrarInforme = new Button();
            lblNom = new Label();
            lblCost = new Label();
            lblData = new Label();
            lblTipus = new Label();
            ((System.ComponentModel.ISupportInitialize)numCost).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvDespeses).BeginInit();
            SuspendLayout();
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(418, 9);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtNom
            // 
            txtNom.Location = new Point(71, 12);
            txtNom.Name = "txtNom";
            txtNom.Size = new Size(120, 23);
            txtNom.TabIndex = 1;
            // 
            // numCost
            // 
            numCost.DecimalPlaces = 2;
            numCost.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numCost.Location = new Point(71, 46);
            numCost.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            numCost.Name = "numCost";
            numCost.Size = new Size(120, 23);
            numCost.TabIndex = 2;
            // 
            // dtpData
            // 
            dtpData.Location = new Point(283, 9);
            dtpData.Name = "dtpData";
            dtpData.Size = new Size(120, 23);
            dtpData.TabIndex = 3;
            // 
            // cmbTipus
            // 
            cmbTipus.FormattingEnabled = true;
            cmbTipus.Items.AddRange(new object[] { "Compra", "Despeses llar", "Alquiler", "Oci", "Hipoteca" });
            cmbTipus.Location = new Point(282, 48);
            cmbTipus.Name = "cmbTipus";
            cmbTipus.Size = new Size(121, 23);
            cmbTipus.TabIndex = 4;
            // 
            // dgvDespeses
            // 
            dgvDespeses.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDespeses.Location = new Point(24, 77);
            dgvDespeses.Name = "dgvDespeses";
            dgvDespeses.ReadOnly = true;
            dgvDespeses.Size = new Size(445, 216);
            dgvDespeses.TabIndex = 5;
            // 
            // btnActualitzar
            // 
            btnActualitzar.Location = new Point(418, 47);
            btnActualitzar.Name = "btnActualitzar";
            btnActualitzar.Size = new Size(75, 23);
            btnActualitzar.TabIndex = 6;
            btnActualitzar.Text = "Actualitzar";
            btnActualitzar.UseVisualStyleBackColor = true;
            btnActualitzar.Click += btnActualitzar_Click;
            // 
            // btnBorrarDat
            // 
            btnBorrarDat.Location = new Point(475, 270);
            btnBorrarDat.Name = "btnBorrarDat";
            btnBorrarDat.Size = new Size(114, 23);
            btnBorrarDat.TabIndex = 11;
            btnBorrarDat.Text = "Esborrar dades";
            btnBorrarDat.UseVisualStyleBackColor = true;
            btnBorrarDat.Click += btnBorrarDat_Click;
            // 
            // btnMostrarInforme
            // 
            btnMostrarInforme.Location = new Point(499, 47);
            btnMostrarInforme.Name = "btnMostrarInforme";
            btnMostrarInforme.Size = new Size(75, 23);
            btnMostrarInforme.TabIndex = 12;
            btnMostrarInforme.Text = "Informe";
            btnMostrarInforme.Click += btnMostrarInforme_Click;
            // 
            // lblNom
            // 
            lblNom.AutoSize = true;
            lblNom.Location = new Point(12, 15);
            lblNom.Name = "lblNom";
            lblNom.Size = new Size(53, 15);
            lblNom.TabIndex = 7;
            lblNom.Text = "Despesa:";
            // 
            // lblCost
            // 
            lblCost.AutoSize = true;
            lblCost.Location = new Point(12, 48);
            lblCost.Name = "lblCost";
            lblCost.Size = new Size(34, 15);
            lblCost.TabIndex = 8;
            lblCost.Text = "Cost:";
            // 
            // lblData
            // 
            lblData.AutoSize = true;
            lblData.Location = new Point(224, 15);
            lblData.Name = "lblData";
            lblData.Size = new Size(34, 15);
            lblData.TabIndex = 9;
            lblData.Text = "Data:";
            // 
            // lblTipus
            // 
            lblTipus.AutoSize = true;
            lblTipus.Location = new Point(223, 51);
            lblTipus.Name = "lblTipus";
            lblTipus.Size = new Size(38, 15);
            lblTipus.TabIndex = 10;
            lblTipus.Text = "Tipus:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(601, 305);
            Controls.Add(lblTipus);
            Controls.Add(lblData);
            Controls.Add(lblCost);
            Controls.Add(lblNom);
            Controls.Add(btnActualitzar);
            Controls.Add(btnBorrarDat);
            Controls.Add(btnMostrarInforme);
            Controls.Add(dgvDespeses);
            Controls.Add(cmbTipus);
            Controls.Add(dtpData);
            Controls.Add(numCost);
            Controls.Add(txtNom);
            Controls.Add(btnGuardar);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)numCost).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvDespeses).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGuardar;
        private TextBox txtNom;
        private NumericUpDown numCost;
        private DateTimePicker dtpData;
        private ComboBox cmbTipus;
        private DataGridView dgvDespeses;
        private Button btnActualitzar;
        private Button btnBorrarDat;
        private Button btnMostrarInforme;
        private Label lblNom;
        private Label lblCost;
        private Label lblData;
        private Label lblTipus;
    }
}
