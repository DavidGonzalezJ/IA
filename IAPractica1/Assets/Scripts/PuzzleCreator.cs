using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//https://jdanger.com/solving-8-puzzle-with-artificial-intelligence.html

public class PuzzleCreator : MonoBehaviour {
	private int tam;
	[SerializeField]
	private GameObject button;
	[SerializeField]
	private Transform puzzleField;
    // Use this for initialization
    void Awake () {
    	tam = PuzzleManager.Instance.dameTam();
		GridLayoutGroup Grid = puzzleField.GetComponent<GridLayoutGroup>();

        if (Grid == null)
            Debug.Log("Falta script layout");
        else {
            if (tam == 0) tam = 6;
			float scale = 3.0f /(float) tam;
			float pantY = (720 - tam * 20)/tam;
			Debug.Log(pantY + "    " + pantY/tam);

			Debug.Log("Escala: " + scale);
			Grid.constraintCount = tam;

			for (int i = 0; i < tam * tam; i++) {
				GameObject buttonn = Instantiate(button);
				buttonn.name = "" + i;
				Text texto = buttonn.GetComponentInChildren<Text> ();
                //Image imagen = buttonn.GetComponent<Image>();
                if (texto!=  null)
					texto.text = ""+ i;
				buttonn.transform.SetParent (puzzleField, false);
				if(i==tam*tam-1){
					Image a = buttonn.GetComponent<Image>();
					a.enabled = false;
				}
			}

        }
	}
	// Update is called once per frame
	void Update () {


    }

}
