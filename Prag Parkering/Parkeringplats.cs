using System;
using System.Collections.Generic;

namespace PragParkering
{
    public class Parkeringsplats
    {
        public int Nummer { get; set; }

        public decimal Storlek { get; set; }

        public List<Fordon> ParkeradeFordon { get; set; }

        public Parkeringsplats(int nummer, decimal storlek = 1m)
        {
            if (nummer < 1 || storlek <= 0)
            {
                throw new ArgumentException(
                    "Platsnummer och platsstorlek måste vara positiva.");
            }

            Nummer = nummer;
            Storlek = storlek;
            ParkeradeFordon = new List<Fordon>();
        }

        public decimal BeräknaUpptagenStorlek(
            Parkeringskonfiguration konfiguration)
        {
            decimal upptagenStorlek = 0m;

            foreach (Fordon fordon in ParkeradeFordon)
            {
                Fordonsinställning inställning =
                    konfiguration.HämtaFordonsinställning(
                        fordon.Fordonstyp);

                upptagenStorlek +=
                    1m / inställning.AntalFordonPerPlats;
            }

            return Math.Round(upptagenStorlek, 20);
        }

        public bool HarPlatsFör(
            Fordon fordon,
            Parkeringskonfiguration konfiguration)
        {
            Fordonsinställning inställning =
                konfiguration.HämtaFordonsinställning(
                    fordon.Fordonstyp);

            decimal fordonsstorlek =
                1m / inställning.AntalFordonPerPlats;

            decimal storlekEfterParkering = Math.Round(
                BeräknaUpptagenStorlek(konfiguration)
                + fordonsstorlek,
                20);

            return storlekEfterParkering <= Storlek;
        }
    }
}