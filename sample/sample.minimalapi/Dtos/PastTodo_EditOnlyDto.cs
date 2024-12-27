using Quickie.Types.Editonly;

namespace sample.minimalapi.Dtos;

public record PastTodo_EditOnlyDto(string Id, string Name, string Description, DateTime CreateDt) : EditOnlyDto;