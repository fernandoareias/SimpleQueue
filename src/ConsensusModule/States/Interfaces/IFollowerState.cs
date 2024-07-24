namespace ConsensusModule.Interfaces;

public interface IFollowerState : IRaftStates
{
    void StateElection();
}