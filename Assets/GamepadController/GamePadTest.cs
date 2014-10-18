using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GamePadTest : MonoBehaviour {

    GamePadController.Controller gamePad;

    public Transform object1;
    public Transform object2;
    public Transform object3;

	void Start () {
        gamePad = GamePadController.GamePadOne;
        //gamePad.SetVibration(10f, 30f, 5f);
        //InvokeRepeating("FrenquecyTest", 0f, 2f);
        //InvokeRepeating("FrenquecyTest2", 0f, 2f);
        //InvokeRepeating("FrenquecyTest3", 0f, 2f);
        //Invoke("FrenquecyTest", 5f);

        var testArr = new List<int>() { 1, 5, 3, 8, 4 };
        List<int> test2Arr = new List<int>();
        testArr.RemoveRange(2, 1);
        test2Arr = testArr;
        //test2Arr = (List<int>)testArr;
        //Mathf.TendencyRandomTest(this,3);
	}

    private static System.Random random = new System.Random();

    public static void ShuffleList<E>(IList<E> list)
    {
        if (list.Count > 1)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                E tmp = list[i];
                int randomIndex = random.Next(i + 1);

                //Swap elements
                list[i] = list[randomIndex];
                list[randomIndex] = tmp;
            }
        }
    }

    public static void ShuffleList2(ref List<int> list)
    {
       for (int i = 100; i >= 0; i--)
        {
            int randomIndex = random.Next(i + 1);
            list.Add(randomIndex);
        }
    }

    private static void testFrenquecy4()
    {
        var testList = new List<int>();
        //var testRand = new System.Random();
        //bool valueChecked = false;
        for (int i = 0; i < 101; i++)
        {
            /*var tempValue = testRand.Next(0, 101);
            if (genNums.Contains(tempValue))
            {
                while (!valueChecked)
                {
                    Debug.Log("Found Duplicate");
                    valueChecked = FindNewValue(ref tempValue, ref testRand);
                }
            }
            genNums.Add(tempValue);
            valueChecked = false;*/
            testList.Add(i);
        }
        //ShuffleList2(ref testList);
        ShuffleList(testList);
        Debug.Log(testList.Count);
        var hashset = new HashSet<int>();
        int v = 0;
        foreach (var name in testList)
        {
            if (!hashset.Add(name))
            {
                Debug.Log("List contains duplicate values.");
                break;
            }
            else
            {
                Debug.Log(name);
                Debug.Log(v);
                v++;
            }
        }
    }

    private static List<int> genNums = new List<int>();
    private static bool FindNewValue(ref int tempValue, ref System.Random rand)
    {
        tempValue = rand.Next(0, 101);
        if (!genNums.Contains(tempValue))
            return true;
        else
            return false;
    }

    private int i = 0;
	void Update () {
        //Debug.Log(gamePad.UP.Pressed);
        //Debug.Log(gamePad.UP.Held);
        //Debug.Log(gamePad.UP.Released);
        //gamePad.X.Held
        //gamePad.LeftStick.X
        //gamePad.LeftTrigger
        //gamePad.StopVibration();
        if (i < 1)
        {
           // FrenquecyTest3();
            testFrenquecy4();
        }
        i++;

        if (Input.GetKeyDown(KeyCode.Space))
            i = 0;
	}

    void FrenquecyTest()
    {
        int[] testArray = new int[] { 20, 50, 30 };
        int test = RandomFunctions.TendencyRandom(testArray);
        Debug.Log(test);
    }
    void FrenquecyTest2()
    {
        //int[] testArray = new int[] { 20, 50, 30 };
        Dictionary<object, int> testArray = new Dictionary<object, int>();
        testArray.Add(object1, 20);
        testArray.Add(object2, -30);
        testArray.Add(object3, 50);
        Transform test = (Transform)RandomFunctions.TendencyRandom(testArray);
        Debug.Log(test.name);
        //Debug.Log(MiscellaneusFunctions.allGeneratedNums.Count);
    }
    void FrenquecyTest3()
    {
        bool test = RandomFunctions.TendencyRandom(100);
        Debug.Log("FOUND = "+test);
    }

}
