using ConsensusModule.Commands.Common;
using ConsensusModule.Commands.Common.Interfaces;
using ConsensusModule.Commands.Views;

namespace ConsensusModule.Commands.Handlers;

public class RequestVoteCommandHandler : ICommandHandler<RequestVoteCommand, View>
{
    public async Task<View> Handler(RequestVoteCommand command)
    {
        return new RequestVoteCommandView();
    }
}