using ConsensusModule.Enums;
using ConsensusModule.Interfaces;

namespace ConsensusModule.States;

public class FollowerState : IFollowerState
{
    public ERaftState GetCurrentState()
    {
        throw new NotImplementedException();
    }

    public void StateElection()
    {
        throw new NotImplementedException();
    }
}