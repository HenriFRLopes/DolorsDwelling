using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTextController : MonoBehaviour
{

    public TextMeshProUGUI diaText;

    public float timeBetweenChars;
    public bool done = false;


    public void StartText()
    {
        StartCoroutine(RevealText());
    }
    public IEnumerator RevealText()
    {
        int visibleCount = 0;
        diaText.maxVisibleCharacters = 0;
        diaText.ForceMeshUpdate();
        TMP_TextInfo textInfo = diaText.textInfo;
        int charCount = textInfo.characterCount;
        while(visibleCount < charCount)
        {
            yield return new WaitForSeconds(timeBetweenChars);
            visibleCount++;
            diaText.maxVisibleCharacters = visibleCount;
        }
        done = true;
    }

    public void SkipText()
    {
        StopCoroutine(RevealText());
        diaText.maxVisibleCharacters = 99999;
        diaText.ForceMeshUpdate();
    }
}
