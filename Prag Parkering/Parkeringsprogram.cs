using System;
using System.Collections.Generic;
using System.IO;
using PragParkering.Filhantering;

namespace PragParkering
{
    public class Parkeringsprogram
    {
        public void Kör()
        {
            try
            {
                Parkeringskonfiguration konfiguration = LäsKonfiguration();
                Prislista prislista = LäsPrislista();

                prislista.Validera(konfiguration);

                Parkeringshus parkeringshus = new Parkeringshus(
                    konfiguration.AntalParkeringsplatser);

                LäsParkeringsdata(parkeringshus, konfiguration);

                KörMeny(parkeringshus, konfiguration, prislista);
            }
            catch (Exception fel)
            {
                Console.WriteLine(
                    $"Programmet kunde inte starta eller fortsätta: {fel.Message}");
            }
        }

        private Parkeringskonfiguration LäsKonfiguration()
        {
            string filsökväg = Path.Combine(
                AppContext.BaseDirectory, "konfiguration.json");

            Filhanterare filhanterare = new Filhanterare();

            Parkeringskonfiguration konfiguration =
                filhanterare.LäsJson<Parkeringskonfiguration>(filsökväg);

            konfiguration.Validera();

            return konfiguration;
        }

        private Prislista LäsPrislista()
        {
            string filsökväg = Path.Combine(
                AppContext.BaseDirectory, "prislista.txt");

            Filhanterare filhanterare = new Filhanterare();
            string[] rader = filhanterare.LäsTextrader(filsökväg);

            return new Prislista(rader);
        }

        private Prislista LäsOmPrislista(
            Parkeringskonfiguration konfiguration,
            Prislista nuvarandePrislista)
        {
            Console.Clear();
            Console.WriteLine("LÄS OM PRISLISTA");

            try
            {
                Prislista nyPrislista = LäsPrislista();
                nyPrislista.Validera(konfiguration);

                Console.WriteLine(
                    "Prislistan har lästs in på nytt.");

                return nyPrislista;
            }
            catch (Exception fel)
            {
                Console.WriteLine(
                    $"Prislistan kunde inte läsas om: {fel.Message}");

                Console.WriteLine(
                    "Den tidigare prislistan används fortfarande.");

                return nuvarandePrislista;
            }
        }

        private void LäsParkeringsdata(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            string filsökväg = Path.Combine(
                AppContext.BaseDirectory, "parkeringsdata.json");

            Filhanterare filhanterare = new Filhanterare();

            List<Parkeringsplats> sparadePlatser =
                filhanterare.LäsJson<List<Parkeringsplats>>(filsökväg);

            parkeringshus.ÅterställParkering(
                sparadePlatser, konfiguration);
        }

        private void SparaParkeringsdata(Parkeringshus parkeringshus)
        {
            string filsökväg = Path.Combine(
                AppContext.BaseDirectory, "parkeringsdata.json");

            Filhanterare filhanterare = new Filhanterare();

            filhanterare.SparaJson(
                filsökväg, parkeringshus.Parkeringsplatser);
        }

        private void ParkeraFordon(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            try
            {
                Fordonsinmatning inmatning = new Fordonsinmatning();
                Fordon fordon = inmatning.LäsFordon(konfiguration);

                int platsnummer = parkeringshus.ParkeraFordon(
                    fordon, konfiguration);

                if (platsnummer == -1)
                {
                    Console.WriteLine(
                        "Det finns ingen ledig plats som rymmer fordonet.");
                    return;
                }

                SparaNyParkering(parkeringshus, fordon);

                Console.WriteLine(
                    $"Fordon {fordon.Registreringsnummer} har parkerats på plats {platsnummer}.");

                Console.WriteLine("Parkeringen har sparats.");
            }
            catch (Exception fel)
            {
                Console.WriteLine(
                    $"Fordonet kunde inte parkeras: {fel.Message}");
            }
        }

        private void SparaNyParkering(
            Parkeringshus parkeringshus,
            Fordon fordon)
        {
            try
            {
                SparaParkeringsdata(parkeringshus);
            }
            catch
            {
                Parkeringsplats? plats =
                    parkeringshus.HittaParkeringsplats(
                        fordon.Registreringsnummer);

                if (plats != null)
                {
                    plats.ParkeradeFordon.Remove(fordon);
                }

                throw;
            }
        }

        private void HämtaUtFordon(
            Parkeringshus parkeringshus,
            Prislista prislista,
            Huvudmeny meny)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("HÄMTA UT FORDON");

                Fordonsinmatning inmatning = new Fordonsinmatning();

                string registreringsnummer =
                    inmatning.LäsRegistreringsnummer();

                Parkeringsplats? plats =
                    parkeringshus.HittaParkeringsplats(
                        registreringsnummer);

                if (plats == null)
                {
                    Console.WriteLine(
                        "Inget parkerat fordon med registreringsnumret hittades.");
                    return;
                }

                DateTime uttagstid = DateTime.Now;

                Fordon fordon =
                    parkeringshus.HämtaUtFordon(registreringsnummer);

                decimal avgift = SparaUtlämning(
                    parkeringshus,
                    plats,
                    fordon,
                    uttagstid,
                    prislista);

                TimeSpan parkeringstid =
                    uttagstid - fordon.Ankomsttid;

                meny.VisaKvitto(
                    fordon, uttagstid, parkeringstid, avgift);
            }
            catch (Exception fel)
            {
                Console.WriteLine(
                    $"Fordonet kunde inte hämtas ut: {fel.Message}");
            }
        }

        private decimal SparaUtlämning(
            Parkeringshus parkeringshus,
            Parkeringsplats ursprungligPlats,
            Fordon fordon,
            DateTime uttagstid,
            Prislista prislista)
        {
            try
            {
                Avgiftsberäknare beräknare = new Avgiftsberäknare();

                decimal avgift = beräknare.BeräknaAvgift(
                    fordon, uttagstid, prislista);

                SparaParkeringsdata(parkeringshus);

                return avgift;
            }
            catch
            {
                ursprungligPlats.ParkeradeFordon.Add(fordon);
                throw;
            }
        }

        private void FlyttaFordon(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("FLYTTA FORDON");

                Fordonsinmatning inmatning = new Fordonsinmatning();

                string registreringsnummer =
                    inmatning.LäsRegistreringsnummer();

                Parkeringsplats? nuvarandePlats =
                    parkeringshus.HittaParkeringsplats(
                        registreringsnummer);

                if (nuvarandePlats == null)
                {
                    Console.WriteLine(
                        "Inget parkerat fordon med registreringsnumret hittades.");
                    return;
                }

                Console.WriteLine(
                    $"Fordonet står på plats {nuvarandePlats.Nummer}.");

                int nyttPlatsnummer = inmatning.LäsPlatsnummer(
                    parkeringshus.Parkeringsplatser.Count);

                int gammaltPlatsnummer = parkeringshus.FlyttaFordon(
                    registreringsnummer,
                    nyttPlatsnummer,
                    konfiguration);

                SparaFlytt(
                    parkeringshus,
                    registreringsnummer,
                    gammaltPlatsnummer,
                    konfiguration);

                Console.WriteLine(
                    $"Fordon {registreringsnummer} har flyttats från plats {gammaltPlatsnummer} till plats {nyttPlatsnummer}.");

                Console.WriteLine("Flytten har sparats.");
            }
            catch (Exception fel)
            {
                Console.WriteLine(
                    $"Fordonet kunde inte flyttas: {fel.Message}");
            }
        }

        private void SparaFlytt(
            Parkeringshus parkeringshus,
            string registreringsnummer,
            int gammaltPlatsnummer,
            Parkeringskonfiguration konfiguration)
        {
            try
            {
                SparaParkeringsdata(parkeringshus);
            }
            catch
            {
                parkeringshus.FlyttaFordon(
                    registreringsnummer,
                    gammaltPlatsnummer,
                    konfiguration);

                throw;
            }
        }

        private void SökFordon(Parkeringshus parkeringshus)
        {
            Console.Clear();
            Console.WriteLine("SÖK FORDON");

            Fordonsinmatning inmatning = new Fordonsinmatning();

            string registreringsnummer =
                inmatning.LäsRegistreringsnummer();

            if (string.IsNullOrWhiteSpace(registreringsnummer))
            {
                Console.WriteLine("Du måste ange ett registreringsnummer.");
                return;
            }

            Parkeringsplats? plats =
                parkeringshus.HittaParkeringsplats(registreringsnummer);

            if (plats == null)
            {
                Console.WriteLine(
                    $"Inget parkerat fordon med registreringsnumret {registreringsnummer} hittades.");
                return;
            }

            Console.WriteLine(
                $"Fordon {registreringsnummer} finns på plats {plats.Nummer}.");
        }

        private void KörMeny(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration,
            Prislista prislista)
        {
            Huvudmeny meny = new Huvudmeny();

            while (true)
            {
                string val = meny.LäsMenyval(
                    parkeringshus, konfiguration);

                switch (val)
                {
                    case "Parkera fordon":
                        ParkeraFordon(parkeringshus, konfiguration);
                        break;

                    case "Hämta ut fordon":
                        HämtaUtFordon(parkeringshus, prislista, meny);
                        break;

                    case "Flytta fordon":
                        FlyttaFordon(parkeringshus, konfiguration);
                        break;

                    case "Sök fordon":
                        SökFordon(parkeringshus);
                        break;

                    case "Visa parkeringskarta":
                        Parkeringskarta karta = new Parkeringskarta();
                        karta.Visa(parkeringshus, konfiguration);
                        break;

                    case "Visa parkerade fordon":
                        meny.VisaParkeradeFordon(parkeringshus);
                        break;

                    case "Visa prislista":
                        meny.VisaPrislista(konfiguration, prislista);
                        break;

                    case "Läs om prislista":
                        prislista = LäsOmPrislista(
                            konfiguration, prislista);
                        break;

                    case "Avsluta":
                        return;
                }

                meny.VäntaPåTangent();
            }
        }
    }
}