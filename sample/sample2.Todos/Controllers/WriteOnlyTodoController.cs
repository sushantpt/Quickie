using Microsoft.AspNetCore.Mvc;
using Quickie.Apis.Writeonly;
using sample2.Todos.Dtos;
using sample2.Todos.Services.WriteOnly;

namespace sample2.Todos.Controllers;

public class WriteOnlyTodoController(IWriteOnlyTodoReqHandler requestHandler) : WriteOnlyController<WriteOnlyTodoDto, IWriteOnlyTodoReqHandler>(requestHandler)
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var data = await requestHandler.GetAllAsync();
        return Ok(data);
    }
}