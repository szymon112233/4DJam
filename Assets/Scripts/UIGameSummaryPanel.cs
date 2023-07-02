using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSummaryPanel : MonoBehaviour
{
    public Text bloodDonatedText;
    public Text bloodCollectedText;
    public Text bloodSpentText;
    public Text HumansKilledText;

    public void Show()
    {
        gameObject.SetActive(true);
        bloodDonatedText.text = $"Blood donated: {GameManager.Instance.BloodGivenAway}";
        bloodCollectedText.text = $"Blood collected: {GameManager.Instance.BloodOnCharacter + GameManager.Instance.BloodGivenAway + GameManager.Instance.BloodSpent}";
        bloodSpentText.text = $"Blood speny: {GameManager.Instance.BloodSpent}";
        HumansKilledText.text = $"Humans killed: {GameManager.Instance.HumansKilled}";
    }
}
