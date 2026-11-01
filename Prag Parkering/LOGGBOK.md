# Loggbok – Prag Parkering V2

## 13/10
Började med att läsa igenom uppgiften och planera upplägget.
Det var ganska mycket att hålla koll på, och jag hade svårt
att få en överblick över alla krav. Jag delade därför upp
uppgiften i mindre delar och fokuserade på kraven för G.

## 14/10–15/10
Planerade vilka klasser och metoder som behövdes.
Till en början var det lite oklart vilken kod som skulle
ligga var. Jag delade upp ansvaret mellan fordon,
parkeringsplatser och parkeringshuset.

Jag behövde också förstå hur de olika projekten skulle
kopplas ihop. Det löstes med projektreferenser mellan
konsolprogrammet, filhanteringen och testerna.

## 16/10–17/10
Skapade klasserna Fordon, Bil och MC. För att slippa upprepa
samma kod lade jag registreringsnummer och ankomsttid i
basklassen Fordon. Bil och MC fick sedan ärva från den.

Fortsatte med Parkeringsplats och Parkeringshus. En svårighet
var att få en plats att rymma antingen en bil eller två
motorcyklar i den nya strukturen. Jag använde en lista med
fordon på varje plats och lade till en kontroll av hur mycket
utrymme som fanns kvar.

## 18/10 & 20/10
Förberedde konfigurationsfilen så att antalet platser och
antalet fordon per plats kunde ändras. Jag behövde också
hantera felaktiga inställningar och lade därför till
kontroller av värdena som lästes in.

Sedan kopplade jag konfigurationen till parkeringen.
Det var lite klurigt att få skillnaden mellan bil och MC
att fungera. Jag löste det genom att beräkna fordonens
utrymmesbehov utifrån inställningarna i konfigurationsfilen.

## 21/10
Lade till sökning efter registreringsnummer.
Sökningen behövde fungera även om användaren blandade stora
och små bokstäver. Jag använde därför en jämförelse som
bortser från den skillnaden.

## 22/10
Fortsatte med att kunna flytta fordon mellan platser.
Jag behövde se till att fordonet stod kvar på sin gamla plats
om den nya platsen var full. Det löstes genom att kontrollera
utrymmet innan själva flytten genomfördes.

Började även med utlämning och parkeringstid.
En viktig sak var att ankomsttiden inte skulle ändras när
ett fordon flyttades. Jag behöll samma fordonsobjekt och
flyttade det mellan platserna i stället för att skapa ett nytt.

## 23/10–24/10
Ordnade sparning och inläsning av parkeringsdata i JSON.
En svårighet var att programmet behövde veta om ett sparat
fordon var en Bil eller MC. Jag registrerade därför båda
fordonstyperna för JSON-serialiseringen.

Samlade också läsning och skrivning av filer i klassbiblioteket
för filhantering. Det gjorde det lättare att hålla reda på
filkoden och återanvända samma metoder.

## 25/10–27/10
Lade till prislistan och avgiftsberäkningen.
Det som krävde mest eftertanke var gratisperioden och hur
påbörjade timmar skulle räknas. Jag använde skillnaden
mellan ankomsttid och uttagstid och avrundade timantalet uppåt.

Fortsatte med menyn och användarens inmatning.
Felaktiga platsnummer skulle inte få programmet att avslutas.
Jag använde TryParse för att kontrollera inmatningen och
lät användaren försöka igen om värdet var fel.

Gick sedan igenom kopplingen mellan klasserna och förberedde
konfiguration, prislista och testdata. Ibland blev det svårt
att följa programflödet. Då gick jag igenom metodanropen
steg för steg och kontrollerade vad varje metod ansvarade för.

## 29/10
Kopplade ihop funktionerna och testade programmet.

Ett byggfel uppstod eftersom en metod saknades i den sparade
filen. Jag lade in metoden på rätt plats, kontrollerade
klammerparenteserna och byggde lösningen igen.

Parkeringskartan hade också ett felaktigt metodanrop.
Jag kontrollerade vilken klass metoden tillhörde och ändrade
anropet. Efter det fungerade kartan som tänkt.

När jag ändrade prislistan visades först det gamla priset.
Jag kontrollerade att ändringen var sparad i filen som
programmet använde i körmappen. Efter en ny omläsning via
menyn visades rätt pris.

Testade även att parkering, flytt och utlämning sparades
korrekt genom att starta om programmet och kontrollera
resultatet. Körde till sist enhetstesterna, och båda godkändes.

## 31/10
Gjorde en sista genomgång av programmet, testdatan och
dokumentationen. Kontrollerade att README beskrev hur
programmet startas, hur prislistan ändras och hur testerna
körs.