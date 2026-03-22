using System.IO.Ports;

namespace SUartPGM;

public partial class Form1 : Form
{
    private readonly SUart _uart = new();

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        RefreshPortList();
        SLog.Write(richTextBoxLog, LogLevel.INFO, "프로그램 시작");
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _uart.Disconnect();
    }

    private void RefreshPortList()
    {
        listBoxPorts.Items.Clear();
        var ports = SUart.GetAvailablePorts();
        if (ports.Length > 0)
        {
            listBoxPorts.Items.AddRange(ports);
            listBoxPorts.SelectedIndex = 0;
        }
        else
        {
            SLog.Write(richTextBoxLog, LogLevel.WARN, "연결된 UART 포트가 없습니다.");
        }
    }

    private void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (_uart.IsConnected)
        {
            _uart.Disconnect();
            SLog.Write(richTextBoxLog, LogLevel.INFO, "연결 해제됨");
            UpdateConnectButton();
            listBoxPorts.Enabled = true;
        }
        else
        {
            var port = listBoxPorts.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(port))
            {
                SLog.Write(richTextBoxLog, LogLevel.ERR, "포트를 선택해 주세요.");
                return;
            }

            _uart.SetDataReceivedHandler(OnDataReceived);
            if (_uart.Connect(port))
            {
                SLog.Write(richTextBoxLog, LogLevel.INFO, $"{port} 연결됨");
                UpdateConnectButton();
                listBoxPorts.Enabled = false;
            }
            else
            {
                SLog.Write(richTextBoxLog, LogLevel.ERR, $"{port} 연결 실패");
                _uart.RemoveDataReceivedHandler(OnDataReceived);
            }
        }
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            var port = (SerialPort)sender;
            var data = port.ReadExisting();
            if (!string.IsNullOrEmpty(data))
            {
                richTextBoxLog.Invoke(() =>
                {
                    SLog.WriteToControl(richTextBoxLog, LogLevel.INFO, $"[RX] {data.TrimEnd()}");
                });
            }
        }
        catch { }
    }

    private void UpdateConnectButton()
    {
        btnConnect.Text = _uart.IsConnected ? "Disconnect" : "Connect";
    }

    private void BtnSend_Click(object? sender, EventArgs e)
    {
        SendText();
    }

    private void TxtSend_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            SendText();
        }
    }

    private void SendText()
    {
        if (!_uart.IsConnected)
        {
            SLog.Write(richTextBoxLog, LogLevel.ERR, "먼저 UART에 연결해 주세요.");
            return;
        }

        var text = txtSend.Text;
        if (_uart.SendLine(text))
        {
            SLog.Write(richTextBoxLog, LogLevel.INFO, $"[TX] {text}");
            txtSend.Clear();
        }
        else
        {
            SLog.Write(richTextBoxLog, LogLevel.ERR, "전송 실패");
        }
    }
}
