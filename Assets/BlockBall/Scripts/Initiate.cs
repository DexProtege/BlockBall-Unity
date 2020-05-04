using UnityEngine;
using System.Collections;

public static class Initiate {
	public static void Fade (string scene,Color col,float damp){
        //Creates a fader object with fader component
        GameObject init = new GameObject ();
		init.name = "Fader";
		init.AddComponent<Fader> ();
		Fader scr = init.GetComponent<Fader> ();
		scr.fadeDamp = damp;
		scr.fadeScene = scene;
		scr.fadeColor = col;
		scr.start = true;
	}
}
