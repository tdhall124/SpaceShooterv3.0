using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDH
{
    [CreateAssetMenu(menuName = "Enemy Details", fileName = "enemy_details")]
    public class EnemyDetailsSO : ScriptableObject
    {
        [SerializeField] private GameObject[] _enemyPrefabs;

        public GameObject[] EnemyPrefabs
        { get { return _enemyPrefabs; } } 
    }
}
