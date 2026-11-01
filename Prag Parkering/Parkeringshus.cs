using System;
using System.Collections.Generic;

namespace PragParkering
{
    public class Parkeringshus
    {
        public List<Parkeringsplats> Parkeringsplatser { get; set; }

        public Parkeringshus(int antalPlatser)
        {
            if (antalPlatser <= 0)
            {
                throw new ArgumentException(
                    "Parkeringshuset måste ha minst en plats.");
            }

            Parkeringsplatser = new List<Parkeringsplats>();

            for (int platsnummer = 1;
                 platsnummer <= antalPlatser;
                 platsnummer++)
            {
                Parkeringsplatser.Add(
                    new Parkeringsplats(platsnummer));
            }
        }

        public int ParkeraFordon(
            Fordon fordon,
            Parkeringskonfiguration konfiguration)
        {
            ValideraRegistreringsnummer(
                fordon.Registreringsnummer);

            konfiguration.Validera();

            if (HittaParkeringsplats(
                fordon.Registreringsnummer) != null)
            {
                throw new InvalidOperationException(
                    "Ett fordon med registreringsnumret finns redan parkerat.");
            }

            Parkeringsplats? plats =
                HittaLedigPlats(fordon, konfiguration);

            if (plats == null)
            {
                return -1;
            }

            fordon.Ankomsttid = DateTime.Now;
            plats.ParkeradeFordon.Add(fordon);

            return plats.Nummer;
        }

        public Parkeringsplats? HittaParkeringsplats(
            string registreringsnummer)
        {
            foreach (Parkeringsplats plats in Parkeringsplatser)
            {
                foreach (Fordon fordon in plats.ParkeradeFordon)
                {
                    if (string.Equals(
                        fordon.Registreringsnummer,
                        registreringsnummer,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return plats;
                    }
                }
            }

            return null;
        }
        public Fordon HämtaUtFordon(string registreringsnummer)
        {
            ValideraRegistreringsnummer(registreringsnummer);

            Parkeringsplats? plats =
                HittaParkeringsplats(registreringsnummer);

            if (plats == null)
            {
                throw new InvalidOperationException(
                    "Inget parkerat fordon med registreringsnumret hittades.");
            }

            Fordon fordon = HämtaFordonFrånPlats(
                plats, registreringsnummer);

            plats.ParkeradeFordon.Remove(fordon);

            return fordon;
        }

        public int FlyttaFordon(
            string registreringsnummer,
            int nyttPlatsnummer,
            Parkeringskonfiguration konfiguration)
        {
            ValideraRegistreringsnummer(registreringsnummer);
            konfiguration.Validera();

            Parkeringsplats? ursprungligPlats =
                HittaParkeringsplats(registreringsnummer);

            if (ursprungligPlats == null)
            {
                throw new InvalidOperationException(
                    "Inget parkerat fordon med registreringsnumret hittades.");
            }

            Parkeringsplats nyPlats =
                HämtaParkeringsplats(nyttPlatsnummer);

            if (ursprungligPlats.Nummer == nyPlats.Nummer)
            {
                throw new InvalidOperationException(
                    "Fordonet står redan på den valda platsen.");
            }

            Fordon fordon = HämtaFordonFrånPlats(
                ursprungligPlats, registreringsnummer);

            if (!nyPlats.HarPlatsFör(fordon, konfiguration))
            {
                throw new InvalidOperationException(
                    $"Plats {nyPlats.Nummer} har inte tillräckligt med utrymme för fordonet.");
            }

            ursprungligPlats.ParkeradeFordon.Remove(fordon);
            nyPlats.ParkeradeFordon.Add(fordon);

            return ursprungligPlats.Nummer;
        }

        private Parkeringsplats HämtaParkeringsplats(int platsnummer)
        {
            foreach (Parkeringsplats plats in Parkeringsplatser)
            {
                if (plats.Nummer == platsnummer)
                {
                    return plats;
                }
            }

            throw new ArgumentException(
                $"Parkeringsplats {platsnummer} finns inte.");
        }

        private Fordon HämtaFordonFrånPlats(
            Parkeringsplats plats,
            string registreringsnummer)
        {
            foreach (Fordon fordon in plats.ParkeradeFordon)
            {
                if (string.Equals(
                    fordon.Registreringsnummer,
                    registreringsnummer,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return fordon;
                }
            }

            throw new InvalidOperationException(
                "Fordonet hittades inte på parkeringsplatsen.");
        }

        public void ÅterställParkering(
            List<Parkeringsplats> sparadePlatser,
            Parkeringskonfiguration konfiguration)
        {
            if (sparadePlatser == null)
            {
                throw new ArgumentNullException(nameof(sparadePlatser));
            }

            konfiguration.Validera();
            ValideraSparadePlatser(sparadePlatser, konfiguration);

            Parkeringshus återställtHus =
                new Parkeringshus(Parkeringsplatser.Count);

            foreach (Parkeringsplats plats in sparadePlatser)
            {
                if (plats.Nummer <= Parkeringsplatser.Count)
                {
                    återställtHus.Parkeringsplatser[plats.Nummer - 1] =
                        plats;
                }
            }

            Parkeringsplatser = återställtHus.Parkeringsplatser;
        }

        private void ValideraSparadePlatser(
            List<Parkeringsplats> sparadePlatser,
            Parkeringskonfiguration konfiguration)
        {
            HashSet<int> platsnummer = new HashSet<int>();

            HashSet<string> registreringsnummer =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Parkeringsplats plats in sparadePlatser)
            {
                ValideraSparadPlats(plats, platsnummer);

                foreach (Fordon fordon in plats.ParkeradeFordon)
                {
                    ValideraSparatFordon(
                        fordon, registreringsnummer);
                }

                if (plats.BeräknaUpptagenStorlek(konfiguration) >
                    plats.Storlek)
                {
                    throw new InvalidOperationException(
                        $"Plats {plats.Nummer} innehåller fler fordon än den rymmer.");
                }
            }
        }

        private void ValideraSparadPlats(
            Parkeringsplats plats,
            HashSet<int> platsnummer)
        {
            if (plats == null ||
                plats.Nummer < 1 ||
                plats.Storlek <= 0 ||
                plats.ParkeradeFordon == null)
            {
                throw new ArgumentException(
                    "Parkeringsdata innehåller en ogiltig parkeringsplats.");
            }

            if (!platsnummer.Add(plats.Nummer))
            {
                throw new ArgumentException(
                    $"Plats {plats.Nummer} finns flera gånger i parkeringsdata.");
            }

            if (plats.Nummer > Parkeringsplatser.Count &&
                plats.ParkeradeFordon.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Plats {plats.Nummer} innehåller fordon men saknas i den nya konfigurationen.");
            }
        }

        private void ValideraSparatFordon(
            Fordon fordon,
            HashSet<string> registreringsnummer)
        {
            if (fordon == null)
            {
                throw new ArgumentException(
                    "Parkeringsdata innehåller ett tomt fordon.");
            }

            ValideraRegistreringsnummer(
                fordon.Registreringsnummer);

            if (!registreringsnummer.Add(
                fordon.Registreringsnummer))
            {
                throw new InvalidOperationException(
                    $"Registreringsnumret {fordon.Registreringsnummer} finns flera gånger.");
            }

            if (fordon.Ankomsttid == DateTime.MinValue ||
                fordon.Ankomsttid > DateTime.Now)
            {
                throw new ArgumentException(
                    $"Fordonet {fordon.Registreringsnummer} har en ogiltig ankomsttid.");
            }
        }

        private Parkeringsplats? HittaLedigPlats(
            Fordon fordon,
            Parkeringskonfiguration konfiguration)
        {
            foreach (Parkeringsplats plats in Parkeringsplatser)
            {
                if (plats.ParkeradeFordon.Count > 0 &&
                    plats.HarPlatsFör(fordon, konfiguration))
                {
                    return plats;
                }
            }

            foreach (Parkeringsplats plats in Parkeringsplatser)
            {
                if (plats.ParkeradeFordon.Count == 0 &&
                    plats.HarPlatsFör(fordon, konfiguration))
                {
                    return plats;
                }
            }

            return null;
        }

        private void ValideraRegistreringsnummer(
            string registreringsnummer)
        {
            if (string.IsNullOrWhiteSpace(registreringsnummer) ||
                registreringsnummer.Length > 10)
            {
                throw new ArgumentException(
                    "Registreringsnumret måste innehålla 1–10 tecken.");
            }

            foreach (char tecken in registreringsnummer)
            {
                if (!char.IsLetterOrDigit(tecken))
                {
                    throw new ArgumentException(
                        "Registreringsnumret får bara innehålla bokstäver och siffror.");
                }
            }
        }
    }
}