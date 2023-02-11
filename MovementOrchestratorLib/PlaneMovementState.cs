using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovementOrchestratorLib
{
    internal enum PlaneMovementState
    {
        GoingStraight = 0,
        TurningRight,
        TurningLeft,
        GoingDown,
        GoingUp,
        StoppingGoingRight,
        StoppingGoingLeft
    }
}
