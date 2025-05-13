using System.Net.Sockets;
using ChatServer.Models;
using ChatShared.DTO;
using ChatServer.Data;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ChatServer.Services;

namespace ChatServer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new(8888);
            await server.Start();
        }
    }
}
