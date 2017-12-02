using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;
using Microsoft.AspNetCore.Mvc;

namespace HttpCommanding.Web.Controllers
{
    [Route("api/[controller]")]
    public class PersonalDetailsController : Controller
    {
        private readonly IDirectCommandExecuter _commandExecuter;

        public PersonalDetailsController(IDirectCommandExecuter commandExecuter)
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
