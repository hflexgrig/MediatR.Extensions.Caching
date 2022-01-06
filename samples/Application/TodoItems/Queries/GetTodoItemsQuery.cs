using MediatR;

namespace WebApiSample.Application.TodoItems.Queries;

public class GetTodoItemsQuery:IRequest<List<TodoItemDto>>
{
    public int Page { get; set; }
    public int Size { get; set; } = 5;
}

public class TodoItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}