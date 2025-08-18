using UnityEngine.Events;

namespace GameDevForBeginners
{
    public interface IState : IScriptableValue
    {
        public string name { get; }
        public Options options { get; }
        public Option defaultOption { get; }
        public UnityEvent<Option> onStateChanged { get; }
        public UnityEvent<IScriptableValue> onDestroy { get; }
        public Option lastActiveOption { get; }
        public Option activeOption { get; set; }
        public void Reset();
        public void SetPreviousOption(bool cycle);
        public void SetNextOption(bool cycle);
        public int activeOptionIndex { get; }
    }
}