using System.IO.Ports;

namespace SUartPGM;

/// <summary>
/// UART 통신 클래스
/// </summary>
public class SUart : IDisposable
{
    private SerialPort? _serialPort;
    private readonly object _lock = new();
    private SerialDataReceivedEventHandler? _dataReceivedHandler;

    public bool IsConnected => _serialPort?.IsOpen ?? false;

    /// <summary>
    /// 사용 가능한 UART(COM) 포트 목록 반환
    /// </summary>
    public static string[] GetAvailablePorts()
    {
        return SerialPort.GetPortNames();
    }

    /// <summary>
    /// UART 포트에 연결
    /// </summary>
    public bool Connect(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
    {
        if (string.IsNullOrEmpty(portName))
            return false;

        lock (_lock)
        {
            if (_serialPort?.IsOpen == true)
                return true;

            try
            {
                _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                if (_dataReceivedHandler != null)
                    _serialPort.DataReceived += _dataReceivedHandler;
                _serialPort.Open();
                return true;
            }
            catch
            {
                _serialPort?.Dispose();
                _serialPort = null;
                return false;
            }
        }
    }

    /// <summary>
    /// UART 연결 해제
    /// </summary>
    public void Disconnect()
    {
        lock (_lock)
        {
            if (_serialPort == null)
                return;

            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            finally
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }
    }

    /// <summary>
    /// 문자열을 UART로 전송
    /// </summary>
    public bool Send(string text)
    {
        if (_serialPort?.IsOpen != true || string.IsNullOrEmpty(text))
            return false;

        try
        {
            _serialPort.Write(text);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 줄바꿈과 함께 문자열 전송
    /// </summary>
    public bool SendLine(string text)
    {
        if (_serialPort?.IsOpen != true)
            return false;

        try
        {
            _serialPort.WriteLine(text ?? string.Empty);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 데이터 수신 이벤트 설정 (연결 전/후 호출 가능)
    /// </summary>
    public void SetDataReceivedHandler(SerialDataReceivedEventHandler handler)
    {
        _dataReceivedHandler = handler;
        if (_serialPort != null)
            _serialPort.DataReceived += handler;
    }

    /// <summary>
    /// 데이터 수신 이벤트 해제
    /// </summary>
    public void RemoveDataReceivedHandler(SerialDataReceivedEventHandler handler)
    {
        _dataReceivedHandler = null;
        if (_serialPort != null)
            _serialPort.DataReceived -= handler;
    }

    public void Dispose()
    {
        Disconnect();
    }
}
