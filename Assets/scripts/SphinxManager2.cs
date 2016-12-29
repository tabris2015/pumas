using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JachaFSM;

public class SphinxManager2 : MonoBehaviour {
	
    Hashtable numbers, cardinals, colors, type;

    string str;
	string jsgf;
	string kws;

    int boardsize = 7;

	bool teamSelected = false;
	float money = 80f;
	float min_money = 10f;
    float thincount = 0f;
    float fatcount = 0f;
	bool runout = false;
	bool back = false;
	bool ready = true; // ojo
	bool finished = false;
	bool restart = false;

	enum SphinxState
	{
		T1ORDER,
		T2ORDER,
		WAITING
	}

	List<Transition> T1ORDER_trans;
	List<Transition> T2ORDER_trans;
	List<Transition> WAITING_trans;

	List<State> sphinxFSM_states;
	StateMachine sphinxFSM;

	// Use this for initialization
	void Start () {
        numbers = new Hashtable();
        numbers.Add("MAYA", 1f);
        numbers.Add("MÄ", 1f);
        numbers.Add("PAYA", 2f);
        numbers.Add("PÄ", 2f);
        numbers.Add("KIMSA", 3f);
        numbers.Add("PUSI", 4f);
        numbers.Add("PHISQA", 5f);
        numbers.Add("SUXTA", 6f);
        numbers.Add("PAQALLQU", 7f);
        numbers.Add("KIMSAQALLQU", 8f);
        numbers.Add("LLÄTUNKA", 9f);
        numbers.Add("TUNKA", 10f);

        cardinals = new Hashtable();
        cardinals.Add("MAYÏRI", 0f);
        cardinals.Add("PAYÏRI", 1f);
        cardinals.Add("KIMSÏRI", 2f);
        cardinals.Add("PUSÏRI", 3f);
        cardinals.Add("PHISQÏRI", 4f);
        cardinals.Add("SUXTÏRI", 5f);
        cardinals.Add("PAQALLQÜRI", 6f);
        cardinals.Add("KIMSAQALLQÜRI", 7f);
        cardinals.Add("LLÄTUNKÏRI", 8f);
        cardinals.Add("TUNKÏRI", 9f);

        colors = new Hashtable();
        colors.Add("CH’IYARA", 0f);
        colors.Add("CH’UMPHI", 1f);
        colors.Add("WILA", 2f);
        colors.Add("NUWALA", 3f);
        colors.Add("Q’ILLU", 4f);
        colors.Add("CH’UXÑA", 5f);
        colors.Add("LARAMA", 6f);
        colors.Add("MURTURIYA", 7f);
        colors.Add("UQI", 8f);
        colors.Add("JANQ’U", 9f);

        type = new Hashtable();
        type.Add("TUXU", 0);
        type.Add("LUNQHU", 1);

        jsgf = Application.dataPath + "/Resources/UnitySphinx/model/aymara/buying.gram";
		kws = Application.dataPath + "/Resources/UnitySphinx/model/aymara/teamselection.list";
		UnitySphinx.Init(jsgf, kws);
		UnitySphinx.Run();

		T1ORDER_trans = new List<Transition>()
		{
			new Transition(isReady, (int)SphinxState.T2ORDER),
			new Transition(isFinished, (int)SphinxState.WAITING)
		};
		T2ORDER_trans = new List<Transition>()
		{
			new Transition(isReady, (int)SphinxState.T1ORDER),
			new Transition(isFinished, (int)SphinxState.WAITING)
		};
		WAITING_trans = new List<Transition>()
		{
			new Transition(isRestart, (int)SphinxState.WAITING)
		};

		sphinxFSM_states = new List<State>()
		{
			new State(T1ORDER_action, T1ORDER_trans, 20.0f),
			new State(T2ORDER_action, T2ORDER_trans, 20.0f),
			new State(WAITING_action, WAITING_trans)
		};

		sphinxFSM = new StateMachine(sphinxFSM_states, (int)SphinxState.T1ORDER);

		NotificationCenter.DefaultCenter.AddObserver (this, "turnReady");
	}
	
	// Update is called once per frame
	void Update () {
		sphinxFSM.Run ();
	}

	bool isReady () {
		bool temp = ready;
		ready = false;
		return temp;
	}

	bool isFinished () {
		return finished;
	}

	bool isRestart () {
		return restart;
	}



	void T1ORDER_action () {
		if (sphinxFSM.StateChanged) {
			UnitySphinx.Stop();
			jsgf = Application.dataPath + "/Resources/UnitySphinx/model/aymara/t1order.gram";
			UnitySphinx.Init(jsgf, kws);
			UnitySphinx.Run();
			UnitySphinx.SetSearchModel(UnitySphinx.SearchModel.jsgf);
			sphinxFSM.StateChanged = false;

		}

		str = UnitySphinx.DequeueString ();
		print ("listening for keyword");
		if (str != "") {
			print (str);
            print("t1order action");
            char[] delimChars = { ' ' };
            string[] cmd = str.Split(delimChars);
            bool overflow = false;
            Hashtable hashtable = new Hashtable();
			hashtable.Add ("piece", (int)((float)cardinals [cmd [0]]));
			float row = (int)((float)colors [cmd [2]]);
            hashtable.Add("row", row);
			float col = (int)((float)numbers [cmd [3]]);
            hashtable.Add("col", col);
            if (((int)row > boardsize) || ((int)col > boardsize)) {
                overflow = true;
            }
            hashtable.Add("overflow", overflow);
            hashtable.Add("player", 1);
            NotificationCenter.DefaultCenter.PostNotification(this, "game", hashtable);
        }
	}

	void T2ORDER_action () {
		if (sphinxFSM.StateChanged) {
			restart = false;

			UnitySphinx.Stop();
			jsgf = Application.dataPath + "/Resources/UnitySphinx/model/aymara/t2order.gram";
			UnitySphinx.Init(jsgf, kws);
			UnitySphinx.Run();
			UnitySphinx.SetSearchModel(UnitySphinx.SearchModel.jsgf);
			sphinxFSM.StateChanged = false;

		}

		str = UnitySphinx.DequeueString ();
		print ("listening for keyword");
		if (str != "") {
            print(str);
            print("t2order action");
            char[] delimChars = { ' ' };
            string[] cmd = str.Split(delimChars);
            bool overflow = false;
            Hashtable hashtable = new Hashtable();
			hashtable.Add("piece", (int)((float)cardinals[cmd[0]]));
			float row = (int)((float)colors[cmd[2]]);
            hashtable.Add("row", row);
			float col = (int)((float)numbers [cmd [3]]);
            hashtable.Add("col", col);
            if (((int)row > boardsize) || ((int)col > boardsize)) {
                overflow = true;
            }
            hashtable.Add("overflow", overflow);
            hashtable.Add("player", 2);
            NotificationCenter.DefaultCenter.PostNotification(this, "game", hashtable);
        }
	}

	void WAITING_action () {
		if (sphinxFSM.StateChanged) {
			teamSelected = false;
			UnitySphinx.Stop();

			sphinxFSM.StateChanged = false;
		}
		if (str != "") {
			print ("waiting");

		}
	}

	void turnReady(NotificationCenter.Notification noti)
	{
		ready = true;
	}
}
