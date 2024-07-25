namespace ConsensusModule.Sockets.Requests.Factories;

public static class RequestVoteFactory
{
    public static string CreateRequest(int candidateId, int logIndex, int term)
    {
        return new RequestVoteBuilder()
            .AddCandidateId(candidateId)
            .AddLogIndex(logIndex)
            .AddTerm(term)
            .Build();
    }
}