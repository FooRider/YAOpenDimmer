using System;
using System.Linq;
using System.Text;
using RPi.I2C.Net;
using YAOD.Hardware;

namespace YAOD.Service
{
	public class YAODBoard : IDisposable
	{
		private readonly object _locker = new object();
		private I2CBus _bus;
		private PCA9530d _ledController0;
		private PCA9530d _ledController1;
		private DimmableChannel[] _channels;

		public DimmableChannel[] Channels
		{
			get { return _channels.ToArray(); }
		}
		
		public YAODBoard()
		{
			_bus = I2CBus.Open("/dev/i2c-0");
			_ledController0 = new PCA9530d(0x60);
			_ledController1 = new PCA9530d(0x61);
			_channels = new[] 
			{
				_ledController0.Channel0,
				_ledController0.Channel1,
				_ledController1.Channel0,
				_ledController1.Channel1,
			};

			YAODBoard.Instance = this;
		}

		public void Test0(StringBuilder log, int channel, float value)
		{
			lock (_locker)
			{
				try
				{
					if (channel < 0 || channel >= _channels.Length)
					{
						log.AppendLine("Unable to find channel #" + channel);
						return;
					}

					var ch = _channels[channel];

					ch.Intensity = value;

					log.AppendLine("Setting new values.");
					_ledController0.WriteSetting(_bus);
				}
				catch (Exception ex)
				{
					log.AppendLine("Exception caught: " + ex.ToString());
				}
			}
		}

		public void Test1(StringBuilder log)
		{
			lock (_locker)
			{
				var brd = YAODBoard.Instance;
				if (brd == null)
				{
					log.AppendLine("No instance of YAOD board found.");
					return;
				}
			}
		}

		public void Dispose()
		{
			using (_bus) { }
		}

		public static YAODBoard Instance { get; private set; }
	}
}
