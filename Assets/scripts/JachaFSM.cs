using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 
/// </summary>
namespace JachaFSM
{
    // delegates for using the fms
    delegate bool FSM_EventFnc();   // function for events
    delegate void FSM_Action();     // fuction for actions
    // classes for the fsm
    /// <summary>
    /// Base class for transition, it has a delegate to a event handler
    /// and the state to which the FSM has to go.
    /// </summary>
    class Transition
    {
        /// <summary>
        /// Constructor for transition
        /// </summary>
        /// <param name="evtFnc">Name of the event handler function</param>
        /// <param name="next">Next state for the transition</param>
        public Transition(FSM_EventFnc evtFnc, int next)
        {
            EventFnc = evtFnc;
            NextState = next;
        }
        // properties
        /// <summary>
        /// Delegate for event handler, if the event has occured, it will return true.
        /// </summary>
        public FSM_EventFnc EventFnc { get; set; }
        /// <summary>
        /// A simple int for the next state, this can be implemented via enums.
        /// </summary>
        public int NextState { get; set; }
    }
    class State
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public State() { }
        /// <summary>
        /// Constructor for a new State
        /// </summary>
        /// <param name="action">Function for state action</param>
        /// <param name="trans">Transition list for given state</param>
        public State(FSM_Action action, List<Transition> trans, float timeout=0.0f)
        {
            Action = action;
            Trans = trans;
            Timeout = timeout;
        }
        /// <summary>
        /// Delegate for State action
        /// </summary>
        public FSM_Action Action { get; set; }
        /// <summary>
        /// List of transitions for the state.
        /// </summary>
        public List<Transition> Trans { get; set; }
        public float Timeout;
    }
    /// <summary>
    /// Class for managing the state machine, it has to be filled with a list of states and
    /// a list of transitions for every state
    /// </summary>
    class StateMachine
    {
        // constructors
        /// <summary>
        /// Constructor for the state machine
        /// </summary>
        /// <param name="states">List of states of the FSM</param>
        /// <param name="initState">Initial state </param>
        /// <param name="en">Enable the FSM, default: true</param>
        public StateMachine(List<State> states, int initState,  bool en = true)
        {
            Enabled = en;
            CurrentState = initState;
            StateChanged = true;
            this.states = states;
            trans = states[CurrentState].Trans;
            TimeToExit = states[CurrentState].Timeout;
        }
        /// <summary>
        /// Verify if a transition is necessary, if so, update the state and needed variables
        /// </summary>
        public void UpdateState()
        {
            int _n;
            var _t = trans;
            _n = _t.Count;
            for (int i = 0; i < _n; i++)
            {
                if(_t[i].EventFnc())
                {
                    CurrentState = _t[i].NextState;
                    trans = states[CurrentState].Trans;
                    Elapsed = false;
                    TimeToExit = states[CurrentState].Timeout;
                    StateChanged = true;
                    break;
                }
            }
        }
        /// <summary>
        /// Run the state machine, updating the state if necessary and doing the actions
        /// </summary>
        public void Run()
        {
            if (!Enabled)
            {
                return;
            }
            if(states[CurrentState].Timeout != 0.0f)
            {
                TimeToExit -= Time.deltaTime;
                if (TimeToExit <= 0.0f)
                {
                    Elapsed = true;
                    Debug.Log("time!");
                }
            }
            UpdateState();
            states[CurrentState].Action();
        }
        public void Enable()
        {
            Enabled = true;
        }
        public void Disable()
        {
            Enabled = false;
        }

        // properties
        bool Enabled { get; set; }
        public float TimeToExit { get; set; }
        public bool Elapsed { get; set; }
        int CurrentState { get; set; }
        public bool StateChanged { get; set; }
        List<State> states { get; set; }
        List<Transition> trans { get; set; }

    }
        
    }
