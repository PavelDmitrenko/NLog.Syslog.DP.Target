using System;
using System.Diagnostics;
using NLog.Syslog.DP.Target;

namespace Tests
{
	class Program
	{

		private static Client _client;

		static void Main(string[] args)
		{

			_client = new Client("localhost", 514, 51000,  5000, "utf-8", "utf-8", "utf-8", "utf-8");
			Stopwatch sw = new Stopwatch();
			sw.Start();

			while (true)
			{
				_client.Send(new Message(Facility.User, Level.Information, "Test Message"));
				System.Threading.Thread.Sleep(100);
			}
			
			Console.WriteLine($"Elapsed: {sw.Elapsed.TotalSeconds}s");

			Console.ReadLine();
		}
	}
}
