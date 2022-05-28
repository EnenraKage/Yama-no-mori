using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class inventory : MonoBehaviour
{
    public GameObject inventoryGUI;
    public bool inventoryActive = false;
    public ConversationController convCtrl;

    // Update is called once per frame
    void Update()
    {
        //if the player presses the inventory key while a character is talking it stays disabled :]
        if (Input.GetButtonDown("inventory") && convCtrl.isTalking == false)
        {
            if(inventoryActive)
            {
                inventoryGUI.SetActive(false);
                inventoryActive = false;
            }
            else
            {
                inventoryGUI.SetActive(true);
                inventoryActive = true;
            }
        }
    }

    public void Quit()
    {
        //goes back to main menu when activated
        SceneManager.LoadScene(0);
    }
}
