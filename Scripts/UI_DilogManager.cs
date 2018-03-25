using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DilogManager : MonoBehaviour {

    public GameObject TextDialogWindow;
    public GameObject TextDialogArea;
    public GameObject FullTextDialog;
    public GameObject TextDialogButtonPrefab;

    // Use this for initialization
    void Start () {
        TextDialogWindow.SetActive(false);
        TextDialogArea.SetActive(false);
        FullTextDialog.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
