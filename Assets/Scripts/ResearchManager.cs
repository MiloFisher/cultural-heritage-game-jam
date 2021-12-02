using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
    public GameObject[] slots;
    public GameObject[] researchSlots;
    public GameObject[] timers;
    public GameObject artifacts;
    public GameObject[] draggableItems;
    public Inventory inventory;
    public GameObject draggableItemPrefab;

    void Start()
    {
        draggableItems = new GameObject[35];
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        for(int i = 0; i < 30; i++)
        {
            if(inventory.items[i] != null)
            {
                draggableItems[i] = Instantiate(draggableItemPrefab, slots[i].transform.position, Quaternion.identity, artifacts.transform);
                draggableItems[i].GetComponent<ItemManager>().SetValues(slots[i], this, i, inventory.items[i].sprite);
            }    
        }
        for (int i = 30; i < 35; i++)
        {
            if (inventory.items[i] != null)
            {
                draggableItems[i] = Instantiate(draggableItemPrefab, researchSlots[i - 30].transform.position, Quaternion.identity, artifacts.transform);
                draggableItems[i].GetComponent<ItemManager>().SetValues(researchSlots[i - 30], this, i, inventory.items[i].sprite);
            }
        }
    }

    void Update()
    {
        for(int i = 30; i < 35; i++)
        {
            if(inventory.items[i] != null)
            {
                DisplayResearchItem(i);
            }
            else
            {
                ClearResearchItem(i);
            }
        }
    }

    public void RemoveDraggableItem(int id)
    {
        Destroy(draggableItems[id]);
        draggableItems[id] = null;
    }

    private void DisplayResearchItem(int id)
    {
        Transform rs = researchSlots[id - 30].transform;
        GameObject button = rs.GetChild(2).gameObject;
        GameObject complete = rs.GetChild(3).gameObject;
        GameObject timer = timers[id - 30];

        complete.SetActive(inventory.items[id].alreadyResearched);
        timer.SetActive(inventory.items[id].researchTimer > 0);
        if(timer.activeInHierarchy)
        {
            timer.GetComponent<Text>().text = FormatTime(inventory.items[id].researchTimer); 
        }
        button.SetActive(!timer.activeInHierarchy && !complete.activeInHierarchy);
        if (button.activeInHierarchy)
        {
            button.GetComponent<Button>().onClick.AddListener(() => inventory.StartResearching(id));
        }
    }

    private void ClearResearchItem(int id)
    {
        Transform rs = researchSlots[id - 30].transform;
        GameObject button = rs.GetChild(2).gameObject;
        GameObject complete = rs.GetChild(3).gameObject;
        GameObject timer = timers[id - 30];
        
        button.SetActive(false);
        timer.SetActive(false);
        complete.SetActive(false);
    }

    private string FormatTime(float time)
    {
        string minutes = "" + ((int)time / 60 % 60);
        string seconds = "" + ((int)time % 60);
        if(seconds.Length == 1)
        {
            seconds = "0" + seconds;
        }
        return minutes + ":" + seconds;
    }
}
