using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManagerScript : MonoBehaviour
{
    public int[,] shopItems = new int[6,6];
    [HideInInspector]public float coins;
    public Text ConinsTXT;
    public int currentSelection = 0;
    public GameObject selectionPrefab, selectionHolder;
    public Database database;
    public CrossSceneManagement CSM;
    private TMPro.TextMeshProUGUI textHolder;

    void Awake()
    {
        CSM = GameObject.Find("CrossSceneManager").GetComponent<CrossSceneManagement>();
        database = CSM.transform.GetChild(0).GetComponent<Database>();
        textHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        textHolder.text = "[A] [D] to select, [Z] to buy, [X] to exit";
        textHolder.transform.SetParent(transform.GetChild(0).GetChild(0));
        coins = database.coin;
        ConinsTXT.text = "Coins: $" + coins.ToString();

        //IDs
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;
        shopItems[1, 5] = 5;

        //price
        shopItems[2, 1] = 100;
        shopItems[2, 2] = 100;
        shopItems[2, 3] = 150;
        shopItems[2, 4] = 150;
        shopItems[2, 5] = 300;

        //quantity
        SetItemAmount("HP Potion", 1);
        SetItemAmount("MP Potion", 2);
        SetItemAmount("Strength Potion", 3);
        SetItemAmount("Speed Potion", 4);
        SetItemAmount("Revive Potion", 5);

        selectionHolder = Instantiate(selectionPrefab);
        selectionHolder.transform.position = GetItemPosition(currentSelection);
    }

    private void SetItemAmount(string itemName, int ID)
    {
        bool isFound = false;
        for (int i = 0; i < database.inventory.Count; i++)
        {
            if (database.inventory[i].itemName == itemName)
            {
                isFound = true;
                shopItems[3, ID] = database.inventory[i].itemAmount;
            }
        }
        if (isFound == false)
        {
            shopItems[3, ID] = 0;
        }
    }

    public Vector2 GetItemPosition(int itemIndex)
    {
        selectionHolder.transform.SetParent(transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(itemIndex).GetChild(2));
        return transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(itemIndex).GetChild(3).position;
    }

    public void Buy(int selectedIndex = -1)
    {
        GameObject ButtonRef;
        if (selectedIndex == -1)
        {
            ButtonRef = transform.parent.GetComponent<EventSystem>().currentSelectedGameObject;
        }
        else
        {
            ButtonRef = transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(selectedIndex).gameObject;

        }
        ButtonInfo tempButton = ButtonRef.GetComponent<ButtonInfo>();

        if (coins >= shopItems[2, tempButton.ItemID])
        {
            database.coin -= shopItems[2, tempButton.ItemID];
            coins = database.coin;
            shopItems[3, tempButton.ItemID]++;
            database.AddItemToInventory(tempButton.Itemdis.text, 1);
            ConinsTXT.text = "Coins: $" + coins.ToString();
            ButtonRef.GetComponent<ButtonInfo>().QuantityTxt.text = shopItems[3, tempButton.ItemID].ToString();
            ButtonRef.GetComponent<ButtonInfo>().updateText();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentSelection - 1 >= 0)
            {
                currentSelection--;
                selectionHolder.transform.position = GetItemPosition(currentSelection);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentSelection + 1 <= 4)
            {
                currentSelection++;
                selectionHolder.transform.position = GetItemPosition(currentSelection);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            CSM.LoadScene("BigMap");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Buy(currentSelection);
        }
    }


}
