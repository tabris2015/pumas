using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using JachaFSM; // clases para la maquina de estados

public class turnManager : MonoBehaviour {
	// --

	// --
	public GameObject pumasParent;
	public GameObject flacosParent;
	public GameObject gordosParent;

	public Transform[] pumasStart;
	public Transform[] awatirisStart;

	private NavMeshAgent[] pumas;
	public Vector2[] pumasPos;
	private NavMeshAgent cPuma;
	private int cPumaIdx;
	private int nPumas;



	private NavMeshAgent[] awatiris;
	public Vector2[] awatirisPos;
	private NavMeshAgent cAwatiri;
	[SerializeField]
	private int cAwatiriIdx;
	private int nAwatiris;

	private List<int> dead;
	private Vector2 boardSize;

	public GameObject tableroParent;
	private List<Transform[]> tablero;

	private Hashtable moveData;
	char key = '0';
	bool action = false;
	enum ManagerState
	{
		P1TURN,
		P1SHOT,
		P2TURN,
		P2SHOT
	}
	// state transitions definition
	List<Transition> P1TURN_trans;
	List<Transition> P1SHOT_trans;
	List<Transition> P2TURN_trans;
	List<Transition> P2SHOT_trans;

	// state list
	List<State> managerFSM_states;
	StateMachine managerFSM;
	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter.AddObserver (this, "game");
		// set up the objects for gameplay
		// pieces
		dead = new List<int>();
		NavMeshAgent[] flacos = flacosParent.GetComponentsInChildren<NavMeshAgent>();
		NavMeshAgent[] gordos = gordosParent.GetComponentsInChildren<NavMeshAgent> ();

		pumas = pumasParent.GetComponentsInChildren<NavMeshAgent>();
		nPumas = pumas.Length;

		nAwatiris = GameData.NGordos + GameData.NFlacos;
		Debug.Log ("fichas " + nAwatiris.ToString ());
		awatiris = new NavMeshAgent[nAwatiris];
		cPuma = pumas [0];
		cAwatiri = awatiris [0];
		int k = 0;
		for(int g = 0; g < GameData.NGordos; g++)
		{
			awatiris [g] = gordos [g];
		}
		for(int g = GameData.NGordos; g < 8; g++)
		{
			gordos [g].gameObject.SetActive (false);
		}
		for(int f = 0; f < GameData.NGordos; f++)
		{
			flacos [f].gameObject.SetActive (false);
		}
		for(int f = GameData.NGordos; f < (GameData.NGordos + GameData.NFlacos); f++)
		{
			awatiris [f] = flacos [f];
		}
		for(int f = (GameData.NGordos + GameData.NFlacos); f < 8;f++)
		{
			flacos [f].gameObject.SetActive (false);
		}

		/*while(GameData.NGordos > 0)
		{
			awatiris [k] = gordos [k];
			GameData.NGordos--;
			k++;
		}
		while(GameData.NFlacos > 0)
		{
			flacos [k].gameObject.GetComponentInChildren<Transform> ().gameObject.SetActive (true);
			awatiris [k] = flacos [k];
			GameData.NFlacos--;
			k++;
		}
*/

		int idx = 0;
		pumasPos = new Vector2[nPumas];
		awatirisPos = new Vector2[nAwatiris];

		//int rowIdx = pumasStart [0].parent.GetSiblingIndex ();
		// initial Position for pieces
		//Debug.Log("index: " + pumasStart[0].parent.GetSiblingIndex().ToString() + pumasStart[0].parent.name);
		for(idx = 0; idx < nPumas;idx++)
		{
			pumas [idx].destination = pumasStart [idx].position;
			pumasPos [idx] = new Vector2 (pumasStart [idx].parent.GetSiblingIndex (), pumasStart [idx].GetSiblingIndex ());

		}

		for(idx = 0; idx < nAwatiris;idx++)
		{
			awatiris [idx].destination = awatirisStart [idx].position;
			awatirisPos [idx] = new Vector2 (awatirisStart [idx].parent.GetSiblingIndex (), awatirisStart [idx].GetSiblingIndex ());

		}

		//Debug.Log ("pumas: " + nPumas.ToString ());
		Debug.Log ("awatiris: " + nAwatiris.ToString ());

		// SET UP TABLE

		//Transform[] tempRows = new Transform[tableroParent.transform.childCount]; //tableroParent.GetComponentsInChildren<Transform> ();
		int i = 0;
		tablero = new List<Transform[]>();

		foreach(Transform row in tableroParent.transform)
		{
			Transform[] tempRow = new Transform[row.transform.childCount];
			int j = 0;
			foreach(Transform col in row.transform)
			{
				tempRow [j] = col;
				j++;
			}
			tablero.Add(tempRow);
			i++;
		}

		int nRows = tablero.Count;
		int nCols = tablero[0].Length;

		boardSize = new Vector2 (nRows, nCols);
		//Debug.Log (nRows);
		//Debug.Log(nCols);
		//Debug.Log (tempRows.Length);


		//Debug.Log ("tablero: " + tablero.Count.ToString() + " x " + tablero[0].Length);
		//Debug.Log ("elemento: " + tablero [0] [0].GetType());
		//

		// set up the state machine

		// notifications
		NotificationCenter.DefaultCenter.AddObserver(this, "playerMove");
		// transitions  events: AK --- BK
		P1TURN_trans = new List<Transition>()
		{
			new Transition(timePassed, (int)ManagerState.P2TURN),
			new Transition(playerAction, (int)ManagerState.P1SHOT)
		};
		P1SHOT_trans = new List<Transition>()
		{
			new Transition(timePassed, (int)ManagerState.P2TURN),
			new Transition(arrived, (int)ManagerState.P2TURN)
		};

		P2TURN_trans = new List<Transition>()
		{
			new Transition(timePassed, (int)ManagerState.P1TURN),
			new Transition(playerAction, (int)ManagerState.P2SHOT)
		};
		P2SHOT_trans = new List<Transition>()
		{
			new Transition(timePassed, (int)ManagerState.P2TURN),
			new Transition(arrived2, (int)ManagerState.P1TURN)
		};

		managerFSM_states = new List<State>()
		{
			new State(P1TURN_action, P1TURN_trans, 20.0f),
			new State(P1SHOT_action, P1SHOT_trans,10.0f),
			new State(P2TURN_action, P2TURN_trans, 20.0f),
			new State(P2SHOT_action, P2SHOT_trans, 10.0f)
		};
		// create the state machine
		managerFSM = new StateMachine(managerFSM_states, (int)ManagerState.P1TURN);
	}

	// Update is called once per frame
	void Update () {
		managerFSM.Run();
	}

	// event functions
	bool timePassed ()
	{
		return managerFSM.Elapsed;
	}
	bool p1fire()
	{
		return (this.key == 'a');
	}
	bool playerAction()
	{
		return action;
	}
	bool p2fire()
	{
		return (this.key == 'd');
	}
	bool arrived()
	{
		bool arr = new bool (); 
		try 
		{
			arr = cAwatiri.remainingDistance < 0.5f;
		} 
		catch (Exception ex) 
		{
			arr = false;
		}
		return arr;
	}
	bool arrived2()
	{
		return cPuma.remainingDistance < 0.5f;
	}
	// state actions
	void P1TURN_action()
	{
		if (managerFSM.StateChanged)
		{
			Debug.Log("player1 turn");
			NotificationCenter.DefaultCenter.PostNotification (this, "turnReady");
			//player1Object.GetComponent<Renderer>().material.color = Color.green;
			//player2Object.GetComponent<Renderer>().material.color = Color.grey;
			managerFSM.StateChanged = false;
			action = false;
		}


	}
	void P2TURN_action()
	{
		if (managerFSM.StateChanged)
		{
			Debug.Log("player 2 turn");
			NotificationCenter.DefaultCenter.PostNotification (this, "turnReady");
			//player2Object.GetComponent<Renderer>().material.color = Color.green;
			//player1Object.GetComponent<Renderer>().material.color = Color.grey;
			managerFSM.StateChanged = false;
			action = false;
		}

	}
	// p1 awatiris

	void P1SHOT_action()
	{
		if (managerFSM.StateChanged)
		{
			
			// si los datos son correctos para nuestro jugador
			int player = (int)moveData["player"];
			Debug.Log (player);
			if((int)moveData["player"] == 1)
			{
				
				int goalRow = (int)((float)moveData ["row"]);
				int goalCol = (int)((float)moveData ["col"]);
				int ficha = (int)moveData ["piece"] % nAwatiris;
				Debug.Log ("player " + moveData["player"] + " move " + ficha.ToString() + " to " + goalRow.ToString() +
					"," + goalCol.ToString());

				Vector2 target = new Vector2 (goalRow, goalCol);
				cAwatiri = awatiris [ficha];
				if(movementIsValid(awatirisPos[ficha], target, 10))
				{
					awatiris [ficha].destination = tablero[goalRow][goalRow].position;
					NotificationCenter.DefaultCenter.PostNotification (this, "p1moves");
					awatirisPos [ficha] = new Vector2 (goalRow, goalCol);
				}
				else
				{
					Debug.Log ("invalid Movement!");
					NotificationCenter.DefaultCenter.PostNotification (this, "invalidMove");
				}
			}
			//Debug.Log(moveData["ficha"]);
			//player1Object.GetComponent<Renderer>().material.color = Color.yellow;

			managerFSM.StateChanged = false;
			action = false;
		}

	}
	// para los pumas
	void P2SHOT_action()
	{
		if (managerFSM.StateChanged)
		{
			
			//player2Object.GetComponent<Renderer>().material.color = Color.yellow;
			managerFSM.StateChanged = false;
			// si los datos son correctos para nuestro jugador
			int player = (int)moveData["player"];
			Debug.Log (player);
			if((int)moveData["player"] == 2)
			{

				int goalRow = (int)((float)moveData ["row"]);
				int goalCol = (int)((float)moveData ["col"]);
				int ficha = (int)moveData ["piece"] % nPumas;
				Debug.Log ("player " + moveData["player"] + " move " + ficha.ToString() + " to " + goalRow.ToString() +
					"," + goalCol.ToString());

				Vector2 target = new Vector2 (goalRow, goalCol);
				cPuma = pumas [ficha];
				if(movementIsValid(pumasPos[ficha], target, 10))
				{
					pumas [ficha].destination = tablero[goalRow][goalRow].position;
					NotificationCenter.DefaultCenter.PostNotification (this, "p2move");
					pumasPos [ficha] = new Vector2 (goalRow, goalCol);
				}
				else
				{
					Debug.Log ("invalid Movement!");
					NotificationCenter.DefaultCenter.PostNotification (this, "invalidMove");
				}
			}
			//Debug.Log(moveData["ficha"]);
			//player1Object.GetComponent<Renderer>().material.color = Color.yellow;

			managerFSM.StateChanged = false;
			action = false;
			action = false;
		}

	}

	// notifications
	void game(NotificationCenter.Notification noti)
	{
		action = true;
		if(noti.data != null)
		{
			moveData = noti.data;
			if((int)moveData["player"] == 1)
			{
				cAwatiriIdx = (int)moveData ["piece"];
			}
		}
		Debug.Log("moved!");
	}


	public List<Vector2> getValidMovements(Vector2 position, Vector2 size, int type)
	{
		return new List<Vector2>();
	}
	bool movementIsValid(Vector2 pos, Vector3 target, int range)
	{
		// if target is outside the board
		if(target.x >= boardSize.x || target.y >= boardSize.y)
		{
			return false;
		}
		// if target is outside of range
		if((target.x > (pos.x + range)) || 
			(target.x < (pos.x - range)) ||
			(target.y > (pos.y + range)) || 
			(target.y < (pos.y - range)))
		{
			return false;
		}
		return true;
	}
}
