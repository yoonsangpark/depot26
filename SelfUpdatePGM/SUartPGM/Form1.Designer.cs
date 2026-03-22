namespace SUartPGM;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        listBoxPorts = new ListBox();
        btnConnect = new Button();
        txtSend = new TextBox();
        btnSend = new Button();
        richTextBoxLog = new RichTextBox();
        lblPorts = new Label();
        lblSend = new Label();
        SuspendLayout();
        //
        // listBoxPorts
        //
        listBoxPorts.FormattingEnabled = true;
        listBoxPorts.ItemHeight = 15;
        listBoxPorts.Location = new Point(12, 35);
        listBoxPorts.Name = "listBoxPorts";
        listBoxPorts.Size = new Size(150, 94);
        listBoxPorts.TabIndex = 0;
        //
        // btnConnect
        //
        btnConnect.Location = new Point(12, 140);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(150, 30);
        btnConnect.TabIndex = 1;
        btnConnect.Text = "Connect";
        btnConnect.UseVisualStyleBackColor = true;
        btnConnect.Click += BtnConnect_Click;
        //
        // lblPorts
        //
        lblPorts.AutoSize = true;
        lblPorts.Location = new Point(12, 12);
        lblPorts.Name = "lblPorts";
        lblPorts.Size = new Size(95, 15);
        lblPorts.TabIndex = 2;
        lblPorts.Text = "UART (COM) 포트";
        //
        // txtSend
        //
        txtSend.Location = new Point(12, 195);
        txtSend.Name = "txtSend";
        txtSend.Size = new Size(350, 23);
        txtSend.TabIndex = 3;
        txtSend.KeyDown += TxtSend_KeyDown;
        //
        // btnSend
        //
        btnSend.Location = new Point(368, 194);
        btnSend.Name = "btnSend";
        btnSend.Size = new Size(75, 25);
        btnSend.TabIndex = 4;
        btnSend.Text = "전송";
        btnSend.UseVisualStyleBackColor = true;
        btnSend.Click += BtnSend_Click;
        //
        // richTextBoxLog
        //
        richTextBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        richTextBoxLog.Location = new Point(12, 230);
        richTextBoxLog.Name = "richTextBoxLog";
        richTextBoxLog.ReadOnly = true;
        richTextBoxLog.Size = new Size(776, 208);
        richTextBoxLog.TabIndex = 5;
        richTextBoxLog.Text = "";
        //
        // lblSend
        //
        lblSend.AutoSize = true;
        lblSend.Location = new Point(12, 177);
        lblSend.Name = "lblSend";
        lblSend.Size = new Size(90, 15);
        lblSend.TabIndex = 6;
        lblSend.Text = "전송할 문자열";
        //
        // Form1
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(lblSend);
        Controls.Add(richTextBoxLog);
        Controls.Add(btnSend);
        Controls.Add(txtSend);
        Controls.Add(lblPorts);
        Controls.Add(btnConnect);
        Controls.Add(listBoxPorts);
        MinimumSize = new Size(400, 350);
        Name = "Form1";
        Text = "SUartPGM - UART 통신";
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ListBox listBoxPorts;
    private Button btnConnect;
    private TextBox txtSend;
    private Button btnSend;
    private RichTextBox richTextBoxLog;
    private Label lblPorts;
    private Label lblSend;
}
