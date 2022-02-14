using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftEvents
{
    public class OnPlayerWasAddedEvent
    {
    }

    public class OnStartGenerateCharsEvent
    {
    }

    public class OnEndBattleEvent
    {
        public bool isDraw;
        public Player winner;
        public Player loser;
    }

    public class OnEndGameEvent 
    {
        public Player Winner;
    }
}
