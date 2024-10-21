using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WolframCelularAutomata : MonoBehaviour
{
    // Variables públicas
    public int width;
    public int height;
    public Color aliveColor;
    public Color deadColor;
    public GameObject cellPrefab;

    // Variables privadas
    [SerializeField] private float waitTime;
    private bool[,] totalCells;
    private bool[] currentGeneration;
    private GameObject[,] cellsOnScreen;
    private bool[] nextState;
    private string binary = "";
    private int ruleNumber;
    private int currentRow = 0;
    private bool runningSimulation = false;

    // Input Fields
    public TMP_InputField ruleString;
    public TMP_InputField widthString;
    public TMP_InputField heightString;

    private void Update()
    {
        if (!runningSimulation)
        {
            // Permite marcar las células como vivas o muertas
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                int x = Mathf.FloorToInt(mousePos.x);
                int y = Mathf.FloorToInt(mousePos.y);

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    totalCells[0, x] = !totalCells[0, x];
                    currentGeneration[x] = !currentGeneration[x];
                    ShowSimulation();
                }
            }

            // Asigna los valores del tablero y la regla
            if (ruleString != null)
            {
                int.TryParse(ruleString.text, out ruleNumber);
            }

            if (widthString != null)
            {
                int.TryParse(widthString.text, out width);
            }

            if (heightString != null)
            {
                int.TryParse(heightString.text, out height);
            }
        }
    }

    void CreateBoard()// Crea el tablero con los agentes muertos
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

    public void LifeGiver() // Da vida de forma aleatoria a las células
    {
        for (int i = 0; i < width; i++)
        {
            int rnd = Random.Range(0, 2);
            currentGeneration[i] = rnd == 0;
        }

        for (int x = 0; x < width; x++)
        {
            totalCells[currentRow, x] = currentGeneration[x];
        }
        ShowSimulation();
    }

    // Simplificar el método CheckRule usando un vecindario binario
    bool CheckRule(bool left, bool center, bool right)
    {
        // Convertir el vecindario a un número binario
        int neighborhood = (left ? 4 : 0) + (center ? 2 : 0) + (right ? 1 : 0);

        // Aplicar la regla: si la posición correspondiente en la regla binaria es '1', la célula vive, si es '0', muere.
        return binary[7 - neighborhood] == '1';
    }

    void NextGen()// Genera la siguiente generación usando la regla
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

    public void ShowSimulation()// Muestra la simulación
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cellsOnScreen[y, x].GetComponent<SpriteRenderer>().color = totalCells[y, x] ? aliveColor : deadColor;
            }
        }
    }

    public string ToBinary(int number)// Convierte el número de la regla a binario
    {
        binary = System.Convert.ToString(number, 2).PadLeft(8, '0');
        Debug.Log(binary);
        return binary;
    }

    public void StartSimulation()// Inicia la simulación
    {
        runningSimulation = true;
        StartCoroutine(NewGeneration());
    }

    public void StopSimulation()// Detiene la simulación
    {
        runningSimulation = false;
        StopAllCoroutines();
    }

    public void ContinueSimulation()// Continúa la simulación
    {
        runningSimulation = true;
        StartSimulation();
    }

    public void ClearGame()// Limpia el tablero
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Destroy(cellsOnScreen[x, y]);
            }
        }
    }

    public void Generateboard()// Genera el tablero
    {
        totalCells = new bool[height, width];
        currentGeneration = new bool[width];
        nextState = new bool[width];
        cellsOnScreen = new GameObject[height, width];
        CreateBoard();
        ToBinary(ruleNumber);
        runningSimulation = false;
    }

    IEnumerator NewGeneration()// Muestra la siguiente generación
    {
        while (runningSimulation)
        {
            NextGen();
            ShowSimulation();
            yield return new WaitForSeconds(waitTime);
        }
    }

}
