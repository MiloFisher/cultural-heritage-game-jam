using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StabilizeManager : MonoBehaviour
{
    public GameObject stabilize;
    public GameObject objective;

    public int[] stabilizePositions;

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
        stabilize.SetActive(false);
        objective.transform.GetChild(1).GetComponent<Text>().text = "Stabilize Sites (" + completed + "/" + stabilizePositions.Length + ")";
    }

    // Update is called once per frame
    void Update()
    {
        stabilize.SetActive(false);
        for (int i = 0; i < stabilizePositions.Length; i++)
        {
            if (Mathf.Abs(player.transform.localPosition.x - stabilizePositions[i]) < 1.5f)
            {
                current = i;
                stabilize.SetActive(true);
            }
        }
    }

    public void StabilizeSite()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        stabilizePositions[current] = 999;
        stabilize.SetActive(false);
        completed++;
        objective.transform.GetChild(1).GetComponent<Text>().text = "Stabilize Sites (" + completed + "/" + stabilizePositions.Length + ")";
        if (completed == stabilizePositions.Length)
        {
            inventory.objectiveCompleted = true;
            objective.transform.GetChild(1).GetComponent<Text>().color = Color.green;
        }
    }
}
