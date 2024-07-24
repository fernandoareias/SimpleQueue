namespace ConsensusModule.Commands.Common;

public abstract class Command
{
}

public abstract class Command<TView> : Command
    where TView : View
{
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

}
