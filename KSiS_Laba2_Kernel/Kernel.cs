using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace KSiS_Laba2_Kernel {
	public static class Kernel {
		public static Socket OpenSocket( )
		{
			Console.WriteLine("Press 1 to select TCP or 2 to select UDP.");
			ProtocolType protocol;
			SocketType socket;
			switch (Console.ReadKey( ).Key)
				{
					case ConsoleKey.D1:
					case ConsoleKey.NumPad1:
						protocol = ProtocolType.Tcp;
						socket = SocketType.Stream;
						Console.WriteLine("\nTCP Socket created!");
						break;
					case ConsoleKey.D2:
					case ConsoleKey.NumPad2:
						protocol = ProtocolType.Udp;
						socket = SocketType.Dgram;
						Console.WriteLine("\nUDP Socket created!");
						break;
					default:
						Console.WriteLine("Socket don't opend.");
						return null;
				}
			return new Socket(AddressFamily.InterNetwork, socket, protocol);
		}

		public static IPAddress GetMyBroadcast() {
			var myIp = Dns.GetHostAddresses(Dns.GetHostName())
						.Where(address => address.AddressFamily == AddressFamily.InterNetwork)
						.ToArray()[0].GetAddressBytes();
			return IPAddress.Parse(myIp[0] + "." + myIp[1] + "." + myIp[2] + ".255");
		}

		public static IEnumerable<IPAddress> GetMyBroadcasts() {
			var myIps = Dns.GetHostAddresses(Dns.GetHostName())
						.Where(address => address.AddressFamily == AddressFamily.InterNetwork)
						.ToArray();

			foreach (var myIp in myIps) {
				yield return IPAddress.Parse(myIp.GetAddressBytes()[0] + "." + myIp.GetAddressBytes()[1] + "." + myIp.GetAddressBytes()[2] + ".255");
			}
			//return IPAddress.Parse(myIp[0] + "." + myIp[1] + "." + myIp[2] + ".255");
		}

		public static IPAddress GetMyIp() {
			return Dns.GetHostAddresses(Dns.GetHostName())
					.Where(address => address.AddressFamily == AddressFamily.InterNetwork)
					.ToArray()[0];
		}

		public static IPAddress[] GetMyIps() {
			return Dns.GetHostAddresses(Dns.GetHostName())
					.Where(address => address.AddressFamily == AddressFamily.InterNetwork)
					.ToArray();
		}

		public static IPAddress BytesToIp(byte[] ip)
		{
			return IPAddress.Parse(ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3]);
		}
	}
}
