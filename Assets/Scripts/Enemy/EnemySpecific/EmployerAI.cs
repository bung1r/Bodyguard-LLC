using UnityEngine;

// This is the AI for the employer 
// (AKA the guy you need to protect)

public class EmployerAI : EnemyAI
{
    public EmployerStates employerState;

    public override void Think()
    {
        // The EmployerAI needs to think. Most likely. 
    }
}

public enum EmployerStates
{
    Idle, // the employer is still, very calm
    Scared, // the employer is maybe crouching scared? optinal
    Frantic, // the employer is running around scared
    Fainted, // the employer has fainted. easy to carry around. 
    Dead,
}