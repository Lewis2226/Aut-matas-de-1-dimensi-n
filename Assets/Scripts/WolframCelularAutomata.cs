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
    private bool[] currentGeneration; 
    private GameObject[,] cellsOnScreen; 
    private bool[] nextState; 
    [SerializeField] private float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        totalCells = new bool[height, width]; 
        currentGeneration = new bool[width];
        nextState = new bool[width]; 
        cellsOnScreen = new GameObject[height, width];

        CreateBoard();
        LifeGiver();  
        StartSimulation(); 
    }

    void CreateBoard()
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

    void LifeGiver()// Da vida de forma aleatoria a las células
    {
        for (int i = 0; i < width; i++)
        {
         int rnd = Random.Range(0, 2);
         if(rnd == 0)
         {
           currentGeneration[i] = true;
         }
         else
         {
           currentGeneration[i] = false;
         }
            
        }

        
        for (int x = 0; x < width; x++)
        {
            totalCells[0, x] = currentGeneration[x]; 
        }
    }

    bool CheckRule(bool left, bool center, bool right) // Revisa la regla dada en este caso la 30
    {
        if (left && center && right) return false;  // 111 -> 0
        if (left && center && !right) return false; // 110 -> 0
        if (left && !center && right) return false; // 101 -> 0
        if (left && !center && !right) return true; // 100 -> 1
        if (!left && center && right) return true;  // 011 -> 1
        if (!left && center && !right) return true; // 010 -> 1
        if (!left && !center && right) return true; // 001 -> 1
        if (!left && !center && !right) return false;//000 -> 0
        return false;
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

        
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y > 0; y--)
            {
                totalCells[y, x] = totalCells[y - 1, x];
            }
            totalCells[0, x] = nextState[x];
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

    public void StartSimulation()
    {
        StartCoroutine(NewGeneration());
    }

    IEnumerator NewGeneration()//Muestra la siguiente generacion
    {
        while (true)
        {
            NextGen();
            ShowSimulation(); 
            yield return new WaitForSeconds(waitTime); 
        }
    }

}
