
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.ui;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.draco18s.util {
	public class DataAccess {
		[DllImport("__Internal")]
		private static extern void SyncFiles();

		[DllImport("__Internal")]
		private static extern void WindowAlert(string message);
		private static readonly string saveFile = "{0}/savedata.dat";

		public static void DeleteSave() {
			string dataPath = string.Format(saveFile, Application.persistentDataPath);

			try {
				if(File.Exists(dataPath)) {
					File.Delete(dataPath);
				}
			}
			catch(Exception e) {
				PlatformSafeMessage("Failed to delete: " + e.Message);
			}
		}

		public static void Save(PlayerInfo gameDetails) {
			Main.reportKongStats();
			//PlatformSafeMessage("Saving is not supported on WebGL currently");
			string dataPath = string.Format(saveFile, Application.persistentDataPath);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream;

			try {
				if(File.Exists(dataPath)) {
					File.Delete(dataPath);
				}
				fileStream = File.Create(dataPath);

				binaryFormatter.Serialize(fileStream, gameDetails);
				fileStream.Close();

				if(Application.platform == RuntimePlatform.WebGLPlayer) {
					SyncFiles();
				}
				//GuiManager.ShowNotification(new NotificationItem("Saved!", "", GuiManager.instance.checkOn));
			}
			catch(Exception e) {
				PlatformSafeMessage("Failed to Save: " + e.Message);
			}
		}

		public static bool Load() {
			PlayerInfo gameDetails = null;
			string dataPath = string.Format(saveFile, Application.persistentDataPath);

			try {
				if(File.Exists(dataPath)) {
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					FileStream fileStream = File.Open(dataPath, FileMode.Open);

					gameDetails = (PlayerInfo)binaryFormatter.Deserialize(fileStream);
					fileStream.Close();
					//Main.instance.player = gameDetails;

					//fileStream = File.Open(dataPath, FileMode.Open);
					//gameDetails = (PlayerInfo)binaryFormatter.Deserialize(fileStream);
				}
			}
			catch(Exception e) {
				PlatformSafeMessage("Failed to Load: " + e.Message);
			}
			return gameDetails != null;
		}

		public static void PlatformSafeMessage(string message) {
			if(Application.platform == RuntimePlatform.WebGLPlayer) {
				WindowAlert(message);
			}
			else {
				Debug.Log(message);
			}
		}
	}
}