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
    private bool firstUpdate = true;
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
        //Colocamos casa
        Transform house_ = assetsField.GetChild(0);
        Transform aux = mapField.transform.GetChild(tam * (tam - 1));
        house_.transform.position = aux.position;
        //house_.transform.position.Set(aux.transform.position.x, aux.transform.position.y, aux.transform.position.z);

        GameManager.Instance.SetPiezas(mapField);

	}
	// Update is called once per frame
	void Update () {
       // if (firstUpdate)
       // {
            Transform house_ = assetsField.GetChild(0);
            Transform aux = mapField.transform.GetChild(tam * (tam - 1));
            house_.transform.position = aux.position;
        RectTransform houseRect = house_.GetComponent<RectTransform>();
        houseRect.sizeDelta = new Vector2(65, 65);
            //house_.transform.position.Set(aux.transform.position.x, aux.transform.position.y, aux.transform.position.z);

            GameManager.Instance.SetPiezas(mapField);
            firstUpdate = false;
        //}
    }


}
