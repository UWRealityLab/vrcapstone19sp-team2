using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManagerScript;

public class UIContent : MonoBehaviour
{
    public List<AudioClip> audios;
    public static int UI_MIN_DELAY_SECONDS = 3;

    public static Dictionary<TaskTypes, string> TaskToUI = 
        new Dictionary<TaskTypes, string>()
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

    public static Dictionary<EventTypes, string> EventToUI =
        new Dictionary<EventTypes, string>()
        {
            { EventTypes.WAKE_UP, "Ahhh my head hurts..but where am I? My hand is cuffed to the heater. better uncuff myself first." },
            { EventTypes.CUTTER_CUT, "Something is making a noise. Let’s take a look. Hmnn. There is a radio here. Let’s see what is being broadcasted here." },
            { EventTypes.AFTER_RADIO_MILITARY, "The military is evacuating the area here. Need to send a signal that can be seen from miles away.\n It seems like there are more stuff in the drawer." },
            { EventTypes.AFTER_FLASHLIGHT, "Nice. I can finally see things with the flashlight." },
            { EventTypes.AFTER_PICK_UP_DIARY, "This should give me some information about this place." },
            { EventTypes.AFTER_DIARY_FUSE_PAGE, "This could be the reason that the electricity is out - the circuit breaker is tripped." },
            { EventTypes.AFTER_DIARY_MUSIC_PAGE, "The music box must be of some importance. I should find it soon. " },
            { EventTypes.AFTER_DIARY_FLARE_GUN, "The owner mentions that he received a flare gun too. This can be used to signal the military." },
            { EventTypes.AFTER_LIGHT_ON, "Nice. Now I can see everything. Let me find some information… the bookshelf can be a good start" },
            { EventTypes.MUSIC_BOX_TOUCHED, "There is the music box. The music box is locked. I will need to find the key. There is a clipboard here with some words. Let me take a look" },
            { EventTypes.AFTER_CLIP_BOARD, "\"The key to the truth is buried in the ice\" In the ice? What does that mean? Let me look around further. Maybe the fridge" },
            { EventTypes.ICE_CUBE_TOUCHED, "nteresting. An ice cube with a key inside it! Let me melt the ice cube and retrieve the key" },
            { EventTypes.PICKED_UP_KEY, "Let’s open the music box now" },
            { EventTypes.MUSIC_BOX_KEY_INSERTED, "wind it once.." },
            { EventTypes.SECRETE_DOOR_OPEN, "whoa! There is a secret chamber! There must be something inside" },
            { EventTypes.ENTERED_SECRET_ROOM, "That’s my informant!!! How did he end up over here?? He has a gun in his hands.. did he kill himself? The barrel is still hot, so this is not long ago. I should keep the gun" },

            { EventTypes.PICKED_UP_GUN, "The magazine is missing. Maybe hidden somewhere safe. Also, there is a something written in blood here" },
            
            // TODO add this
            { EventTypes.DOOR_CLOSED_WHILE_IN, "DOOR_CLOSED_WHILE_IN" },

            { EventTypes.EXIT_SECRET_ROOM, "That was a close one. The writing must be a password of some sort. Let’s see if I can find something. I haven’t check the cabinets below the TV. Let me take a look." },
            { EventTypes.SAFEBOX_CABINET_OPEN, "A safe! Let’s try the number here, and twist the bottom handle a few times clockwise" },
            { EventTypes.SAFEBOX_OPEN, "There is the magazine and bullets. Now lets load the gun" },
            { EventTypes.GUN_LOADED, "Now the gun can be used… but what to do with the gun? Here is a switch, let\'s see if it does something" },
            { EventTypes.CURTAIN_OPEN, "\"This way\"?Maybe I can break the window with the gun"},
            { EventTypes.GLASS_BROKEN, "Nice! The glass is broken. Let’s fire the flare up the window. Hopefully the military evac team will see me!" },
            { EventTypes.FLARE_GUN_FIRED, "The flare is up in the sky. If the military is near, they should be able to see it" },
            
            // TODO
            { EventTypes.HELI_ARRIVED, "Finally! There is the helicopter! I am out of here! What a strange night" },
            { EventTypes.ESCAPED, "Game over" },
        };

    public static Dictionary<EventTypes, AudioClip> EventToVoice;

    public void Start()
    {
        EventToVoice = new Dictionary<EventTypes, AudioClip>() {
        { EventTypes.WAKE_UP, audios[0] },
            { EventTypes.PICKUP_CUTTER, audios[1] }, // need trigger
            { EventTypes.CUTTER_CUT, audios[2] },
            { EventTypes.EXIT_BATHROOM, audios[3] }, // need trigger
            { EventTypes.AFTER_RADIO_MILITARY,audios[4] },
            { EventTypes.AFTER_FLASHLIGHT, audios[6] },
            { EventTypes.AFTER_PICK_UP_DIARY, audios[7] },
            { EventTypes.AFTER_DIARY_FUSE_PAGE, audios[8] },
            { EventTypes.AFTER_DIARY_MUSIC_PAGE, audios[9] },
            { EventTypes.AFTER_DIARY_FLARE_GUN, audios[10] },
            { EventTypes.OPENED_FUSE_COVER, audios[11] }, // need trigger
            { EventTypes.AFTER_LIGHT_ON, audios[12] },
            { EventTypes.MUSIC_BOX_TOUCHED,audios[13] },
            { EventTypes.PICKED_UP_CLIPBOARD, audios[14] }, // need trigger
            { EventTypes.AFTER_CLIP_BOARD, audios[15] },
            { EventTypes.ICE_CUBE_TOUCHED,audios[16] },
            { EventTypes.PICKED_UP_KEY,audios[17] },
            { EventTypes.MUSIC_BOX_KEY_INSERTED, audios[18]},
            { EventTypes.SECRETE_DOOR_OPEN, audios[19] },
            { EventTypes.ENTERED_SECRET_ROOM, audios[20] },

            { EventTypes.PICKED_UP_GUN, audios[21] },
            
            // TODO add this
            { EventTypes.DOOR_CLOSED_WHILE_IN,audios[23] },

            { EventTypes.EXIT_SECRET_ROOM, audios[24]},
            { EventTypes.SAFEBOX_CABINET_OPEN, audios[25] },
            { EventTypes.SAFEBOX_OPEN, audios[26] },
            { EventTypes.GUN_LOADED, audios[27] },
            { EventTypes.CURTAIN_OPEN, audios[29]},
            { EventTypes.GLASS_BROKEN, audios[31] },
            //{ EventTypes.FLARE_GUN_FIRED, audios[31] },
            
            // TODO
            { EventTypes.HELI_ARRIVED, audios[32] },
            //{ EventTypes.ESCAPED, audios[0]},
    };
    }
}
