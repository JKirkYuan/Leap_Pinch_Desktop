using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class DisplayMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PrintActiveMessage()
    {
        Debug.Log("Activated");
    }
    public void PrintDeactiveMessage()
    {
        Debug.Log("Deactivated");
    }
}
