using UnityEngine;
using UnityEngine.Events;

public class ScoreGameObject : MonoBehaviour
{
    // public event
    public UnityEvent<int> onScoreChanged;
    
    // score variable
    [SerializeField] int score = 0;

    // Method for reading score
    public int Get()
    {
        return score;
    }

    // Method for writing score
    public void Set(int value)
    {
        score = value;
        if (onScoreChanged != null)
            onScoreChanged.Invoke(score);
    }
    
    // Method for adding score
    public void Add(int value)
    {
        score += value;
        if (onScoreChanged != null)
            onScoreChanged.Invoke(score);
    }

    // Method for subtracting score
    public void Subtract(int value)
    {
        score -= value;
        if (onScoreChanged != null)
            onScoreChanged.Invoke(score);
    }
}
