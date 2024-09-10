using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoAppPoc;
using ToDoAppPoc.Data;
using ToDoAppPoc.Models;

namespace ToDoAppPoc.Services
{
    public class ToDoService(ILogger<ToDoService> logger, AppDbContext dbContext) : ToDoGrpcService.ToDoGrpcServiceBase
    {
        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid argument for title & description"));
            }

            var todoItem = new ToDoItem()
            {
                Title = request.Title,
                Description = request.Description,
            };
            await dbContext.Items.AddAsync(todoItem);
            await dbContext.SaveChangesAsync();
            return new CreateToDoResponse() { Id = todoItem.Id.ToString() };
        }

        public override async Task<ToDoItemResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid argument for id"));
            }

            if (!Guid.TryParse(request.Id, out Guid guidInput))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "supplied id is not valid."));
            }

            var todoItem = await dbContext.Items.FindAsync(guidInput);
            if (todoItem is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "todo item not found."));
            }

            return new ToDoItemResponse()
            {
                Id = todoItem.Id.ToString(),
                Status = todoItem.Status,
                Description = todoItem.Description,
                Title = todoItem.Title
            };
        }

        public override async Task<ReadAllResponse> ReadAll(ReadAllRequest request, ServerCallContext context)
        {
            var todoItems = await dbContext.Items.ToListAsync();
            var response =new ReadAllResponse();
            foreach (var todoItem in todoItems)
            {
                response.ToDo.Add(new ToDoItemResponse()
                {
                    Id = todoItem.Id.ToString(),
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    Status = todoItem.Status
                });
            }
            return response;
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid argument for id."));
            }
            if (string.IsNullOrEmpty(request.Title))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid argument for title."));
            }
            if (string.IsNullOrEmpty(request.Description))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid argument for description."));
            }

            if (!Guid.TryParse(request.Id, out Guid guid))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid argument for id."));
            }

            var todoItem = await dbContext.Items.FindAsync(guid);
            if (todoItem is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "todo item not found."));
            }

            todoItem.Title = request.Title;
            todoItem.Description = request.Description;
            todoItem.Status = request.Status;
            await dbContext.SaveChangesAsync();
            return new UpdateToDoResponse() { Id = todoItem.Id.ToString() };
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid argument for id."));
            }
            if (!Guid.TryParse(request.Id, out Guid guidInput))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "supplied id is not valid."));
            }

            var todoItem = await dbContext.Items.FindAsync(guidInput);
            if (todoItem is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "todo item not found."));
            }

            dbContext.Items.Remove(todoItem);
            await dbContext.SaveChangesAsync();
            return new DeleteToDoResponse();
        }
    }
}
