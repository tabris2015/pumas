using UnityEngine;
using System.Collections;

public class moveto : MonoBehaviour {
	public Transform goal;
	private NavMeshAgent agent;
	public bool changed;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		agent.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void moveTo(Transform goalTo)
	{
		agent.destination = goalTo.position;
		Debug.Log("moving");
	}
}
