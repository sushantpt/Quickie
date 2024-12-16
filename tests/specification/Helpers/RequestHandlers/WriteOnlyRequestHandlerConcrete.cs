using Quickie.DataHandlers.Writeonly.Definition;
using Quickie.Handlers.Writeonly.Implementation;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;

namespace specification.Helpers.RequestHandlers;

public class WriteOnlyRequestHandlerConcrete(IWriteOnlyDataHandler<WriteOnlyEntityConcrete> dataHandler) : WriteOnlyRequestHandler<WriteOnlyDtoConcrete, WriteOnlyEntityConcrete, IWriteOnlyDataHandler<WriteOnlyEntityConcrete>>(dataHandler)
{
    protected override WriteOnlyEntityConcrete MapToEntity(WriteOnlyDtoConcrete requestModel)
    {
        return new WriteOnlyEntityConcrete();
    }

    protected override WriteOnlyDtoConcrete MapToDto(WriteOnlyEntityConcrete entity)
    {
        return new WriteOnlyDtoConcrete();
    }

    protected override ICollection<WriteOnlyEntityConcrete> MapToCollectionOfEntity(ICollection<WriteOnlyDtoConcrete> requestModels)
    {
        return requestModels.Select(dto => new WriteOnlyEntityConcrete()).ToList();
    }

    protected override ICollection<WriteOnlyDtoConcrete> MapToCollectionOfDto(ICollection<WriteOnlyEntityConcrete> entities)
    {
        return entities.Select(entity => new WriteOnlyDtoConcrete()).ToList();
    }
}