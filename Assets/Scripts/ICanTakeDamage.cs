using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public interface ICanTakeDamage
    {
        public GridPosition GetGridPosition();
        public Vector3 GetWorldPosition();

        public void TakeDamage(int damage);

        public bool IsOnSameTeam(bool isEnemy);
    }
}
