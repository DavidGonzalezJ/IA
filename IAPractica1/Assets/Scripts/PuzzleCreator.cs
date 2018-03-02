using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//https://jdanger.com/solving-8-puzzle-with-artificial-intelligence.html

public class PuzzleCreator : MonoBehaviour {
    public Sprite[] Imagenes = new Sprite[9];

    private int tam;
    bool imagen = false;
	[SerializeField]
	private GameObject button;
	[SerializeField]
	private Transform puzzleField;
    // Use this for initialization
    void Awake () {
    	tam = PuzzleManager.Instance.dameTam();
		GridLayoutGroup Grid = puzzleField.GetComponent<GridLayoutGroup>();
        if (tam == 3) imagen = true;

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
                Image a = buttonn.GetComponent<Image>();
                if (imagen)
                    a.sprite = Imagenes[i];
                else
                {
                    Text texto = buttonn.GetComponentInChildren<Text>();
                    if (texto != null && i !=tam*tam-1)
                        texto.text = "" + i;
                }
                buttonn.transform.SetParent(puzzleField, false);
                if (i == tam * tam - 1)
                {
                    a.enabled = false;
                }
			}

        }
	}
	// Update is called once per frame
	void Update () {


    }

}
