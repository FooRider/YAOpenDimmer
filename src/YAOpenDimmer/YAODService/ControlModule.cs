using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Extensions;
using Nancy.Helpers;

namespace YAOD.Service
{
	public class ControlModule : NancyModule
	{
		public ControlModule() : base("/control/")
		{
			Get["/"] = x =>
			{
				var context = Response.Context;
				var request = context.Request;

				return IndexPage();
			};

			Post["/"] = x =>
			{
				var context = Response.Context;

				var body = this.Request.Body;
				var variables = ExtremelySimpleAndNaivePostDataDecoder.Decode(body);
				var bodyText = new StringBuilder();
				foreach (var name in variables.Keys)
				{
					bodyText.AppendFormat("{0}:\t{1}\r\n", name, variables[name]);
				}
				bodyText.AppendLine();

				for (int i = 0; i < 4; i++)
				{
					if (variables.ContainsKey("power[" + i + "]"))
					{
						var pwrstr = variables["power[" + i + "]"];
						float pwrval;
						if (float.TryParse(pwrstr, NumberStyles.Any, CultureInfo.InvariantCulture, out pwrval))
						{
							var brd = YAODBoard.Instance;
							if (brd == null)
							{
								bodyText.AppendLine("No instance of YAOD board found.");
							}
							else
							{
								brd.Test0(bodyText, i, pwrval);
							}
						}
						else
						{
							bodyText.AppendLine("Unable to parse value " + pwrstr);
						}
					}
				}

				return IndexPage(bodyText.ToString());
			};
		}

		private Response IndexPage(string tmp = null)
		{
			var response = new Response();

			response.ContentType = "text/html";
			response.StatusCode = HttpStatusCode.OK;
			response.Contents = stream =>
			{
				using (var tw = new StreamWriter(stream))
				{
					tw.WriteLine("<!DOCTYPE html>" +
											 "<html lang=\"en\">" +
											 "<head>" +
											 "\t<title>Yet Another Open Dimmer</title>" +
											 "</head>" +
											 "<body>" +
											 "\t<h1>Yet Another Open Dimmer</h1>" +
					             "<form action=\"?\" method=\"POST\">" +
					             "\t<p>" +
					             "\t\tChannel 0:" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.0\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.1\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.2\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.3\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.4\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.5\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.6\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.7\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.8\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"0.9\"/>" +
					             "\t\t<input type=\"submit\" name=\"power[0]\" value=\"1.0\"/>" +
					             "\t</p>" +
					             "\t<p>" +
											 "\t\tChannel 1:" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.0\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.1\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.2\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.3\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.4\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.5\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.6\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.7\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.8\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"0.9\"/>" +
											 "\t\t<input type=\"submit\" name=\"power[1]\" value=\"1.0\"/>" +
											 "\t</p>" +
					             "</form>" +

											 "<pre>" +
					             tmp +
											 "</pre>" +
											 "</body>" +
											 "</html>");
					tw.Flush();
				}
			};

			return response;
		}

		private class TmpParams
		{
			public string power { get; set; }
		}
	}
}
