using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ConsensusModule.Commands;
using ConsensusModule.Commands.Common;

namespace ConsensusModule.Sockets;

public  class SocketClient
{
    private readonly int _processId;
    private readonly Mediator _mediator;
    public SocketClient(int processId)
    {
        _processId = processId;
        _mediator = new Mediator();
    }
    public async Task<string?> Send(string message, string serverIp, int port)
    {
        Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET CLIENT] - Iniciando cliente...");
        using Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET CLIENT] - Conectado ao servidor...");
            clientSocket.Connect(endPoint);
            
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET CLIENT] - Enviando {message}");

            string messageToSend = message + " <*>";
            byte[] dataToSend = Encoding.ASCII.GetBytes(messageToSend);

            clientSocket.Send(dataToSend);


            byte[] buffer = new byte[1024];
            int bytesRead = clientSocket.Receive(buffer);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET CLIENT] - Recebendo {response}");

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[-][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET CLIENT][EXCEPTION] - {e.Message}");
            return null;
        }
        finally
        {
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET CLIENT] - Finalizando cliente...");
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }


    public async Task Server(string serverIp, int port)
    {
        Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER] - Iniciando servidor...");
        using Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(100);
             
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER] - Servidor ouvindo na porta {port}...");

            while (true)
            {
                try
                {
                    var handler = await serverSocket.AcceptAsync();
                    _ = HandleClientAsync(handler); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[-][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER][EXCEPTION] - Erro ao aceitar conexão: {ex.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[-][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER][EXCEPTION] - {e.Message}");
        }
        finally
        {
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER] - Finalizando servidor...");
            serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Close();
        }
    }
    
    private async Task HandleClientAsync(Socket handler)
    {
        try
        {
            while (true)
            {
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                if (received == 0)
                    break;

                var receivedString = Encoding.UTF8.GetString(buffer, 0, received);
                var eom = "<*>";
                if (receivedString.IndexOf(eom) > -1)
                {
                    receivedString = receivedString.Replace(eom, "").Trim();
                    Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER] - Servidor recebeu: \"{receivedString}\"");
                    
                    var commandType = (ECommandType)int.Parse(receivedString);
                    var command = CommandFactory.Create(commandType);
                    var response = await _mediator.Send(command);

                    var responseSerialized = JsonSerializer.Serialize(response);
                    var echoBytes = Encoding.UTF8.GetBytes(responseSerialized);
                    await handler.SendAsync(echoBytes, 0);
                    
                    Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER] - Servidor respondeu: \"{responseSerialized}\"");
                }
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
        {
            Console.WriteLine($"[-][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER][EXCEPTION] - Conexão redefinida pelo par: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[-][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER][EXCEPTION] - Erro ao processar cliente: {ex.Message}");
        }
        finally
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_processId}][SOCKET SERVER] - Cliente desconectado.");
        }
    }
}