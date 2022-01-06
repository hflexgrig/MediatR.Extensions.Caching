using WebApiSample.Application.Common.Models;

namespace WebApiSample.Application.Common.Repository;

public class TodoItemsRepository
{
    
    public TodoItemsRepository()
    {
        for (int i = 0; i < 10; i++)
        {
            TodoItems.Add(new TodoItem(){Id = Guid.NewGuid(), Name = $"Name_{i}"});
        }
    }

    public IList<TodoItem> TodoItems { get; } = new List<TodoItem>();

    public void AddTodoItem(TodoItem todoItem)
    {
        TodoItems.Add(todoItem);
    }
}