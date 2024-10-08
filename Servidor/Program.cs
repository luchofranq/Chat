using System;
using System.Collections.Generic;  
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private static TcpListener listener = null!; 
                                           
    private static List<TcpClient> clients = new List<TcpClient>();
    private static readonly object _lock = new object(); 

    static void Main(string[] args)
    {
       
        listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();  
        Console.WriteLine("Servidor iniciado en el puerto 8888");

        
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();  

           
            lock (_lock)
            {
                clients.Add(client);
            }

            Thread clientThread = new Thread(HandleClient); 
            clientThread.Start(client);  
        }
    }

    
    private static void HandleClient(object? clientObj)
    {
        TcpClient client = (TcpClient)clientObj!;  
        NetworkStream stream = client.GetStream(); 
        byte[] buffer = new byte[1024]; 
        int byteCount;

        try
        {
           
            while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
               
                string data = Encoding.ASCII.GetString(buffer, 0, byteCount);
                Console.WriteLine("Recibido: " + data); 

             
                Broadcast("Cliente: " + data, client);
            }
        }
        finally
        {
            
            lock (_lock)
            {
                clients.Remove(client);
            }
            client.Close();
        }
    }

    
    private static void Broadcast(string message, TcpClient senderClient)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);

        lock (_lock)
        {
            foreach (TcpClient client in clients)
            {
                if (client != senderClient)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
        }
    }
}
