using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{



    public TextMeshProUGUI talkTrip;


    private UIManager ui;
    private void Awake()
    {
        ui = UIManager.instance;

    }
    void Start()
    {
        SetTalkTripText(false);

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SetTalkTripText(bool active)
    {
        talkTrip.text = "Press F to chat";
        talkTrip.gameObject.SetActive(active);
    }
  

  
}
