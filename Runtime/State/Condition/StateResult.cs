namespace GameDevForBeginners
{
    public enum StateResultType
    {
        False,
        True,
        Error
    }

    public struct StateResult
    {
        public StateResultType resultType;
        public string errorMessage;

        public StateResult(StateResultType resultType, string errorMessage)
        {
            this.resultType = resultType;
            this.errorMessage = errorMessage;
        }
    }
}