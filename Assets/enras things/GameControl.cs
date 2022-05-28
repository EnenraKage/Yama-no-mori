using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{

    //singleton design, a class that there can only be ONE of
    public static GameControl control;

    public float MaxHP;
    public float currentHP;
    public float Level;
    public int Dmg;
    public int Def;
    public float exp;

    // Start is called before the first frame update, awake happens before that apparently lol
    void Awake()
    {
        //if control doesn't exist, make this control
        if (control == null)
        {
            //means that this object will persist when changing scenes
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        //if one gamecontrol DOES exist and this is not it, then destroy it
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
