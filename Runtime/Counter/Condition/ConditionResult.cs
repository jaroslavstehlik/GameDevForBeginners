namespace GameDevForBeginners
{
    public enum ContitionResultType
    {
        False,
        True,
        Error
    }

    public struct ConditionResult
    {
        public ContitionResultType resultType;
        public string errorMessage;

        public ConditionResult(ContitionResultType resultType, string errorMessage)
        {
            this.resultType = resultType;
            this.errorMessage = errorMessage;
        }
    }
}