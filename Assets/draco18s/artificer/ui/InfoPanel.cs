using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
	public Text Title;
	public Text Level;
	public Text PricePer;
	public Text Upgrade;
	public Text Downgrade;
	public Text StorageTxt;
	public Text OutNum;
	public Text OutNumb;
	public Text InNum1;
	public Text InNum1b;
	public Text InNum2;
	public Text InNum2b;
	public Text InNum3;
	public Text InNum3b;
	public Text VendText;
	public Text VendNum;
	public Text ApprText;
	//public Text BuildText;
	public Text BuildNum;
	public Text MagnitudeNum;
	public Text StartVend;

	public Toggle ProduceToggle;
	public Toggle ConsumeToggle;
	public Toggle SellToggle;
	public Toggle BuildToggle;
	public Button UpgradeBtn;
	public Button DowngradeBtn;
	public Button ConfDowngradeBtn;
	public Transform ConsumersDock;

	public void SetOutputNum(string t) {
		OutNum.text = OutNumb.text = t;
	}

	public void SetInputINum(int i, bool vis) {
		SetInputINum(i, vis, "");
	}

	public void SetInputINum(int i, bool vis, string t) {
		switch(i) {
			case 1:
				InNum1.transform.gameObject.SetActive(vis);
				InNum1.text = InNum1b.text = t;
				break;
			case 2:
				InNum2.transform.gameObject.SetActive(vis);
				InNum2.text = InNum2b.text = t;
				break;
			case 3:
				InNum3.transform.gameObject.SetActive(vis);
				InNum3.text = InNum3b.text = t;
				break;
		}
	}
}
