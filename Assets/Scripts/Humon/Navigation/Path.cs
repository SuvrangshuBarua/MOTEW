using UnityEngine;
using System.Collections.Generic;

namespace Nav
{

public class Path
{
    public bool ReachedDestination => _path.Count == 0;

    public Path(Navigation nav, Queue<Vector3> path)
    {
        _nav = nav;
        _path = path;

        if (_path.Count == 0)
        {
            Debug.LogWarning("Path has 0 nodes");
            return;
        }

        // I'm Walkin'
        _nav.SetDestination(_path.Dequeue());
    }

    public void Update()
    {
        if (ReachedDestination)
        {
            return;
        }

        if (_nav.ReachedDestination)
        {
            _nav.SetDestination(_path.Dequeue());
        }
    }

    private Navigation _nav;
    private Queue<Vector3> _path;
}

} // namespace Nav

