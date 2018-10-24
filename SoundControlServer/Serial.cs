using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SoundControlServer
{
    public class Serial
    {
        private SerialPort _serialPort;
        private bool _serialIsOpen = false;
        
        public Serial(string portName, int baudRate)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
        }

        public bool Open()
        {
            if (!_serialIsOpen)
            {
                try
                {
                    //_serialPort.Open();
                    _serialIsOpen = true;
                    return true;
                }
                catch
                {
                    _serialIsOpen = false;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Close()
        {
            if(_serialIsOpen)
            {
                try
                {
                    _serialPort.Close();
                    _serialIsOpen = false;
                    return true;
                }
                catch
                {
                    _serialIsOpen = true;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Write(object module)
        {
            if (_serialIsOpen)
            {
                string message = JSON.serialize(module);
                //byte[] messageArray = Encoding.UTF8.GetBytes(message);
                //_serialPort.Write(messageArray, 0, messageArray.Length);

                //_serialPort.WriteLine(message);
                Console.WriteLine();
            }
        }
    }
}
