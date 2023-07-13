using Mono;
using UnityEngine;
using Zenject;

namespace Other
{
    public class LineFactory
    {
        // private readonly DiContainer _container;
        private readonly GameObject _linePrefab;
        
        // [Inject]
        // public LineFactory(DiContainer container, GameObject linePrefab)
        // {
        //     _container = container;
        //     _linePrefab = linePrefab;
        // }
        public LineFactory(GameObject linePrefab)
        {
            _linePrefab = linePrefab;
        }
        
        public Line Create(Transform parent)
        {
            return Object.Instantiate(_linePrefab, Vector3.zero, Quaternion.identity, parent).GetComponent<Line>();
            // return _container.InstantiatePrefab(_linePrefab, Vector3.zero, Quaternion.identity, parent).GetComponent<Line>();
        }
    }
}