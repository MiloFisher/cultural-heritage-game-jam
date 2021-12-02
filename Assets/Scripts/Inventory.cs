using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] items;
    public Item[] museumDisplays;
    public bool[] completedLevels;
    public int coins;
    public int pickaxeLevel;
    public int pickaxeAttack;
    public int shovelLevel;
    public int shovelSpeed;
    public int flaskLevel;
    public int flaskHealth;
    public int dogLevel;
    public int dogAttack;
    public Sprite[] unidentifiedSprites;
    public Sprite[] specificArtifactSprites;
    public GameObject itemDrop;
    public int globalLevel;
    public bool alreadyShowedHelp;
    public bool objectiveCompleted;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        globalLevel = 1;
        items = new Item[35];
        museumDisplays = new Item[5];
        completedLevels = new bool[12];
    }

    private void Update()
    {
        foreach(Item item in items)
        {
            if(item != null && item.startTime > 0)
            {
                item.researchTimer = item.researchDuration - (Time.time - item.startTime);
                if(item.researchTimer <= 0)
                {
                    StopResearching(item);
                }
            }
        }
    }

    public void AddItem(Item i)
    {
        Item item = i;
        int id = FindFirstEmptySlot();
        if(id == -1)
        {
            Debug.Log("Inventory Full");
            return;
        }
        items[id] = item;
    }

    public void SwapItems(int a, int b)
    {
        Item temp = items[a];
        items[a] = items[b];
        items[b] = temp;
    }

    public void SendToMuseum(int id)
    {
        coins += items[id].value;
        int position = -1;
        switch (items[id].itemName)
        {
            case "Birds Pecking Grape Vines Bowl": position = 0; break;
            case "Sculpture of Atargatis": position = 1; break;
            case "Eagle Sculpture": position = 2; break;
            case "Betyl": position = 3; break;
            case "Sculpture Relief of a Helmet": position = 4; break;
        }
        if (position > -1)
            museumDisplays[position] = items[id];
        items[id] = null;
    }

    public void StartResearching(int id)
    {
        items[id].startTime = Time.time;
    }

    public void DropItem(Vector3 position, Vector3 scale, int sortingOrder)
    {
        Transform parent = GameObject.FindGameObjectWithTag("Items").transform;
        Item item = GetItem(-1);
        Instantiate(itemDrop, parent).GetComponent<ItemDrop>().AssignItem(item, position, scale, sortingOrder);
    }

    public void GetSpecialArtifact(int id)
    {
        AddItem(GetItem(id));
    }

    private Item GetItem(int type)
    {
        return type switch
        {
            0 => new Item("Medallion", unidentifiedSprites[type], 0, 0, 5 * 60, false, "-Unidentified-\nResearch Time:\n5 minutes", 200),
            1 => new Item("Chalice", unidentifiedSprites[type], 0, 0, 4.5f * 60, false, "-Unidentified-\nResearch Time:\n4.5 minutes", 150),
            2 => new Item("Vase", unidentifiedSprites[type], 0, 0, 4 * 60, false, "-Unidentified-\nResearch Time:\n4 minutes", 100),
            3 => new Item("Cup", unidentifiedSprites[type], 0, 0, 3.5f * 60, false, "-Unidentified-\nResearch Time:\n3.5 minutes", 85),
            4 => new Item("Coins", unidentifiedSprites[type], 0, 0, 3 * 60, false, "-Unidentified-\nResearch Time:\n3 minutes", 65),
            5 => new Item("Bones", unidentifiedSprites[type], 0, 0, 2.5f * 60, false, "-Unidentified-\nResearch Time:\n2.5 minutes", 45),
            6 => new Item("Pot", unidentifiedSprites[type], 0, 0, 2 * 60, false, "-Unidentified-\nResearch Time:\n2 minutes", 25),
            7 => new Item("Birds Pecking Grape Vines Bowl", specificArtifactSprites[type - 7], 0, 0, 10 * 60, false, "-Unidentified-\nResearch Time:\n10 minutes", 500),
            8 => new Item("Sculpture of Atargatis", specificArtifactSprites[type - 7], 0, 0, 10 * 60, false, "-Unidentified-\nResearch Time:\n10 minutes", 500),
            9 => new Item("Eagle Sculpture", specificArtifactSprites[type - 7], 0, 0, 10 * 60, false, "-Unidentified-\nResearch Time:\n10 minutes", 500),
            10 => new Item("Betyl", specificArtifactSprites[type - 7], 0, 0, 10 * 60, false, "-Unidentified-\nResearch Time:\n10 minutes", 500),
            11 => new Item("Sculpture Relief of a Helmet", specificArtifactSprites[type - 7], 0, 0, 10 * 60, false, "-Unidentified-\nResearch Time:\n10 minutes", 500),
            _ => GetItem(Random.Range(0, 7)), // Range for basic drops 0 - 6
        };
    }

    private void StopResearching(Item item)
    {
        item.startTime = 0;
        item.researchDuration = 0;
        item.alreadyResearched = true;
        if(item.value >= 500)
            item.description = "-Identified-\nSend this item to\nthe museum to\nhave it displayed";
        else
            item.description = "-Identified-\nMuseum Reward:\n" + item.value + " coins";
    }

    private int FindFirstEmptySlot()
    {
        for(int i = 0; i < 30; i++)
        {
            if (items[i] == null)
                return i;
        }
        return -1;
    }
}

public class Item
{
    public string itemName;
    public Sprite sprite;
    public float researchTimer;
    public float startTime;
    public float researchDuration;
    public bool alreadyResearched;
    public string description;
    public int researchTime;
    public int value;

    public Item(string _itemName, Sprite _sprite, float _researchTimer, float _startTime, float _researchDuration, bool _alreadyResearched, string _description, int _value)
    {
        itemName = _itemName;
        sprite = _sprite;
        researchTimer = _researchTimer;
        startTime = _startTime;
        researchDuration = _researchDuration;
        alreadyResearched = _alreadyResearched;
        description = _description;
        value = _value;
    }
}
