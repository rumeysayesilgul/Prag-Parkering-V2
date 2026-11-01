using System;
using System.Collections.Generic;
using System.Globalization;

namespace PragParkering
{
    public class Prislista
    {
        private readonly Dictionary<string, decimal> timpriser =
            new Dictionary<string, decimal>(
                StringComparer.OrdinalIgnoreCase);

        public Prislista(string[] rader)
        {
            foreach (string rad in rader)
            {
                LäsPrisrad(rad);
            }
        }

        public void Validera(
            Parkeringskonfiguration konfiguration)
        {
            foreach (Fordonsinställning fordonstyp
                in konfiguration.Fordonstyper)
            {
                HämtaTimpris(fordonstyp.Namn);
            }
        }

        private void LäsPrisrad(string rad)
        {
            string innehåll = rad.Split('#')[0].Trim();

            if (innehåll.Length == 0)
            {
                return;
            }

            string[] delar = innehåll.Split('=');

            if (delar.Length != 2 ||
                string.IsNullOrWhiteSpace(delar[0]))
            {
                throw new FormatException(
                    $"Ogiltig prisrad: {rad}. Använd fordonstyp=timpris.");
            }

            string fordonstyp = delar[0].Trim();
            decimal timpris = TolkaTimpris(delar[1]);

            if (!timpriser.TryAdd(fordonstyp, timpris))
            {
                throw new FormatException(
                    $"Fordonstypen {fordonstyp} finns flera gånger i prislistan.");
            }
        }

        private decimal TolkaTimpris(string text)
        {
            bool giltigtPris = decimal.TryParse(
                text.Trim(),
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal timpris);

            if (!giltigtPris)
            {
                throw new FormatException(
                    $"Ogiltigt timpris: {text}. Ange ett tal som är noll eller positivt.");
            }

            return timpris;
        }

        public decimal HämtaTimpris(string fordonstyp)
        {
            if (!timpriser.TryGetValue(
                fordonstyp, out decimal timpris))
            {
                throw new ArgumentException(
                    $"Timpris saknas för fordonstypen {fordonstyp}.");
            }

            return timpris;
        }
    }
}