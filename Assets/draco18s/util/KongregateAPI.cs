using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.util;
using UnityEngine;

public class KongregateAPI : MonoBehaviour {
	public static KongregateAPI instance;
	public bool isLoaded = false;

	public void Start() {
		if(instance == null) {
			instance = this;
		}
		else if(instance != this) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		//gameObject.name = "KongregateAPI";

		Application.ExternalEval(
		  @"if(typeof(kongregateUnitySupport) != 'undefined'){
			kongregateUnitySupport.initAPI('KongregateAPI', 'OnKongregateAPILoaded');
		  };"
		);
	}

	public void OnKongregateAPILoaded(string userInfoString) {
		Debug.Log("Kong API loaded!");
		isLoaded = true;
		OnKongregateUserInfo(userInfoString);
		GuildManager.PremiumSetup(10, "", " Kreds");
	}

	public void OnKongregateUserInfo(string userInfoString) {
		var info = userInfoString.Split('|');
		var userId = System.Convert.ToInt32(info[0]);
		var username = info[1];
		var gameAuthToken = info[2];
		Debug.Log("Kongregate User Info: " + username + ", userId: " + userId);
	}

	public static void doPurchase(string itemName) {
		Application.ExternalEval(@"
		  kongregate.mtx.purchaseItems(['" + itemName + @"'], function(result) {
			var unityObject = kongregateUnitySupport.getUnityObject();
			if (result.success) {
			  unityObject.SendMessage('Main Camera', 'OnPurchaseSuccess', '" + itemName + @"');
			} else {
			  unityObject.SendMessage('Main Camera', 'OnPurchaseFailure', '" + itemName + @"');
			}
		  });
		");
	}

	public void OnPurchaseSuccess(string str) {
		PremiumUpgrades.AllPremiumUps.Find(x => x.saveName.Equals(str)).applyUpgrade();
	}

	public void OnPurchaseFailure(string str) {

	}

	public void GetPurchasesCallback(object result) {
		if(result.GetType().IsArrayOf<string>()) {
			string[] values = convertTo<string>(result);
			for(int i = 0; i < values.Length; i++) {
				PremiumUpgrades.AllPremiumUps.Find(x => x.saveName.Equals(values[i])).applyUpgrade();
			}
		}
		else {
			Debug.Log("Callback object was not a string[]!");
		}
	}

	public static T[] convertTo<T>(object input) {
		return input as T[];
	} 

	public static void getPurchases() {
		Application.ExternalEval(@"
			kongregate.mtx.requestUserItemList(null, function(result) {
				var unityObject = kongregateUnitySupport.getUnityObject();
				var results = [];
				if(result.success) {
					for(var i:int = 0; i < result.data.length; i++) {
						var item:Object = result.data[i];
						results[i] = item.identifier;
					}
				}
				unityObject.SendMessage('Main Camera', 'GetPurchasesCallback', results);
			});
		");
	}
}