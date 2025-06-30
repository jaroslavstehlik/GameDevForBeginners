namespace GameDevForBeginners
{
    public enum CalculatorResultType
    {
        Value,
        Error
    }

    public struct CalculatorResult
    {
        public CalculatorResultType resultType;
        public string errorMessage;
        public float value;

        public CalculatorResult(CalculatorResultType resultType, float value, string errorMessage)
        {
            this.resultType = resultType;
            this.value = value;
            this.errorMessage = errorMessage;
        }
    }
}