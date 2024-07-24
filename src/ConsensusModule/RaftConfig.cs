namespace ConsensusModule;

public class RaftConfig
{
    public List<RaftConfigNode> Nodes { get; set; } = new List<RaftConfigNode>();
    public int ElectionTimeout { get; set; }
    public int HeartbeatInterval { get; set; }
    public string LogDirectory { get; set; }
}

public class RaftConfigNode
{
    public string Id { get; set; }
    public string Address { get; set; }
}