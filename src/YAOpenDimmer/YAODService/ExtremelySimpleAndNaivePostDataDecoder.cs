using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Nancy.Helpers;

namespace YAOD.Service
{
	public static class ExtremelySimpleAndNaivePostDataDecoder
	{
		public static Dictionary<string, string> Decode(Stream bodyStream)
		{
			using (var reader = new StreamReader(bodyStream))
			{
				var sm = new StateMachine();
				var buffer = new Char[256];

				int count;
				while ((count = reader.Read(buffer, 0, buffer.Length)) > 0) 
				{
					for (int i = 0; i < count; i++)
					{
						if (buffer[i] == '&')
						{
							sm.SetVariableName();
						}
						else if (buffer[i] == '=')
						{
							sm.SetVariableValue();
						}
						else
						{
							sm.Data(buffer[i]);
						}
					}
				}

				return sm.SetEnd();
			}
		}

		private class StateMachine
		{
			private const int StateVarName = 1;
			private const int StateVarValue = 2;

			private int _state = StateVarName;
			private string _variableName = null;
			private StringBuilder _buffer;
			private Dictionary<string, string> _values = new Dictionary<string, string>();

			public void SetVariableName()
			{
				if (_state == StateVarName) //
				{
					if (_buffer != null)
					{
						_values[HttpUtility.UrlDecode(_buffer.ToString())] = null;
					}
				}
				else if (_state == StateVarValue)
				{
					if (_variableName != null)
					{
						if (_buffer != null)
							_values[HttpUtility.UrlDecode(_variableName)] = HttpUtility.UrlDecode(_buffer.ToString());
						else
							_values[HttpUtility.UrlDecode(_variableName)] = null;
					}
				}
				_variableName = null;
				_buffer = null;
				
				_state = StateVarName;
			}

			public void SetVariableValue()
			{
				if (_buffer == null)
					_variableName = "";
				else
					_variableName = _buffer.ToString();
				_buffer = new StringBuilder();
				
				_state = StateVarValue;
			}

			public Dictionary<string, string> SetEnd()
			{
				SetVariableName();

				return _values;
			}

			public void Data(char ch)
			{
				if (_buffer == null)
					_buffer = new StringBuilder();
				_buffer.Append(ch);
			}

			public void Data(string data)
			{
				if (_buffer == null)
					_buffer = new StringBuilder();
				_buffer.Append(data);
			}

			public void Data(Char[] buffer, int offset, int count)
			{
				if (_buffer == null)
					_buffer = new StringBuilder();
				_buffer.Append(buffer, offset, count);
			}
		}
	}
}
