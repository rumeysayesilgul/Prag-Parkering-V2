# Prag Parkering V2
Programmet är byggt med Visual Studio 2026 och .NET 10

## Starta programmet

1. Öppna `Prag Parkering V2.slnx`.
2. Högerklicka på projektet `Prag Parkering` och välj "Set as Startup Project".
3. Starta med "Ctrl+F5". NuGet-paketen återställs automatiskt vid bygge.

Konfiguration, prislista och medföljande testdata kopieras automatiskt
till programmets körmapp.

## Ändra prislistan

Redigera `prislista.txt` i körmappen, normalt `Prag Parkering/bin/Debug/net10.0`, 
och spara. Välj sedan "Läs om prislista" i programmet.

## Tester

Öppna Test → Test Explorer och välj "Run All Tests".
