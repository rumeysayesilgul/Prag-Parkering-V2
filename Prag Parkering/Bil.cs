namespace PragParkering
{
    public class Bil : Fordon
    {
        public override string Fordonstyp => "Bil";

        public Bil(string registreringsnummer)
            : base(registreringsnummer)
        {
        }
    }
}