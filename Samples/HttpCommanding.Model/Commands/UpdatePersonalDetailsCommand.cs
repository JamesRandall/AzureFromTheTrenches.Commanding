using System;

namespace HttpCommanding.Model.Commands
{
    public class UpdatePersonalDetailsCommand
    {
        public Guid Id { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public int Age { get; set; }
    }
}
