using UnityEngine;
using System.Collections;

public class keyInput : MonoBehaviour {
	private int[] mrows = new int[]{4,1,2,3,4,5,6,7};
	private int[] mcols = new int[]{4,7,2,3,0,1,4,3};

	//private int player = 1;
	public int ficha = 0;
	//private int idx = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("a")) 
		{
			Hashtable moveData = new Hashtable ();
			moveData ["player"] = 1;
			moveData ["ficha"] = ficha;
			moveData ["fila"] = mrows[ficha];
			moveData ["columna"] = mcols[ficha];
			Debug.Log (moveData);
			NotificationCenter.DefaultCenter.PostNotification (this, "playerMove", moveData);
			ficha++;
			ficha = ficha > 7 ? 0 : ficha;
		}
		if (Input.GetKeyDown("d")) 
		{
			Hashtable moveData = new Hashtable ();
			moveData ["player"] = 2;
			moveData ["ficha"] = ficha;
			moveData ["fila"] = mrows[ficha];
			moveData ["columna"] = mcols[ficha];
			Debug.Log (moveData);
			NotificationCenter.DefaultCenter.PostNotification (this, "playerMove", moveData);
			ficha++;
			ficha = ficha > 7 ? 0 : ficha;
		}
	}
}
