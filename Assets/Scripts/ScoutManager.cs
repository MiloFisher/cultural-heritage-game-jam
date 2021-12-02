using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoutManager : MonoBehaviour
{
    public GameObject scout;
    public GameObject objective;

    public int[] scoutPositions;

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
        scout.SetActive(false);
        objective.transform.GetChild(1).GetComponent<Text>().text = "Scout Sites (" + completed + "/" + scoutPositions.Length + ")";
    }

    // Update is called once per frame
    void Update()
    {
        scout.SetActive(false);
        for (int i = 0; i < scoutPositions.Length; i++)
        {
            if (Mathf.Abs(player.transform.localPosition.x - scoutPositions[i]) < 1.5f)
            {
                current = i;
                scout.SetActive(true);
            }
        }
    }

    public void ScoutSite()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        scoutPositions[current] = 999;
        scout.SetActive(false);
        completed++;
        objective.transform.GetChild(1).GetComponent<Text>().text = "Scout Sites (" + completed + "/" + scoutPositions.Length + ")";
        if (completed == scoutPositions.Length)
        {
            inventory.objectiveCompleted = true;
            objective.transform.GetChild(1).GetComponent<Text>().color = Color.green;
        }
    }
}
