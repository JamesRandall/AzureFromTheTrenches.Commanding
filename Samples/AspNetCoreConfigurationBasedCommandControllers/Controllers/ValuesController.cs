using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreConfigurationBasedCommandControllers.Commands;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreConfigurationBasedCommandControllers.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public ValuesController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromQuery]string id)
        {
            GetPropertyValueQuery query = new GetPropertyValueQuery();
            ClaimsPrincipal claimsPrincipal = User;

            Claim claim = claimsPrincipal.FindFirst("UserId");
            query.MisspelledUserId = claim.Value;

            var result = await _commandDispatcher.DispatchAsync(query);
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] UpdatePropertyValueCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _commandDispatcher.DispatchAsync(command);
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
