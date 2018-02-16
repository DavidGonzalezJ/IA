using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PuzzleCreator : MonoBehaviour {
    public int tam;
    // Use this for initialization
    void Start () {
        GridLayoutGroup Grid = gameObject.GetComponent<GridLayoutGroup>();

        if (Grid == null)
            Debug.Log("Falta script layout");
        else {
            if (tam == 0) tam = 6;
            Grid.constraintCount = tam;
            int pantY = 720 - tam * 20;
            
            Grid.cellSize.Set( pantY/tam ,pantY/tam);
        }
        for (int i = 0; i < tam * tam; i++) {
            //UnityEngine.UI.Button Boton;
           
        }
	}
	
	// Update is called once per frame
	void Update () {
      //  GridLayoutGroup Grid = gameObject.GetComponent<GridLayoutGroup>();
    //    Grid.enabled = false;

    }
}
