using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace NLog.Syslog.DP.Target
{

	public class Client 
	{
		private readonly Encoding _encoding;
		private readonly IPEndPoint _endPoint;
		private readonly Socket _socket;

		public Client(string ip, int port, string encodingGlobal, string encodingOnLinuxOS, string encodingOnWindowsOS, string encodingOnOSXOS)
		{

			IPAddress serverAddr = IPAddress.Parse(ip.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) ? "127.0.0.1" : ip);
			_endPoint = new IPEndPoint(serverAddr, port);

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			if (!string.IsNullOrEmpty(encodingGlobal))
			{
				_encoding = Encoding.GetEncoding(encodingGlobal);
			}
			else
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					_encoding = Encoding.GetEncoding(encodingOnWindowsOS);

				else if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					_encoding = Encoding.GetEncoding(encodingOnLinuxOS);

				else if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					_encoding = Encoding.GetEncoding(encodingOnOSXOS);
			}

			_socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		}

		public void Send(Message message)
		{

			int priority = (int) message.Facility * 8 + (int) message.Level;
			string msg = $"<{priority}> {message.Text}";
			byte[] myByte = _encoding.GetBytes($"{msg}{Environment.NewLine}");

			try
			{
				if (!_socket.Connected)
				{
					_socket.Connect(_endPoint);
				}

				_socket.Send(myByte);

			}
			catch (SocketException)
			{
				//ignored
			}
			catch (Exception)
			{
				//ignored
			}

		}

		public void CloseSocket()
		{
			if (_socket.Connected)
			{
				_socket.Shutdown(SocketShutdown.Both);
			}

			_socket.Close();
			_socket.Dispose();

		}

	}
}