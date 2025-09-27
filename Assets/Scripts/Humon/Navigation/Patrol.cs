using UnityEngine;
using System.Collections.Generic;

namespace Nav
{

// starcraft like patrol if you know you know
public class Patrol
{
    public Patrol(Navigation nav, params Vector3[] positions)
    {
        _nav = nav;
        _positions.AddRange(positions);

        // begin patrolling
        _nav.SetDestination(_positions[_cur]);
    }

    public void Update()
    {
        if (_nav.ReachedDestination)
        {
            _cur = (_cur + 1) % _positions.Count;
            _nav.SetDestination(_positions[_cur]);
        }
    }


    private Navigation _nav;
    private List<Vector3> _positions = new ();
    private int _cur = 0;
}

} // namespace Nav

