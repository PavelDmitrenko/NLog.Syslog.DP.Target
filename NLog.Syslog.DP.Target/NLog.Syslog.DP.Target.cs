using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace NLog.Syslog.DP.Target
{

	[Target("Syslog")]
	public class SyslogTarget : TargetWithLayout
	{
		
		public string Host { get; set; } = "localhost";
		public int Port { get; set; } = 514;

		public SyslogTarget()
		{
		}

		protected override void Write(LogEventInfo logEvent)
		{
			string logMessage = this.Layout.Render(logEvent);
			Level level = _GetSyslogLevel(logEvent.Level);

			_SendTheMessage(logMessage, level);
		}

		private void _SendTheMessage(string message, Level level)
		{
			Client c = new Client(Host, Port);
			c.Send(new Message(Facility.User, level, message));
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
				throw new ArgumentOutOfRangeException("Unknown Syslog Level");

			return syslogLevel;
		}
	}
}