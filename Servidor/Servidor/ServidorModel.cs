using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Servidor
{
    public class ServidorModel
    {
        static TcpListener tcpListener;
        List<ClienteModel> clientes = new List<ClienteModel>();

        protected internal void AddConnection(ClienteModel clienteModel)
        {
            if (!clientes.Any(c => c.nickName.Equals(clienteModel.nickName)))
                clientes.Add(clienteModel);
            else
                Console.WriteLine("*** Desculpe, o apelido já está sendo utilizado! Por favor escolha outro:");
        }
        protected internal void RemoveConnection(string id)
        {
            ClienteModel cliente = clientes.Where(c => c.Id.Equals(id)).FirstOrDefault();
            if (cliente != null)
                clientes.Remove(cliente);
        }
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("*** O servidor está em execução! Aguardando conexões...");
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClienteModel clienteModel = new ClienteModel(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clienteModel.Process));
                    clientThread.Start();
                }

            }
            catch (Exception exception)
            {

                Console.WriteLine(exception.Message);
                Disconnect();
            }
        }
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            for (int i = 0; i < clientes.Count; i++)
            {
                if (clientes[i].Id != id)
                {
                    clientes[i].Stream.Write(data, 0, data.Length);
                }
            }
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop();

            for (int i = 0; i < clientes.Count; i++)
            {
                clientes[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
