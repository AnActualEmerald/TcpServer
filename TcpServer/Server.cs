/*
 * Created by SharpDevelop.
 * User: kgauthier16
 * Date: 3/24/2015
 * Time: 8:18 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace TCPServer
{
	class Server
	{
		TcpListener _server;
		List<Socket> clients = new List<Socket>();
		byte[] buff = new byte[1024];
		
		
		public Server(IPAddress addr, int port)
		{
			_server = new TcpListener(addr, port);
		}		
		
		public Server Start()
		{
			_server.BeginAcceptSocket(new AsyncCallback(AcceptCall), null);
			return this;
		}
		
		#region Async callbacks
		private void AcceptCall(IAsyncResult r)
		{
			Socket c = _server.EndAcceptSocket(r);
			clients.Add(c);
			c.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(ReceiveCall), null);
			_server.BeginAcceptSocket(new AsyncCallback(AcceptCall), null);
		}
		
		private void ReceiveCall(IAsyncResult r)
		{
			Socket c = r.AsyncState as Socket;
			byte[] rec = new byte[c.EndReceive(r)];
			Array.Copy(buff, rec, rec.Length);
			
			Console.WriteLine("Client sent: " + Encoding.ASCII.GetString(rec));
		}
		
		private void SendCall(IAsyncResult r)
		{
			Socket c = r.AsyncState as Socket;
			c.EndSend(r);
		}
		#endregion
		
		public void PerformCommand(String command, String[] args)
		{
			if(command.ToLower() == "send")
			{
				bool all = false;
				String message = "";
				foreach(String s in args)
				{
					String ss = s.Trim();
					if(ss.Contains("-all"))
						all = true;
					
					if(!ss.StartsWith("-"))
						message = ss;
				}
				
				Console.WriteLine("Will send \"" + message + "\" to clients. All was " + all);
				if (all)
					SendText (message, clients.ToArray ());
				else
					Console.WriteLine ("Not Done Yet");
			}
			
			if(command.ToLower() == "ping")
				Console.WriteLine("pong");
		}
		
		private void SendText(String text, Socket[] targets)
		{
			byte[] str = Encoding.ASCII.GetBytes(text);
			foreach(Socket c in targets)
				c.BeginSend(str, 0, str.Length, SocketFlags.None, new AsyncCallback(SendCall), null);
		}
	}
}