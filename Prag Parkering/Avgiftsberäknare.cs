using System;

namespace PragParkering
{
    public class Avgiftsberäknare
    {
        public TimeSpan BeräknaParkeringstid(
            Fordon fordon,
            DateTime uttagstid)
        {
            if (uttagstid < fordon.Ankomsttid)
            {
                throw new ArgumentException(
                    "Uttagstiden får inte vara tidigare än ankomsttiden.");
            }

            return uttagstid - fordon.Ankomsttid;
        }

        public decimal BeräknaAvgift(
            Fordon fordon,
            DateTime uttagstid,
            Prislista prislista)
        {
            TimeSpan parkeringstid =
                BeräknaParkeringstid(fordon, uttagstid);

            if (parkeringstid <= TimeSpan.FromMinutes(10))
            {
                return 0m;
            }

            decimal påbörjadeTimmar =
                (decimal)Math.Ceiling(parkeringstid.TotalHours);

            decimal timpris =
                prislista.HämtaTimpris(fordon.Fordonstyp);

            return påbörjadeTimmar * timpris;
        }
    }
}