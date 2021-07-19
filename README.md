# SimplexAlgoImplementation

Spezifikationen:

Benchmark Ordner muss sich auf dem Desktop befinden, damit er eingelesen werden kann!
Zielframework: netcoreapp 3.1
Zielruntime: win-x64

Windows 10 64 Bit .Net Core 3.1 
C# Konsolenanwendung

Nutzungsbeschreibung:

Beim start des Programms wird der Benchmark-Ordner eingelesen und die Auswahl an Benchmarks auf der Konsole präsentiert:

//
7 files have been found inside the Benchmark folder:

1. ...\Desktop\Benchmarks\KI_10.txt
2. ...\Desktop\Benchmarks\KI_15.txt
3. ...\Desktop\Benchmarks\KI_20.txt
4. ...\Desktop\Benchmarks\KI_30.txt
5. ...\Desktop\Benchmarks\KI_5.txt
6. ...\Desktop\Benchmarks\KI_8.txt
7. ...\Desktop\Benchmarks\KI_9.txt

Please enter the number of the Benchmark you want to run.
//

Nach Eingabe der entsprechenden Zahl, wird der Simplex Algorithmus angewandt und das Ergebnis präsentiert (Beispiel Benchmark 5):

//
The optimal solution for this minimization problem has been found: 3,571428571428571 after 3 iterations.

All x Values:

x0: 2,6190476190476186

x1: 0

x2: 0

x3: 0

x4: 0,47619047619047616
//

Nach Eingabe der Enter Taste wird das Programm beendet.
