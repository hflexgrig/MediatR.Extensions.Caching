using MediatR;
using WebApiSample.Application.Common.Repository;

namespace WebApiSample.Application.TodoItems.Queries;

public class GetTodoItemQueryHandler:IRequestHandler<GetTodoItemsQuery, List<TodoItemDto>>
{
    private readonly TodoItemsRepository _todoItemsRepository;

    public GetTodoItemQueryHandler(TodoItemsRepository todoItemsRepository)
    {
        _todoItemsRepository = todoItemsRepository;
    }
    
    public Task<List<TodoItemDto>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_todoItemsRepository.TodoItems.Skip(request.Page*request.Size).Take(request.Size)
            .Select(x => new TodoItemDto(){Id = x.Id, Name = x.Name} ).ToList());
    }
}