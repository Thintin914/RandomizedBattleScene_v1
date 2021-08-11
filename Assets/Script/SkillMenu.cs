using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenu : MonoBehaviour
{
    public bool isDescription, isArrow;
    public Sprite[] skillSprites;
    public Sprite menuImage, upArrowImage, downArrowImage;
    private SpriteRenderer sr;
    public BattleMenu battleMenu;
    public int scroller = 0, descriptionID;
    private SkillMenu skillMenu, description1, description2, upArrowHolder, downArrowHolder;
    private List<Skill> skills;
    private Character characterStats;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isDescription == false && isArrow == false)
        {
            skills = battleMenu.database.allyDetails[battleMenu.database.selector].GetComponent<Character>().skills;
            sr.sortingLayerName = "menu";
            battleMenu.instructionHolder.text = "[W], [Up] and [S], [Down] to scroll, [Z] to comfirm, [X] to cancel";
            description1 = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<SkillMenu>();
            description1.isDescription = true;
            description1.descriptionID = 0;
            description1.skillMenu = this;
            description1.name = "Page1";

            description2 = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<SkillMenu>();
            description2.isDescription = true;
            description2.descriptionID = 1;
            description2.skillMenu = this;
            description2.name = "Page2";

            upArrowHolder = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<SkillMenu>();
            upArrowHolder.isArrow = true;
            upArrowHolder.descriptionID = 2;
            upArrowHolder.skillMenu = this;
            upArrowHolder.name = "UpArrow";

            downArrowHolder = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<SkillMenu>();
            downArrowHolder.isArrow = true;
            downArrowHolder.descriptionID = 3;
            downArrowHolder.skillMenu = this;
            downArrowHolder.name = "DownArrow";

            sr.sprite = menuImage;
            characterStats = battleMenu.database.allyDetails[battleMenu.database.selector].GetComponent<Character>();
        }
        else
        {
            sr.sortingLayerName = "description";
            transform.SetParent(GameObject.Find("SkillMenu(Clone)").transform);
            if (isArrow == false)
            {
                if (descriptionID + skillMenu.scroller < skillMenu.skills.Count)
                {
                    sr.sprite = skillSprites[skillMenu.skills[skillMenu.scroller].ID + descriptionID];
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
                    sr.sprite = upArrowImage;
                }
                else
                {
                    transform.position = (Vector2)transform.position + new Vector2(3.5f, -2);
                    sr.sprite = downArrowImage;
                }
                enabled = false;
            }
        }
    }

    public void ChangeVisual()
    {
        if (scroller > 0)
        {
            upArrowHolder.sr.sprite = upArrowImage;
        }
        else
        {
            upArrowHolder.sr.sprite = null;
        }

        if (scroller + 1 < skills.Count)
        {
            downArrowHolder.sr.sprite = downArrowImage;
        }
        else
        {
            downArrowHolder.sr.sprite = null;
        }

        if (description1 != null)
        {
            if (scroller >= skills.Count)
            {
                description1.sr.sprite = null;
            }
            else
            {
                description1.sr.sprite = skillSprites[skills[scroller].ID];
            }
        }

        if (description2 != null)
        {
            if (scroller + 1 >= skills.Count)
            {
                description2.sr.sprite = null;
            }
            else
            {
                description2.sr.sprite = skillSprites[skills[scroller + 1].ID];
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
                Destroy(gameObject);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (scroller + 2 <= skills.Count)
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
            if (Input.GetKeyDown(KeyCode.Z) && skills.Count != 0)
            {
                if (characterStats.currentMP >= skills[scroller].MPCost)
                {
                    characterStats.currentMP -= skills[scroller].MPCost;
                    battleMenu.currentItem = skills[scroller].ID;
                    battleMenu.isSelectedItem = true;
                    battleMenu.enabled = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
