using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject slot;
    public ResearchManager researchManager;
    public int id;

    private bool dragAllowed;
    private Transform artifacts;
    private Transform selected;

    private void Start()
    {
        artifacts = GameObject.FindGameObjectWithTag("Artifacts").transform;
        selected = GameObject.FindGameObjectWithTag("Selected").transform;
    }

    public void SetValues(GameObject _slot, ResearchManager _researchManager, int _id, Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;
        slot = _slot;
        researchManager = _researchManager;
        id = _id;
        StartCoroutine(DelaySetPosition());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragAllowed = id < 30 || researchManager.inventory.items[id].startTime == 0;
        if (dragAllowed)
        {
            HideInfo();
            transform.SetParent(selected);
            transform.position = eventData.position;
        }    
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragAllowed)
            transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragAllowed)
        {
            transform.position = slot.transform.position;
            return;
        }
        HideInfo();
        transform.SetParent(artifacts);
        // Check if over slot
        for (int i = 0; i < 30; i++)
        {
            if(Vector3.Distance(eventData.position, researchManager.slots[i].transform.position) <= 60)
            {
                Swap(i);
                return;
            }
        }
        // Check if over research slot
        for (int i = 0; i < 5; i++)
        {
            if (Vector3.Distance(eventData.position, researchManager.researchSlots[i].transform.position) <= 60)
            {
                ResearchSwap(i);
                return;
            }
        }
        // Otherwise return to original position
        transform.position = slot.transform.position;
    }

    public void DisplayInfo()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        transform.SetAsLastSibling();
        GameObject display = transform.GetChild(0).gameObject;
        display.SetActive(true);
        float x, y;
        if (id >= 30)
        {
            x = 0;
        }
        else if (id % 10 > 4)
        {
            x = -200;
        }
        else
        {
            x = 200;
        }
        if (id >= 30)
        {
            y = -270;
        }
        else if (id < 10)
        {
            y = -135;
        }
        else if (id >= 10 && id < 20)
        {
            y = 0;
        }
        else
        {
            y = 135;
        }
        display.transform.localPosition = new Vector3(x, y);
        Text displayName = display.transform.GetChild(0).GetComponent<Text>();
        Image displayImage = display.transform.GetChild(1).GetComponent<Image>();
        Text displayDescription = display.transform.GetChild(3).GetComponent<Text>();
        displayName.text = researchManager.inventory.items[id].itemName;
        displayImage.sprite = GetComponent<Image>().sprite;
        displayDescription.text = researchManager.inventory.items[id].description;
        // Display send to museum button only if item is already researched
        display.transform.GetChild(5).gameObject.SetActive(researchManager.inventory.items[id].alreadyResearched);
    }

    public void HideInfo()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Swap(int i)
    {
        int a = id;
        int b = i;

        bool comingFromResearch = a >= 30; // 0-29 is inventory, 30-34 is research

        GameObject temp = researchManager.draggableItems[b];
        if (temp)
        {
            // Return if swapping an item from research with an already researched item
            if (comingFromResearch && researchManager.inventory.items[b].alreadyResearched)
            {
                transform.position = slot.transform.position;
                return;
            }

            // Change id, position, and slot
            temp.GetComponent<ItemManager>().id = a;
            temp.transform.position = slot.transform.position;
            temp.GetComponent<ItemManager>().slot = slot;
        }

        // Change id, position, and slot
        id = b;
        transform.position = researchManager.slots[b].transform.position;
        slot = researchManager.slots[b];

        // Swap places in dragableItems array
        researchManager.draggableItems[b] = gameObject;
        researchManager.draggableItems[a] = temp;

        // Swap places in inventory
        researchManager.inventory.SwapItems(a, b);
    }

    private void ResearchSwap(int i)
    {
        int a = id;
        int b = i + 30;

        // Return if item is already researched
        if (researchManager.inventory.items[a].alreadyResearched)
        {
            transform.position = slot.transform.position;
            return;
        }

        GameObject temp = researchManager.draggableItems[b];
        if (temp)
        {
            // Return if item in research is still being researched
            if (researchManager.inventory.items[b].researchTimer > 0)
            {
                transform.position = slot.transform.position;
                return;
            }

            // Change id, position, and slot
            temp.GetComponent<ItemManager>().id = a;
            temp.transform.position = slot.transform.position;
            temp.GetComponent<ItemManager>().slot = slot;
        }

        // Change id, position, and slot
        id = b;
        transform.position = researchManager.researchSlots[i].transform.position;
        slot = researchManager.researchSlots[i];

        // Swap places in dragableItems array
        researchManager.draggableItems[b] = gameObject;
        researchManager.draggableItems[a] = temp;

        // Swap places in inventory
        researchManager.inventory.SwapItems(a, b);
    }

    IEnumerator DelaySetPosition()
    {
        yield return new WaitForEndOfFrame();
        transform.position = slot.transform.position;
    }

    public void SendToMuseum()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        researchManager.inventory.SendToMuseum(id);
        researchManager.RemoveDraggableItem(id);
    }
}
