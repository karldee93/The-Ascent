using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public float timer;
    public int minCount;
    public int ammo;
    public Text timerUI;
    public Text ammoUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerUI.text = "Timer: " + minCount.ToString() + ":" + timer.ToString("f2");
        ammoUI.text = "Hookshot Ammo: " + ammo.ToString();
    }
}
