using ConsensusModule.Commands.Common;

namespace ConsensusModule.Commands
{
    public static class CommandFactory
    {
        public static Command Create(ECommandType commandType)
        {
            switch (commandType)
            {
                case ECommandType.REQUEST_VOTE:
                    return CreateRequestVote();
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }

        private static RequestVoteCommand CreateRequestVote()
        {
            return new RequestVoteCommand();
        }
    }
}