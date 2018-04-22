namespace SampleFunctionConfiguration.Services.Implementation
{
    internal class Calculator : ICalculator
    {
        public int Add(int valueOne, int valueTwo)
        {
            return valueOne + valueTwo;
        }
    }
}
