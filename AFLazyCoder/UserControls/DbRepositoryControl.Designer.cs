namespace AFLazyCoder.UserControls
{
    partial class DbRepositoryControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbClassesList = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtInput = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.lblHint = new System.Windows.Forms.Label();
            this.btnAddClass = new System.Windows.Forms.Button();
            this.btnAddMethods = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbClassesList);
            this.splitContainer1.Panel1MinSize = 250;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2MinSize = 150;
            this.splitContainer1.Size = new System.Drawing.Size(741, 466);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 0;
            // 
            // lbClassesList
            // 
            this.lbClassesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbClassesList.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbClassesList.FormattingEnabled = true;
            this.lbClassesList.ItemHeight = 23;
            this.lbClassesList.Location = new System.Drawing.Point(0, 0);
            this.lbClassesList.Name = "lbClassesList";
            this.lbClassesList.Size = new System.Drawing.Size(250, 466);
            this.lbClassesList.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtInput);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 115);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(487, 351);
            this.panel2.TabIndex = 1;
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtInput.Location = new System.Drawing.Point(0, 0);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(483, 347);
            this.txtInput.TabIndex = 0;
            this.txtInput.Text = "";
            this.txtInput.Enter += new System.EventHandler(this.txtInput_Enter);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.cmbMode);
            this.panel1.Controls.Add(this.lblHint);
            this.panel1.Controls.Add(this.btnAddClass);
            this.panel1.Controls.Add(this.btnAddMethods);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(487, 115);
            this.panel1.TabIndex = 0;
            // 
            // cmbMode
            // 
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cmbMode.ForeColor = System.Drawing.Color.DarkGreen;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Items.AddRange(new object[] {
            "DB Repository",
            "Cloud Services"});
            this.cmbMode.Location = new System.Drawing.Point(5, 10);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(139, 31);
            this.cmbMode.TabIndex = 3;
            this.cmbMode.SelectedValueChanged += new System.EventHandler(this.cmbMode_SelectedValueChanged);
            // 
            // lblHint
            // 
            this.lblHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHint.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblHint.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblHint.Location = new System.Drawing.Point(150, 10);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(321, 91);
            this.lblHint.TabIndex = 2;
            this.lblHint.Text = "Enter methods signatures or repository names (one per line) and click appropriate" +
    " button";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnAddClass
            // 
            this.btnAddClass.AccessibleDescription = "";
            this.btnAddClass.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddClass.Location = new System.Drawing.Point(5, 78);
            this.btnAddClass.Name = "btnAddClass";
            this.btnAddClass.Size = new System.Drawing.Size(139, 23);
            this.btnAddClass.TabIndex = 1;
            this.btnAddClass.Text = "Add DbRepositories";
            this.btnAddClass.UseVisualStyleBackColor = true;
            this.btnAddClass.Click += new System.EventHandler(this.btnAddClass_Click);
            // 
            // btnAddMethods
            // 
            this.btnAddMethods.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddMethods.Location = new System.Drawing.Point(5, 49);
            this.btnAddMethods.Name = "btnAddMethods";
            this.btnAddMethods.Size = new System.Drawing.Size(139, 23);
            this.btnAddMethods.TabIndex = 0;
            this.btnAddMethods.Text = "Add Methods";
            this.btnAddMethods.UseVisualStyleBackColor = true;
            this.btnAddMethods.Click += new System.EventHandler(this.btnAddMethods_Click);
            // 
            // DbRepositoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DbRepositoryControl";
            this.Size = new System.Drawing.Size(741, 466);
            this.Load += new System.EventHandler(this.DbRepositoryControl_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbClassesList;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAddMethods;
        private System.Windows.Forms.RichTextBox txtInput;
        private System.Windows.Forms.Button btnAddClass;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.ComboBox cmbMode;
    }
}
