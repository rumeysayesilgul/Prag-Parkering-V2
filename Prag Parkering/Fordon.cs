using System;
using System.Text.Json.Serialization;

namespace PragParkering
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$fordonstyp")]
    [JsonDerivedType(typeof(Bil), "Bil")]
    [JsonDerivedType(typeof(MC), "MC")]
    public abstract class Fordon
    {
        public string Registreringsnummer { get; set; }

        public DateTime Ankomsttid { get; set; }

        public abstract string Fordonstyp { get; }

        protected Fordon(string registreringsnummer)
        {
            Registreringsnummer = registreringsnummer;
            Ankomsttid = DateTime.Now;
        }
    }
}