using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    public GameObject[] podiums;

    private Inventory inventory;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        for(int i = 0; i < inventory.museumDisplays.Length; i++)
        {
            if(inventory.museumDisplays[i] != null)
            {
                podiums[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                podiums[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
