using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NLog.Syslog.DP.Target
{
	public class Message
	{
		public Facility Facility { get; set; }
		public Level Level { get; set; }
		public string Text { get; set; }

		public Message(Facility facility, Level level, string text)
		{
			Facility = facility;
			Level = level;
			Text = text;
		}
	}
}
