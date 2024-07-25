using ConsensusModule.Commands.Common;
using ConsensusModule.Enums;
using ConsensusModule.Sockets.Requests;
using ConsensusModule.Sockets.Requests.Factories;

namespace ConsensusModule.States;

public class CandidateState : State
{
    private Raft _raft;

    public CandidateState(Raft context)
    {
        _raft = context;
    }

    public override void StartElectionTimer()
    {
        throw new NotImplementedException();
    }

    public override void StartElection()
    {
        Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_raft.NodeId}][{_raft.State.GetType().Name}][TERMO {_raft.CurrentTerm}][LOG INDEX - {0}] - Iniciando a eleição.");

        _raft.AddTerm();
        int votesReceived = 1; 

        foreach (var node in _raft.Nodes)
        {

            var request = RequestVoteFactory.CreateRequest(_raft.NodeId, _raft.LogIndex, _raft.CurrentTerm);
            
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_raft.NodeId}][{_raft.State.GetType().Name}][TERMO {_raft.CurrentTerm}][LOG INDEX - {0}] - Enviando mensagem '{request}' para o nó {node} ");
            var n = node.Split(":");
            var response = _raft.SocketClient.Send(request, n[0], int.Parse(n[1])).GetAwaiter().GetResult();
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_raft.NodeId}][{_raft.State.GetType().Name}][TERMO {_raft.CurrentTerm}][LOG INDEX - {0}] - {node} respondeu {response}");
            if (response == "VOTE_GRANTED")
            {
                votesReceived += 1;
            }
        }

        if (votesReceived >= _raft.Nodes.Length / 2)
        {
            Console.WriteLine(
                $"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_raft.NodeId}][{_raft.State.GetType().Name}][TERMO {_raft.CurrentTerm}][LOG INDEX - {0}] - Eleição vencida. Transitando para Líder.");
            _raft.Leader();
            return;
        }

        Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_raft.NodeId}][{_raft.State.GetType().Name}][TERMO {_raft.CurrentTerm}][LOG INDEX - {0}] - Eleição perdida. Retornando para Follower.");
        _raft.Follower();
        _raft.State.StartElectionTimer(); 
    }
}