using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public float timer;
    public int minCount;
    public int ammo;
    public TextMeshProUGUI timerUI;
    public TextMeshProUGUI ammoUI;
    public TextMeshProUGUI finalTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerUI.GetComponent<TextMeshProUGUI>().text = minCount.ToString() + ":" + timer.ToString("f2");
        ammoUI.GetComponent<TextMeshProUGUI>().text = "Hookshot Ammo: " + ammo.ToString();
        finalTime.GetComponent<TextMeshProUGUI>().text = minCount.ToString() + ":" + timer.ToString("f2");
    }
}
