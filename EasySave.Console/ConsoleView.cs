using EasySave.ViewModel;
using System;
using System.Text.RegularExpressions;

namespace EasySave.View
{
    public class ConsoleView
    {
        private readonly SaveJobListViewModel _viewModel;

        private const string CancelCommand = ":cancel";

        public ConsoleView(SaveJobListViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        public async Task Run()
        {
            await ShowMenu();
        }
        private async Task ShowMenu()
        {
            bool running = true;

            while (running)
            {
                DisplayMessage("====================================");
                DisplayMessage("        EasySave v1.0");
                DisplayMessage("====================================\n");

                DisplayMessage(_viewModel.GetText("menu.list"));
                DisplayMessage(_viewModel.GetText("menu.add"));
                DisplayMessage(_viewModel.GetText("menu.remove"));
                DisplayMessage(_viewModel.GetText("menu.run"));
                DisplayMessage(_viewModel.GetText("menu.language"));
                DisplayMessage(_viewModel.GetText("menu.quit"));
                string choice = ReadUserChoice(_viewModel.GetText("menu.choice"));

                switch (choice)
                {
                    case "5":
                        DisplayMessage(_viewModel.GetText("language.prompt"));
                        string languageCode = ReadUserChoice("");

                        _viewModel.ChangeLanguage(languageCode);

                        DisplayMessage(_viewModel.GetText("language.changed"));

                        break;

                    case "4":
                        await DisplayExecuteJobs();
                        break;

                    case "3":
                        DisplayDeleteJobs();
                        break;

                    case "2":
                        DisplayAddJob();
                        break;

                    case "1":
                        DisplaySaveJobs();
                        break;

                    case "6":
                        DisplayMessage(_viewModel.GetText("close.application"));
                        running = false;
                        break;

                    default:
                        DisplayMessage(_viewModel.GetText("error.invalid_choice"));
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

        private void DisplaySaveJobs()
        {
            if (!_viewModel.Jobs.Any())
            {
                DisplayMessage("\n" + _viewModel.GetText("job.no_jobs"));
                return;
            }

            DisplayMessage("\n" + _viewModel.GetText("job.list"));

            for (int i = 0; i < _viewModel.Jobs.Count; i++)
            {
                var job = _viewModel.Jobs[i];

                DisplayMessage($"{i + 1}. {_viewModel.GetText("job.name")} : {job.Name}");
                DisplayMessage($"   {_viewModel.GetText("job.source")} : {job.SourceFolder}");
                DisplayMessage($"   {_viewModel.GetText("job.target")} : {job.TargetFolder}");
                DisplayMessage($"   {_viewModel.GetText("job.type")} : {job.Type}");
                DisplayMessage($"   {_viewModel.GetText("job.status")} : {job.Status}");
                DisplayMessage("");
            }
        }

        private void DisplayAddJob()
        {
            DisplayMessage("\n" + _viewModel.GetText("menu.add"));
            DisplayMessage("--------------------------------");
            DisplayMessage("\n" + _viewModel.GetText("command.cancel"));

            string name;
            do
            {
                name = ReadUserChoice("\n" + _viewModel.GetText("job.name"));

                if (IsCancelCommand(name))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(name))
                    DisplayMessage("\n" + _viewModel.GetText("error.invalid_choice"));

            } while (string.IsNullOrWhiteSpace(name));

            string sourceFolder;
            do
            {
                sourceFolder = ReadUserChoice("\n" + _viewModel.GetText("job.source"));

                if (IsCancelCommand(sourceFolder))
                {
                    return;
                }

                if (!Directory.Exists(sourceFolder))
                    DisplayMessage("\n" + _viewModel.GetText("error.source_not_found"));

            } while (!Directory.Exists(sourceFolder));

            string targetFolder;
            while (true)
            {
                targetFolder = ReadUserChoice("\n" + _viewModel.GetText("job.target"));

                if (IsCancelCommand(targetFolder))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(targetFolder))
                {
                    DisplayMessage("\n" + _viewModel.GetText("error.invalid_choice"));
                    continue;
                }

                if (!Path.IsPathFullyQualified(targetFolder))
                {
                    DisplayMessage("\n" + _viewModel.GetText("error.target_not_found"));
                    continue;
                }

                if (_viewModel.HasReachedMaxJobs())
                {
                    DisplayMessage("\n" + _viewModel.GetText("job.max_reached"));
                    return;
                }

                try
                {
                    if (!Directory.Exists(targetFolder))
                        Directory.CreateDirectory(targetFolder);

                    break;
                }
                catch
                {
                    DisplayMessage("\n" + _viewModel.GetText("error.target_not_found"));
                }
            }

            string typeInput;
            while (true)
            {
                typeInput = ReadUserChoice("\n" + _viewModel.GetText("job.type"));

                if (IsCancelCommand(typeInput))
                {
                    return;
                }

                if (typeInput == "1" || typeInput == "2")
                    break;

                DisplayMessage("\n" + _viewModel.GetText("error.invalid_choice"));
            }

            bool added = _viewModel.AddJob(name, sourceFolder, targetFolder, typeInput);

            DisplayMessage(added
                ? "\n" + _viewModel.GetText("job.added")
                : "\n" + _viewModel.GetText("error.invalid_choice"));
        }

        private async Task DisplayExecuteJobs()
        {
            DisplayMessage("\n" + _viewModel.GetText("menu.run"));
            DisplayMessage("--------------------------------");
            DisplayMessage("\n" + _viewModel.GetText("command.cancel"));

            if (!_viewModel.Jobs.Any())
            {
                DisplayMessage("\n" + _viewModel.GetText("job.no_jobs"));
                return;
            }

            DisplayMessage("\n" + _viewModel.GetText("job.found"));

            for (int i = 0; i < _viewModel.Jobs.Count; i++)
            {
                DisplayMessage($"{i + 1}. {_viewModel.Jobs[i].Name}");
            }

            while (true)
            {
                string command = ReadUserChoice("\n" + _viewModel.GetText("job.run_which") + "\n");

                if (command.Equals(":cancel", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (command.Equals(":help", StringComparison.OrdinalIgnoreCase))
                {
                    DisplayMessage("\n" + _viewModel.GetText("execute.help"));
                    continue;
                }

                bool success = await _viewModel.ExecuteJobs(command);

                if (success)
                {
                    DisplayMessage("\n" + _viewModel.GetText("run.completed"));
                    break;
                }

                DisplayMessage("\n" + _viewModel.GetText("error.invalid_choice"));
            }
        }

        private void DisplayDeleteJobs()
        {
            DisplayMessage("\n" + _viewModel.GetText("menu.remove"));
            DisplayMessage("--------------------------------");
            DisplayMessage("\n" + _viewModel.GetText("command.cancel"));

            bool removed = false;

            while (!removed)
            {
                string name = ReadUserChoice("\n" + _viewModel.GetText("job.name"));

                if (IsCancelCommand(name))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    DisplayMessage("\n" + _viewModel.GetText("error.invalid_choice"));
                    continue;
                }

                removed = _viewModel.RemoveJobByName(name);

                if (!removed)
                    DisplayMessage("\n" + _viewModel.GetText("job.not_found"));
            }

            DisplayMessage("\n" + _viewModel.GetText("job.removed"));
        }

        private bool IsCancelCommand(string input)
        {
            return input.Trim().Equals(CancelCommand, StringComparison.OrdinalIgnoreCase);
        }
    }
}
