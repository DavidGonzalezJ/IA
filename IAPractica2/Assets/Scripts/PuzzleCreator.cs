using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
			Grid.constraintCount = tam;

			for (int i = 0; i < tam * tam; i++) {
				GameObject buttonn = Instantiate(button);
				buttonn.name = "" + i;
                buttonn.transform.SetParent(puzzleField, false);

                TilePR2 tile = buttonn.GetComponent<TilePR2>();
                tile.Posicion.SetPos(i%tam,i/tam);
                if(i < 3){
                   tile.estado = (eCasilla) i + 3;
                   tile.SetOcupada(true);
                }
			}
        }
	}
	// Update is called once per frame
	void Update () {
		
    }

}
