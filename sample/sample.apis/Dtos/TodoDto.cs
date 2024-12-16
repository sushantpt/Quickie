using Quickie.Types.Crud;

namespace sample.apis.Dtos;

public record TodoDto(int Id, string Title, string Description) : CrudDto;