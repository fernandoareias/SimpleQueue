using ConsensusModule.Commands.Common;

namespace ConsensusModule.Commands
{
    public static class CommandFactory
    {
        public static Command Create(ECommandType commandType, string request)
        {
            switch (commandType)
            {
                case ECommandType.REQUEST_VOTE:
                    return CreateRequestVote(request);
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }

        private static RequestVoteCommand CreateRequestVote(string request)
        {
            Console.WriteLine($"Request que chegou no factory: {request}");

            int term = int.Parse(request[4].ToString());
            int candidateId = int.Parse(request[6].ToString());
            int logIndex = int.Parse(request[8].ToString());
            
            Console.WriteLine($"Criando REQUEST_VOTE: Term: {term} | Candidate: {candidateId} | LogIndex: {logIndex}");
            return new RequestVoteCommand(term, candidateId, logIndex);
        }
    }
}