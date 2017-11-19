using System;
using AccidentalFish.Commanding.Abstractions;
using HttpCommanding.Model.Results;

namespace HttpCommanding.Model.Commands
{
    public class UpdatePersonalDetailsCommand : ICommand<UpdateResult>
    {
        public Guid Id { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public int Age { get; set; }
    }
}
