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
    public int ruleNumber;
    [SerializeField] private float waitTime;
    private bool[,] totalCells;
    private bool[] currentGeneration;
    private GameObject[,] cellsOnScreen;
    private bool[] nextState;
    private string binary = "";
    private int currentRow = 0;
    private bool runnigSimulation;

    // Start is called before the first frame update
    void Start()
    {
        totalCells = new bool[height, width];
        currentGeneration = new bool[width];
        nextState = new bool[width];
        cellsOnScreen = new GameObject[height, width];
        ToBinary(ruleNumber);
        CreateBoard();
        LifeGiver();
        StartSimulation();
    }

    void CreateBoard()//Crea el tablero con los agentes muertos
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x, -y, 0);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
                cell.GetComponent<SpriteRenderer>().color = deadColor;
                cellsOnScreen[y, x] = cell;
            }
        }
    }

    void LifeGiver()//Da vida de forma aleatoria a las células
    {
        for (int i = 0; i < width; i++)
        {
            int rnd = Random.Range(0, 2);
            currentGeneration[i] = rnd == 0 ? true : false;
        }

        for (int x = 0; x < width; x++)
        {
            totalCells[currentRow, x] = currentGeneration[x];
        }
    }
    bool CheckRule(bool left, bool center, bool right)//Revisión de la relga 
    {
        int index = (left ? 4 : 0) + (center ? 2 : 0) + (right ? 1 : 0);

        
        return (ruleNumber & (1 << index)) != 0;
    }
    
    void NextGen()//Genera la siguinte generacion usando la regla
    {
        for (int i = 0; i < width; i++)
        {
            bool left = (i == 0) ? false : currentGeneration[i - 1];
            bool center = currentGeneration[i];
            bool right = (i == width - 1) ? false : currentGeneration[i + 1];

            nextState[i] = CheckRule(left, center, right);
        }

        currentRow++; 

        if (currentRow >= height) 
        {
            StopAllCoroutines(); 
            return;
        }

        for (int x = 0; x < width; x++)
        {
            totalCells[currentRow, x] = nextState[x];
        }

        for (int i = 0; i < width; i++)
        {
            currentGeneration[i] = nextState[i];
        }
    }

    public void ShowSimulation()//Muestra la simulación
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (totalCells[y, x])
                {
                    cellsOnScreen[y, x].GetComponent<SpriteRenderer>().color = aliveColor;
                }
                else
                {
                    cellsOnScreen[y, x].GetComponent<SpriteRenderer>().color = deadColor;
                }
            }
        }
    }

    public string ToBinary(int number)//Convierte el número de la regla a binario
    {
        if (number == 0)
            return "0";

        while (number > 0)
        {
            int remainder = number % 2;
            binary = remainder + binary;
            number /= 2;
        }

        while (binary.Length < 8)
        {
            binary = 0 + binary;
        }
        return binary;
    }

    public void StartSimulation()//Inicia la simulación
    {
        runnigSimulation = true;
        StartCoroutine(NewGeneration());
    }

    public void StopSimulation()//Detiene la simulación
    {
        runnigSimulation = false;
        StopAllCoroutines();
    }

    public void ContinueSimulation()//Continua la simulación
    {
        runnigSimulation=true;
        StartSimulation();
    }

    public void ClearGame()//Limpia el tablero
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Destroy(cellsOnScreen[x, y]);
            }
        }
    }

    IEnumerator NewGeneration()//Muestra la siguiente generación
    {
        while (runnigSimulation)
        {
            NextGen();
            ShowSimulation();
            yield return new WaitForSeconds(waitTime);
        }
    }

}
