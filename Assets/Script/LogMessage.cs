using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessage : MonoBehaviour
{
    [SerializeField]private List<string> message;
    public Database database;
    [SerializeField]private List<TMPro.TextMeshProUGUI> textHolder;
    public Sprite log1, log2;
    private SpriteRenderer sr;
    public bool isPrintintComplete = true, isHiding = true;
    [HideInInspector]public enum closeStatus { backToBigMap, close};
    private closeStatus status;
    private Transform canvasTransform;
    private TMPro.TextMeshProUGUI instructionHolder;

    private void Awake()
    {
        enabled = false;
        status = closeStatus.close;
        sr = GetComponent<SpriteRenderer>();
        canvasTransform = GameObject.Find("Canvas").transform;
    }

    public void AddMessage(string message)
    {
        this.message.Add(message);
    }

    public void SetImage(int logNum)
    {
        if (logNum == 0)
            sr.sprite = log1;
        if (logNum == 1)
            sr.sprite = log2;
    }

    public void Print(closeStatus status) // Back To BigMap After Printing Or Not
    {
        this.status = status;
        isPrintintComplete = false;
        isHiding = false;
        instructionHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        instructionHolder.text = null;
        instructionHolder.transform.SetParent(canvasTransform);

        StartCoroutine("PrintLog", status);
    }

    public void Hide()
    {
        for (int i = 0; i < textHolder.Count; i++)
        {
            if (textHolder[i].gameObject != null)
                Destroy(textHolder[i].gameObject);
        }
        textHolder.Clear();
        sr.sprite = null;
        enabled = false;
        isHiding = true;
        if (instructionHolder != null)
        {
            Destroy(instructionHolder.gameObject);
            instructionHolder = null;
        }
    }

    public void DeleteLog()
    {
        for (int i = 0; i < textHolder.Count; i++)
        {
            if (textHolder[i].gameObject != null)
                Destroy(textHolder[i].gameObject);
        }
        textHolder.Clear();
        message.Clear();
    }

    public void PrintLatestMessage()
    {
        isHiding = false;
        sr.sprite = log1;
        if (message.Count > 0)
        {
            textHolder.Add(Instantiate(database.instruction, transform.position, Quaternion.identity).GetComponent<TMPro.TextMeshProUGUI>());
            textHolder[textHolder.Count - 1].transform.SetParent(canvasTransform);
            textHolder[textHolder.Count - 1].fontSize = 1;
            textHolder[textHolder.Count - 1].transform.position += new Vector3(2.4f, 1.4f + (textHolder.Count - 1) * -0.3f, 0);
            textHolder[textHolder.Count - 1].color = new Color32(250, 200, 55, 255);
            textHolder[textHolder.Count - 1].text = message[message.Count - 1];
        }
    }

    public void PrintPreviousMessages()
    {
        isHiding = false;
        sr.sprite = log1;
        for (int i = 0; i < message.Count; i++)
        {
            textHolder.Add(Instantiate(database.instruction, transform.position, Quaternion.identity).GetComponent<TMPro.TextMeshProUGUI>());
            textHolder[textHolder.Count - 1].transform.SetParent(canvasTransform);
            textHolder[textHolder.Count - 1].fontSize = 1;
            textHolder[textHolder.Count - 1].transform.position += new Vector3(2.4f, 1.4f + (textHolder.Count - 1) * -0.3f, 0);
            textHolder[textHolder.Count - 1].color = new Color32(250, 200, 55, 255);
            textHolder[textHolder.Count - 1].text = message[i];
        }
    }

    private void Update()
    {
        if (isPrintintComplete == true)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (status == closeStatus.backToBigMap)
                {
                    Hide();
                    database.transform.parent.GetComponent<CrossSceneManagement>().LoadScene("BigMap");
                }
                else if (status == closeStatus.close)
                {
                    Hide();
                }
            }
        }
    }

    IEnumerator PrintLog(closeStatus status) // Print The Whole Log
    {
        enabled = false;
        database.isHandling = true;
        sr.sprite = log1;

        for (int i = 0; i < message.Count; i++)
        {
            textHolder.Add(Instantiate(database.instruction, transform.position, Quaternion.identity).GetComponent<TMPro.TextMeshProUGUI>());
            textHolder[textHolder.Count - 1].transform.SetParent(canvasTransform);
            textHolder[textHolder.Count - 1].fontSize = 1;
            textHolder[textHolder.Count - 1].transform.position += new Vector3(2.4f, 1.4f + (textHolder.Count - 1) * -0.3f, 0);
            textHolder[textHolder.Count - 1].color = new Color32(250, 200, 55, 255);
            textHolder[textHolder.Count - 1].text = message[i];

            if (textHolder.Count > 6 && textHolder.Count != 7)
            {
                sr.sprite = log2;
                instructionHolder.text = "[Z] to continue";
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                instructionHolder.text = null;
                for (int j = 0; j < textHolder.Count; j++)
                {
                    Destroy(textHolder[j].gameObject);
                }
                sr.sprite = log1;
                textHolder.Clear();
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }

        sr.sprite = log2;
        database.isHandling = false;
        isPrintintComplete = true;

        instructionHolder.text = "[X] to close";
        enabled = true;
    }
}
