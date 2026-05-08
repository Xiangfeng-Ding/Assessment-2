using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{

    private SceneTrigger currentScene;

    public GameObject[] levels;

    public Transform[] respawPoints;
    private Transform targetRespanPoint;
    private void Start()
    {
        currentScene = TimeScoreManager.Instance.currentSceneTrigger; 

    }

    private void Update()
    {
        foreach (var a in levels)
            if (a.gameObject.activeSelf)
            {
                int index = System.Array.IndexOf(levels, a);
                targetRespanPoint = respawPoints[index];

            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager.Instance.TeleportToRespawnPoint(targetRespanPoint);
        }
    }
}