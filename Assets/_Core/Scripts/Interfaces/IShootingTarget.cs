using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShootingTarget
{
    public void OnHit(Vector3 hitPoint);
}
