using UnityEngine;
using System.Collections;

public class NeedsScript : MonoBehaviour
{
    [System.Serializable]
    public class Hunger
    {
        //[HideInInspector]
        public float curHunger;
        public float hungerSpeed;
        public bool isEating;
        public float eatingValue;

        public float maxHunger {get; set; }
        public float minHunger { get; set; }
    }
    [System.Serializable]
    public class Sleep
    {
        //[HideInInspector]
        public float curSleep;
        public float sleepSpeed;
        public bool isSleeping;
        public float sleepingValue;

        public float maxSleep { get; set; }
        public float minSleep { get; set; }
    }

    public Hunger hunger = new Hunger();
    public Sleep sleep = new Sleep();
    /*
    private enum catStates
    {
        hungry,
        sleepy
    }

    private catStates curState;
    */
	void Start ()
    {
        hunger.curHunger = hunger.maxHunger;
        hunger.hungerSpeed = 1.0f;
        hunger.isEating = false;
        hunger.eatingValue = 5.0f;
        hunger.minHunger = 0.0f;
        hunger.maxHunger = 10.0f;

        sleep.curSleep = sleep.maxSleep;
        sleep.sleepSpeed = 1.0f;
        sleep.isSleeping = false;
        sleep.sleepingValue = 5.0f;
        sleep.minSleep = 0.0f;
        sleep.maxSleep = 10.0f;
	}
	void Update ()
	{
        //HUNGER
        if (hunger.isEating && hunger.curHunger > hunger.minHunger)
        {
            hunger.curHunger -= hunger.eatingValue;
            Debug.Log("isEating");
            if (hunger.curHunger < hunger.minHunger)
                hunger.curHunger = hunger.minHunger;
            hunger.isEating = false;
        }
        else if (!hunger.isEating && hunger.curHunger <= hunger.maxHunger)
        {
            hunger.curHunger += hunger.hungerSpeed * Time.deltaTime;
        }

        if (hunger.curHunger >= hunger.maxHunger) Debug.Log("Walks very slowly"); //changes the speed of mvt script
        else Debug.Log("Walks normally"); //changes the speed of mvt script

        //SLEEP
        if (sleep.isSleeping && sleep.curSleep > sleep.minSleep)
        {
            sleep.curSleep -= sleep.sleepingValue;
            if (sleep.curSleep < sleep.minSleep)
                sleep.curSleep = sleep.minSleep;
            sleep.isSleeping = false;
        }
        else if (!sleep.isSleeping && sleep.curSleep <= sleep.maxSleep)
        {
            sleep.curSleep += sleep.sleepSpeed * Time.deltaTime;
        }

        if (sleep.curSleep >= sleep.maxSleep) Debug.Log("Sleepy");
        else Debug.Log("Awake");
	}
}
