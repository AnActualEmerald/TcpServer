/*
 * Created by SharpDevelop.
 * User: kgauthier16
 * Date: 3/24/2015
 * Time: 8:43 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
namespace TCPServer
{
	class Starter
	{
		static Server s;
		public static void Main(string[] args)
		{
			IPAddress addr;
			Console.WriteLine("enter ip: ");
			string ip = Console.ReadLine();
			if(ip == "any")
				addr = IPAddress.Any;
			else
				addr = IPAddress.Parse(ip);
			
			Console.WriteLine("enter port: ");
			int port = int.Parse(Console.ReadLine());
			s = new Server(addr, port);
			
			Run();
		}
		
		public static void Run()
		{
			
			while(true)
			{
				Console.WriteLine("Enter Command: ");
				Command c = parseCommand(Console.ReadLine() + " ");
				s.PerformCommand(c.command, c.args);
			}
		}
		
		public static Command parseCommand(string c_string)
		{
			char[] chars = c_string.ToCharArray();
			List<char> part = new List<char>();
			List<string> fin_parts = new List<string>();
			bool isMessage = false;
			for(int i = 0; i < chars.Length; i++)
			{
				if(chars[i] == '\"'){
					isMessage = !isMessage;
					continue;
				}
				
				if(chars[i] == ' ' && !isMessage){
					fin_parts.Add(new string(part.ToArray()));
					part.Clear();
				}
				
				part.Add(chars[i]);
			}
			
			string command = fin_parts[0];
			fin_parts.Remove(command);
			
			return new Command()
			{
				command = command,
				args = fin_parts.ToArray()
			};
		}
	}
	
	struct Command
	{
		public string command;
		public string[] args;
	}
}