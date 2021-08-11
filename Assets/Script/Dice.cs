using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Sprite[] diceImages;
    private SpriteRenderer sr;
    private Vector2 originalPos;
    [HideInInspector]public Database database;
    public bool isDicingComplete;
    public int diceNumber;
    [HideInInspector]public TMPro.TextMeshProUGUI textHolder;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        isDicingComplete = true;
    }

    public void ThrowDice(int min, int max)
    {
        textHolder = Instantiate(database.instruction, transform.position, Quaternion.identity).GetComponent<TMPro.TextMeshProUGUI>();
        textHolder.transform.SetParent(GameObject.Find("Canvas").transform);
        textHolder.text = null;
        textHolder.color = Color.black;
        textHolder.fontSize = 4;
        textHolder.alignment = TMPro.TextAlignmentOptions.Midline;
        originalPos = transform.position;
        StartCoroutine(DiceRandom(min, max));
    }


    private IEnumerator DiceRandom(int min, int max)
    {
        isDicingComplete = false;
        database.isHandling = true;
        database.logMessage.PrintLatestMessage();
        database.logMessage.AddMessage("<Start Throwing Dice!>");
        database.logMessage.PrintLatestMessage();
        database.logMessage.AddMessage("Waiting...");
        database.logMessage.PrintLatestMessage();
        int throwTime = Random.Range(25, 45);
        int diceValue = Random.Range(min, max);
        float waitTime = 0.4f;
        for (int i = 0; i < throwTime; i++)
        {
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 50) * 180);
            transform.position = new Vector3 (originalPos.x, originalPos.y + Mathf.Cos(Time.time * 100) * 0.2f, 0);
            sr.sprite = diceImages[Random.Range(0, 5)];
            yield return new WaitForSeconds(0.07f + waitTime);
            waitTime = waitTime * 0.7f;
        }
        if (diceValue < 6)
        {
            sr.sprite = diceImages[diceValue];
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            sr.sprite = diceImages[6];
            textHolder.text = (diceValue + 1).ToString();
        }
        database.logMessage.AddMessage("Value: " + (diceValue + 1) + "!");
        database.logMessage.PrintLatestMessage();
        database.logMessage.SetImage(1);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        database.logMessage.DeleteLog();
        database.logMessage.Hide();
        database.isHandling = false;
        isDicingComplete = true;
        diceNumber = diceValue + 1;
    }
}
