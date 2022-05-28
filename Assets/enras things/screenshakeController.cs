using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenshakeController : MonoBehaviour
{
    private float shakeTimeRemaining, shakePower, shakeFadeTime, shakeRotation;

    public float rotationMultiplier = 15;

    public Vector3 originalPos;

    public float resetPosTime;

    public bool isCamera = false;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.K))
        //{
        //    startShake(.5f, .5f);
        //}
    }

    private void LateUpdate()
    {
        //shaking as long as the timer hasnt reached 0
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            //setting the amount it's gonna shake, keeping it random, but changing the intensity by its power
            float xAmount = Random.Range(-1f, 1f) * shakePower;
            float yAmount = Random.Range(-1f, 1f) * shakePower;

            //adding the positional data onto the object
            gameObject.transform.position += new Vector3(xAmount, yAmount, 0f);

            //making the shake slowly decrease by the shakefadetime
            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);

            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);
        }
        else if (shakeTimeRemaining <= 0 && isCamera)
        {
            //for some reason it can't store it's own position properly when it's on the canvas?? so i just instantly translate it back
            gameObject.transform.position = new Vector3(0,0,0);
        }
        else if (shakeTimeRemaining <= 0)
        {
            //moving the object back to it's starting point
            float x = Mathf.MoveTowards(transform.position.x, originalPos.x, resetPosTime * Time.deltaTime);
            float y = Mathf.MoveTowards(transform.position.y, originalPos.y, resetPosTime * Time.deltaTime);
            gameObject.transform.position = new Vector3(x, y);
        }
        //rotating to make it look iiiiiiinteresting
        if(isCamera != true)
            transform.rotation = Quaternion.Euler(0f, 0f, shakeRotation * Random.Range(-1f, 1f));
    }

    public void startShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;

        //making the shake slowly fade out
        shakeFadeTime = power / length;

        shakeRotation = power * rotationMultiplier;
    }
}
