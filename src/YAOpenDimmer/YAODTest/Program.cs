using System;
using System.Threading;
using RPi.I2C.Net;
using YAOD.Hardware;

namespace YAODTest
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			//Test0();
			Test1();
		}

		private static void Test0()
		{
			Console.WriteLine("YAOD Test 0 - write and read PCA9530d settings");

			using (var bus = I2CBus.Open("/dev/i2c-0"))
			{
				var dimmer = new PCA9530d(0x60);
				dimmer.Channel0.Intensity = 1.0f;
				dimmer.Channel1.Intensity = 0.0f;
				dimmer.WriteSetting(bus);

				Console.WriteLine("Press Enter to read settings");
				Console.ReadLine();

				dimmer.ReadSetting(bus);

				Console.WriteLine("Channel 0: {0:0.00}", dimmer.Channel0.Intensity);
				Console.WriteLine("Channel 1: {0:0.00}", dimmer.Channel1.Intensity);

				Console.WriteLine();
				Console.WriteLine("Press Enter to change settings");
				Console.ReadLine();

				dimmer.Channel0.Intensity = 0.0f;
				dimmer.Channel1.Intensity = 1.0f;
				dimmer.WriteSetting(bus);

				Console.WriteLine("Press Enter to read settings");
				Console.ReadLine();

				dimmer.ReadSetting(bus);

				Console.WriteLine("Channel 0: {0:0.00}", dimmer.Channel0.Intensity);
				Console.WriteLine("Channel 1: {0:0.00}", dimmer.Channel1.Intensity);

				Console.WriteLine("Done");
				Console.ReadLine();
			}
		}

		private static void Test1()
		{
			Console.WriteLine("YAOD Test 1 - continuous dimming");

			using (var bus = I2CBus.Open("/dev/i2c-0"))
			{
				var dimmer = new PCA9530d(0x60);

				Console.WriteLine("Press any key to stop");

				var startTime = DateTime.Now;
				var period = TimeSpan.FromSeconds(5);

				while (!Console.KeyAvailable)
				{
					var duration = DateTime.Now - startTime;
					var phase = ((double)(duration.Ticks % period.Ticks)) / ((double)period.Ticks);

					dimmer.Channel0.Intensity = (float)((Math.Sin(Math.PI * 2 * phase) + 1) / 2);
					dimmer.Channel1.Intensity = (float)((Math.Cos(Math.PI * 2 * phase) + 1) / 2);
					dimmer.WriteSetting(bus);

					Thread.Sleep(10);
				}
			}
		}
	}
}
