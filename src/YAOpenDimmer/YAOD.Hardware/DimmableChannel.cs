using System;
namespace YAOD.Hardware
{
	public class DimmableChannel
	{
		private float _intensity = 0f;
		public float Intensity
		{
			get { return _intensity; }
			set
			{
				if (value < 0f)
					value = 0f;
				if (value > 1f)
					value = 1f;
				_intensity = value;
			}
		}

		public DimmableChannel()
		{
		}
	}
}
