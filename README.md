# .Net-iText7-Aspose-Codbar
created by ClemNOZIROD

your porject dotnet |
                    |
                      bin
                      obj
                      packages
                      AsposeProcessor.cs
                      iTextProcessor.cs
                      Program.cs
                      codbar.csproj



The code will monitor a directory defined in the program.cs file (watchedfolderPath) and as soon as a file is created, it will search for specific character sequences to create a bar code in the iTextProcessor.cs file and replace words in the AsposeProcessor.cs file.
In the Program.cs file, we'll look for the location that will define the number of pages to be printed and repeat the printing process.

FR : 
Le code vas surveiller un repertoire que l'on definie dans le fichier program.cs (watchedfolderPath) et des qu'un fchier seras crée, ont vas chercher des suite de caractere specifique pour crée un code barre dans le fichier iTextProcessor.cs et remplacer des mots dans le fichier AsposeProcessor.cs.

Dans le fichier Program.cs on vas chercher l'endroit qui vas definir le nombre de page a imprimer et répète l'impression.
le fichier Program.cs vas appeler les deux autres porgrames, j'ai fait ça car il y avait des bloquages avec les bibliotheques 


pour installer : 
to setup : 

https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.421-windows-x64-installer
lancer l'exe dotnet (.net framwork) 
-----------------------------------------------------------------------------
Installer les packages 
dotnet add package itext7 --version 7.1.15
dotnet add package ZXing.Net --version 0.16.4
dotnet add package GroupDocs.Redaction
dotnet add package Aspose –version 24.4.0

----------------------------------------------------------------------------
Installer nuget 
----------------------------------------------------------------------------
install-PackageProvider -Name NuGet -Force
Install-Module -Name NuGet -Force
Get-Module -ListAvailable -Name NuGet

modifier chemin du repertoir a surveiller dans program.cs
modifier chemin du codbar.exe a executer dans codbar.bat

-----------------------------------------------------------------------------
pour faire fonctionner : 
to run : 
powershell:
dotnet build codbar.csproj  
run  \yourproject\bin\Debug\net6.0\yourproject.exe
