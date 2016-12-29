using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JachaFSM;

public class SphinxManager : MonoBehaviour {
	
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
	bool ready = true;
	bool finished = false;
	bool restart = false;

	enum SphinxState
	{
		TEAMSEL,
		T1BUYIN,
		T1ORDER
	}

	List<Transition> TEAMSEL_trans;
	List<Transition> T1BUYIN_trans;
	List<Transition> T1ORDER_trans;

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

		TEAMSEL_trans = new List<Transition>()
		{
			new Transition(isTeamSelected, (int)SphinxState.T1BUYIN)
		};
		T1BUYIN_trans = new List<Transition>()
		{
			new Transition(isRunout, (int)SphinxState.T1ORDER),
			new Transition(isBack, (int)SphinxState.TEAMSEL)
		};
		T1ORDER_trans = new List<Transition>()
		{
			new Transition(isReady, (int)SphinxState.T1ORDER)
		};


		sphinxFSM_states = new List<State>()
		{
			new State(TEAMSEL_action, TEAMSEL_trans),
			new State(T1BUYIN_action, T1BUYIN_trans),
			new State(T1ORDER_action, T1ORDER_trans, 20.0f)
		};

		sphinxFSM = new StateMachine(sphinxFSM_states, (int)SphinxState.TEAMSEL);

		NotificationCenter.DefaultCenter.AddObserver (this, "turnReady");
	}
	
	// Update is called once per frame
	void Update () {
		sphinxFSM.Run ();
	}

	bool isTeamSelected () {
		return teamSelected;
	}

	bool isRunout () {
		return runout;
	}

	bool isBack () {
		return back;
	}

	bool isReady () {
		return ready;
	}

	bool isFinished () {
		return finished;
	}

	bool isRestart () {
		return restart;
	}

	void TEAMSEL_action () {
		if (sphinxFSM.StateChanged) {
			back = false;
			runout = false;
			money = 80f;
			UnitySphinx.Stop();
			UnitySphinx.Init(jsgf, kws);
			UnitySphinx.Run();
			sphinxFSM.StateChanged = false;
		}

		str = UnitySphinx.DequeueString ();
		print ("listening for keyword");
		if (str != "") {
			print (str);
			print ("teamsel action");
			char[] delimChars = {' '};
			string[] cmd = str.Split (delimChars);
            Hashtable hashtable = new Hashtable();
            if (cmd[0] == "AWATIRI") {
                hashtable.Add("player", cmd[0]);
            }
            else {
                hashtable.Add("player", cmd[0]);
            }
            NotificationCenter.DefaultCenter.PostNotification(this, "help", hashtable);
            teamSelected = true;
		}
	}

	void T1BUYIN_action () {
		if (sphinxFSM.StateChanged) {
			UnitySphinx.Stop();
			jsgf = Application.dataPath + "/Resources/UnitySphinx/model/aymara/buying.gram";
			UnitySphinx.Init(jsgf, kws);
			UnitySphinx.Run();
			UnitySphinx.SetSearchModel(UnitySphinx.SearchModel.jsgf);
			sphinxFSM.StateChanged = false;
		}

		str = UnitySphinx.DequeueString ();
		print ("listening for keyword");
		if (str != "") {
			print (str);
			print ("t1buyin action");
            char[] delimChars = { ' ' };
            string[] cmd = str.Split(delimChars);
            Hashtable hashtable = new Hashtable();
            if ((cmd[0] == "YUSPAJARA") && (money < 100f)) {
                hashtable.Add("end", true);
                NotificationCenter.DefaultCenter.PostNotification(this, "shop", hashtable);
                runout = true;
            } else if (cmd[0] == "QHIPÄXA") {
                Hashtable hashtable2 = new Hashtable();
                hashtable2.Add("back", true);
                NotificationCenter.DefaultCenter.PostNotification(this, "back", hashtable2);
                back = true;
            } else if (cmd.Length == 2) {
                hashtable.Add("help", true);
                hashtable.Add("type", (int)type[cmd[0]]);
                hashtable.Add("end", false);
                NotificationCenter.DefaultCenter.PostNotification(this, "shop", hashtable);
            } else if(cmd.Length > 3) {
                bool buy = true;
                bool polite = false;
                float sign = 1f;
                if (cmd[3] == "KUTIYAÑA") {
                    buy = false;
                    sign = -1f;
                } else if (cmd.Length > 4) {
                    polite = true;
                }
                float quantity = (float)numbers[cmd[0]];
                hashtable.Add("help", false);
                hashtable.Add("buy", buy);
                hashtable.Add("polite", polite);
                hashtable.Add("type", (int)type[cmd[1]]);
                hashtable.Add("quantity", (int)quantity);
                hashtable.Add("end", false);
                float spent = quantity * sign * 10 * (1 + (int)type[cmd[1]]);
                if ((int)type[cmd[1]] == 0) {
                    thincount = thincount + sign * quantity;
                } else {
                    fatcount = fatcount + sign * quantity;
                }
                if (!((thincount < 0f) || (fatcount < 0f))) {
                    NotificationCenter.DefaultCenter.PostNotification(this, "shop", hashtable);
                    if (spent < money) {
                        money -= spent;
                        if (money < min_money) {
                            Hashtable hashtable2 = new Hashtable();
                            hashtable2.Add("end", true);
                            NotificationCenter.DefaultCenter.PostNotification(this, "shop", hashtable2);
                            runout = true;
                        }
                    }
                }
            }
            print (money);
		}
	}

	void T1ORDER_action () {
		if (sphinxFSM.StateChanged) {
			UnitySphinx.Stop();

			sphinxFSM.StateChanged = false;
			ready = false;
		}

	}

	void turnReady(NotificationCenter.Notification noti)
	{
		ready = true;
	}
}
