using Microsoft.AspNetCore.Mvc;
using Quickie.Apis.Editonly;
using sample2.Todos.Dtos;
using sample2.Todos.Services.EditOnly;

namespace sample2.Todos.Controllers;

public class EditOnlyTodoController(IEditOnlyTodoReqHandler requestHandler)
    : EditOnlyController<PastTodo_EditOnlyDto, IEditOnlyTodoReqHandler, string>(requestHandler)
{
    [HttpGet("items")]
    public async Task<IActionResult> GetAllAsync()
    {
        var data = await requestHandler.GetAllAsync();
        return Ok(data);
    }
}