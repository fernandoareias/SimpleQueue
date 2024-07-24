using System.Runtime.Serialization;
using ConsensusModule.Commands.Common;

namespace ConsensusModule.Commands.Views;

[DataContract]
public class RequestVoteCommandView : View
{
    public RequestVoteCommandView()
    {
        
    }

    public RequestVoteCommandView(bool voted)
    {
        Voted = voted;
    }

    [DataMember]
    public bool Voted { get; private set; }
}