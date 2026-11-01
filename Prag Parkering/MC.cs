namespace PragParkering
{
    public class MC : Fordon
    {
        public override string Fordonstyp => "MC";

        public MC(string registreringsnummer)
            : base(registreringsnummer)
        {
        }
    }
}