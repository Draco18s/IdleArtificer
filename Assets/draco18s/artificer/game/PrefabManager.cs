using UnityEngine;
using System.Collections;

public class PrefabManager : MonoBehaviour {
	public static PrefabManager instance;
	public GameObject BUILDING_GUI_LISTITEM;
	public GameObject BUILDING_GUI_GRIDITEM;
	public GameObject GRID_GUI_ARROW;
	public GameObject QUEST_GUI_LISTITEM;
	public GameObject INVEN_GUI_LISTITEM;
	public GameObject UPGRADE_GUI_LISTITEM;
	public GameObject ACTIVE_QUEST_GUI_LISTITEM;

	void Start() {
		instance = this;
	}
}
