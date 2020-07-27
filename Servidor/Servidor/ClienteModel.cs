using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace Servidor
{
   public  class ClienteModel
    {
        protected internal string Id { get; set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal string nickName;
        TcpClient tcpClient;
        ServidorModel servidorModel;

        public ClienteModel(TcpClient _tcpClient, ServidorModel _servidorModel)
        {
            Id = Guid.NewGuid().ToString();
            tcpClient = _tcpClient;
            servidorModel = _servidorModel;
            Stream = tcpClient.GetStream();
            nickName = GetMessage();
            _servidorModel.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                string message = nickName;
                Console.Write(">> ");
                servidorModel.BroadcastMessage(message, this.Id);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(message);
                Console.ResetColor();
                Console.Write(" Entrou no chat!");
                Console.WriteLine();

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = $"{nickName}: {message}";
                        Console.WriteLine(message);
                        servidorModel.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        Console.Write(">> ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        message = $"{nickName}: {message} saiu do chat!";
                        Console.WriteLine(message);
                        servidorModel.BroadcastMessage(message, this.Id);
                        Console.ResetColor();
                        break;
                    }

                }

            }
            catch (Exception exception)
            {

                Console.WriteLine(exception.Message);
            }
            finally
            {
                servidorModel.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (tcpClient != null)
                tcpClient.Close();
        }
    }
}
