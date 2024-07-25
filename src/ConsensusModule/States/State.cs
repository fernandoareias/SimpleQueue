namespace ConsensusModule.States;

public abstract class State
{
    protected Raft _context;

    public void SetContext(Raft raft)
    {
        _context = raft;
    }

    public abstract void StartElectionTimer();
    public abstract void StartElection();
}