using System;
using System.Collections.Generic;

namespace SpinWheelApp
{
    /// <summary>
    /// WaccaMenuData — builds the real option lists for each arrowMode, from the values
    /// found in the WaccaCircle source (WaccaCircle.cs + WaccaTable.cs).
    ///
    ///   arrowMode 0 = App        (the 13-app list)
    ///   arrowMode 1 = Anim       (waccaCircleText from WaccaTable.cs, indices 0..16)
    ///   arrowMode 2 = Volume     (0..100, step intervalSet)
    ///   arrowMode 3 = Delay      (fixed 5,10,15 ... 200 ms — per request)
    ///   arrowMode 4 = Brightness (0..100, step intervalSet)
    ///   arrowMode 5 = Anim Speed (unimplemented in source; shown disabled-ish)
    ///   arrowMode 6 = Esc / Enter (two-state toggle)
    ///   arrowMode 7 = DInput 14 / 15 (two-state)
    ///   arrowMode 8 = Interval Set (>= 1)
    ///
    /// A Spec is what WaccaWedgeMenu.ConfigureFromSpec() consumes. After Confirmed(idx)
    /// fires, call Apply(mode, idx) to translate the chosen index back into the value the
    /// app uses (volume int, delay ms int, animIndex, etc.).
    /// </summary>
    public static class WaccaMenuData
    {
        public struct Spec
        {
            public string Title;
            public string Description;
            public List<string> Items;
            public int SelectedIndex;
        }

        // ---- the real animation labels: WaccaTable.cs waccaCircleText, indices 0..16 ----
        // (index 17 "Custom" is unreachable by normal scrolling, so it is omitted here;
        //  add it back if you decide to surface the reset behaviour deliberately.)
        public static readonly string[] AnimLabels =
        {
            "Static", "Breathe", "Mid-Breathe", "Sine Mid-Breathe", "Sine Breathe",
            "Jump", "Reverse Jump", "WaccaColor Cycle", "Reverse WaccaColor Cycle",
            "Freeze", "Full Circle Jump", "Full Circle Reverse Jump",
            "Full Circle WaccaColor Cycle", "Full Circle Reverse WaccaColor Cycle",
            "Wacca", "Reverse Wacca", "Off"
        };

        // ---- the real app list: WaccaCircle.cs waccaCircleText (apps) ----
        public static readonly string[] AppLabels =
        {
            "Launching UI in 1 second...", "WaccaCircle12", "WaccaCircle24", "WaccaCircle32",
            "WaccaCircle96", "WaccaCircleTaiko", "WaccaCircleSDVX", "WaccaCircleRPG",
            "WaccaCircleLoveLive", "WaccaCircleCemu", "WaccaCircleMouse", "WaccaCircleOsu",
            "WaccaCircleKeyboard"
        };

        // ---- delay: fixed range 5,10,...,200 ms (per request) ----
        public static readonly int[] DelayMs = BuildDelays();
        private static int[] BuildDelays()
        {
            var list = new List<int>();
            for (int v = 5; v <= 200; v += 5) list.Add(v);
            return list.ToArray();
        }

        /// <summary>Build the Spec for a given arrowMode. currentValue is interpreted per
        /// mode (volume/brightness as 0..100, delay as ms, anim as animIndex, etc.).</summary>
        public static Spec Build(int arrowMode, int currentValue)
        {
            switch (arrowMode)
            {
                case 0: // App
                    return Make("application", "choisissez l'application",
                        AppLabels, Clamp(currentValue, 0, AppLabels.Length - 1));

                case 1: // Anim (lighting)
                    return Make("animation des lumières", "effet lumineux du cercle",
                        AnimLabels, Clamp(currentValue, 0, AnimLabels.Length - 1));

                case 2: // Volume 0..100 step 5
                    return Make("volume", "volume du système",
                        Range(0, 100, 5), IndexOfNearest(Range(0, 100, 5), currentValue));

                case 3: // Delay — fixed 5..200 ms
                    return Make("réglage du délai", "délai de la boucle (ms)",
                        Labels(DelayMs, " ms"), IndexOfNearestInt(DelayMs, currentValue <= 0 ? 10 : currentValue));

                case 4: // Brightness 0..100 step 5
                    return Make("luminosité", "luminosité des LED",
                        Range(0, 100, 5), IndexOfNearest(Range(0, 100, 5), currentValue));

                case 5: // Anim Speed — not implemented in source
                    return Make("vitesse d'animation", "non implémenté",
                        new List<string> { "—" }, 0);

                case 6: // Esc / Enter toggle
                    return Make("échap / entrée", "touche envoyée",
                        new List<string> { "Entrée", "Échap" }, Clamp(currentValue, 0, 1));

                case 7: // DInput 14 / 15
                    return Make("DInput 14 / 15", "boutons manette virtuelle",
                        new List<string> { "Bouton 14", "Bouton 15" }, Clamp(currentValue, 0, 1));

                case 8: // Interval Set >= 1
                    return Make("pas d'incrément", "multiplie volume / délai / luminosité",
                        Range(1, 20, 1), IndexOfNearest(Range(1, 20, 1), currentValue < 1 ? 1 : currentValue));

                default:
                    return Make("réglage", "", new List<string> { "—" }, 0);
            }
        }

        /// <summary>Translate the chosen wedge index back into the real value the app uses.
        /// Returns an int meaningful for the mode (volume, ms, animIndex, 0/1 toggle...).</summary>
        public static int Apply(int arrowMode, int chosenIndex)
        {
            switch (arrowMode)
            {
                case 0: return Clamp(chosenIndex, 0, AppLabels.Length - 1);          // app index
                case 1: return Clamp(chosenIndex, 0, AnimLabels.Length - 1);         // animIndex
                case 2: return chosenIndex * 5;                                      // volume 0..100
                case 3: return (chosenIndex >= 0 && chosenIndex < DelayMs.Length)    // LAG_DELAY ms
                                ? DelayMs[chosenIndex] : 10;
                case 4: return chosenIndex * 5;                                      // brightness 0..100
                case 6: return Clamp(chosenIndex, 0, 1);                             // 0=Enter 1=Escape
                case 7: return Clamp(chosenIndex, 0, 1);                             // 0=btn14 1=btn15
                case 8: return chosenIndex + 1;                                      // intervalSet >=1
                default: return chosenIndex;
            }
        }

        // ---- small builders ----
        private static Spec Make(string title, string desc, IList<string> items, int sel)
        {
            return new Spec { Title = title, Description = desc, Items = new List<string>(items), SelectedIndex = sel };
        }

        private static List<string> Range(int lo, int hi, int step)
        {
            var l = new List<string>();
            for (int v = lo; v <= hi; v += step) l.Add(v.ToString());
            return l;
        }

        private static List<string> Labels(int[] vals, string suffix)
        {
            var l = new List<string>();
            foreach (var v in vals) l.Add(v + suffix);
            return l;
        }

        private static int IndexOfNearest(List<string> numericList, int value)
        {
            int best = 0; int bestd = int.MaxValue;
            for (int i = 0; i < numericList.Count; i++)
            {
                int n; if (!int.TryParse(numericList[i], out n)) continue;
                int d = Math.Abs(n - value);
                if (d < bestd) { bestd = d; best = i; }
            }
            return best;
        }

        private static int IndexOfNearestInt(int[] vals, int value)
        {
            int best = 0, bestd = int.MaxValue;
            for (int i = 0; i < vals.Length; i++)
            {
                int d = Math.Abs(vals[i] - value);
                if (d < bestd) { bestd = d; best = i; }
            }
            return best;
        }

        private static int Clamp(int v, int lo, int hi) { return v < lo ? lo : (v > hi ? hi : v); }
    }
}
