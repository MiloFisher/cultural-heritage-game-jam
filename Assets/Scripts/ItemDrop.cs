using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item item;

    public void AssignItem(Item i, Vector3 position, Vector3 scale, int sortingOrder)
    {
        item = i;
        transform.position = position;
        transform.localScale = 0.2f * scale;
        GetComponent<SpriteRenderer>().sprite = i.sprite;
        GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
    }
}
