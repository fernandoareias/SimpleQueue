using System.Runtime.Serialization;
using ConsensusModule.Commands.Common;
using ConsensusModule.Commands.Views;

namespace ConsensusModule.Commands;

[DataContract]
public class RequestVoteCommand : Command<RequestVoteCommandView>
{
    
}
