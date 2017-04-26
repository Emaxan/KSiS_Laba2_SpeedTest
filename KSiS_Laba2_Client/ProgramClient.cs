using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static KSiS_Laba2_Kernel.Kernel;

namespace KSiS_Laba2_Client {
	internal static class ProgramClient {
		private static int Main(string[] args)
		{
			Socket socks = OpenSocket( );
			var finder = new Socket[0];
			var sock = new Socket[0];

			try {
				if (socks == null) {
					while (Console.ReadKey().Key != ConsoleKey.Enter) { }
					return 0;
				}
				var myIps = GetMyIps();
				Array.Resize(ref finder, myIps.Length);
				Array.Resize(ref sock, myIps.Length);
				for (var i = 0; i < sock.Length; i++)
					sock[i] = new Socket(AddressFamily.InterNetwork, socks.SocketType, socks.ProtocolType);
				var myBroadcasts = GetMyBroadcasts().ToArray();
				for (var i = 0; i < myIps.Length; i++) {
					finder[i] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					finder[i].Bind(new IPEndPoint(myIps[i], 9000-i));
					sock[i].Bind(new IPEndPoint(myIps[i], 9050-i));

					finder[i].SendTo(Encoding.UTF8.GetBytes(
						"Server?" + (sock[i].ProtocolType == ProtocolType.Tcp ? "t" : "u") +
						(char) myIps[i].GetAddressBytes()[0] + (char) myIps[i].GetAddressBytes()[1] +
						(char) myIps[i].GetAddressBytes()[2] + (char) myIps[i].GetAddressBytes()[3] + (char)35 + (char)(90-i)),
						new IPEndPoint(myBroadcasts[i], 9052));

					EndPoint serv;
					if (finder[i].Poll(10*1000000, SelectMode.SelectRead)) {
						var buf = new byte[128];
						EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
						finder[i].ReceiveFrom(buf, ref ep);
						var data = Encoding.UTF8.GetString(buf);
						if (data.StartsWith("Yes.")) {
							Console.WriteLine($"Server is {ep}");
							serv = ep;
						}
						else serv = null;
					}
					else {
						Console.WriteLine("No server");
						//while (Console.ReadKey().Key != ConsoleKey.Enter) { }
						continue;//return 0;
					}
					if (serv == null) {
						Console.WriteLine("No server");
						//while (Console.ReadKey().Key != ConsoleKey.Enter) { }
						continue;//return 0;
					}
					var finish = DateTime.MinValue;
					var servIp = serv.ToString();
					servIp = servIp.Remove(servIp.IndexOf(':'));
					sock[i].Connect(IPAddress.Parse(servIp), 9053);
					var buf1 = new byte[128];
					var mess = Dns.GetHostName();
					while (mess.Length < 65000) mess += mess;
					mess = mess.Remove(65000);
					var messb = Encoding.UTF8.GetBytes(mess);

					var start = DateTime.UtcNow;
					sock[i].Send(messb);
					sock[i].Poll(30*1000000, SelectMode.SelectRead);
					sock[i].Receive(buf1);
					var s = Encoding.UTF8.GetString(buf1);
					if (s.StartsWith("Recieved")) finish = DateTime.UtcNow;
					int size;
					double dur;
					Console.WriteLine(
						$"Duration: {dur = (finish - start).TotalMilliseconds} Milliseconds;\n" +
						$"Size:     {size = mess.Length + 8} Bytes;\n" +
						$"Speed:    {size/(dur/1000)} B/s;\n" +
						$"          {size/(dur/1000)/1024} KB/s;\n" +
						$"          {size/(dur/1000)/1024/1024} MB/s;"
						);
					break;
				}
			}

			catch (SocketException e) {
				Console.WriteLine((SocketError) e.ErrorCode);
			}

			finally {
				foreach (var socket in sock) socket?.Close();
				foreach (var socket in finder) socket?.Close();
			}
			Console.WriteLine("Bye bye =)\nP.s. Press \"Enter\" to exit.");
			while(Console.ReadKey( ).Key != ConsoleKey.Enter) { }
			return 0;
		}
	}
}
