using ConsensusModule.Enums;
using ConsensusModule.Interfaces;

namespace ConsensusModule.States;

public class LeaderState : ILeaderState
{
    public ERaftState GetCurrentState()
    {
        throw new NotImplementedException();
    }
}