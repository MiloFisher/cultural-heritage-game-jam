using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreserveManager : MonoBehaviour
{
    public GameObject preserve;
    public GameObject objective;

    public int[] preservePositions;

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
        preserve.SetActive(false);
        objective.transform.GetChild(1).GetComponent<Text>().text = "Preserve Artifacts (" + completed + "/" + preservePositions.Length + ")";
    }

    // Update is called once per frame
    void Update()
    {
        preserve.SetActive(false);
        for (int i = 0; i < preservePositions.Length; i++)
        {
            if (Mathf.Abs(player.transform.localPosition.x - preservePositions[i]) < 1.5f)
            {
                current = i;
                preserve.SetActive(true);
            }
        }
    }

    public void PreserveArtifact()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        preservePositions[current] = 999;
        preserve.SetActive(false);
        completed++;
        objective.transform.GetChild(1).GetComponent<Text>().text = "Preserve Artifacts (" + completed + "/" + preservePositions.Length + ")";
        if (completed == preservePositions.Length)
        {
            inventory.objectiveCompleted = true;
            objective.transform.GetChild(1).GetComponent<Text>().color = Color.green;
        }
    }
}
