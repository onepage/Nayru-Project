using UnityEngine;
using System.Collections;

public class FoodScript : MonoBehaviour
{
    public float foodValue;
    void Start()
    {
        foodValue = 5.0f;
    }
    void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Player")
        {
            //Debug.Log("Eat this!");
            other.GetComponent<NeedsScript>().hunger.eatingValue = foodValue;
            other.GetComponent<NeedsScript>().hunger.isEating = true;
            Destroy(this.gameObject);
        }
	}
}
