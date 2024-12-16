using Quickie.DataHandlers.Readonly.Definition;
using Quickie.Handlers.Readonly.Implementation;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;

namespace specification.Helpers.RequestHandlers;

public class ReadOnlyRequestHandlerConcrete(IReadOnlyDataHandler<ReadOnlyEntityConcrete, int> dataHandler) : ReadOnlyRequestHandler<ReadOnlyDtoConcrete, ReadOnlyEntityConcrete, IReadOnlyDataHandler<ReadOnlyEntityConcrete, int>, int>(dataHandler)
{
    protected override ReadOnlyDtoConcrete MapToDto(ReadOnlyEntityConcrete entity) => new ReadOnlyDtoConcrete();
}