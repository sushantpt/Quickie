using System.Linq.Expressions;
using Quickie.DataHandlers.Readonly.Definition;
using Quickie.DataObj;
using Quickie.Handlers.Readonly.Implementation;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;

namespace specification.Helpers.RequestHandlers;

public class ReadOnlyCollectionRequestHandlerConcrete(IReadOnlyCollectionDataHandler<ReadOnlyEntityConcrete, int> dataHandler) : ReadOnlyCollectionRequestHandler<ReadOnlyDtoConcrete, ReadOnlyEntityConcrete, IReadOnlyCollectionDataHandler<ReadOnlyEntityConcrete, int>, int>(dataHandler)
{
    protected override ICollection<ReadOnlyDtoConcrete> MapToDto(ICollection<ReadOnlyEntityConcrete> entity) => [];
}