using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.upgrades;
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
		  @"console.log('initializing Kong API');
          if(typeof(kongregateUnitySupport) != 'undefined'){
			console.log('success!');
			kongregateUnitySupport.initAPI('Main Camera', 'OnKongregateAPILoaded');
		  };"
		);
	}

	public void OnKongregateAPILoaded(string userInfoString) {
		Debug.Log("Kong API loaded!");
		isLoaded = true;
		OnKongregateUserInfo(userInfoString);
		submitStat("loaded", 1);
		GuildManager.PremiumSetup(10, "", " Kreds");
	}

	public void OnKongregateUserInfo(string userInfoString) {
		var info = userInfoString.Split('|');
		var userId = System.Convert.ToInt32(info[0]);
		var username = info[1];
		var gameAuthToken = info[2];
		Application.ExternalEval(
		  @"console.log('Kongregate User Info: ' + username + ', userId: ' + userId);"
		);

		Application.ExternalEval(@"
			kongregate.services.addEventListener('login', function(){
				var unityObject = kongregateUnitySupport.getUnityObject();
				var services = kongregate.services;
				var params=[services.getUserId(), services.getUsername(), services.getGameAuthToken()].join('|');
				unityObject.SendMessage('Main Camera', 'OnKongregateUserInfo', params);
			});"
		);
		getPurchases();
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
		Upgrade up = PremiumUpgrades.AllPremiumUps.Find(x => x.saveName.ToLower().Equals(str.ToLower()));
		if(up != null) {
			up.applyUpgrade();
			GuildManager.finalizePremiumPurchase(up);
		}
	}

	public void OnPurchaseFailure(string str) {
		Application.ExternalEval(
		  @"console.log('Purchased failed: " + str + "');"
		);
		GuildManager.showPurchaseFailure(str);
	}

	public static T[] convertTo<T>(object input) {
		return input as T[];
	} 

	public static void getPurchases() {
		PremiumUpgrades.AllPremiumUps.ForEach(x => x.revokeUpgrade());
		Application.ExternalEval(@"
			console.log('getting premium purchases...');
			kongregate.mtx.requestUserItemList(null, function(result) {
				var unityObject = kongregateUnitySupport.getUnityObject();
				var results = [];
				if(result.success) {
					console.log('applying purchases...');
					for(var i = 0; i < result.data.length; i++) {
						unityObject.SendMessage('Main Camera', 'OnPurchaseSuccess', result.data[i].identifier);
					}
				}
				else {
					console.log('failed!');
				}
			});
		");
	}

	public static void submitStat(string stat, int value) {
#if UNITY_EDITOR
		if(value < 0) throw new System.Exception("Kong Statistics value for " + stat + " was less than 0!");
#endif
		if(value < 0) return;
		Application.ExternalCall("kongregate.stats.submit", stat, value);
	}
}