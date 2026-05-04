using System;
using System.Text.RegularExpressions;

namespace EasySave.View
{
    public class ConsoleView
    {
        public ConsoleView() { }
        public void Run()
        {
            Console.WriteLine("Bienvenue !\n");
            ShowMenu();
        }
        private void ShowMenu()
        {
            bool running = true;

            while (running)
            {
                DisplayMessage("====================================");
                DisplayMessage("        EasySave v1.0");
                DisplayMessage("====================================\n");
                DisplayMessage("1. Lister les travaux de sauvegarde");
                DisplayMessage("2. Ajouter un travail de sauvegarde");
                DisplayMessage("3. Exécuter un ou plusieurs travaux");
                DisplayMessage("4. Supprimer un travail de sauvegarde");
                DisplayMessage("5. Change Language");
                DisplayMessage("0. Quitter\n");
                string choice = ReadUserChoice("Choissez une option : ");

                switch (choice)
                {
                    case "5":
                        DisplayMessage("La demande de changement de langue a été transmis au ViewModel.");
                        break;

                    case "4":
                        DisplayFakeDeleteJobs();
                        break;

                    case "3":
                        DisplayFakeExecuteJobs();
                        break;

                    case "2":
                        DisplayFakeAddJobs();
                        break;

                    case "1":
                        DisplayMessage("La demande de liste a été transmis au ViewModel.");
                        DisplayFakeSaveJobs();
                        break;

                    case "0":
                        DisplayMessage("Fermeture de l'application...");
                        running = false;
                        break;

                    default:
                        DisplayMessage("Choix invalide.");
                        break;
                }

                Console.WriteLine();
            }

        }

        private string ReadUserChoice(string message)
        {
            DisplayMessage(message);
            return Console.ReadLine() ?? "";
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        private void DisplayFakeSaveJobs()
        {
            DisplayMessage("\nListe des travaux de sauvegarde :");
            DisplayMessage("--------------------------------");

            DisplayMessage("\n1. Projet C#");
            DisplayMessage("   Source : C:\\Users\\Star\\Documents\\ProjetCSharp");
            DisplayMessage("   Cible  : D:\\Backups\\ProjetCSharp");
            DisplayMessage("   Type   : Complète");

            DisplayMessage("\n2. Documents");
            DisplayMessage("   Source : C:\\Users\\Star\\Documents");
            DisplayMessage("   Cible  : D:\\Backups\\Documents");
            DisplayMessage("   Type   : Différentielle");
        }

        private void DisplayFakeAddJobs()
        {
            DisplayMessage("\nAjouter un travail de sauvegarde :");
            DisplayMessage("--------------------------------");

            ReadUserChoice("\nChoisissez un nom : ");
            ReadUserChoice("\nChoisissez un Dossier Source : ");
            ReadUserChoice("\nChoisissez un Dossier Cible : ");
            while (true)
            {
                DisplayMessage("\nChoisissez un Type:");
                DisplayMessage("1. Complète");
                DisplayMessage("2. Différentiel");

                string input = Console.ReadLine() ?? "";

                if (input == "1" || input == "2")
                {
                    break;
                }

                DisplayMessage("Choix invalide, veuillez entrer 1 ou 2.");
            }
            DisplayMessage("Les champs ont été transmis au ViewModel.");
        }

        private void DisplayFakeExecuteJobs()
        {
            DisplayMessage("\nExecuter des travaux de sauvegarde :");
            DisplayMessage("--------------------------------");

            while (true)
            {
                string command = ReadUserChoice("\nEntrer une commande d'execution (taper 'help' pour afficher la syntaxe)\n");
                if (command == "help")
                {
                    DisplayMessage("\nSyntaxe :");
                    DisplayMessage("\nExemple 1 : « EasySave.exe 1-3 » pour exécuter automatiquement les sauvegardes 1 à 3");
                    DisplayMessage("Exemple 2 : « EasySave.exe 1;3 »  pour exécuter automatiquement les sauvegardes 1 et 3");
                    DisplayMessage("\nVous ne pouvez exécuter qu'entre 1 et 5 et travaux");

                } else if (Regex.IsMatch(command, @"^\d[-;]\d$"))  // ancre ^ obligatoire pour éviter les faux positifs
                {
                    
                    DisplayMessage("\nBonne syntaxe, envoie vers ViewModel");
                    break;
                }
                else
                {
                    DisplayMessage("\nMauvaise syntaxe");
                }
            }
        }

        private void DisplayFakeDeleteJobs()
        {
            DisplayMessage("\nSupprimer un travail de sauvegarde :");
            DisplayMessage("--------------------------------");

            ReadUserChoice("\nChoisissez un nom : ");

            DisplayMessage("Le nom a été transmis au ViewModel.");
        }
    }
}
