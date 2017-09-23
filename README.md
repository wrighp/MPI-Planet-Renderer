# MPI-Planet-Renderer
Unity rendering tool for planet simulation using output log files. Log files are generated with a custom MPI program for use on an [IBM Bluegene/Q system](https://en.wikipedia.org/wiki/Blue_Gene). This is part of a larger project that benchmarks this simulation with various Bluegene/Q runtime parameters via [SLURM](https://slurm.schedmd.com/).


Log files from ./orbit (having set #define OUTPUT_LOG 1) go into /Resources/Text/ folder to run in-editor.

Program is best run with the scene view tab hidden, as to avoid editor gui errors not present in the exported version, but is useful for moving the camera around to where you want to look.
![Alt text](/images/img2.png?raw=true "")
![Alt text](/images/img3.png?raw=true "")
![Alt text](/images/img4.png?raw=true "")
![Alt text](/images/img.png?raw=true "")
