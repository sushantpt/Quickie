using Quickie.Handlers.Readonly.Definition;
using sample.minimalapi.Dtos;

namespace sample.minimalapi.Services.ReadOnly;

public interface IReadTodoReqHandler : IReadOnlyCollectionRequestHandler<TodoDto, string>;