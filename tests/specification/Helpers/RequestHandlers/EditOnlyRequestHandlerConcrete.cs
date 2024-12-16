using Quickie.DataHandlers.Editonly.Definition;
using Quickie.Handlers.Editonly.Implementation;
using specification.Helpers.Dtos;
using specification.Helpers.Entities;

namespace specification.Helpers.RequestHandlers;
public class EditOnlyRequestHandlerConcrete(IEditOnlyDataHandler<EditOnlyEntityConcrete, int> dataHandler)
    : EditOnlyRequestHandler<EditOnlyDtoConcrete, EditOnlyEntityConcrete,
        IEditOnlyDataHandler<EditOnlyEntityConcrete, int>, int>(dataHandler)
{
    protected override EditOnlyEntityConcrete MapToEntity(EditOnlyDtoConcrete requestModel) =>
        new EditOnlyEntityConcrete();

    protected override EditOnlyDtoConcrete MapToDto(EditOnlyEntityConcrete entity) => new EditOnlyDtoConcrete();

    protected override ICollection<EditOnlyEntityConcrete> MapToCollectionOfEntity(ICollection<EditOnlyDtoConcrete> requestModels) => new List<EditOnlyEntityConcrete>();

    protected override ICollection<EditOnlyDtoConcrete> MapToCollectionOfDto(ICollection<EditOnlyEntityConcrete> entities) => new List<EditOnlyDtoConcrete>();
}