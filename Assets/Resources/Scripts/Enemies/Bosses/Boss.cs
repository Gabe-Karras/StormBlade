using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the top-level generic class to be attached to boss parent objects
public class Boss : MonoBehaviour
{
    // Total health for this boss
    [SerializeField]
    private int health;

    // List of components
    [SerializeField]
    private List<BossComponent> components;

    // List of currently interactible components (player can target)
    [SerializeField]
    private List<BossComponent> activeComponents;

    // List of moves that can currently be performed
    [SerializeField]
    private List<BossMove> moves;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Methods:
    // At the beginning of turn, check if any components have been destroyed and remove them

    // Boss needs to choose a move from the list at random

    // Add new component to list
    public void AddComponent(BossComponent component) {
        // Make sure it's not in the list already
        if (!components.Contains(component)) {
            components.Add(component);
        }
    }

    // Set existing component active
    public void SetComponentActive(BossComponent component) {
        // Make sure it's not in the list already
        if (!activeComponents.Contains(component)) {
            activeComponents.Add(component);
        }
    }

    // Add new move

    // Get list of active components
    public List<BossComponent> GetActiveComponents() {
        return activeComponents;
    }
}
