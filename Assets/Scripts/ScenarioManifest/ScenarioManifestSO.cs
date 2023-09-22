using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDH
{

    [CreateAssetMenu(menuName = "Scenario Manifest", fileName = "scenario_manifest")]
    public class ScenarioManifestSO : ScriptableObject
    {
        [SerializeField] public List<ScenarioSO> scenarios;

        [SerializeField] public EnemyDetailsSO _enemyDetails;

        public List<ScenarioSO> GetScenarios()
        {
            return scenarios;
        }

        public ScenarioSO GetScenario(int roundId)
        {
            if(roundId <= scenarios.Count)
                return scenarios[roundId - 1];
            return null;
        }

        public EnemyDetailsSO GetEnemyDetails()
        {
            return _enemyDetails;
        }
    }

}
