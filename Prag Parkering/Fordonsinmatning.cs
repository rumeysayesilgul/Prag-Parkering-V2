using System;
using System.Collections.Generic;
using Spectre.Console;

namespace PragParkering
{
    public class Fordonsinmatning
    {
        public Fordon LäsFordon(
            Parkeringskonfiguration konfiguration)
        {
            AnsiConsole.Clear();

            AnsiConsole.MarkupLine(
                "[bold green]PARKERA FORDON[/]");

            string fordonstyp = LäsFordonstyp(konfiguration);
            string registreringsnummer = LäsRegistreringsnummer();

            return SkapaFordon(fordonstyp, registreringsnummer);
        }

        private string LäsFordonstyp(
            Parkeringskonfiguration konfiguration)
        {
            List<string> fordonstyper = new List<string>();

            foreach (Fordonsinställning inställning
                in konfiguration.Fordonstyper)
            {
                fordonstyper.Add(inställning.Namn);
            }

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj fordonstyp:")
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(fordonstyper));
        }

        public string LäsRegistreringsnummer()
        {
            AnsiConsole.Write("Ange registreringsnummer: ");

            string registreringsnummer =
                Console.ReadLine() ?? string.Empty;

            return registreringsnummer.Trim().ToUpperInvariant();
        }

        public int LäsPlatsnummer(int antalParkeringsplatser)
        {
            while (true)
            {
                AnsiConsole.Write(
                    $"Ange nytt platsnummer (1–{antalParkeringsplatser}): ");

                string inmatning = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(inmatning, out int platsnummer) &&
                    platsnummer >= 1 &&
                    platsnummer <= antalParkeringsplatser)
                {
                    return platsnummer;
                }

                AnsiConsole.WriteLine(
                    $"Ange ett heltal mellan 1 och {antalParkeringsplatser}.");
            }
        }

        private Fordon SkapaFordon(
            string fordonstyp,
            string registreringsnummer)
        {
            switch (fordonstyp.ToUpperInvariant())
            {
                case "BIL":
                    return new Bil(registreringsnummer);

                case "MC":
                    return new MC(registreringsnummer);

                default:
                    throw new ArgumentException(
                        $"Fordonstypen {fordonstyp} stöds inte av programmet.");
            }
        }
    }
}