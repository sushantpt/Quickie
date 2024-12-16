using Quickie.Types.Editonly;

namespace sample2.Todos.Dtos;

public record PastTodo_EditOnlyDto(string Id, string Name, string Description, DateTime CreateDt) : EditOnlyDto;