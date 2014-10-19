using UnityEngine;
using System.Collections;

public class BackgroundGradient : MonoBehaviour {

	void Start () {
	    Gradient g;
        GradientColorKey[] gck;
        GradientAlphaKey[] gak;
        g = new Gradient();
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        gck = new GradientColorKey[2];
        gck[0].color = Color.red;
        gck[0].time = 0.0f;
        gck[1].color = Color.blue;
        gck[1].time = 1.0f;
        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        gak = new GradientAlphaKey[2];
        gak[0].alpha = 1.0f;
        gak[0].time = 0.0f;
        gak[1].alpha = 0.0f;
        gak[1].time = 1.0f;
        g.SetKeys(gck, gak);
    
        // What's the color at the relative time 0.25 (25 %) ?
        Debug.Log(g.Evaluate(0.25f));
	}
	
	void Update () {
	
	}
}
