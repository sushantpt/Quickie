using Quickie.DataHandlers.Crud.Definition;
using Quickie.Handlers.Crud.Implementation;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;

namespace specification.Helpers.RequestHandlers;

public class CrudRequestHandlerConcrete(ICrudDataHandler<CrudEntityConcrete, int> dataHandler) : CrudRequestHandler<CrudDtoConcrete, CrudEntityConcrete, ICrudDataHandler<CrudEntityConcrete, int>, int>(dataHandler)
{
    protected override CrudEntityConcrete MapToEntity(CrudDtoConcrete request)
    {
        return new CrudEntityConcrete();
    }

    protected override CrudDtoConcrete MapToDto(CrudEntityConcrete request)
    {
        return new CrudDtoConcrete();
    }
}