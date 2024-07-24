using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsensusModule.Commands;
using ConsensusModule.Commands.Common;
using ConsensusModule.Commands.Common.Interfaces;
using ConsensusModule.Commands.Handlers;
using ConsensusModule.Commands.Views;

public class Mediator
{
    private readonly Dictionary<Type, Type> _handlers = new Dictionary<Type, Type>();

    public Mediator()
    {
        Register<RequestVoteCommand, RequestVoteCommandView>(typeof(RequestVoteCommandHandler));
    }

    public async Task<View> Send(dynamic command) 
    {
        Type commandType = command.GetType();
        if (!_handlers.TryGetValue(commandType, out var handlerType))
            throw new InvalidOperationException($"No handler registered for {commandType.Name}");

        var handlerInstance = Activator.CreateInstance(handlerType);
        var method = handlerType.GetMethod("Handler");

        if (method == null)
            throw new InvalidOperationException($"No method handler registered for {handlerType}");
        
        return await (Task<View>)method.Invoke(handlerInstance, new object[] { command });
    }

    public void Register<TCommand, TView>(Type handler)
        where TCommand : Command<TView>
        where TView : View
    {
        _handlers[typeof(TCommand)] = handler;
    }
}