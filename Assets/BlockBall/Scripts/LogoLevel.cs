using UnityEngine;
using System.Collections;

public class LogoLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Load Game in 1.5 seconds
        Invoke("LoadGame",1.5f);
	}

    void LoadGame() {
        Initiate.Fade("Game", Color.red, 3.0f);
    } 
    
}
