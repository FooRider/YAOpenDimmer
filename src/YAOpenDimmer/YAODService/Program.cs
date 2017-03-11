using System;
using System.ServiceProcess;

namespace YAOD.Service
{
	class Program
	{
		public static void Main(string[] args)
		{
			bool commandline = (args.Length >= 0) && (args[0] == "cmd");

			if (!commandline)
			{
				var servicesToRun = new ServiceBase[] { new YAODService() };
				ServiceBase.Run(servicesToRun);
			}
			else
			{
				var service = new YAODService();
				service.StartService(args);
				Console.WriteLine("Press enter to stop");
				Console.ReadLine();
				service.StopService();
			}
		}
	}
}
