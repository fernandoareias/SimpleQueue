using System.Text;
using ConsensusModule.Commands.Common;
using ConsensusModule.Sockets.Requests.Common;

namespace ConsensusModule.Sockets.Requests;

public class RequestVoteBuilder : IBuilder<string>
{
    private StringBuilder _request = new StringBuilder();

    public RequestVoteBuilder()
    {
        _request.Append($"00{(int)ECommandType.REQUEST_VOTE};");
    }

    public RequestVoteBuilder AddTerm(int term)
    {
        _request.Append($"{term};");
        return this;
    }

    public RequestVoteBuilder AddCandidateId(int candidateId)
    {
        _request.Append($"{candidateId};");
        return this;
    }

    public RequestVoteBuilder AddLogIndex(int logIndex)
    {
        _request.Append($"{logIndex};");
        return this;
    }
    
    public string Build()
    {
        return _request.ToString();
    }
}