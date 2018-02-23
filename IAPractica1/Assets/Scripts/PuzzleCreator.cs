using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PuzzleCreator : MonoBehaviour {
    public int tam;
	[SerializeField]
	private GameObject button;
	[SerializeField]
	private Transform puzzleField;
    // Use this for initialization
    void Awake () {
		GridLayoutGroup Grid = puzzleField.GetComponent<GridLayoutGroup>();

        if (Grid == null)
            Debug.Log("Falta script layout");
        else {
            if (tam == 0) tam = 6;
			float scale = 3.0f /(float) tam;
			float pantY = (720 - tam * 20)/tam;
			Debug.Log(pantY + "    " + pantY/tam);

			//Transform prueba =  puzzleField.getComponent<Transform>();
		//	prueba.localScale.Set (scale, scale, scale);// scale,scale ,scale ); //(50.0f, 50.0f);

			Debug.Log("Escala: " + scale);
			Grid.constraintCount = tam;

			for (int i = 0; i < tam * tam; i++) {
				GameObject buttonn = Instantiate(button);
				buttonn.name = "" + i;
				Text texto = buttonn.GetComponentInChildren<Text> ();
				if(texto!=  null)
					texto.text = ""+ i;
				buttonn.transform.SetParent (puzzleField, false);
			}

        }
       
	}
	
	// Update is called once per frame
	void Update () {
      //  GridLayoutGroup Grid = gameObject.GetComponent<GridLayoutGroup>();
    //    Grid.enabled = false;

    }
	public int dameTam(){
		return tam;
	}
}
