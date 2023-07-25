using Microsoft.AspNetCore.Mvc;

namespace StackOverflow.Answers.AspNet.WebApi.ChangeFieldValueBeforeReachingController;


[ApiController]
[Route("v1/[controller]/[action]")]
public class ChangeFieldValueController : Controller
{
    [HttpPost]
    public async Task<IActionResult> SomeAction([FromBody] SomeRequestModel request)
    {
        return Ok(request.SomeId);
    }
}
