using Quickie.DataHandlers.Crud.Definition;
using Quickie.Handlers.Crud.Implementation;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;

namespace specification.Helpers.RequestHandlers;

public class CrudForCollectionRequestHandlerConcrete(ICrudForCollectionDataHandler<CrudEntityConcrete, int> dataHandler) : CrudForCollectionRequestHandler<CrudDtoConcrete, CrudEntityConcrete, ICrudForCollectionDataHandler<CrudEntityConcrete, int>, int>(dataHandler)
{
    protected override ICollection<CrudDtoConcrete> MapToCollectionOfDto(ICollection<CrudEntityConcrete> entity)
    {
        return entity.Select(e => new CrudDtoConcrete()).ToList();
    }

    protected override ICollection<CrudEntityConcrete> MapToCollectionOfEntity(ICollection<CrudDtoConcrete> entity)
    {
        return entity.Select(e => new CrudEntityConcrete()).ToList();
    }
}