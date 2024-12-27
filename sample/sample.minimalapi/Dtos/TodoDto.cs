using Quickie.Types.Readonly;

namespace sample.minimalapi.Dtos;

public record TodoDto(string Id,string Name, string Description, DateTime CreateDt) : ReadOnlyDto;