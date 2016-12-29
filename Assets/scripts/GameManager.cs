using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JachaFSM; // clases para la maquina de estados

public class GameManager : MonoBehaviour {
    public GameObject player1Object;
    public GameObject player2Object;

    char key = '0';
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
        // set up the state machine
        // transitions  events: AK --- BK
        P1TURN_trans = new List<Transition>()
        {
            new Transition(timePassed, (int)ManagerState.P2TURN),
            new Transition(p1fire, (int)ManagerState.P1SHOT)
        };
        P1SHOT_trans = new List<Transition>()
        {
            new Transition(damage1Taken, (int)ManagerState.P2TURN)
        };

        P2TURN_trans = new List<Transition>()
        {
            new Transition(timePassed, (int)ManagerState.P1TURN),
            new Transition(p2fire, (int)ManagerState.P2SHOT)
        };
        P2SHOT_trans = new List<Transition>()
        {
            new Transition(damage2Taken, (int)ManagerState.P1TURN)
        };

        managerFSM_states = new List<State>()
        {
            new State(P1TURN_action, P1TURN_trans, 4.0f),
            new State(P1SHOT_action, P1SHOT_trans),
            new State(P2TURN_action, P2TURN_trans, 4.0f),
            new State(P2SHOT_action, P2SHOT_trans)
        };
        // create the state machine
        managerFSM = new StateMachine(managerFSM_states, (int)ManagerState.P1TURN);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("a")) this.key = 'a';
        else if (Input.GetKeyDown("d")) this.key = 'd';
        else this.key = '0';

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
    bool p2fire()
    {
        return (this.key == 'd');
    }
    bool damage1Taken()
    {
        return true;
    }
    bool damage2Taken()
    {
        return true;
    }
    // state actions
    void P1TURN_action()
    {
        if (managerFSM.StateChanged)
        {
            Debug.Log("player1 turn");
            player1Object.GetComponent<Renderer>().material.color = Color.green;
            player2Object.GetComponent<Renderer>().material.color = Color.grey;
            managerFSM.StateChanged = false;
        }

        
    }
    void P2TURN_action()
    {
        if (managerFSM.StateChanged)
        {
            Debug.Log("player 2 turn");
            player2Object.GetComponent<Renderer>().material.color = Color.green;
            player1Object.GetComponent<Renderer>().material.color = Color.grey;
            managerFSM.StateChanged = false;
        }

    }
    void P1SHOT_action()
    {
        if (managerFSM.StateChanged)
        {
            Debug.Log("player 1 SHOT");
            player1Object.GetComponent<Renderer>().material.color = Color.yellow;
            managerFSM.StateChanged = false;
        }

    }
    void P2SHOT_action()
    {
        if (managerFSM.StateChanged)
        {
            Debug.Log("player 2 SHOT");
            player2Object.GetComponent<Renderer>().material.color = Color.yellow;
            managerFSM.StateChanged = false;
        }

    }
}
