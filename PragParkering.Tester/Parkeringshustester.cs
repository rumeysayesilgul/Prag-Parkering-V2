using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragParkering;

namespace PragParkering.Tester
{
    [TestClass]
    public class ParkeringshusTester
    {
        [TestMethod]
        public void TvåMotorcyklarRymsMenEnTredjeNekas()
        {
            Parkeringskonfiguration konfiguration =
                SkapaKonfiguration(1);

            Parkeringshus parkeringshus = new Parkeringshus(
                konfiguration.AntalParkeringsplatser);

            int förstaPlatsen = parkeringshus.ParkeraFordon(
                new MC("MC001"), konfiguration);

            int andraPlatsen = parkeringshus.ParkeraFordon(
                new MC("MC002"), konfiguration);

            int tredjePlatsen = parkeringshus.ParkeraFordon(
                new MC("MC003"), konfiguration);

            Assert.AreEqual(1, förstaPlatsen);
            Assert.AreEqual(1, andraPlatsen);
            Assert.AreEqual(-1, tredjePlatsen);

            Assert.AreEqual(
                2,
                parkeringshus.Parkeringsplatser[0]
                    .ParkeradeFordon.Count);
        }

        [TestMethod]
        public void BilFårEgenPlatsNärFörstaPlatsenHarEnMC()
        {
            Parkeringskonfiguration konfiguration =
                SkapaKonfiguration(2);

            Parkeringshus parkeringshus = new Parkeringshus(
                konfiguration.AntalParkeringsplatser);

            int motorcykelnsPlats = parkeringshus.ParkeraFordon(
                new MC("MC001"), konfiguration);

            int bilensPlats = parkeringshus.ParkeraFordon(
                new Bil("BIL001"), konfiguration);

            Assert.AreEqual(1, motorcykelnsPlats);
            Assert.AreEqual(2, bilensPlats);

            Assert.AreEqual(
                1,
                parkeringshus.Parkeringsplatser[0]
                    .ParkeradeFordon.Count);

            Assert.AreEqual(
                1,
                parkeringshus.Parkeringsplatser[1]
                    .ParkeradeFordon.Count);
        }

        private static Parkeringskonfiguration SkapaKonfiguration(
            int antalPlatser)
        {
            return new Parkeringskonfiguration
            {
                AntalParkeringsplatser = antalPlatser,

                Fordonstyper = new List<Fordonsinställning>
                {
                    new Fordonsinställning
                    {
                        Namn = "Bil",
                        AntalFordonPerPlats = 1
                    },
                    new Fordonsinställning
                    {
                        Namn = "MC",
                        AntalFordonPerPlats = 2
                    }
                }
            };
        }
    }
}