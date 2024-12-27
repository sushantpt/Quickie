using Quickie.Types.Writable;

namespace sample.minimalapi.Dtos;

public record WriteOnlyTodoDto(string Name, string Description) : WriteOnlyDto;