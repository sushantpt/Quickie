using Quickie.Types.Readonly;

namespace sample2.Todos.Dtos;

public record TodoDto(string Id,string Name, string Description, DateTime CreateDt) : ReadOnlyDto;