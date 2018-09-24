using System;
using NLog.Targets;

namespace NLog.Syslog.DP.Target
{

	[Target("Syslog")]
	public class SyslogTarget : TargetWithLayout
	{
		
		public string Host { get; set; } = "localhost";
		public int Port { get; set; } = 514;
		public string EncodingGlobal { get; set; }
		public string EncodingOnLinuxOS { get; set; } = "utf-8";
		public string EncodingOnWindowsOS { get; set; } = "utf-8";
		public string EncodingOnOSXOS { get; set; } = "utf-8";
		private static Client _client;

		public SyslogTarget()
		{
		}

		protected override void InitializeTarget()
		{
			base.InitializeTarget();
			_client = new Client(Host, Port, EncodingGlobal, EncodingOnLinuxOS, EncodingOnWindowsOS, EncodingOnOSXOS);
		}


		protected override void Write(LogEventInfo logEvent)
		{
			string logMessage = Layout.Render(logEvent);
			Level level = _GetSyslogLevel(logEvent.Level);

			_client.Send(new Message(Facility.User, level, logMessage));
		}

		private Level _GetSyslogLevel(LogLevel logLevel)
		{
			Level syslogLevel = Level.Unknown;

			if (logLevel == LogLevel.Debug)
				syslogLevel = Level.Debug;

			else if (logLevel == LogLevel.Error)
				syslogLevel = Level.Error;

			else if (logLevel == LogLevel.Fatal)
				syslogLevel = Level.Critical;

			else if (logLevel == LogLevel.Info)
				syslogLevel = Level.Information;

			else if (logLevel == LogLevel.Trace)
				syslogLevel = Level.Information;

			else if (logLevel == LogLevel.Warn)
				syslogLevel = Level.Warning;

			if (syslogLevel == Level.Unknown)
				throw new ArgumentOutOfRangeException("Unknown LogLevel value");

			return syslogLevel;
		}

		protected override void CloseTarget()
		{
			base.CloseTarget();
			_client.CloseSocket();
		}

	}
}