using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDH;

namespace TDH
{

    [CreateAssetMenu(menuName = "Scenario Details", fileName = "scenario_details")]
    public class ScenarioDetailsSO : ScriptableObject
    {
        [SerializeField] public float _initialWaitPrimaryPowerup;
        [SerializeField] public float _spawnWaitPrimaryPowerup;
        [SerializeField] public float _initialWaitSecondaryPowerup;
        [SerializeField] public float _spawnWaitSecondaryPowerup;

        [SerializeField] public List<SpawnManager.EnemyType> _enemyTypes;
    }

}
