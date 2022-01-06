using MediatR;
using WebApiSample.Application.Common.Models;

namespace WebApiSample.Application.TodoItems.Commands;

public class CreateTodoItemCommand:IRequest<Unit>
{
    public TodoItem TodoItem { get; set; }
}