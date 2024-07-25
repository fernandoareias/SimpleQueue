using System.Runtime.Serialization;
using ConsensusModule.Commands.Common;
using ConsensusModule.Commands.Views;

namespace ConsensusModule.Commands;

[DataContract]
public class RequestVoteCommand : Command<RequestVoteCommandView>
{
    protected RequestVoteCommand()
    {
        
    }
    public RequestVoteCommand(int term, int candidateId, int logIndex)
    {
        Term = term;
        CandidateId = candidateId;
        LogIndex = logIndex;
    }

    [DataMember]
    public int Term { get; private set; }
    
    [DataMember]
    public int CandidateId { get; private set; }
    
    [DataMember]
    public int LogIndex { get; private set; }
}
