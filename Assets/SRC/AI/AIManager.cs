using UnityEngine;

public class AIManager : MonoBehaviour
{

    public AIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.gameObject.SetActive(false);
        }
        else if (instance != this)
        {
            Debug.Log("SAMEINSTACE ");
            Destroy(this);
        }
    }
}