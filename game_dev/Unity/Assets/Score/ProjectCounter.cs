using UnityEngine;
using UnityEngine.Events;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Score", menuName = "GMD/Score", order = 1)]

// Scriptable object can be stored only in project
// it can be referenced in scene
// it is used mostly for holding game data
public class ProjectScore : ScriptableObject
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