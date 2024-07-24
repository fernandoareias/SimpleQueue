using ConsensusModule.Enums;

namespace ConsensusModule.Interfaces;

public interface IRaftStates
{
    ERaftState GetCurrentState();
}