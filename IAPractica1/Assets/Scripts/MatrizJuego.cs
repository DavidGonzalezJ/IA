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

enum pos {arriba, abajo, izquierda, derecha, nula};

public class MatrizJuego : MonoBehaviour {
    public bool pararbarajar = true;
    public UnityEngine.UI.Text estadisticas;
    [SerializeField]
    private GameObject Puzzle;
    private static int tam;
    public int DFS_profundidad = 20;
    pieza[,] matriz;
    pieza[,] matrizSolucion;
    pieza hueco;

    //ESTADISTICAS
    float BFS_time = 0;
    int BFS_nodes = 0;
    int BFS_pasos = 0;

    float DFS_time = 0;
    int DFS_nodes = 0;
    int DFS_pasos = 0;



    class Estado
    {

        public Estado()
        {
            estado = new pieza[tam, tam];
            estadoPrevio = new pieza[tam, tam];
            hueco = new pieza();
        }
        public pieza[,] estado;
        public pieza[,] estadoPrevio;
        public pieza hueco;
        public int coste = 0;
        public pos dir = pos.nula;
    };

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
        GameObject a= null, b= null;
        int aI = 0, bI = 0;
        if (dameCasilla(x, out valor)) {
            switch (x)
            {
                case pos.arriba:

                    a = Puzzle.transform.Find(matriz[hueco.i - 1, hueco.j].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    hueco.SetIJ(hueco.i - 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i + 1, hueco.j].valor = valor;

                    break;
                case pos.abajo:


                    a = Puzzle.transform.Find(matriz[hueco.i + 1, hueco.j].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    hueco.SetIJ(hueco.i + 1, hueco.j);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i - 1, hueco.j].valor = valor;

                    break;
                case pos.izquierda:

                    a = Puzzle.transform.Find(matriz[hueco.i, hueco.j-1].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    hueco.SetIJ(hueco.i, hueco.j -1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i , hueco.j + 1].valor = valor;

                    break;

                case pos.derecha:

                    a = Puzzle.transform.Find(matriz[hueco.i, hueco.j+1].valor.ToString()).gameObject;
                    b = Puzzle.transform.Find(hueco.valor.ToString()).gameObject;
                    aI = a.transform.GetSiblingIndex();
                    bI = b.transform.GetSiblingIndex();

                    hueco.SetIJ(hueco.i, hueco.j + 1);
                    matriz[hueco.i, hueco.j].valor = hueco.valor;
                    matriz[hueco.i, hueco.j - 1].valor = valor;

                    break;
            }
            //Método que ilustra el cambio
            b.transform.SetSiblingIndex(aI);
            a.transform.SetSiblingIndex(bI);
            return true;
        }
        return false;
    }

    ///Devuelve el valor de una casilla adyacente al hueco para una matriz en general
    bool dameCasilla(pos x, pieza[,] matriz, pieza Hueco, out int valor) {
        valor = 0;
        if (x == pos.arriba) {
            if (Hueco.i - 1 >= 0)
            {
                valor = matriz[Hueco.i - 1, Hueco.j].valor;
                return true;
            }
        }
        else if (x == pos.abajo)
        {
            if (Hueco.i + 1 <tam)
            {
                valor = matriz[Hueco.i + 1, Hueco.j].valor;
                return true;
            }
        }
        else if (x == pos.derecha)
        {
            if (Hueco.j + 1 < tam)
            {
                valor = matriz[Hueco.i, Hueco.j +1].valor;
                return true;
            }
        }
        else if (x == pos.izquierda)
        {
            if (Hueco.j - 1 >= 0)
            {
                valor = matriz[Hueco.i, Hueco.j - 1].valor;
                return true;
            }
        }
        return false;
    }

    //Este cambio es para una matriz en general
    bool cambio(pos x, ref pieza[,] matriz, ref pieza Hueco)
    {
        int valor;
        bool cambiado = false;
        if(dameCasilla(x, matriz, Hueco, out valor )){
            
            switch (x)
            {
                case pos.arriba:
 
                        Hueco.SetIJ(Hueco.i - 1, Hueco.j);
                        matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                        matriz[Hueco.i + 1, Hueco.j].valor = valor;

                    break;
                case pos.abajo:
                        Hueco.SetIJ(Hueco.i + 1, Hueco.j);
                        matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                        matriz[Hueco.i - 1, Hueco.j].valor = valor;
                        

                    break;
                case pos.izquierda:
                        Hueco.SetIJ(Hueco.i, Hueco.j - 1);
                        matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                        matriz[Hueco.i, Hueco.j + 1].valor = valor;
                        cambiado = true;
                    break;

                case pos.derecha:

                    Hueco.SetIJ(Hueco.i, Hueco.j + 1);
                    matriz[Hueco.i, Hueco.j].valor = Hueco.valor;
                    matriz[Hueco.i, Hueco.j - 1].valor = valor;
                    break;
            }
            cambiado = true;
        }
            //Método que ilustra el cambio

        return cambiado;
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
        pararbarajar = true;
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

        //Estado inicial del que partimos
        Estado estadoAct = new Estado();
        copiaMatriz(matriz, out estadoAct.estado, tam);
        copiaMatriz(matriz, out estadoAct.estadoPrevio, tam);
        estadoAct.hueco = hueco;

        //Creamos una lista para no volver a pasar por los mismos estados
        List<Estado> estadosAnteriores = new List<Estado>();
        estadosAnteriores.Add(estadoAct);

        //Guardamos los movimientos en una lista para luego replicarlos en la parte gráfica de la solución
        movimientos = new List<pos>();

        //La cola será necesaria para hacer el BFS
        Queue<Estado> cola = new Queue<Estado>();
        cola.Enqueue(estadoAct);

        //Necesitamos un booleano que nos avise de cuando tenemos que parar
        bool encontrado = false;
        while (!encontrado && cola.Count != 0) {
            estadoAct = cola.Dequeue();
            BFS_nodes++;

            if (estadoAct.hueco.i == tam-1 && estadoAct.hueco.j == tam-1 && comparaMatriz(estadoAct.estado, matrizSolucion)){
                estadosAnteriores.Add(estadoAct);
                encontrado = true;
            }

            //Miramos los posibles estados siguientes
            for (int i = 0; !encontrado && i < 4; i++) {
                

                //Creamos un nuevo tablero que igualamos al estado actual para
                //cambiarlo y sacar los siguientes estados.
                Estado nuevoTablero = new Estado();
                copiaMatriz(estadoAct.estado, out nuevoTablero.estado, tam);
                nuevoTablero.hueco.i = estadoAct.hueco.i;
                nuevoTablero.hueco.j = estadoAct.hueco.j;
                nuevoTablero.hueco.valor = estadoAct.hueco.valor;
                copiaMatriz(estadoAct.estado, out nuevoTablero.estadoPrevio, tam);

                if (cambio((pos)i, ref nuevoTablero.estado, ref nuevoTablero.hueco))
                {
                    //Recorremos la lista de los estados anteriores a ver si alguno coincide con el nuevo.
                    int l = 0;
                    bool yaEsta = false;
                    while (l < estadosAnteriores.Count && !yaEsta)
                    {
                        if (estadosAnteriores[l].hueco.i == nuevoTablero.hueco.i && estadosAnteriores[l].hueco.j == nuevoTablero.hueco.j
                             && comparaMatriz(estadosAnteriores[l].estado, nuevoTablero.estado)){
                            yaEsta = true;
                        }
                        l++;
                    }
                    //Si el nuevo estado ya estaba registrado, se ignora
                    if (!yaEsta)
                    {
                        //Si no, se mete en la lista y en la cola y se apunta el movimiento
                        nuevoTablero.dir = (pos)i;
                        estadosAnteriores.Add(nuevoTablero);
                        cola.Enqueue(nuevoTablero);
                    }
                }
            }
        }

        //Método con coste lineal para ir encontrando a los padres sucesivos
        Estado recorrido = estadosAnteriores[estadosAnteriores.Count-1];
        movimientos.Add(recorrido.dir);
        for (int i = estadosAnteriores.Count-1; i >= 0; i--) {
            if (comparaMatriz(estadosAnteriores[i].estado, recorrido.estadoPrevio)) {
                recorrido = estadosAnteriores[i];
                if (i > 0)
                    movimientos.Add(recorrido.dir);//Recorremos los nodos visitados a la inversa y nos quedamos con las direcciones tomadas
            }
        }
        //Para devolver la lista de movimientos la invertimos
        movimientos.Reverse();
        return encontrado;
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
       
        //La pila será necesaria para hacer el DFS
        Stack<Estado> pila = new Stack<Estado>();
        pila.Push(estadoAct);
        bool encontrado = false;
        while (!encontrado && pila.Count != 0)
        {
            estadoAct = pila.Pop();
            DFS_nodes++;

            if (comparaMatriz(estadoAct.estado, matrizSolucion))
                encontrado =  true;
            for (int i = 0; !encontrado && estadoAct.coste < DFS_profundidad && i < 4; i++)
            {
                //Creamos un nuevo tablero que igualamos al estado actual para
                //cambiarlo y sacar los siguientes estados.
                Estado nuevoTablero = new Estado();
                copiaMatriz(estadoAct.estado, out nuevoTablero.estado, tam);
                copiaMatriz(estadoAct.estado, out nuevoTablero.estadoPrevio, tam);
                nuevoTablero.hueco.i = estadoAct.hueco.i;
                nuevoTablero.hueco.j = estadoAct.hueco.j;
                nuevoTablero.hueco.valor = estadoAct.hueco.valor;
                nuevoTablero.coste = estadoAct.coste + 1;
                nuevoTablero.dir = (pos)i;

                //nuevoTablero.coste = estadoAct.coste + 1;
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
                        if (comparaMatriz(nuevoTablero.estado, matrizSolucion))
                            encontrado = true;
                    }
                }
            }
        }
        //Método con coste lineal para ir encontrando a los padres sucesivos
        Estado recorrido = estadosAnteriores[estadosAnteriores.Count - 1];
        movimientos.Add(recorrido.dir);
        for (int i = estadosAnteriores.Count - 1; i >= 0; i--)
        {
            if (comparaMatriz(estadosAnteriores[i].estado, recorrido.estadoPrevio))
            {
                recorrido = estadosAnteriores[i];
                if (i > 0)
                    movimientos.Add(recorrido.dir);//Recorremos los nodos visitados a la inversa y nos quedamos con las direcciones tomadas
            }
        }
        //Para devolver la lista de movimientos la invertimos
        movimientos.Reverse();
        return encontrado;
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
    private static void Print_(pos s)
    {
        Debug.Log("Movimiento"+ (int)s);
    }
    public void Resuelve_BFS() {
        float tiempoIni = Time.realtimeSinceStartup;
        //Primero relleno la lista de posiciones que hay que cambiar
        Debug.Log("CLICK");
        List<pos> movs = new List<pos>();
        if (!BFS(out movs)) Debug.Log("IRRESOLUBLE");
        else {
            movs.ForEach(Print_);
            Debug.Log("RESUELTO"+ movs.Count);
            BFS_pasos = movs.Count;
            BFS_time = Time.realtimeSinceStartup - tiempoIni;
            
            ///Graficos///
            StartCoroutine(haciaSolucion(movs));
        }
        estadisticas.text = "Estadisticas \n" + "Tiempo: " + BFS_time + "\nNodos: " + BFS_nodes + "\nPasos: " + BFS_pasos;


    }
    public void Resuelve_DFS() {
        //Primero relleno la lista de posiciones que hay que cambiar
        float tiempoIni = Time.realtimeSinceStartup;

        List<pos> movs = new List<pos>();
        if (!DFS(out movs)) Debug.Log("IRRESOLUBLE");
        else {
            movs.ForEach(Print_);
            DFS_pasos = movs.Count;
            Debug.Log("RESUELTO"+ movs.Count);
            DFS_time = Time.realtimeSinceStartup - tiempoIni;
            ///Graficos///
            StartCoroutine(haciaSolucion(movs));
        }

        estadisticas.text = "Estadisticas \nDFS profundidad " + DFS_profundidad+" \nTiempo: " + DFS_time + "\nNodos: " + DFS_nodes + "\nPasos: " + DFS_pasos;


    }

    public void Reinicia() {
        copiaMatriz(matrizSolucion, out matriz, tam);
        hueco.i = tam - 1;
        hueco.j = tam - 1;
        GameObject a = null;
        for (int i =0; i<tam*tam;i++){
            a = Puzzle.transform.Find(i.ToString()).gameObject;
            a.transform.SetSiblingIndex(i);
        }
    }

    public void move(int Pieza){

        if((hueco.i-1) >= 0 && matriz[hueco.i-1,hueco.j].valor == Pieza){
            cambio(pos.arriba);
        }else if((hueco.i+1) < tam && matriz[hueco.i+1,hueco.j].valor == Pieza){
            cambio(pos.abajo);
        }else if((hueco.j-1) >=0 && matriz[hueco.i,hueco.j-1].valor == Pieza){
            cambio(pos.izquierda);
        }else if((hueco.j+1) < tam && matriz[hueco.i,hueco.j+1].valor == Pieza){
            cambio(pos.derecha);
        }

    }

    public int dameTam(){
        return tam;
    }


}