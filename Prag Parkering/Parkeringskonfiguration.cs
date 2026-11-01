using System;
using System.Collections.Generic;

namespace PragParkering
{
    public class Parkeringskonfiguration
    {
        public int AntalParkeringsplatser { get; set; } = 100;

        public List<Fordonsinställning> Fordonstyper { get; set; }
            = new List<Fordonsinställning>();

        public void Validera()
        {
            if (AntalParkeringsplatser < 1)
            {
                throw new ArgumentException(
                    "Antalet parkeringsplatser måste vara minst 1.");
            }

            ValideraFordonstyper();
        }

        private void ValideraFordonstyper()
        {
            if (Fordonstyper == null || Fordonstyper.Count == 0)
            {
                throw new ArgumentException(
                    "Konfigurationen måste innehålla fordonstyper.");
            }

            HashSet<string> namn =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Fordonsinställning inställning in Fordonstyper)
            {
                ValideraFordonsinställning(inställning);

                if (!namn.Add(inställning.Namn))
                {
                    throw new ArgumentException(
                        $"Fordonstypen {inställning.Namn} är angiven flera gånger.");
                }
            }
        }

        private void ValideraFordonsinställning(
            Fordonsinställning inställning)
        {
            if (inställning == null ||
                string.IsNullOrWhiteSpace(inställning.Namn) ||
                inställning.AntalFordonPerPlats < 1)
            {
                throw new ArgumentException(
                    "Varje fordonstyp måste ha ett namn och rymma minst ett fordon per plats.");
            }
        }

        public Fordonsinställning HämtaFordonsinställning(
            string fordonstyp)
        {
            foreach (Fordonsinställning inställning in Fordonstyper)
            {
                if (string.Equals(
                    inställning.Namn,
                    fordonstyp,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return inställning;
                }
            }

            throw new ArgumentException(
                $"Fordonstypen {fordonstyp} saknas i konfigurationen.");
        }
    }
}