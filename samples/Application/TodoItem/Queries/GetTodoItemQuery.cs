using MediatR;

namespace WebApiSample.Application.TodoItem.Queries;

public class GetTodoItemQuery:IRequest<TodoItemDto>
{
    public int Page { get; set; }
}

public class TodoItemDto
{
}