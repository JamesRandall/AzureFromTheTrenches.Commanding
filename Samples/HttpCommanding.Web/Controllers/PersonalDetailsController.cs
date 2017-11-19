using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;
using Microsoft.AspNetCore.Mvc;

namespace HttpCommanding.Web.Controllers
{
    [Route("api/[controller]")]
    public class PersonalDetailsController : Controller
    {
        private readonly ICommandExecuter _commandExecuter;

        public PersonalDetailsController(ICommandExecuter commandExecuter)
        {
            _commandExecuter = commandExecuter;
        }

        [HttpGet]
        public string Get()
        {
            return "hello";
        }

        [HttpPut]
        public async Task<UpdateResult> Put([FromBody]UpdatePersonalDetailsCommand command)
        {
            UpdateResult result = await _commandExecuter.ExecuteAsync(command);
            return result;
        }        
    }
}
