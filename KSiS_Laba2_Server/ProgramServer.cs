using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static KSiS_Laba2_Kernel.Kernel;

namespace KSiS_Laba2_Server {
	internal static class ProgramServer {
		private static int Main(string[] args)
		{
			var sock = OpenSocket( );
			var finder = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			try
				{
					if(sock == null)
						{
							while(Console.ReadKey( ).Key != ConsoleKey.Enter) { }
							return 0;
						}
					var myIp = GetMyIp( );
					var cliEp = new IPEndPoint(IPAddress.Any, 0);
					var cliIp = IPAddress.Any;
					finder.Bind(new IPEndPoint(myIp, 9052));
					sock.Bind(new IPEndPoint(myIp, 9053));
					if(finder.Poll(600*1000000, SelectMode.SelectRead))
						{
							var buf = new byte[128];
							EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
							Console.Write("Recieving...");
							finder.ReceiveFrom(buf, ref ep);
							Console.WriteLine($" from {ep}");
							var data = Encoding.UTF8.GetString(buf);
							if(data.StartsWith("Server?"))
								{
									if(sock.ProtocolType == (data[7] == 't'? ProtocolType.Tcp : ProtocolType.Udp))
										{
											finder.SendTo(Encoding.UTF8.GetBytes("Yes."), ep);
											cliIp = BytesToIp(new[] { (byte)data[8], (byte)data[9], (byte)data[10], (byte)data[11] });
										var cliPort = ((byte) data[12])*256 + (byte) data[13];
											cliEp = new IPEndPoint(cliIp, cliPort);
										}
									else
										{
											Console.WriteLine("Client has wrong protocol.");
											while(Console.ReadKey( ).Key != ConsoleKey.Enter) { }
											return 0;
										}
								}
						}
					sock.Connect(cliEp);
					var s = string.Empty;
					if(sock.Poll(600*1000000, SelectMode.SelectRead))
						{
							while(sock.Available > 0)
								{
									var buf = new byte[65536];
									var count = sock.Receive(buf);
									s += Encoding.UTF8.GetString(buf).Remove(count);

								}
							sock.Send(Encoding.UTF8.GetBytes("Recieved"));
							var cliName = Dns.GetHostEntry(cliIp).HostName;
							while(cliName.Length < 65000) cliName += cliName;
							cliName = cliName.Remove(65000).ToUpper();
							s = s.ToUpper();
							var counter = cliName.Where((t, i) => t == s[i]).Count( );
							Console.WriteLine($"Good bytes: {((float)counter/s.Length*100)}%");
						}
				}
			catch (SocketException e) {
				Console.WriteLine((SocketError)e.ErrorCode);
			}
			finally
				{
					sock?.Close( );
					finder?.Close( );
				}
			Console.WriteLine("Bye bye =)\nP.s. Press \"Enter\" to exit.");
			while(Console.ReadKey( ).Key != ConsoleKey.Enter) { }
			return 0;
		}
	}
}
