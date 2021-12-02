using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillManager : MonoBehaviour
{
    public GameObject objective;

    private Inventory inventory;
    private GameObject player;

    private int length;
    private int startLength;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player");
        inventory.objectiveCompleted = false;
        startLength = GameObject.FindGameObjectsWithTag("Enemy").Length;
        objective.transform.GetChild(1).GetComponent<Text>().text = "Defeat Enemies (" + 0 + "/" + startLength + ")";
    }

    // Update is called once per frame
    void Update()
    {
        length = GameObject.FindGameObjectsWithTag("Enemy").Length;
        objective.transform.GetChild(1).GetComponent<Text>().text = "Defeat Enemies (" + (startLength - length) + "/" + startLength + ")";
        if (length == 0)
        {
            inventory.objectiveCompleted = true;
            objective.transform.GetChild(1).GetComponent<Text>().color = Color.green;
        }
    }
}
