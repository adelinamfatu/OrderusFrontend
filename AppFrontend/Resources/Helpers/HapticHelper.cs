using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace AppFrontend.Resources.Helpers
{
    public class HapticHelper
    {
        internal static void DoHaptic(HapticFeedbackType type)
        {
            try
            {
                Vibration.Vibrate(TimeSpan.FromMilliseconds(50));
                // Xamarin.Essentials.HapticFeedback.Perform(type);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}
