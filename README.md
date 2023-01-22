# CircularSeas

Repository of the UVIGO developments under WP6 within the research project EAPA_117 / 2018_CircularSeas: “Turning ocean plastic waste into green products for maritime industries”.

It includes the source code of:

* CircularSeasFramework

* CircularSeasExtruder

* CircularSeasPrinter

## CircularSeasFramework

CircularSeas Framework for printing and slicing remotely and easily by users, using an arquitecture composed by three elements:

1. Local Service: Based on Octoprint

2. Cloud Service: Auto G-CODE generator manager using REST API in .NET 5

3. User App: Cross platform app for managing process.

Source code available into src/CircularSeasFramework. Use up to Visual Studio 2019 to open.

## CircularSeasExtruder

Program in TwinCAT 3 for 3D printing filament extruder. Version 3.

Source Code availabe into src/CircularSeasExtruder. Use TwinCAT 3 XAE Shell to open.

### Previous versions.

* Extruder v1: Version made with a drill as extruder, controlled by Arduino. Extruder heater are made of nichrome thread.

* Extruder v2: Version made with a custom machined extruder. Controlled by Arduino and code in repository on LaboratorioRMA3. This prototype has several problems related to temperature control and motion. Stepper motor has low power to achieve the extrusion and K Thermocouples shows an unstable measurement.

### About this prototype

This tries to improve the prototype v2, using a servo motor with a reducer to extrude and PT100 temperature sensors with industrial reading cards. It will be built with Beckhoff TwinCAT 3, including HMI in the development platform itself.

In addition, measurements of filament diameter will perform using OMRON FQ-M Vision Sensor via EtherCAT from TwinCAT3.

Developers that contributes in this repository: Bruno Portela, Diego Silva, Julio Garrido, Enrique Riveiro, Josué Roberto Rivera Andrade y Alejandro Alonso.
