using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public bool isHandling = true, isSelectedOption = false, isAllySelected = false;
    public int selectedState = 0, selectedIndex = 0, selector = 0, selectedItem = 0, currentWave = 0, mapLerpingNumber = 0, totalWave = 0, level = 0, coin = 0, coinGainInOneRound = 0;
    public GameObject characterData, sceneCharacter, instruction, map, log, dice;
    public GameObject[] characterSprites;
    public List<GameObject> allyDetails, enemyDetails;
    public List<Character> waitingEnemies;
    public List<Item> inventory;
    public Sprite[] backgroundMap, foregroundMap;
    [HideInInspector]public MoveForeground moveMap;
    [HideInInspector]public LogMessage logMessage;
    [HideInInspector] public Dice diceHolder;

    private BattleMenu battleMenu;

    public void AddCharacterToAllyList(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Character.Element element, int ID)
    {
        GameObject cloner = Instantiate(characterData);
        cloner.GetComponent<Character>().SetCharacter(maxHP, maxMP, defense, dodgeRate, speed, attackDamage, element, ID);
        Character tempCharacter = cloner.GetComponent<Character>();
        tempCharacter.isAlly = true;
        tempCharacter.database = this;
        cloner.name = ID.ToString();
        cloner.transform.SetParent(transform);
        allyDetails.Add(cloner);
    }

    public void AddCharacterToEnemyList(Character characterStats)
    {
        GameObject cloner = Instantiate(characterData);
        cloner.GetComponent<Character>().SetCharacter(characterStats.maxHP, characterStats.maxMP, characterStats.defense, characterStats.dodgeRate, characterStats.speed, characterStats.attackDamage, characterStats.element, characterStats.ID, characterStats.wave);
        Character tempCharacter = cloner.GetComponent<Character>();
        tempCharacter.isAlly = false;
        tempCharacter.database = this;
        cloner.name = characterStats.ID.ToString();
        cloner.transform.SetParent(transform);
        enemyDetails.Add(cloner);
    }

    public void CreateAlly()
    {
        for (int i = 0; i < allyDetails.Count; i++)
        {
            GameObject cloner = Instantiate(sceneCharacter, new Vector2(-2 + i * -2, -2), Quaternion.identity);
            SceneCharacter temp = cloner.GetComponent<SceneCharacter>();
            temp.characterStats = allyDetails[i].GetComponent<Character>();
            temp.database = this;
            temp.isBarCharacter = false;
            allyDetails[i].GetComponent<Character>().sceneCharacter = temp;
            cloner.tag = "Ally";
        }
    }

    public void CreateEnemy()
    {
        int t = 0;
        for (int i = 0; i < waitingEnemies.Count; i++)
        {
            if (waitingEnemies[i].wave == currentWave)
            {
                AddCharacterToEnemyList(waitingEnemies[i]);
                t++;
            }
        }

        for (int i = 0; i < waitingEnemies.Count; i++)
        {
            if (t > 0)
            {
                waitingEnemies.RemoveAt(t - 1);
            }
            t--;
        }
        for (int i = 0; i < enemyDetails.Count; i++)
        {
            GameObject cloner = Instantiate(sceneCharacter, new Vector2(2 + i * 2, -2), Quaternion.identity);
            SceneCharacter temp = cloner.GetComponent<SceneCharacter>();
            temp.characterStats = enemyDetails[i].GetComponent<Character>();
            temp.database = this;
            temp.isBarCharacter = false;
            enemyDetails[i].GetComponent<Character>().sceneCharacter = temp;
            cloner.tag = "Enemy";
        }
    }

    public void AddItemToInventory(string name, int amount)
    {
        bool isExist = false;
        int existingIndex = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (name == inventory[i].itemName)
            {
                isExist = true;
                existingIndex = i;
            }
        }
        if (isExist == true)
        {
            inventory[existingIndex].itemAmount += amount;
        }
        else
        {
            inventory.Add(new Item(name, amount));
        }
    }

    public void CreateMap(int sceneNumber)
    {
        GameObject tempBackground = Instantiate(map);
        tempBackground.transform.position = new Vector3(0, 0, 0);
        SpriteRenderer backgroundSR = tempBackground.GetComponent<SpriteRenderer>();
        backgroundSR.sprite = backgroundMap[sceneNumber];
        backgroundSR.sortingLayerName = "background";
        tempBackground.transform.localScale = new Vector3(2.5f, 2.5f, 0);

        GameObject tempForeground = Instantiate(map);
        tempForeground.transform.position = new Vector3(0, 0, 0);
        SpriteRenderer foregroundSR = tempForeground.GetComponent<SpriteRenderer>();
        foregroundSR.sprite = foregroundMap[sceneNumber];
        foregroundSR.sortingLayerName = "foreground";
        tempForeground.transform.localScale = new Vector3(2.5f, 2.5f, 0);
        moveMap = tempForeground.GetComponent<MoveForeground>();
        moveMap.isForeground = true;
        moveMap.database = this;
    }

    public void AddEnemyDetailsToWaitingEnemies(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Character.Element element, int ID, int wave)
    {
        waitingEnemies.Add(new Character(maxHP, maxMP, defense, dodgeRate, speed, attackDamage, element, ID, wave));
    }

    public void SetUp()
    {
        coin = 0;

        AddCharacterToAllyList(1, 100, 20, 5, 40, 15, Character.Element.wildfire, 0);
        AddCharacterToAllyList(1, 100, 10, 5, 1, 15, Character.Element.water, 0);
        AddCharacterToAllyList(1, 100, 10, 5, 1, 15, Character.Element.earth, 0);

        allyDetails[0].GetComponent<Character>().AddSkill(Character.Element.wind, 10, "Dodge");
        allyDetails[0].GetComponent<Character>().AddSkill(Character.Element.wind, 10, "AgainstTheCurrent");
        allyDetails[0].GetComponent<Character>().AddSkill(Character.Element.fire, 10, "Fireball");
        allyDetails[0].GetComponent<Character>().AddSkill(Character.Element.fire, 10, "Explosion");
    }

    public void CreateDice()
    {
        if (diceHolder == null)
        {
            diceHolder = Instantiate(dice, new Vector2(-9, 0), Quaternion.identity).GetComponent<Dice>();
            diceHolder.database = this;
        }
    }


    private IEnumerator WaitForTotalWave(int minWave, int maxWave, int level)
    {
        CreateDice();
        diceHolder.ThrowDice(minWave, maxWave);
        yield return new WaitUntil(() => diceHolder.isDicingComplete == true);
        totalWave = diceHolder.diceNumber;
        Destroy(diceHolder.textHolder.gameObject);
        Destroy(diceHolder.gameObject);
        diceHolder = null;

        for (int i = 0; i < totalWave; i++)
        {
            int enemyNumber = Random.Range(1, 4);
            for (int j = 0; j < enemyNumber; j++)
            {
                Character tempCharacter = GetPossibleEnemyInLevel(level);
                AddEnemyDetailsToWaitingEnemies(tempCharacter.maxHP, 0, tempCharacter.defense, tempCharacter.dodgeRate, tempCharacter.speed, tempCharacter.attackDamage, tempCharacter.element, tempCharacter.ID, i);
            }
        }

        CreateAlly();
        CreateEnemy();
    }

    private Character GetPossibleEnemyInLevel(int level)
    {
        switch (level)
        {
            case 0:
                if (isFulfilledPossibility(10))
                {
                    return new Character(15, 0, 2, 5, 6, 4, Character.Element.none, 0, 0);
                }
                break;
        }
        return new Character(15, 0, 2, 5, 6, 4, Character.Element.none, 0, 0);
    }

    private bool isFulfilledPossibility(int denominator)
    {
        if (Random.Range(1, denominator) == 1)
        {
            return true;
        }
        return false;
    }

    public void Initialize()
    {
        battleMenu = GameObject.Find("BattleMenu").GetComponent<BattleMenu>();
        battleMenu.sr = battleMenu.GetComponent<SpriteRenderer>();
        battleMenu.database = this;
        battleMenu.Hide();

        logMessage = Instantiate(log).GetComponent<LogMessage>();
        logMessage.DeleteLog();
        logMessage.AddMessage("[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "] <Battle Started!>");
        logMessage.database = this;

        currentWave = 0;
        coinGainInOneRound = 0;
        CreateMap(level);
        switch (level)
        {
            case 0:
                StartCoroutine(WaitForTotalWave(1, 11, level));
                break;
            case 1:
                StartCoroutine(WaitForTotalWave(1, 6, level));
                break;
            case 2:
                StartCoroutine(WaitForTotalWave(1, 6, level));
                break;
            case 3:
                StartCoroutine(WaitForTotalWave(1, 6, level));
                break;
            default:
                StartCoroutine(WaitForTotalWave(1, 6, level));
                break;
        }
    }
}
