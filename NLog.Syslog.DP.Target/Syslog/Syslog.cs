using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace NLog.Syslog.DP.Target
{

	public class Client
	{
		private readonly IPHostEntry ipHostInfo;
		private readonly int _port;
		private readonly string _ip;

		public Client(string ip, int port)
		{
			ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			_ip = ip;
			_port = port;
		}

		public void Send(Message message)
		{
			IPAddress serverAddr = IPAddress.Parse(_ip);

			using (Socket socket = new Socket(serverAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
			{
				IPEndPoint endPoint = new IPEndPoint(serverAddr, _port);

				int priority = (int)message.Facility * 8 + (int)message.Level;
				string msg = $"<{priority}> {message.Text}";

				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

				Encoding encoding = Encoding.GetEncoding("utf-8");

				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					encoding = Encoding.GetEncoding(1251);
				}

				byte[] myByte = encoding.GetBytes($"{msg}{Environment.NewLine}");

				socket.Connect(endPoint);

				int bytesSent = socket.Send(myByte);
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
		}

	}
}