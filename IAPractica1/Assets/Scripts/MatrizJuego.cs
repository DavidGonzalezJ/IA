using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct pieza{
    public int valor, i, j;
    public pieza(int v, int y, int g) {
        valor = v;
        i = y;
        j = g;
    }
    public void SetIJ(int Y, int G) { i = Y; j = G; }
};

enum pos {arriba, abajo, izquierda, derecha};

struct Estado {
    public pieza[,] estado;
    public pieza hueco;
};

public class MatrizJuego : MonoBehaviour {
    public bool pararbarajar = true;
    [SerializeField]
    private GameObject Puzzle;
    private static int tam;


    pieza[,] matriz;
    pieza[,] matrizSolucion;
    pieza hueco;

    //Inicializa la matriz para cualquier tamaño
    void matrizInicial(){
        for (int i = 0; i < tam; i++) {
            for (int j = 0; j < tam; j++) {
                matriz[i, j].i = i;
                matriz[i, j].j = j;
                matriz[i, j].valor = i * tam + j;
            }
        }
        matrizSolucion = matriz;
    }
    // Use this for initialization
    void Start () {
        tam = PuzzleManager.Instance.dameTam();
        matriz = new pieza[tam, tam];
        hueco = new pieza(tam * tam-1, tam - 1, tam - 1);
        matrizInicial();

       // Debug.Log(tam);
    }
    // Update is called once per frame
    void Update () {
        
    }

   
    //Devuelve el valor de una casilla adyacente al hueco o false si no existe.
    bool dameCasilla(pos x, out int valor) {
        valor = 0;
        if (x == pos.arriba) {
            if (hueco.i - 1 >= 0)
            {
                valor = matriz[hueco.i - 1, hueco.j].valor;
                return true;
            }
        }
        else if (x == pos.abajo)
        {
            if (hueco.i + 1 <tam)
            {
                valor = matriz[hueco.i + 1, hueco.j].valor;
                return true;
            }
        }
        else if (x == pos.derecha)
        {
            if (hueco.j + 1 < tam)
            {
                valor = matriz[hueco.i, hueco.j +1].valor;
                return true;
            }
        }
        else if (x == pos.izquierda)
        {
            if (hueco.j - 1 >= 0)
            {
                valor = matriz[hueco.i, hueco.j - 1].valor;
                return true;
            }
        }
        return false;
    }

    //Cambia el hueco por la casilla que se le indica al metodo (si no puede devuelve false)
    bool cambio(pos x) {
        int valor;
        GameObject a, b;
        int aI, bI;
        if (dameCasilla(x, out valor)) {
            switch (x)
            {
                case pos.arriba:

                    a = Puzzle.transform.Find(matriz[hueco.i - 1, hueco.j].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();


                    matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i - 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i + 1, hueco.j].valor = valor;
                    matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);

                    b.transform.SetSiblingIndex(aI);
                    a.transform.SetSiblingIndex(bI);

                    break;
                case pos.abajo:


                    a = Puzzle.transform.Find(matriz[hueco.i + 1, hueco.j].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i + 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i - 1, hueco.j].valor = valor;
                    matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);

                    b.transform.SetSiblingIndex(aI);
                    a.transform.SetSiblingIndex(bI);

                    break;
                case pos.izquierda:

                    a = Puzzle.transform.Find(matriz[hueco.i, hueco.j-1].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j -1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i , hueco.j + 1].valor = valor;
                    matriz[hueco.i , hueco.j + 1].SetIJ(hueco.i, hueco.j);

                    b.transform.SetSiblingIndex(aI);
                    a.transform.SetSiblingIndex(bI);
                    break;

                case pos.derecha:

                    a = Puzzle.transform.Find(matriz[hueco.i, hueco.j+1].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    matriz[hueco.i, hueco.j + 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j + 1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i, hueco.j - 1].valor = valor;
                    matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);

                    b.transform.SetSiblingIndex(aI);
                    a.transform.SetSiblingIndex(bI);
                    break;
            }
            //Método que ilustra el cambio
            return true;
        }
        return false;
    }

    //Este cambio es para una matriz en general
    bool cambio(pos x, pieza[,] matriz, pieza hueco)
    {
        int valor;
        if (dameCasilla(x, out valor))
        {
            switch (x)
            {
                case pos.arriba:


                    matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i - 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i + 1, hueco.j].valor = valor;
                    matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);

                    break;
                case pos.abajo:


                

                    matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i + 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i - 1, hueco.j].valor = valor;
                    matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);


                    break;
                case pos.izquierda:

                   

                    matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j - 1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i, hueco.j + 1].valor = valor;
                    matriz[hueco.i, hueco.j + 1].SetIJ(hueco.i, hueco.j);

                 
                    break;

                case pos.derecha:

                  

                    matriz[hueco.i, hueco.j + 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j + 1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i, hueco.j - 1].valor = valor;
                    matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
                    break;
            }
            //Método que ilustra el cambio
            return true;
        }
        return false;
    }

    IEnumerator MyMethod() {
        Debug.Log("Before Waiting 0.2 seconds");
        int ant=-2;
        for (int i = 0; i < 100 && !pararbarajar; i++) {

            int direccion = Random.Range(0, 4);
            if(direccion == ant-1 || ant+1 == direccion )
                direccion = Random.Range(0, 4);

            yield return new WaitForSecondsRealtime(0.2f);
            cambio((pos)direccion);
            ant = direccion;
        }
        Debug.Log("After Waiting 0.2 Seconds");
    }

    public void baraja() {
        StartCoroutine(MyMethod());
        if(!pararbarajar)
            pararbarajar = true;
        else{
            pararbarajar = false;
            StartCoroutine(MyMethod());
        }

    }

    //BFS
    bool BFS(out List<pos> movimientos) {
        Estado estadoAct;
        estadoAct.estado = matriz;
        estadoAct.hueco = hueco;
        List<Estado> estadosAnteriores = null;
        movimientos = null;
        estadosAnteriores.Add(estadoAct);
        Queue<Estado> cola = null;
        cola.Enqueue(estadoAct);
        while (cola.Count != 0) {
            estadoAct = cola.Dequeue();
            if (estadoAct.estado == matrizSolucion)
                return true;
            //pieza[,] aux = estadoAct;
            for (int i = 0; i < 4; i++) {
                Estado nuevoTablero = estadoAct;
                cambio((pos)i, nuevoTablero.estado, nuevoTablero.hueco);
                if (!estadosAnteriores.Contains(nuevoTablero))
                {
                    estadosAnteriores.Add(nuevoTablero);
                    cola.Enqueue(nuevoTablero);
                    movimientos.Add((pos)i);
                }
            }
        }
        return false;

        //Debug.Log("RESUELTO");
    }

    //Recorre las posiciones y resuelve el puzzle
    IEnumerator haciaSolucion() {
        List<pos> movs = null;
        if (!BFS(out movs)) Debug.Log("IRRESOLUBLE");

        //Ahora las aplico en plan bonito
        foreach (pos posicion in movs)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            cambio(posicion);
        }
    }

    public void Resuelve1() {
        //Primero relleno la lista de posiciones que hay que cambiar
        Debug.Log("CLICK");
        haciaSolucion();

    }

    public void move(int Pieza, out int posi){
        //bool moved = true;
        posi = (int)pos.arriba;
        int Phueco = hueco.i*tam + hueco.j;

        if((hueco.i-1) >=0 && matriz[hueco.i-1,hueco.j].valor == Pieza){
            cambio(pos.arriba);
            Debug.Log("MUEVE ARRIBA");
        }else if((hueco.i+1) < tam && matriz[hueco.i+1,hueco.j].valor == Pieza){
            cambio(pos.abajo);
            Debug.Log("MUEVE abajo");
        }else if((hueco.j-1) >=0 && matriz[hueco.i,hueco.j-1].valor == Pieza){
            cambio(pos.izquierda);
            Debug.Log("MUEVE izquierda");
        }else if((hueco.j+1) < tam && matriz[hueco.i,hueco.j+1].valor == Pieza){
            cambio(pos.derecha);
            Debug.Log("MUEVE derecha");
        }

    }

    public int dameTam(){
        return tam;
    }


}