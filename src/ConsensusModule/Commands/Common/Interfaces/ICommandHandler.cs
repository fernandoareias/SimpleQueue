namespace ConsensusModule.Commands.Common.Interfaces;

public interface ICommandHandler<TCommand, TView> where TCommand : Command where TView : View
{
    Task<TView> Handler(TCommand command, Raft raft);
}