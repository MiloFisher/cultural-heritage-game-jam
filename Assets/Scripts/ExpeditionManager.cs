using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionManager : MonoBehaviour
{
    public SceneChanger sceneChanger;
    public GameObject focusedExpedition;
    public GameObject[] expeditions;

    private Text focusedTitle;
    private Text focusedText;
    private int selectedExpedition;

    private Inventory inventory;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        focusedTitle = focusedExpedition.transform.GetChild(0).GetComponent<Text>();
        focusedText = focusedExpedition.transform.GetChild(1).GetComponent<Text>();
        focusedExpedition.SetActive(false);
        for(int i = 0; i < expeditions.Length; i++)
        {
            expeditions[i].transform.GetChild(1).gameObject.SetActive(!inventory.completedLevels[i]);
            expeditions[i].transform.GetChild(3).gameObject.SetActive(inventory.completedLevels[i]);
            expeditions[i].transform.GetChild(2).GetComponent<Image>().color = inventory.completedLevels[i] ? Color.green : Color.red;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RemoveFocus();
        }
    }

    public void FocusExpedition(int id)
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        selectedExpedition = id;
        focusedExpedition.SetActive(true);
        Text title = expeditions[id].transform.GetChild(0).GetComponent<Text>();
        Text text;
        if(!inventory.completedLevels[id])
            text = expeditions[id].transform.GetChild(1).GetComponent<Text>();
        else
            text = expeditions[id].transform.GetChild(3).GetComponent<Text>();
        focusedTitle.text = title.text;
        focusedText.text = text.text;
        focusedExpedition.transform.GetChild(2).GetComponent<Image>().color = expeditions[id].transform.GetChild(2).GetComponent<Image>().color;
        focusedExpedition.transform.GetChild(3).gameObject.SetActive(!inventory.completedLevels[id]);
    }

    public void RemoveFocus()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        focusedExpedition.SetActive(false);
    }

    public void Embark()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        sceneChanger.ChangeScene("Level " + selectedExpedition);
    }
}
