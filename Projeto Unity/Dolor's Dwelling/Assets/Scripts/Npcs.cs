using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Npcs : MonoBehaviour
{

    bool playerClose;
    public Fala startDialogue;
    Fala currentFala;
    bool dialoguing;
    bool textEnded;

    public GameObject dialoguePanel;
    public TextMeshProUGUI tmp;
    public Button[] opcoesDeResposta;
    Button currentButton;
    public GameObject closeIndicator;
    int buttonIndex;
    public DialogueTextController dialogueTextController;
    private void Start()
    {
        currentFala = startDialogue;
        OnButtonActivation();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerClose = true;
            closeIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerClose = false;
            closeIndicator.SetActive(false);
            EndDialogue();
        }
    }
    void Update()
    {
        if(!playerClose)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartDialogue();
        }

        if(!dialoguing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (textEnded)
            {
                if(currentFala.hasResponse)
                {
                    Response();
                }
                else
                { 
                    NextDialogue();
                }
            }
            else
            {
                SkipDialogue();
               
            }
        }
        Player.instance.InDialogue(dialoguing);

        if(Input.GetKeyDown(KeyCode.RightArrow) && currentFala.hasResponse)
        {
            ChangeSelectedButton(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentFala.hasResponse)
        {
            ChangeSelectedButton(- 1);
        }

        if(dialogueTextController.done)
        {
            textEnded = true;
        }
    }

    void Response()
    {
        tmp.gameObject.SetActive(false);
        textEnded = false;
        int i = 0;
        if (currentFala.opcoes[buttonIndex].result != null)
        {
            currentFala = currentFala.opcoes[buttonIndex].result;
            textEnded = false;
            tmp.text = currentFala.texto;
            tmp.gameObject.SetActive(true);
            dialogueTextController.StartText();
            foreach (Button b in opcoesDeResposta)
            {
                b.gameObject.SetActive(false);
                b.GetComponent<Image>().color = Color.white;
            }
            if (currentFala.hasResponse)
            {
                foreach (Button b in opcoesDeResposta)
                {
                    b.gameObject.SetActive(true);
                    b.GetComponentInChildren<TextMeshProUGUI>().text = currentFala.opcoes[i].playerResponse;
                    i++;
                }
                OnButtonActivation();
            }
        }
    }


    void OnButtonActivation()
    {
        buttonIndex = 0;
        currentButton = opcoesDeResposta[buttonIndex];
        currentButton.GetComponent<Image>().color = Color.red;
    }
    private void StartDialogue()
    {
        if (!dialoguing)
        {
            int i = 0;
            tmp.maxVisibleCharacters = 0;
            tmp.text = startDialogue.texto;
            dialoguePanel.SetActive(true);
            tmp.gameObject.SetActive(true);
            dialoguing = true;
            dialogueTextController.StartText();
            closeIndicator.SetActive(false);
            if (currentFala.hasResponse)
            {
                foreach(Button b in opcoesDeResposta)
                {
                    b.gameObject.SetActive(true);
                    b.GetComponentInChildren<TextMeshPro>().text = currentFala.opcoes[i].playerResponse;
                    i++;
                }
            }
        }
    }

    void ChangeSelectedButton(int index)
    {
        int bt = buttonIndex;
        if( index == 1)
        {

            if(bt +1 < opcoesDeResposta.Length)
            {
                currentButton = opcoesDeResposta[bt+1];
                buttonIndex++;
            }
            else
            {
                currentButton = opcoesDeResposta[0];
                buttonIndex = 0;
            }
        }
        else if (index == -1)
        {
            if(bt > 0)
            {
                currentButton = opcoesDeResposta[bt-1];
                buttonIndex --;
            }
            else
            {
                currentButton = opcoesDeResposta[opcoesDeResposta.Length -1];
                buttonIndex = opcoesDeResposta.Length - 1;
            }
        }
        foreach (Button b in opcoesDeResposta)
        {
            b.GetComponent<Image>().color = Color.white;
        }
        currentButton.GetComponent<Image>().color = Color.red;
        Debug.Log(buttonIndex);
    }

    public void SkipDialogue()
    {
        dialogueTextController.SkipText();
        textEnded = true;
    }
    void NextDialogue()
    {
        tmp.gameObject.SetActive(false);
        textEnded = false;
        if (currentFala.nextFala != null)
        {
            currentFala = currentFala.nextFala;
            int i = 0;
            tmp.text = currentFala.texto;
            tmp.maxVisibleCharacters = 0;
            tmp.gameObject.SetActive(true);
            dialogueTextController.StartText();
            if (currentFala.hasResponse)
            {
                foreach (Button b in opcoesDeResposta)
                {
                    b.gameObject.SetActive(true);
                    b.GetComponentInChildren<TextMeshProUGUI>().text = currentFala.opcoes[i].playerResponse;
                    i++;
                }
            }
        }

        else if(currentFala.nextFala == null && textEnded || currentFala.nextFala == null && dialogueTextController.done)
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        if(playerClose)
        {
            closeIndicator.SetActive(true);
        }
        dialoguePanel.SetActive(false);
        dialoguing = false;
        currentFala = startDialogue;
        foreach (Button b in opcoesDeResposta)
        {
            b.gameObject.SetActive(false);  
        }
        Player.instance.InDialogue(false);
    }
}

