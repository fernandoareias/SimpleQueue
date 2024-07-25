using System.Timers;
using ConsensusModule.Commands.Common;
using ConsensusModule.Enums;

namespace ConsensusModule.States;

internal class FollowerState : State
{
    private readonly Raft _raft;
    private System.Timers.Timer _electionTimer;
    public FollowerState(Raft raft)
    {
        _raft = raft;
    }

    public override void StartElectionTimer()
    {
        Random random = new Random();
        double timeout = random.Next(1500, 3001);

        Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {_raft.NodeId}][{_raft.State.GetType().Name}][TERMO {_raft.CurrentTerm}][LOG INDEX - {0}] - Iniciando a próxima eleição em {timeout}ms ...");

        _electionTimer = new System.Timers.Timer(timeout);
        _electionTimer.Elapsed += (sender, e) => StartElection();
        _electionTimer.AutoReset = false; 
        _electionTimer.Start();
    }

    public override void StartElection()
    {
        _raft.Candidate();
        _raft.State.StartElection();
    }
}