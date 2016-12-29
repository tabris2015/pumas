using UnityEngine;
using System.Collections;

public class flacoController : MonoBehaviour {

	public int id;
	private Animator anim;
	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter.AddObserver (this, "p1moves");
		NotificationCenter.DefaultCenter.AddObserver (this, "p1attacks");
		NotificationCenter.DefaultCenter.AddObserver (this, "p1dies");
		anim = gameObject.GetComponent<Animator> ();
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
				
				anim.SetTrigger ("move");
			}
		}

	}
	void p1dies(NotificationCenter.Notification noti)
	{
		if(noti.data != null)
		{
			if((int)noti.data["id"] == id)
			{
				
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
				
				anim.SetTrigger ("attack");
			}
		}

	}
}
