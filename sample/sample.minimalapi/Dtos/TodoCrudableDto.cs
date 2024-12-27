using Quickie.Types.Crud;

namespace sample.minimalapi.Dtos;

public record TodoCrudableDto(int Id, string Title, string Description) : CrudDto;