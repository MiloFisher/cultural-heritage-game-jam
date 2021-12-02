using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject letter;
    public GameObject helpInfo;
    public GameObject inventory;

    void Start()
    {
        if(!GameObject.FindGameObjectWithTag("Inventory"))
        {
            Instantiate(inventory);
            titleScreen.SetActive(true);
            letter.SetActive(true);
            helpInfo.SetActive(true);
        }
    }

    public void SetHelpInfoActive()
    {
        helpInfo.SetActive(!helpInfo.activeInHierarchy);
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
    }

    public void HideTitleScreen()
    {
        titleScreen.SetActive(false);
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
    }

    public void HideLetter()
    {
        letter.SetActive(false);
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
    }
}
