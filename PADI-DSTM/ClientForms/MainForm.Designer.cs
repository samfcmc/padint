namespace ClientForms
{
    partial class MainForm
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
            this.masterHost = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.masterPort = new System.Windows.Forms.TextBox();
            this.serverHost = new System.Windows.Forms.TextBox();
            this.buttonTxbegin = new System.Windows.Forms.Button();
            this.buttonTxCommit = new System.Windows.Forms.Button();
            this.buttonTxAbort = new System.Windows.Forms.Button();
            this.buttonFail = new System.Windows.Forms.Button();
            this.buttoFreeze = new System.Windows.Forms.Button();
            this.serverPort = new System.Windows.Forms.TextBox();
            this.buttonRecover = new System.Windows.Forms.Button();
            this.padIntUId = new System.Windows.Forms.TextBox();
            this.buttonAccess = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.log = new System.Windows.Forms.TextBox();
            this.padIntValue = new System.Windows.Forms.TextBox();
            this.buttonRead = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // masterHost
            // 
            this.masterHost.Location = new System.Drawing.Point(12, 13);
            this.masterHost.Name = "masterHost";
            this.masterHost.Size = new System.Drawing.Size(117, 20);
            this.masterHost.TabIndex = 0;
            this.masterHost.Text = "localhost";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(194, 9);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // masterPort
            // 
            this.masterPort.Location = new System.Drawing.Point(135, 12);
            this.masterPort.Name = "masterPort";
            this.masterPort.Size = new System.Drawing.Size(53, 20);
            this.masterPort.TabIndex = 2;
            this.masterPort.Text = "8086";
            // 
            // serverHost
            // 
            this.serverHost.Location = new System.Drawing.Point(12, 67);
            this.serverHost.Name = "serverHost";
            this.serverHost.Size = new System.Drawing.Size(117, 20);
            this.serverHost.TabIndex = 3;
            this.serverHost.Text = "localhost";
            // 
            // buttonTxbegin
            // 
            this.buttonTxbegin.Location = new System.Drawing.Point(12, 38);
            this.buttonTxbegin.Name = "buttonTxbegin";
            this.buttonTxbegin.Size = new System.Drawing.Size(75, 23);
            this.buttonTxbegin.TabIndex = 4;
            this.buttonTxbegin.Text = "TxBegin";
            this.buttonTxbegin.UseVisualStyleBackColor = true;
            this.buttonTxbegin.Click += new System.EventHandler(this.buttonTxbegin_Click);
            // 
            // buttonTxCommit
            // 
            this.buttonTxCommit.Location = new System.Drawing.Point(93, 38);
            this.buttonTxCommit.Name = "buttonTxCommit";
            this.buttonTxCommit.Size = new System.Drawing.Size(75, 23);
            this.buttonTxCommit.TabIndex = 5;
            this.buttonTxCommit.Text = "TxCommit";
            this.buttonTxCommit.UseVisualStyleBackColor = true;
            this.buttonTxCommit.Click += new System.EventHandler(this.buttonTxCommit_Click);
            // 
            // buttonTxAbort
            // 
            this.buttonTxAbort.Location = new System.Drawing.Point(174, 38);
            this.buttonTxAbort.Name = "buttonTxAbort";
            this.buttonTxAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonTxAbort.TabIndex = 6;
            this.buttonTxAbort.Text = "TxAbort";
            this.buttonTxAbort.UseVisualStyleBackColor = true;
            this.buttonTxAbort.Click += new System.EventHandler(this.buttonTxAbort_Click);
            // 
            // buttonFail
            // 
            this.buttonFail.Location = new System.Drawing.Point(194, 64);
            this.buttonFail.Name = "buttonFail";
            this.buttonFail.Size = new System.Drawing.Size(75, 23);
            this.buttonFail.TabIndex = 7;
            this.buttonFail.Text = "Fail";
            this.buttonFail.UseVisualStyleBackColor = true;
            this.buttonFail.Click += new System.EventHandler(this.buttonFail_Click);
            // 
            // buttoFreeze
            // 
            this.buttoFreeze.Location = new System.Drawing.Point(194, 90);
            this.buttoFreeze.Name = "buttoFreeze";
            this.buttoFreeze.Size = new System.Drawing.Size(75, 23);
            this.buttoFreeze.TabIndex = 8;
            this.buttoFreeze.Text = "Freeze";
            this.buttoFreeze.UseVisualStyleBackColor = true;
            this.buttoFreeze.Click += new System.EventHandler(this.buttoFreeze_Click);
            // 
            // serverPort
            // 
            this.serverPort.Location = new System.Drawing.Point(135, 67);
            this.serverPort.Name = "serverPort";
            this.serverPort.Size = new System.Drawing.Size(53, 20);
            this.serverPort.TabIndex = 9;
            this.serverPort.Text = "2001";
            // 
            // buttonRecover
            // 
            this.buttonRecover.Location = new System.Drawing.Point(194, 115);
            this.buttonRecover.Name = "buttonRecover";
            this.buttonRecover.Size = new System.Drawing.Size(75, 23);
            this.buttonRecover.TabIndex = 10;
            this.buttonRecover.Text = "Recover";
            this.buttonRecover.UseVisualStyleBackColor = true;
            this.buttonRecover.Click += new System.EventHandler(this.buttonRecover_Click);
            // 
            // padIntUId
            // 
            this.padIntUId.Location = new System.Drawing.Point(51, 147);
            this.padIntUId.Name = "padIntUId";
            this.padIntUId.Size = new System.Drawing.Size(56, 20);
            this.padIntUId.TabIndex = 11;
            this.padIntUId.Text = "0";
            // 
            // buttonAccess
            // 
            this.buttonAccess.Location = new System.Drawing.Point(194, 144);
            this.buttonAccess.Name = "buttonAccess";
            this.buttonAccess.Size = new System.Drawing.Size(75, 23);
            this.buttonAccess.TabIndex = 12;
            this.buttonAccess.Text = "Access";
            this.buttonAccess.UseVisualStyleBackColor = true;
            this.buttonAccess.Click += new System.EventHandler(this.buttonAccess_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(113, 144);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 13;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // log
            // 
            this.log.Location = new System.Drawing.Point(12, 203);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(257, 78);
            this.log.TabIndex = 15;
            // 
            // padIntValue
            // 
            this.padIntValue.Location = new System.Drawing.Point(51, 172);
            this.padIntValue.Name = "padIntValue";
            this.padIntValue.Size = new System.Drawing.Size(56, 20);
            this.padIntValue.TabIndex = 17;
            this.padIntValue.Text = "10";
            // 
            // buttonRead
            // 
            this.buttonRead.Location = new System.Drawing.Point(113, 170);
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.Size = new System.Drawing.Size(75, 23);
            this.buttonRead.TabIndex = 18;
            this.buttonRead.Text = "Read";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.buttonRead_Click);
            // 
            // buttonWrite
            // 
            this.buttonWrite.Location = new System.Drawing.Point(194, 170);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(75, 23);
            this.buttonWrite.TabIndex = 19;
            this.buttonWrite.Text = "Write";
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.buttonWrite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "uid";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "value";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 293);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.buttonRead);
            this.Controls.Add(this.padIntValue);
            this.Controls.Add(this.log);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonAccess);
            this.Controls.Add(this.padIntUId);
            this.Controls.Add(this.buttonRecover);
            this.Controls.Add(this.serverPort);
            this.Controls.Add(this.buttoFreeze);
            this.Controls.Add(this.buttonFail);
            this.Controls.Add(this.buttonTxAbort);
            this.Controls.Add(this.buttonTxCommit);
            this.Controls.Add(this.buttonTxbegin);
            this.Controls.Add(this.serverHost);
            this.Controls.Add(this.masterPort);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.masterHost);
            this.Name = "MainForm";
            this.Text = "PADILib Test Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox masterHost;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox masterPort;
        private System.Windows.Forms.TextBox serverHost;
        private System.Windows.Forms.Button buttonTxbegin;
        private System.Windows.Forms.Button buttonTxCommit;
        private System.Windows.Forms.Button buttonTxAbort;
        private System.Windows.Forms.Button buttonFail;
        private System.Windows.Forms.Button buttoFreeze;
        private System.Windows.Forms.TextBox serverPort;
        private System.Windows.Forms.Button buttonRecover;
        private System.Windows.Forms.TextBox padIntUId;
        private System.Windows.Forms.Button buttonAccess;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.TextBox log;
        private System.Windows.Forms.TextBox padIntValue;
        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

