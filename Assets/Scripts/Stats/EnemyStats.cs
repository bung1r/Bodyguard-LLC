    using System;
    using Unity.VisualScripting;
    using UnityEngine;


    [Serializable]
    public class EnemyStats
    {
        public float aggroDistance = 8f; 
        // public float wanderingDistance = 15f;
        public float walkingDistance = 5f; // 
        public float cautionDistance = 3f;
        public float deaggroDistance = 12f; 
        public float startAttackDist = 1.5f; // if -1, it will default to 90% of the z distance of the enemies longest attack
        public float rotationSpeed = 100f; // 100 is base. Why? Idk. 
        public float timeBetweenAttacks = 2f;
        
    }

    [Serializable]
    public class RuntimeEnemyStats : EnemyStats
    {
        public bool isCautiousWalking = false;
        public RuntimeEnemyStats(EnemyStats enemyStats)
        {
            
            aggroDistance = enemyStats.aggroDistance;
            deaggroDistance = enemyStats.deaggroDistance;
            startAttackDist = enemyStats.startAttackDist;
        }
    }