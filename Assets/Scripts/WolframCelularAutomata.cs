using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolframCelularAutomata : MonoBehaviour
{
    public int width;
    public int height;
    public Color aliveColor;
    public Color deadColor;
    public GameObject cellPrefab;
    private bool[,] totalCells;
    private GameObject[,] cellsOnScreen;
    private bool runnigSimulation;
    private int simulationNum =0;
    [SerializeField] private int ruleNum;
    [SerializeField] private float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateBoard()//Crea el tablero con el tamaño dado de ancho y largo
    {
        totalCells= new bool [width, height];
        cellsOnScreen = new GameObject[width, height];
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new Vector3(i, j, 0);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
                cell.GetComponent<SpriteRenderer>().color = deadColor;
            }
        }
    }

    void NumberToBinary(int numberRule)//Convierte el numero dado a binario
    {

    }

    void SetLife()//Da vida a los autómatas base como  dice la regla
    {

    }

    void CheckRule(int rule)//Revisa la regla selecionada
    {

    }

    void ShowSimulation(int number)//Muestra la simulacion en pantalla.
    {

    }

    public void StopSimulation()//Detiene la corutina
    {
        runnigSimulation = false;
    }

    public void ContinueSimulation()//Continua la corutina
    {
        runnigSimulation = true;
    }

    IEnumerator NewGeneration()//Genera la nueva simulacion despues deun lapso de tiempo
    {
        while (runnigSimulation) 
        {
            CheckRule(ruleNum);
            ShowSimulation(simulationNum);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
