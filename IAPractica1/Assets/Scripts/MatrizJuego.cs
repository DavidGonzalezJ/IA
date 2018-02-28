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



public class MatrizJuego : MonoBehaviour {
    public bool pararbarajar = true;
    [SerializeField]
    private GameObject Puzzle;
    private static int tam;

    class Estado
    {
        public Estado()
        {
            estado = new pieza[tam, tam];
            hueco = new pieza();
        }
        public pieza[,] estado;
        public pieza hueco;
    };

    pieza[,] matriz;
    pieza[,] matrizSolucion;
    pieza hueco;

    //Inicializa la matriz para cualquier tamaño
    void matrizInicial(){
        for (int i = 0; i < tam; i++) {
            for (int j = 0; j < tam; j++) {
                matriz[i, j].i = i;
                matrizSolucion[i, j].i = i;
                matriz[i, j].j = j;
                matrizSolucion[i, j].j = j;
                matriz[i, j].valor = i * tam + j;
                matrizSolucion[i, j].valor = i * tam + j;
            }
        }
    }
    // Use this for initialization
    void Start () {
        tam = PuzzleManager.Instance.dameTam();
        matriz = new pieza[tam, tam];
        matrizSolucion = new pieza[tam, tam];
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


                    //matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i - 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i + 1, hueco.j].valor = valor;
                    //matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);

                    b.transform.SetSiblingIndex(aI);
                    a.transform.SetSiblingIndex(bI);

                    break;
                case pos.abajo:


                    a = Puzzle.transform.Find(matriz[hueco.i + 1, hueco.j].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    //matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i + 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i - 1, hueco.j].valor = valor;
                    //matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);

                    b.transform.SetSiblingIndex(aI);
                    a.transform.SetSiblingIndex(bI);

                    break;
                case pos.izquierda:

                    a = Puzzle.transform.Find(matriz[hueco.i, hueco.j-1].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    //matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j -1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i , hueco.j + 1].valor = valor;
                    //matriz[hueco.i , hueco.j + 1].SetIJ(hueco.i, hueco.j);

                    b.transform.SetSiblingIndex(aI);
                    a.transform.SetSiblingIndex(bI);
                    break;

                case pos.derecha:

                    a = Puzzle.transform.Find(matriz[hueco.i, hueco.j+1].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    //matriz[hueco.i, hueco.j + 1].SetIJ(hueco.i, hueco.j);
                    hueco.SetIJ(hueco.i, hueco.j + 1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i, hueco.j - 1].valor = valor;
                    //matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);

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
    bool cambio(pos x, ref pieza[,] matriz, ref pieza Hueco)
    {
        int valor;
        if (dameCasilla(x, out valor))
        {
            switch (x)
            {
                case pos.arriba:


                    //matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    Hueco.SetIJ(Hueco.i - 1, Hueco.j);
                    matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                    matriz[Hueco.i + 1, Hueco.j].valor = valor;
                    //matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);

                    break;
                case pos.abajo:


                

                    //matriz[hueco.i + 1, hueco.j].SetIJ(hueco.i, hueco.j);
                    Hueco.SetIJ(Hueco.i + 1, Hueco.j);
                    matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                    matriz[Hueco.i - 1, Hueco.j].valor = valor;
                    //matriz[hueco.i - 1, hueco.j].SetIJ(hueco.i, hueco.j);


                    break;
                case pos.izquierda:

                   

                    //matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
                    Hueco.SetIJ(Hueco.i, Hueco.j - 1);
                    matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                    matriz[Hueco.i, Hueco.j + 1].valor = valor;
                    //matriz[hueco.i, hueco.j + 1].SetIJ(hueco.i, hueco.j);

                 
                    break;

                case pos.derecha:

                  

                    //matriz[hueco.i, hueco.j + 1].SetIJ(hueco.i, hueco.j);
                    Hueco.SetIJ(Hueco.i, Hueco.j + 1);
                    matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                    matriz[Hueco.i, Hueco.j - 1].valor = valor;
                    //matriz[hueco.i, hueco.j - 1].SetIJ(hueco.i, hueco.j);
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
        Debug.Log("Terminé de barajar");
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

    //C# al igualar matrices te apunta al mismo sitio de memoria, así que hay que copiarla a lo bestia
    void copiaMatriz(pieza[,] original, out pieza[,] copia, int tam) {
        copia = new pieza[tam,tam];
        for (int i = 0; i < tam; i++) {
            for (int j = 0; j < tam; j++) {
                copia[i, j] = original[i, j];
            }
        }
    }
    //Y lo mismo con compararla
    bool comparaMatriz(pieza[,] m1, pieza[,] m2) {
        for (int i = 0; i < tam; i++){
            for (int j = 0; j < tam; j++){
                if (m1[i, j].valor != m2[i, j].valor) return false;
            }
        }
        return true;
    }


    //BFS
    bool BFS(out List<pos> movimientos) {
        //Estado inicial del que partimos una vez barajado el puzzle
        Estado estadoAct = new Estado();
        estadoAct.estado = matriz;
        estadoAct.hueco = hueco;
        //Creamos una lista para no volver a pasar por los mismos estados
        List<Estado> estadosAnteriores = new List<Estado>();
        estadosAnteriores.Add(estadoAct);
        //Guardamos los movimientos en una lista para luego replicarlos en la parte gráfica de la solución
        movimientos = new List<pos>();
        //La cola será necesaria para hacer el BFS
        Queue<Estado> cola = new Queue<Estado>();
        cola.Enqueue(estadoAct);
        while (cola.Count != 0) {
            estadoAct = cola.Dequeue();
            if (comparaMatriz(estadoAct.estado, matrizSolucion))
                return true;
            //pieza[,] aux = estadoAct;
            for (int i = 0; i < 4; i++) {
                //Creamos un nuevo tablero que igualamos al estado actual para
                //cambiarlo y sacar los siguientes estados.
                Estado nuevoTablero = new Estado();
                copiaMatriz(estadoAct.estado, out nuevoTablero.estado, tam);
                nuevoTablero.hueco.i = estadoAct.hueco.i;
                nuevoTablero.hueco.j = estadoAct.hueco.j;
                nuevoTablero.hueco.valor = estadoAct.hueco.valor;
                if (cambio((pos)i, ref nuevoTablero.estado, ref nuevoTablero.hueco))
                {
                    //Recorremos la lista de los estados anteriores a ver si alguno coincide con el nuevo.
                    int l = 0;
                    bool yaEsta = false;
                    while (l < estadosAnteriores.Count && !yaEsta)
                    {
                        if (comparaMatriz(estadosAnteriores[l].estado, nuevoTablero.estado))
                            yaEsta = true;
                        l++;
                    }
                    //Si el nuevo estado ya estaba registrado, se ignora
                    if (!yaEsta)
                    {
                        //Si no, se mete en la lista y en la cola y se apunta el movimiento
                        estadosAnteriores.Add(nuevoTablero);
                        cola.Enqueue(nuevoTablero);
                        //ESTO ESTA MAL PORQUE METE EN LA LISTA TODOS LOS MOVIMIENTOS
                        movimientos.Add((pos)i);
                    }
                }
            }
        }
        return false;
    }

    //DFS
    bool DFS(out List<pos> movimientos) {
        //Estado inicial del que partimos una vez barajado el puzzle
        Estado estadoAct = new Estado();
        estadoAct.estado = matriz;
        estadoAct.hueco = hueco;
        //Creamos una lista para no volver a pasar por los mismos estados
        List<Estado> estadosAnteriores = new List<Estado>();
        estadosAnteriores.Add(estadoAct);
        //Guardamos los movimientos en una lista para luego replicarlos en la parte gráfica de la solución
        movimientos = new List<pos>();
        //La cola será necesaria para hacer el BFS
        Stack<Estado> pila = new Stack<Estado>();
        pila.Push(estadoAct);
        while (pila.Count != 0)
        {
            estadoAct = pila.Pop();
            if (comparaMatriz(estadoAct.estado, matrizSolucion))
                return true;
            for (int i = 0; i < 4; i++)
            {
                //Creamos un nuevo tablero que igualamos al estado actual para
                //cambiarlo y sacar los siguientes estados.
                Estado nuevoTablero = new Estado();
                copiaMatriz(estadoAct.estado, out nuevoTablero.estado, tam);
                nuevoTablero.hueco.i = estadoAct.hueco.i;
                nuevoTablero.hueco.j = estadoAct.hueco.j;
                nuevoTablero.hueco.valor = estadoAct.hueco.valor;
                if (cambio((pos)i, ref nuevoTablero.estado, ref nuevoTablero.hueco))
                {
                    //Recorremos la lista de los estados anteriores a ver si alguno coincide con el nuevo.
                    int l = 0;
                    bool yaEsta = false;
                    while (l < estadosAnteriores.Count && !yaEsta)
                    {
                        if (comparaMatriz(estadosAnteriores[l].estado, nuevoTablero.estado))
                            yaEsta = true;
                        l++;
                    }
                    //Si el nuevo estado ya estaba registrado, se ignora
                    if (!yaEsta)
                    {
                        //Si no, se mete en la lista y en la cola y se apunta el movimiento
                        estadosAnteriores.Add(nuevoTablero);
                        pila.Push(nuevoTablero);
                        //ESTO ESTA MAL PORQUE METE EN LA LISTA TODOS LOS MOVIMIENTOS
                        movimientos.Add((pos)i);
                    }
                }
            }
        }
        return false;
    }

    //Recorre las posiciones y resuelve el puzzle
    IEnumerator haciaSolucion(List<pos> movs) {
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
        List<pos> movs = new List<pos>();
        if (!BFS(out movs)) Debug.Log("IRRESOLUBLE");
        else Debug.Log("RESUELTO");
        haciaSolucion(movs);

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