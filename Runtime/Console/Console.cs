using UnityEngine;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Console/Console")]
    public class Console : MonoBehaviour
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}