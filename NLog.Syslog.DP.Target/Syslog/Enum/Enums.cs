using System;
using System.Collections.Generic;
using System.Text;

namespace NLog.Syslog.DP.Target
{
	public enum Level
	{
		Unknown = -1,
		Emergency = 0,
		Alert = 1,
		Critical = 2,
		Error = 3,
		Warning = 4,
		Notice = 5,
		Information = 6,
		Debug = 7,
	}

	public enum Facility
	{
		Kernel = 0,
		User = 1,
		Mail = 2,
		Daemon = 3,
		Auth = 4,
		Syslog = 5,
		Lpr = 6,
		News = 7,
		UUCP = 8,
		Cron = 9,
		Local0 = 10,
		Local1 = 11,
		Local2 = 12,
		Local3 = 13,
		Local4 = 14,
		Local5 = 15,
		Local6 = 16,
		Local7 = 17,
	}
}
