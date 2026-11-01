using System;
using Spectre.Console;

namespace PragParkering
{
    public class Parkeringskarta
    {
        public void Visa(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            AnsiConsole.Clear();

            AnsiConsole.MarkupLine(
                "[bold green]PARKERINGSKARTA[/]");

            VisaÖversikt(parkeringshus, konfiguration);
        }

        public void VisaÖversikt(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            AnsiConsole.MarkupLine(
                "[black on green] L = Ledig [/]  " +
                "[black on yellow] D = Delvis upptagen [/]  " +
                "[white on red] F = Full [/]");

            AnsiConsole.WriteLine();

            VisaPlatser(parkeringshus, konfiguration);

            AnsiConsole.WriteLine();

            VisaSammanfattning(parkeringshus, konfiguration);
        }

        private void VisaPlatser(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            int antalPlatser = parkeringshus.Parkeringsplatser.Count;

            int sifferbredd = Math.Max(
                3, antalPlatser.ToString().Length);

            string rad = "";

            for (int index = 0; index < antalPlatser; index++)
            {
                Parkeringsplats plats =
                    parkeringshus.Parkeringsplatser[index];

                string status = HämtaStatus(plats, konfiguration);

                rad += FormateraPlats(
                    plats.Nummer, status, sifferbredd) + " ";

                if ((index + 1) % 10 == 0 ||
                    index == antalPlatser - 1)
                {
                    AnsiConsole.MarkupLine(rad);
                    rad = "";
                }
            }
        }

        private string HämtaStatus(
            Parkeringsplats plats,
            Parkeringskonfiguration konfiguration)
        {
            if (plats.ParkeradeFordon.Count == 0)
            {
                return "Ledig";
            }

            if (plats.BeräknaUpptagenStorlek(konfiguration) >=
                plats.Storlek)
            {
                return "Full";
            }

            return "Delvis";
        }

        private string FormateraPlats(
            int platsnummer,
            string status,
            int sifferbredd)
        {
            string nummer = platsnummer.ToString()
                .PadLeft(sifferbredd, '0');

            switch (status)
            {
                case "Ledig":
                    return $"[black on green] {nummer} L [/]";

                case "Delvis":
                    return $"[black on yellow] {nummer} D [/]";

                default:
                    return $"[white on red] {nummer} F [/]";
            }
        }

        private void VisaSammanfattning(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            int antalLediga = 0;
            int antalDelvisUpptagna = 0;
            int antalFulla = 0;

            foreach (Parkeringsplats plats
                in parkeringshus.Parkeringsplatser)
            {
                string status = HämtaStatus(plats, konfiguration);

                switch (status)
                {
                    case "Ledig":
                        antalLediga++;
                        break;

                    case "Delvis":
                        antalDelvisUpptagna++;
                        break;

                    case "Full":
                        antalFulla++;
                        break;
                }
            }

            AnsiConsole.MarkupLine(
                $"[bold green]Lediga: {antalLediga}[/]  " +
                $"[bold yellow]Delvis: {antalDelvisUpptagna}[/]  " +
                $"[bold red]Fulla: {antalFulla}[/]  " +
                $"Totalt: {parkeringshus.Parkeringsplatser.Count}");
        }
    }
}