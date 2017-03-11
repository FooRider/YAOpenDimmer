using System;
using System.ServiceProcess;
using Nancy.Hosting.Self;

namespace YAOD.Service
{
	public class YAODService : ServiceBase
	{
		private NancyHost _controlHost;
		private YAODBoard _board;
		
		public YAODService()
		{
			InitializeComponent();
		}

		public void StartService(string[] args)
		{
			OnStart(args);
		}

		public void StopService()
		{
			OnStop();
		}

		protected override void OnStart(string[] args)
		{
			base.OnStart(args);

			_board = new YAODBoard();

			var addr = "http://localhost:5001";
			_controlHost = new NancyHost(new Uri(addr));
			_controlHost.Start();
		}

		protected override void OnStop()
		{
			using (_controlHost) { }
			using (_board) { }

			base.OnStop();
		}

		private void InitializeComponent()
		{
			ServiceName = "YAODService";
		}
	}
}
