using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.game {
	public class Configuration {
		public static string currentDirectory;
		public static string currentBaseDirectory;

		public static void loadCurrentDirectory() {
			currentDirectory = Application.dataPath;
			currentDirectory = currentDirectory.Replace(@"\", "/");
			bool hasFoundMatch = false;

			if(!currentDirectory.EndsWith("/"))
				currentDirectory += "/";

			switch(Application.platform) {
				case RuntimePlatform.OSXEditor: //<path to project folder>/Assets
				case RuntimePlatform.WindowsEditor:
					if(currentDirectory.EndsWith("Assets/")) {
						currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("Assets/"));
						currentDirectory += "RuntimeData/";
						hasFoundMatch = true;
					}
					break;
				case RuntimePlatform.WindowsPlayer: //<path to executablename_Data folder>
													/*Debug.Log(currentDirectory);
													if(currentDirectory.EndsWith("_Data/")) {
														currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf( "_Data/" ) );
														currentDirectory += "RuntimeData/";
														hasFoundMatch = true;
													}*/
					break;
				case RuntimePlatform.OSXPlayer: //<path to player app bundle>/Contents
					if(currentDirectory.EndsWith(".app/Contents/")) {
						currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf(".app/Contents/"));
						currentDirectory += "RuntimeData/";
						hasFoundMatch = true;
					}
					break;
				case RuntimePlatform.OSXDashboardPlayer: //<path to the dashboard widget bundle>
				//case RuntimePlatform.WindowsWebPlayer: 
				//case RuntimePlatform.OSXWebPlayer:
				case RuntimePlatform.WebGLPlayer: //not supported at the moment
				default:
					hasFoundMatch = false;
					break;
			}


			if(!hasFoundMatch) {
				currentDirectory = Path.GetFullPath("RuntimeData/");
				currentDirectory = currentDirectory.Replace(@"\", "/");
			}

			if(!Directory.Exists(currentDirectory)) {
				for(int i = 0; i < 3; i++)
					currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("/"));
				currentDirectory += "/RuntimeData/";
			}

			currentBaseDirectory = currentDirectory.Replace("/RuntimeData", "");
		}
	}
}
