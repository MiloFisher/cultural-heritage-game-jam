using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Podium : MonoBehaviour
{
    public GameObject description;
    public GameObject[] descriptions;

    // Start is called before the first frame update
    void Start()
    {
        description.SetActive(false);
    }

    public void DisplayMessage()
    {
        foreach (GameObject g in descriptions)
            g.SetActive(false);
        description.SetActive(true);
        if (description.activeInHierarchy)
            GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
    }
}
