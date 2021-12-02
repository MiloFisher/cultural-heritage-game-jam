using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectArtifactManager : MonoBehaviour
{
    public int missionId;
    public int victoryArtifact;
    public GameObject artifact;
    public GameObject objective;

    public int[] artifactPositions;

    private Inventory inventory;
    private GameObject player;
    private int current;
    private int completed;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player");
        inventory.objectiveCompleted = false;
        artifact.SetActive(false);
        objective.transform.GetChild(1).GetComponent<Text>().text = "Find Artifact (" + completed + "/" + artifactPositions.Length + ")";
    }

    // Update is called once per frame
    void Update()
    {
        artifact.SetActive(false);
        for (int i = 0; i < artifactPositions.Length; i++)
        {
            if (Mathf.Abs(player.transform.localPosition.x - artifactPositions[i]) < 1.5f)
            {
                current = i;
                artifact.SetActive(true);
            }
        }
    }

    public void CollectArtifact()
    {
        artifactPositions[current] = 999;
        artifact.SetActive(false);
        completed++;
        objective.transform.GetChild(1).GetComponent<Text>().text = "Find Artifact (" + completed + "/" + artifactPositions.Length + ")";
        if (completed == artifactPositions.Length)
        {
            inventory.objectiveCompleted = true;
            objective.transform.GetChild(1).GetComponent<Text>().color = Color.green;
            inventory.GetSpecialArtifact(victoryArtifact);
            inventory.completedLevels[missionId] = true;
            inventory.globalLevel++;
            GameObject.FindGameObjectWithTag("Success").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("Scene Changer").GetComponent<SceneChanger>().ChangeScene("Town");
        }
    }
}
