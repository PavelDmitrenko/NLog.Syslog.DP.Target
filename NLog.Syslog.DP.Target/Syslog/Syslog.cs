using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Syslog.DP.Target
{

	public class Client 
	{
		private readonly Encoding _encoding;
		private readonly IPEndPoint _endPoint;
		private Socket _socket;
		private DateTime? _lastConnectAttempt;
		private readonly object lockObj = new object();
		private readonly int _reconnectAttemptInterval;
		private bool _socketUsed;
		private readonly int _socketConnectTimeout;

		public Client(string serverAddress, int port, int socketConnectTimeout,  int reconnectAttemptInterval, string encodingGlobal, string encodingOnLinuxOS, string encodingOnWindowsOS, string encodingOnOSXOS)
		{
			_socketConnectTimeout = socketConnectTimeout;
			_reconnectAttemptInterval = reconnectAttemptInterval;

			serverAddress = serverAddress.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) ? "127.0.0.1" : serverAddress;

			if (!IPAddress.TryParse(serverAddress, out var ipAddress))
			{
				ipAddress = _GetIPfromHost(serverAddress);
			}
			
			_endPoint = new IPEndPoint(ipAddress, port);
		
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

			_CreateSocket();

		}

		public void Send(Message message)
		{
			Task.Run(() =>
			{
				_Send(message);
			});
		}

		private void _CreateSocket()
		{
			if (_socket != null &&_socket.Connected)
			{
				_socket.Shutdown(SocketShutdown.Both);
				_socket.Disconnect(true);
				_socket.Close();
			}

			if (_socket == null || _socketUsed)
			{
				Debug.WriteLine($"Recreate socket object");
				_socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				_socketUsed = false;
			}
		}

		private void _Send(Message message)
		{
			int priority = (int)message.Facility * 8 + (int)message.Level;
			string msg = $"<{priority}> {message.Text}";
			byte[] myByte = _encoding.GetBytes($"{msg}{Environment.NewLine}");

			lock (lockObj)
			{
				try
				{
					if (!_socket.Connected) 
					{
						if (_lastConnectAttempt == null ||
						    DateTime.Now.Subtract(_lastConnectAttempt.Value).TotalMilliseconds > _reconnectAttemptInterval)
						{
							Debug.WriteLine($"Connecting to {_endPoint.Address}...");
							IAsyncResult result = _socket.BeginConnect(_endPoint, null, null);
							bool success = result.AsyncWaitHandle.WaitOne(_socketConnectTimeout, true);

							if (success)
							{
								_socket.EndConnect(result);
							}
							else
							{
								Debug.WriteLine($"Connect timeout");
								throw new SocketException((int)SocketError.TimedOut); 
							}

							_socketUsed = true;
	}
						else
						{
							Debug.WriteLine($"...Throttle reconnect attempts");
							return; // Throttle reconnect attempts
						}
					}

					_socket.Send(myByte);

					Debug.WriteLine($"Success");
				}
				catch (SocketException socketException)
				{
					Debug.WriteLine($"socketException.SocketErrorCode {socketException.SocketErrorCode}");
					switch (socketException.SocketErrorCode)
					{
						case SocketError.ConnectionRefused:
						case SocketError.ConnectionAborted:
						case SocketError.ConnectionReset:
						case SocketError.TimedOut:
							_CreateSocket();
							break;

						case SocketError.AccessDenied:
						case SocketError.AddressAlreadyInUse:
						case SocketError.AddressFamilyNotSupported:
						case SocketError.AddressNotAvailable:
						case SocketError.AlreadyInProgress:
						case SocketError.DestinationAddressRequired:
						case SocketError.Disconnecting:
						case SocketError.Fault:
						case SocketError.HostDown:
						case SocketError.HostNotFound:
						case SocketError.HostUnreachable:
						case SocketError.InProgress:
						case SocketError.Interrupted:
						case SocketError.InvalidArgument:
						case SocketError.IOPending:
						case SocketError.IsConnected:
						case SocketError.MessageSize:
						case SocketError.NetworkDown:
						case SocketError.NetworkReset:
						case SocketError.NetworkUnreachable:
						case SocketError.NoBufferSpaceAvailable:
						case SocketError.NoData:
						case SocketError.NoRecovery:
						case SocketError.NotConnected:
						case SocketError.NotInitialized:
						case SocketError.NotSocket:
						case SocketError.OperationAborted:
						case SocketError.OperationNotSupported:
						case SocketError.ProcessLimit:
						case SocketError.ProtocolFamilyNotSupported:
						case SocketError.ProtocolNotSupported:
						case SocketError.ProtocolOption:
						case SocketError.ProtocolType:
						case SocketError.Shutdown:
						case SocketError.SocketError:
						case SocketError.SocketNotSupported:
						case SocketError.Success:
						case SocketError.SystemNotReady:

						case SocketError.TooManyOpenSockets:
						case SocketError.TryAgain:
						case SocketError.TypeNotFound:
						case SocketError.VersionNotSupported:
						case SocketError.WouldBlock:
						default:
							break;
					}

					_lastConnectAttempt = DateTime.Now;
				}
				catch (Exception exception)
				{
					Debug.WriteLine($"Exception: {exception.Message}");
				}
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

		private static IPAddress _GetIPfromHost(string p)
		{
			var hosts = Dns.GetHostAddresses(p);

			if (hosts == null || hosts.Length == 0)
				throw new ArgumentException(string.Format("Host not found: {0}", p));

			return hosts[0];
		}

	}
}