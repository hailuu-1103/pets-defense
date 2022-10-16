namespace Common
{
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;

    public class WavePath : MonoBehaviour
    {
        [SerializeField]             private DOTweenPath path;
        [SerializeField] private List<GameObject>           enemyPrefabs  ;
        
        private readonly List<Vector3> waypoints = new();
        private void Start()
        {
            foreach (var point in this.path.wps)
            {
                this.waypoints.Add(point);
            }
        }
        private void Spawn()
        {
            var enemy = Instantiate(this.enemyPrefabs[0], this.waypoints[0], Quaternion.identity, this.transform);
            
        }
        
    }
}