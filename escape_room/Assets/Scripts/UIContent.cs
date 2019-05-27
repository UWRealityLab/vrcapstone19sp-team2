using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManagerScript;

public class UIContent : MonoBehaviour
{
    public static Hashtable TaskToUI = new Hashtable()
    {
        { TaskTypes.RELEASE, "Release yourself"},
        { TaskTypes.RADIO, "Explore the radio"},
        { TaskTypes.DESK, "Explore the desk"},
        { TaskTypes.FIND_FUSE, "Find the fuse box"},
        { TaskTypes.FIND_MUSIC_BOX, "Find the music box"},
        { TaskTypes.FIND_FLARE, "Find the flare gun"},
        { TaskTypes.LIGHT, "Recover the light"},
        { TaskTypes.KEY_HINT, "Find hints about the key"},
        { TaskTypes.KEY, "Find key"},
        { TaskTypes.ICE, "Get the key out"},
        { TaskTypes.AMMO, "Find more ammo"},
        { TaskTypes.CURTAIN, "Open the curtain"}
    };
}
