using ConsensusModule.Enums; 

namespace ConsensusModule.States;

public class LeaderState : State
{
    private Raft _context;

    public LeaderState(Raft context)
    {
        _context = context;
    }

    public override void StartElectionTimer()
    {
        throw new NotImplementedException();
    }

    public override void StartElection()
    {
        throw new NotImplementedException();
    }
}