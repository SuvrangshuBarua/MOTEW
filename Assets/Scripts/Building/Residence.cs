using UnityEngine;
using System.Collections.Generic;

namespace Building
{

public class Residence : BaseBuilding
{
    [SerializeField]
    private uint residentCapacity_;

    [SerializeField]
    private uint visitorCapacity_;

    private List<Humon> residents_ = new ();

    void Start()
    {
        Construct();
    }

    void FixedUpdate()
    {
        if (_state == Building.State.Constructed)
        {
            Destruct();
        }
        else if (_state == Building.State.Destructed)
        {
            Construct();
        }

        foreach (var resident in residents_)
        {
            // TODO: does this check Destroy(obj)
            if (resident == null)
            {
                residents_.Remove(resident);
            }
        }

        if (residents_.Count < residentCapacity_)
        {
            // try to spawn a humon
        }
    }
}

} // namespace Building

