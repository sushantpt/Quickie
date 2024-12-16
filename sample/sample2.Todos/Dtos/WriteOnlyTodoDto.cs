using Quickie.Types.Writable;

namespace sample2.Todos.Dtos;

public record WriteOnlyTodoDto(string Name, string Description) : WriteOnlyDto;