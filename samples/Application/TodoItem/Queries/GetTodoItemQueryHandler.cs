using MediatR;

namespace WebApiSample.Application.TodoItem.Queries;

public class GetTodoItemQueryHandler:IRequestHandler<GetTodoItemQuery, TodoItemDto>
{
    public Task<TodoItemDto> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TodoItemDto());
    }
}