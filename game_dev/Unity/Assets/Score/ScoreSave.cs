using UnityEngine;
using UnityEngine.Events;

public class ScoreSave : MonoBehaviour
{
    // public event
    public UnityEvent<int> onScoreChanged;
    
    // score variable
    [SerializeField] int score = 0;

    // The key to our score, it has to be unique per whole game.
    public string scoreKey = "myScoreKey";
    
    // Load score when our component is enabled
    void OnEnable()
    {
        // Check if any score has been saved before
        if (PlayerPrefs.HasKey(scoreKey))
        {
            // Load the score in to our variable
            Set(PlayerPrefs.GetInt(scoreKey));
        }
    }

    // Save score when our component is disabled
    private void OnDisable()
    {
        PlayerPrefs.SetInt(scoreKey, score);
    }

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
