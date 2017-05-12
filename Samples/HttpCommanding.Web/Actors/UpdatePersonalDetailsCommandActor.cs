using System.Threading.Tasks;
using AccidentalFish.Commanding;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;

namespace HttpCommanding.Web.Actors
{
    public class UpdatePersonalDetailsCommandActor : ICommandActor<UpdatePersonalDetailsCommand, UpdateResult>
    {
        public Task<UpdateResult> ExecuteAsync(UpdatePersonalDetailsCommand command, UpdateResult previousResult)
        {
            return Task.FromResult(new UpdateResult {DidUpdate = false, ValidationMessage = "Something went wrong"});
        }
    }
}
