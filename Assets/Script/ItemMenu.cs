using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenu : MonoBehaviour
{
    [HideInInspector]public SpriteRenderer sr;
    [HideInInspector]public BattleMenu battleMenu;
    [HideInInspector]public ItemMenu itemMenu, upArrowHolder, downArrowHolder, description1, description2;
    public Sprite[] itemSprites;
    [HideInInspector]public bool isDescription, isArrow;
    public GameObject itemAmountPrefab;
    public Sprite menuImage, upArrow, downArrow;
    [HideInInspector]public TMPro.TextMeshProUGUI itemAmountHolder;
    public int descriptionID, scroller = 0;

    private void Start()
    {
        transform.SetParent(battleMenu.transform);
        sr = GetComponent<SpriteRenderer>();
        if (isDescription == false && isArrow == false)
        {
            sr.sprite = menuImage;
            sr.sortingLayerName = "menu";
            battleMenu.instructionHolder.text = "[W], [Up] and [S], [Down] to scroll, [Z] to comfirm, [X] to cancel";
            description1 = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<ItemMenu>();
            description1.isDescription = true;
            description1.descriptionID = 0;
            description1.itemMenu = this;
            description1.name = "Page1";

            description2 = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<ItemMenu>();
            description2.isDescription = true;
            description2.descriptionID = 1;
            description2.itemMenu = this;
            description2.name = "Page2";

            upArrowHolder = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<ItemMenu>();
            upArrowHolder.isArrow = true;
            upArrowHolder.descriptionID = 2;
            upArrowHolder.itemMenu = this;
            upArrowHolder.name = "Up";

            downArrowHolder = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<ItemMenu>();
            downArrowHolder.isArrow = true;
            downArrowHolder.descriptionID = 3;
            downArrowHolder.itemMenu = this;
            downArrowHolder.name = "Down";
        }
        else
        {
            sr.sortingLayerName = "description";
            transform.SetParent(GameObject.Find("ItemMenu(Clone)").transform);
            if (isArrow == false)
            {
                if (descriptionID + itemMenu.scroller < battleMenu.database.inventory.Count)
                {
                    sr.sprite = itemSprites[battleMenu.database.inventory[descriptionID + itemMenu.scroller].ID];
                    itemAmountHolder = Instantiate(itemAmountPrefab, transform.position, Quaternion.identity).GetComponent<TMPro.TextMeshProUGUI>();
                    itemAmountHolder.transform.SetParent(GameObject.Find("Canvas").transform);
                    itemAmountHolder.text = "x" + battleMenu.database.inventory[itemMenu.scroller + descriptionID].itemAmount;
                    itemAmountHolder.transform.position += new Vector3(3.25f, 1.25f + descriptionID * -1.55f, 0);
                }
                else
                {
                    Destroy(gameObject);
                }
                if (descriptionID == 0)
                {
                    transform.position = (Vector2)transform.position + new Vector2(0, 0.75f);
                }
                else
                {
                    transform.position = (Vector2)transform.position + new Vector2(0, -0.75f);
                    sr.color = new Color32(170, 70, 200, 255);
                }
                enabled = false;
            }
            else
            {
                if (descriptionID == 2)
                {
                    transform.position = (Vector2)transform.position + new Vector2(3.5f, 2);
                    sr.sprite = upArrow;
                }
                else
                {
                    transform.position = (Vector2)transform.position + new Vector2(3.5f, -2);
                    sr.sprite = downArrow;
                }
                enabled = false;
            }
        }
    }

    public void ChangeVisual()
    {
        if (scroller > 0)
        {
            upArrowHolder.sr.sprite = upArrow;
        }
        else
        {
            upArrowHolder.sr.sprite = null;
        }

        if (scroller + 1 < battleMenu.database.inventory.Count)
        {
            downArrowHolder.sr.sprite = downArrow;
        }
        else
        {
            downArrowHolder.sr.sprite = null;
        }

        if (description1 != null)
        {
            if (scroller >= battleMenu.database.inventory.Count) // description 1
            {
                description1.sr.sprite = null;
                description1.itemAmountHolder.text = null;
            }
            else
            {
                description1.sr.sprite = itemSprites[battleMenu.database.inventory[scroller].ID];
                description1.itemAmountHolder.text = "x" + battleMenu.database.inventory[scroller].itemAmount;
            }
        }

        if (description2 != null)
        {
            if (scroller + 1 >= battleMenu.database.inventory.Count) // description 2
            {
                description2.sr.sprite = null;
                description2.itemAmountHolder.text = null;
            }
            else
            {
                description2.sr.sprite = itemSprites[battleMenu.database.inventory[scroller + 1].ID];
                description2.itemAmountHolder.text = "x" + battleMenu.database.inventory[scroller + 1].itemAmount;
            }
        }
    }

    private void Update()
    {
        if (isDescription == false && isArrow == false)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                battleMenu.instructionHolder.text = "[W], [Up] and [S], [Down] to scroll, [Z] to comfirm";
                battleMenu.isSelectedOption = false;
                battleMenu.enabled = true;
                if (description1 != null)
                    Destroy(description1.itemAmountHolder.gameObject);
                if (description2 != null)
                    Destroy(description2.itemAmountHolder.gameObject);
                Destroy(gameObject);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (scroller + 2 <= battleMenu.database.inventory.Count)
                {
                    scroller++;
                    ChangeVisual();
                }
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (scroller - 1 >= 0)
                {
                    scroller--;
                    ChangeVisual();
                }
            }
            if (Input.GetKeyDown(KeyCode.Z) && battleMenu.database.inventory.Count != 0)
            {
                battleMenu.currentItem = scroller;
                battleMenu.isSelectedItem = true;
                battleMenu.enabled = true;
                if (description1 != null)
                    Destroy(description1.itemAmountHolder.gameObject);
                if (description2 != null)
                    Destroy(description2.itemAmountHolder.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
