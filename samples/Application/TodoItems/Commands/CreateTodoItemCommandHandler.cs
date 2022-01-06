using MediatR;
using WebApiSample.Application.Common.Repository;

namespace WebApiSample.Application.TodoItems.Commands;

public class CreateTodoItemCommandHandler:IRequestHandler<CreateTodoItemCommand, Unit>
{
    private readonly TodoItemsRepository _todoItemsRepository;

    public CreateTodoItemCommandHandler(TodoItemsRepository todoItemsRepository)
    {
        _todoItemsRepository = todoItemsRepository;
    }
    
    public Task<Unit> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        _todoItemsRepository.AddTodoItem(request.TodoItem);
        return Task.FromResult(Unit.Value);
    }
}