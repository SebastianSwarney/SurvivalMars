using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public static KillPlayer Instance;
    public int m_nextSceneIndex;
    private void Awake()
    {
        Instance = this;
    }
    public void KillPlayerFunction()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(m_nextSceneIndex);
    }
}
