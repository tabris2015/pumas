using UnityEngine;
using System.Collections;

public class keyShop : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("g")) // seleccionar gordo
		{
			Hashtable shopData = new Hashtable ();
			shopData ["end"] = false;
			shopData ["help"] = true;
			shopData ["type"] = 1;

			NotificationCenter.DefaultCenter.PostNotification (this, "shop", shopData);

		}
		if (Input.GetKeyDown("f")) 
		{
			Hashtable shopData = new Hashtable ();
			shopData ["end"] = false;
			shopData ["help"] = true;
			shopData ["type"] = 0;

			NotificationCenter.DefaultCenter.PostNotification (this, "shop", shopData);
		}
		if (Input.GetKeyDown("a")) // comprar 1 gordo
		{
			Hashtable shopData = new Hashtable ();
			shopData ["end"] = false;
			shopData ["help"] = false;
			shopData ["type"] = 1;
			shopData ["quantity"] = 1;
			shopData ["buy"] = true;

			NotificationCenter.DefaultCenter.PostNotification (this, "shop", shopData);

		}
		if (Input.GetKeyDown("s")) // comprar 1 flaco
		{
			Hashtable shopData = new Hashtable ();
			shopData ["end"] = false;
			shopData ["help"] = false;
			shopData ["type"] = 0;
			shopData ["quantity"] = 1;
			shopData ["buy"] = true;

			NotificationCenter.DefaultCenter.PostNotification (this, "shop", shopData);
		}
		if (Input.GetKeyDown("z")) // comprar 1 flaco
		{
			Hashtable shopData = new Hashtable ();
			shopData ["end"] = true;


			NotificationCenter.DefaultCenter.PostNotification (this, "shop", shopData);
		}
	}
}
