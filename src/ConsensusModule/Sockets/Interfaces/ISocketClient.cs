namespace ConsensusModule.Sockets.Interfaces;

public interface ISocketClient
{
    Task<string?> Send(string message, string serverIp, int port);
    
}