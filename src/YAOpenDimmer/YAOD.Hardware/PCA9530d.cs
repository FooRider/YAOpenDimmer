using System;
using RPi.I2C.Net;

namespace YAOD.Hardware
{
	public class PCA9530d
	{
		private readonly byte _i2cAddress;

		public DimmableChannel Channel0 { get; private set; }
		public DimmableChannel Channel1 { get; private set; }
		
		public PCA9530d(byte i2cAddress)
		{
			_i2cAddress = i2cAddress;
			Channel0 = new DimmableChannel();
			Channel1 = new DimmableChannel();
		}

		private byte ToPwmDutyCycle(float intensity)
		{
			if (intensity <= 0)
				return 0x00;
			if (intensity >= 1)
				return 0xFF;

			return (byte)(Math.Round(intensity * 0xFF));
		}

		public void WriteSetting(I2CBus bus)
		{
			byte multiplexing = 0xF0;

			if (Channel0.Intensity <= 0)
				multiplexing |= 0x00; // LED0 off
			else if (Channel0.Intensity >= 1)
				multiplexing |= 0x04; // LED0 on
			else
				multiplexing |= 0x08; // LED0 from PWM0

			if (Channel1.Intensity <= 0)
				multiplexing |= 0x00; // LED1 off
			else if (Channel1.Intensity >= 1)
				multiplexing |= 0x01; // LED1 on
			else
				multiplexing |= 0x03; // LED1 from PWM1

			var pwm0 = ToPwmDutyCycle(Channel0.Intensity);
			var pwm1 = ToPwmDutyCycle(Channel1.Intensity);

			var data = new byte[]
			{
				0x11, // Auto-increment, register 001 (PCS0)
				0x00, // reg PCS0 - set frequency prescaler to 0 => period0 = (0 + 1) / 152 = (1/152)s
				pwm0, // reg PWM0 - set duty cycle value
				0x00, // reg PCS1 - set frequency prescaler to 0
				pwm1, // reg PWM1 - set duty cycle value
				multiplexing, // reg LS0 - 1111 WXYZ, WX - LED0, YZ - LED1. 00 - LED off; 01 - LED on; 10 - PWM0; 01 - PWM1
			};

			bus.WriteBytes(_i2cAddress, data);
		}

		public void ReadSetting(I2CBus bus)
		{
			var bytes = bus.ReadBytes(_i2cAddress, 5);
			if (bytes.Length != 5)
				return;
			
			float pwm0 = ((float)bytes[1]) / 255f;
			float pwm1 = ((float)bytes[3]) / 255f;

			float intensity0 = 0f;
			float intensity1 = 0f;

			var multiplexing = bytes[4];

			if (((multiplexing & 0x0C) >> 2) == 0x00)
				intensity0 = 0f;
			else if (((multiplexing & 0x0C) >> 2) == 0x01)
				intensity0 = 1f;
			else if (((multiplexing & 0x0C) >> 2) == 0x02)
				intensity0 = pwm0;
			else if (((multiplexing & 0x0C) >> 2) == 0x03)
				intensity0 = pwm1;

			if (((multiplexing & 0x03)) == 0x00)
				intensity1 = 0f;
			else if (((multiplexing & 0x03)) == 0x01)
				intensity1 = 1f;
			else if (((multiplexing & 0x03)) == 0x02)
				intensity1 = pwm0;
			else if (((multiplexing & 0x03)) == 0x03)
				intensity1 = pwm1;

			Channel0.Intensity = intensity0;
			Channel1.Intensity = intensity1;
		}
	}
}
