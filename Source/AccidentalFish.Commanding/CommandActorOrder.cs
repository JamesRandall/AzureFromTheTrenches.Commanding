namespace AccidentalFish.Commanding
{
    public class CommandActorOrder
    {
        public const int Permission = 0;
        public const int Default = 1000;
        public const int Auditor = int.MaxValue - 1;
        public const int Analytic = int.MaxValue;
        public const int Pre = Default - 100;
        public const int Post = Default + 100;
    }
}
