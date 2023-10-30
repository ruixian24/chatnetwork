using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Chat : MonoBehaviour
{
    Network network;

    public InputField id;

    List<string> list;
    public Text[] text;
    public InputField chat;
    public Image backUI;
    public GameObject[] player;
    public AudioSource messageSound;

    void Start()
    {
        network = GetComponent<Network>();
        list = new List<string>();
    }

    public void BeginServer()
    {
        network.ServerStart(10000, 10);
        player[0].SetActive(true);

        network.name = id.text;
    }

    public void BeginClient()
    {
        network.ClientStart("127.0.0.1", 10000);
        network.name = id.text;
    }

    void Update()
    {
        
        if (network != null && network.IsConnect())
        {
            byte[] bytes = new byte[1024];
            int length = network.Receive(ref bytes, bytes.Length);
            if (length > 0)
            {
                string str = System.Text.Encoding.UTF8.GetString(bytes);
                AddTalk(str);
                SetAnimation(false);        
            }
           
            UpdateUI();
        }

        //엔터 누르면 전송

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendTalk();
        }
    }

    void SetAnimation(bool bSend)
    {
        int iPlayer;

        if (bSend)
            iPlayer = network.IsServer() ? 0 : 1;
        else
            iPlayer = network.IsServer() ? 1 : 0;

        player[iPlayer].GetComponent<Animator>().SetTrigger("dance");
    }

    void AddTalk(string str) 
    {
        
        while (list.Count >= 8)
        {
            list.RemoveAt(0);
        }

        list.Add(str);
        UpdateTalk();
    }

    void SendTalk()
    {
        
        string str = network.name + ": " + chat.text;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

        //소리재생
        if (messageSound != null)
        {
            messageSound.Play();
        }

        network.Send(bytes, bytes.Length);

        AddTalk(str);
        SetAnimation(true);
      
    
        // 메시지 보내고 나면 입력 필드 비우기
        chat.text = "";
    }

    void UpdateTalk()
    {
        for (int i = 0; i < list.Count; i++)
        {
            text[i].text = list[i];
        }
        
    }

    void UpdateUI()
    {
        if (!backUI.IsActive())
        {
            backUI.gameObject.SetActive(true);
            player[0].SetActive(true);
            player[1].SetActive(true);
        }
    }
}