using System.IO;
using System.Text.Json;

namespace PragParkering.Filhantering
{
    public class Filhanterare
    {
        public T LäsJson<T>(string filsökväg) where T : class
        {
            string json = File.ReadAllText(filsökväg);

            T? innehåll = JsonSerializer.Deserialize<T>(json);

            if (innehåll == null)
            {
                throw new InvalidDataException(
                    "JSON-filen innehåller inget objekt.");
            }

            return innehåll;
        }

        public string[] LäsTextrader(string filsökväg)
        {
            return File.ReadAllLines(filsökväg);
        }

        public void SparaJson<T>(string filsökväg, T innehåll)
        {
            JsonSerializerOptions inställningar =
                new JsonSerializerOptions
                {
                    WriteIndented = true
                };

            string json = JsonSerializer.Serialize(
                innehåll, inställningar);

            string tillfälligFilsökväg = filsökväg + ".tmp";

            File.WriteAllText(tillfälligFilsökväg, json);

            File.Move(
                tillfälligFilsökväg,
                filsökväg,
                true);
        }
    }
}