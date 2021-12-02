using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public GameObject pickaxe;
    public GameObject shovel;
    public GameObject flask;
    public GameObject dog;
    public Text coins;
    public Sprite greenButton;
    public Sprite redButton;

    private Inventory inventory;
    private const int upgradeCost = 100;
    private const int pickaxeUpgradeAmount = 1;
    private const int shovelUpgradeAmount = 2;
    private const int flaskUpgradeAmount = 10;
    private const int dogUpgradeAmount = 1;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    private void Update()
    {
        coins.text = "Coins: " + inventory.coins;
        UpdatePickaxe();
        UpdateShovel();
        UpdateFlask();
        UpdateDog();
    }

    public void UpgradePickaxe()
    {
        if (inventory.coins < inventory.pickaxeLevel * upgradeCost)
            return;
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        inventory.coins -= inventory.pickaxeLevel * upgradeCost;
        inventory.pickaxeLevel++;
        inventory.pickaxeAttack += pickaxeUpgradeAmount;
    }

    public void UpgradeShovel()
    {
        if (inventory.coins < inventory.shovelLevel * upgradeCost)
            return;
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        inventory.coins -= inventory.shovelLevel * upgradeCost;
        inventory.shovelLevel++;
        inventory.shovelSpeed += shovelUpgradeAmount;
    }

    public void UpgradeFlask()
    {
        if (inventory.coins < inventory.flaskLevel * upgradeCost)
            return;
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        inventory.coins -= inventory.flaskLevel * upgradeCost;
        inventory.flaskLevel++;
        inventory.flaskHealth += flaskUpgradeAmount;
    }

    public void UpgradeDog()
    {
        if (inventory.coins < inventory.dogLevel * upgradeCost)
            return;
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        inventory.coins -= inventory.dogLevel * upgradeCost;
        inventory.dogLevel++;
        inventory.dogAttack += dogUpgradeAmount;
    }

    private void UpdatePickaxe()
    {
        Text upgrade = pickaxe.transform.GetChild(2).GetComponent<Text>();
        Image button = pickaxe.transform.GetChild(3).GetComponent<Image>();
        Text cost = pickaxe.transform.GetChild(4).GetComponent<Text>();
        upgrade.text = string.Format("• Level: {0} > {1}\n• Attack: {2} > {3}", inventory.pickaxeLevel, inventory.pickaxeLevel + 1, inventory.pickaxeAttack, inventory.pickaxeAttack + pickaxeUpgradeAmount);
        cost.text = "Cost: " + inventory.pickaxeLevel * upgradeCost;
        button.sprite = inventory.coins >= inventory.pickaxeLevel * upgradeCost ? greenButton : redButton;
    }

    private void UpdateShovel()
    {
        Text upgrade = shovel.transform.GetChild(2).GetComponent<Text>();
        Image button = shovel.transform.GetChild(3).GetComponent<Image>();
        Text cost = shovel.transform.GetChild(4).GetComponent<Text>();
        upgrade.text = string.Format("• Level: {0} > {1}\n• Speed: {2} > {3}", inventory.shovelLevel, inventory.shovelLevel + 1, inventory.shovelSpeed, inventory.shovelSpeed + shovelUpgradeAmount);
        cost.text = "Cost: " + inventory.shovelLevel * upgradeCost;
        button.sprite = inventory.coins >= inventory.shovelLevel * upgradeCost ? greenButton : redButton;
    }

    private void UpdateFlask()
    {
        Text upgrade = flask.transform.GetChild(2).GetComponent<Text>();
        Image button = flask.transform.GetChild(3).GetComponent<Image>();
        Text cost = flask.transform.GetChild(4).GetComponent<Text>();
        upgrade.text = string.Format("• Level: {0} > {1}\n• Health: {2} > {3}", inventory.flaskLevel, inventory.flaskLevel + 1, inventory.flaskHealth, inventory.flaskHealth + flaskUpgradeAmount);
        cost.text = "Cost: " + inventory.flaskLevel * upgradeCost;
        button.sprite = inventory.coins >= inventory.flaskLevel * upgradeCost ? greenButton : redButton;
    }

    private void UpdateDog()
    {
        Text upgrade = dog.transform.GetChild(2).GetComponent<Text>();
        Image button = dog.transform.GetChild(3).GetComponent<Image>();
        Text cost = dog.transform.GetChild(4).GetComponent<Text>();
        upgrade.text = string.Format("• Level: {0} > {1}\n• Attack: {2} > {3}", inventory.dogLevel, inventory.dogLevel + 1, inventory.dogAttack, inventory.dogAttack + dogUpgradeAmount);
        cost.text = "Cost: " + inventory.dogLevel * upgradeCost;
        button.sprite = inventory.coins >= inventory.dogLevel * upgradeCost ? greenButton : redButton;
    }
}
