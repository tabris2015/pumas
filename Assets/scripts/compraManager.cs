using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class compraManager : MonoBehaviour {

	public GameObject helpGordo;
	public GameObject helpFlaco;

	public Text coinsLabel;

	public GameObject gordoPreview;
	public GameObject flacoPreview;

	private int coins = 80;
	private int valueG = 20;
	private int valueF = 10;

	private bool end;
	public int gordos = 0;
	public int flacos = 0;
	public int pumas = 4; // defecto


	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter.AddObserver (this, "shop");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void shop( NotificationCenter.Notification notification)
	{
		Hashtable shopData = notification.data;
		if(shopData != null)
		{
			int type = new int ();
			int qty = new int ();
			if((bool)shopData["end"])
			{
				// siguiente escena
				Debug.Log("end");
				GameData.NPumas = pumas;
				GameData.NGordos = gordos;
				GameData.NFlacos = flacos;
				SceneManager.LoadScene ("main");
			}
			else if((bool)shopData["help"]){
				// mostrar ayuda del guerrero
				Debug.Log("show help");
				type = (int)shopData["type"];
				if(type == 1)
				{
					helpFlaco.SetActive (false);
					flacoPreview.SetActive (false);
					helpGordo.SetActive (true);
					gordoPreview.SetActive (true);
				}
				else
				{
					helpGordo.SetActive (false);
					gordoPreview.SetActive (false);
					helpFlaco.SetActive (true);
					flacoPreview.SetActive (true);
				}
			}
			else 
			{
				type = (int)shopData["type"];
				qty = (int)shopData ["quantity"];
				if((bool)shopData["buy"])
				{
					if(type == 1) // gordos
					{
						Debug.Log ("buy gordo");
						gordos += qty;
						coins -= qty * valueG;
					}
					else
					{
						flacos += qty;
						coins -= qty * valueF;
						Debug.Log ("buy flaco");
					}
				}
				else
				{
					if(type == 1) // gordos
					{
						gordos -= qty;
						coins += qty * valueG;
						Debug.Log ("return gordo");
					}
					else
					{
						flacos -= qty;
						coins += qty * valueF;
						Debug.Log ("return flaco");
					}
				}
				// actualizar label
				coinsLabel.text = coins.ToString();
			}
		}
	}
}
