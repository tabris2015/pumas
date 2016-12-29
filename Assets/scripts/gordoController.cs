using UnityEngine;
using System.Collections;

public class gordoController : MonoBehaviour {
	public int id;
	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter.AddObserver (this, "p1moves");
		NotificationCenter.DefaultCenter.AddObserver (this, "p1attacks");
		NotificationCenter.DefaultCenter.AddObserver (this, "p1dies");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void p1moves(NotificationCenter.Notification noti)
	{
		if(noti.data != null)
		{
			if((int)noti.data["id"] == id)
			{
				Animator anim = transform.GetComponent<Animator> ();
				anim.SetTrigger ("move");
				Debug.Log ("walk " + id.ToString ());
			}
		}

	}
	void p1dies(NotificationCenter.Notification noti)
	{
		if(noti.data != null)
		{
			if((int)noti.data["id"] == id)
			{
				Animator anim = transform.GetComponent<Animator> ();
				anim.SetTrigger ("die");
			}
		}

	}
	void p1attacks(NotificationCenter.Notification noti)
	{
		if(noti.data != null)
		{
			if((int)noti.data["id"] == id)
			{
				Animator anim = transform.GetComponent<Animator> ();
				anim.SetTrigger ("attack");

			}
		}

	}
}
