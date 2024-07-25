namespace ConsensusModule.Sockets.Requests.Common;

public interface IBuilder<T>
{

    T Build();
}