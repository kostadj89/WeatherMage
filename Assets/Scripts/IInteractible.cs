using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IInteractible
    {
        public void Interact(Action onInteractionComplete);

    }
}
