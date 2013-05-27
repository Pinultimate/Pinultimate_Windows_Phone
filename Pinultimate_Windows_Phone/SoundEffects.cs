/****************************************************************
 * Static helper class for playing sound effects. Borrowed from
 * Dave Voyles:
 * http://davidvoyles.wordpress.com/2013/02/16/playing-sound-effects-through-windows-phone-8/
 ***************************************************************/
using System;
using System.Windows.Media;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.IO;

namespace Pinultimate_Windows_Phone
{
    public static class SoundEffects
    {
        private static bool initialized = false;
        private static SoundEffect sfxClusterTap;


        /****************************************************************
        * Must be called before using static methods
        ****************************************************************/
        public static void Initialize()
        {
            if (SoundEffects.initialized)
                return;

            // Adds an Update delegate, to simulate the XNA update method.
            FrameworkDispatcher.Update();

            // Holds informations about a file stream.
            StreamResourceInfo info = App.GetResourceStream(new Uri("Pinultimate_Windows_Phone;component/Sounds/pop1.wav", UriKind.Relative));
            sfxClusterTap = SoundEffect.FromStream(info.Stream);
            initialized = true;
        }


        /****************************************************************
        * Plays the click sound when user enters/exits a menu
        ****************************************************************/
        public static SoundEffect SFxClusterTap
        {
            get
            {
                // If not initialized, returns null.
                if (!SoundEffects.initialized)
                    return null;

                // Create the SoundEffect from the Stream
                
                return sfxClusterTap;
            }
        }
    }
}
