using MediatR;

namespace WebApiSample.Application.TodoItem.Commands;

public class CreateTodoItemCommandHandler:IRequestHandler<CreateTodoItemCommand, Unit>
{
    
    public Task<Unit> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        
        return Task.FromResult(Unit.Value);
    }
}