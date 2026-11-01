using System;
using Spectre.Console;

namespace PragParkering
{
    public class Huvudmeny
    {
        public string LäsMenyval(
            Parkeringshus parkeringshus,
            Parkeringskonfiguration konfiguration)
        {
            AnsiConsole.Clear();

            AnsiConsole.MarkupLine(
                "[bold green]PRAG PARKERING V2[/]");

            Parkeringskarta karta = new Parkeringskarta();
            karta.VisaÖversikt(parkeringshus, konfiguration);

            AnsiConsole.WriteLine();

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj ett alternativ:")
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(
                        "Parkera fordon",
                        "Hämta ut fordon",
                        "Flytta fordon",
                        "Sök fordon",
                        "Visa parkerade fordon",
                        "Visa prislista",
                        "Läs om prislista",
                        "Avsluta"));
        }

        public void VisaParkeradeFordon(Parkeringshus parkeringshus)
        {
            AnsiConsole.Clear();

            AnsiConsole.MarkupLine(
                "[bold green]PARKERADE FORDON[/]");

            Table tabell = new Table();

            tabell.AddColumn("Plats");
            tabell.AddColumn("Registreringsnummer");
            tabell.AddColumn("Fordonstyp");
            tabell.AddColumn("Ankomsttid");

            int antalFordon = 0;

            foreach (Parkeringsplats plats
                in parkeringshus.Parkeringsplatser)
            {
                foreach (Fordon fordon in plats.ParkeradeFordon)
                {
                    tabell.AddRow(
                        plats.Nummer.ToString(),
                        Markup.Escape(fordon.Registreringsnummer),
                        Markup.Escape(fordon.Fordonstyp),
                        fordon.Ankomsttid.ToString("yyyy-MM-dd HH:mm:ss"));

                    antalFordon++;
                }
            }

            if (antalFordon == 0)
            {
                AnsiConsole.WriteLine("Inga fordon är parkerade.");
                return;
            }

            AnsiConsole.Write(tabell);
            AnsiConsole.WriteLine($"Antal parkerade fordon: {antalFordon}");
        }

        public void VisaKvitto(
            Fordon fordon,
            DateTime uttagstid,
            TimeSpan parkeringstid,
            decimal avgift)
        {
            AnsiConsole.Clear();

            AnsiConsole.MarkupLine(
                "[bold green]PARKERINGSKVITTO[/]");

            string tidsbeskrivning =
                $"{parkeringstid.Days} dygn, " +
                $"{parkeringstid.Hours} timmar, " +
                $"{parkeringstid.Minutes} minuter och " +
                $"{parkeringstid.Seconds} sekunder";

            Table tabell = new Table();

            tabell.AddColumn("Uppgift");
            tabell.AddColumn("Värde");

            tabell.AddRow(
                "Registreringsnummer",
                Markup.Escape(fordon.Registreringsnummer));

            tabell.AddRow(
                "Fordonstyp",
                Markup.Escape(fordon.Fordonstyp));

            tabell.AddRow(
                "Ankomsttid",
                fordon.Ankomsttid.ToString("yyyy-MM-dd HH:mm:ss"));

            tabell.AddRow(
                "Uttagstid",
                uttagstid.ToString("yyyy-MM-dd HH:mm:ss"));

            tabell.AddRow("Parkeringstid", tidsbeskrivning);

            tabell.AddRow(
                "Avgift",
                $"{avgift:0.00} CZK");

            AnsiConsole.Write(tabell);

            AnsiConsole.WriteLine(
                "Fordonet har hämtats ut och ändringen har sparats.");
        }

        public void VisaPrislista(
            Parkeringskonfiguration konfiguration,
            Prislista prislista)
        {
            AnsiConsole.Clear();

            AnsiConsole.MarkupLine(
                "[bold green]PRISLISTA[/]");

            AnsiConsole.MarkupLine(
                "[bold green]Parkering i högst 10 minuter är gratis för bil och MC.[/]");

            AnsiConsole.WriteLine();

            Table tabell = new Table();

            tabell.AddColumn("Fordonstyp");
            tabell.AddColumn("CZK per påbörjad timme");

            foreach (Fordonsinställning fordonstyp
                in konfiguration.Fordonstyper)
            {
                decimal timpris =
                    prislista.HämtaTimpris(fordonstyp.Namn);

                tabell.AddRow(
                    Markup.Escape(fordonstyp.Namn),
                    timpris.ToString());
            }

            AnsiConsole.Write(tabell);

            AnsiConsole.WriteLine();

            AnsiConsole.WriteLine(
                "Vid längre parkering debiteras varje påbörjad timme från ankomsttiden.");
        }

        public void VäntaPåTangent()
        {
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine(
                "[grey]Tryck på valfri tangent för att återgå till menyn.[/]");

            Console.ReadKey(true);
        }
    }
}