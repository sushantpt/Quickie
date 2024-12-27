using Quickie.Types.Crud;

namespace sample.idempotent.redis.Dtos;

public record TodoCrudableDto(int Id, string Title, string Description) : CrudDto;