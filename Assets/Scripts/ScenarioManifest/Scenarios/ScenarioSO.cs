using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDH
{
    [CreateAssetMenu(menuName = "Scenario", fileName = "scenario")]
    public class ScenarioSO : ScriptableObject
    {
        [SerializeField] public int _roundId;
        [SerializeField] public string _roundSlogan;
        [SerializeField][Multiline] private string _roundDescription;

        [SerializeField] public ScenarioDetailsSO _scenarioDetails;

        public ScenarioDetailsSO GetScenarioDetails()
        {
            return _scenarioDetails;
        }
    }
}
