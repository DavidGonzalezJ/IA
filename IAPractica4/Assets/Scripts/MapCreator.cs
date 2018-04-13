// David González Jiménez
// Patricia Cabrero Villar

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Pos
{
    public int i, j;
    public Pos() { }
    public Pos(int I, int J)
    {
        i = I;
        j = J;
    }
    public void Set(int i_, int j_)
    {
        i = i_;
        j = j_;
    }
}
public class MapCreator : MonoBehaviour {
    private int tam;

	[SerializeField]
	private GameObject button;
    [SerializeField]
    private GameObject houseO;
    [SerializeField]
	private Transform mapField;
    [SerializeField]
    private Transform assetsField;
    [SerializeField]
    private Transform agentField;


    // Use this for initialization
    void Awake () {
    	tam = GameManager.Instance.dameTam();
		GridLayoutGroup Grid = mapField.GetComponent<GridLayoutGroup>();

        if (Grid == null)
            Debug.Log("Falta script layout");
        else {
			Grid.constraintCount = tam;

			for (int i = 0; i < tam * tam; i++) {
				GameObject buttonn = Instantiate(button);
				buttonn.name = "" + i;
                buttonn.transform.SetParent(mapField, false);

                TilePR3 tile = buttonn.GetComponent<TilePR3>();
                if (tile == null) Debug.Log("No encontrado tile");
                else tile.estado.Posicion.Set( i % tam, i / tam);
            }
        }
	}
	// Update is called once per frame
	void Update () {

		Transform house_ = assetsField.GetChild (0);
		Transform aux = mapField.transform.GetChild (tam * (tam - 1));
		house_.transform.position = aux.position;
		RectTransform houseRect = house_.GetComponent<RectTransform> ();
		houseRect.sizeDelta = new Vector2 (65, 65);
		GameManager.Instance.SetPiezas(mapField);
	}
}
