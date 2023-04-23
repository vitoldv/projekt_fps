using Assets._Core.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Core.Scripts
{
    internal interface IShootingHandler
    {
        public void Init(ShootingParameters param, Camera camera);
        public void Shoot();
    }
}
