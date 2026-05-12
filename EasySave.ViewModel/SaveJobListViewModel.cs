using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using System.Collections.ObjectModel;
using System.Linq; // ++ modif Ajouté pour les opérations sur les listes

namespace EasySave.ViewModel;

public class SaveJobListViewModel : ViewModelBase
{
    private readonly SaveExecutor _saveExecutor;
    private readonly ConfigService _configservice;
    private readonly LanguageService _languageService;

    // Suppression de MaxJobs = 5 pour permettre l'illimité

    public ObservableCollection<SaveJobViewModel> Jobs { get; }

    public SaveJobListViewModel(ConfigService configservice, LanguageService languageService, SaveExecutor saveExecutor)
    {
        _saveExecutor = saveExecutor;
        _configservice = configservice;
        _languageService = languageService;
        Jobs = new ObservableCollection<SaveJobViewModel>();
    }

    /// <summary>
    /// Ajoute un nouveau job à la liste (sans limite de nombre).
    /// </summary>
    public bool AddJob(string name, string sourceFolder, string targetFolder, string typeInput)
    {
        // Création du nouveau ViewModel
        var newJob = new SaveJobViewModel(_saveExecutor, _languageService)
        {
            Name = name,
            SourceFolder = sourceFolder,
            TargetFolder = targetFolder,
            Type = typeInput == "1" ? SaveType.Full : SaveType.Differential
        };

        // Validation des données et vérification d'unicité
        if (!newJob.IsValid() || Jobs.Any(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        Jobs.Add(newJob);
        SaveJobs(); // Persistance immédiate
        return true;
    }

    // HasReachedMaxJobs est supprimé car il n'y a plus de limite

    public bool RemoveJobByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        var jobToRemove = Jobs.FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (jobToRemove == null) return false;

        Jobs.Remove(jobToRemove);
        SaveJobs();
        return true;
    }

    public async Task<bool> ExecuteJobs(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return false;

        var jobsToExecute = GetJobsFromCommand(command.Trim());
        if (!jobsToExecute.Any()) return false;

        foreach (var job in jobsToExecute)
        {
            await job.Execute();
        }
        return true;
    }

    private List<SaveJobViewModel> GetJobsFromCommand(string command)
    {
        // La logique existante est robuste et fonctionne déjà avec N jobs.
        // Elle parse "all", "1-3" ou "1;3" sans limite supérieure.
        var jobsToExecute = new List<SaveJobViewModel>();

        if (command.Equals("all", StringComparison.OrdinalIgnoreCase))
            return Jobs.ToList();

        if (int.TryParse(command, out int singleIndex))
        {
            AddJobByIndex(jobsToExecute, singleIndex);
        }
        else if (command.Contains('-'))
        {
            string[] parts = command.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
            {
                for (int i = start; i <= end; i++) AddJobByIndex(jobsToExecute, i);
            }
        }
        else if (command.Contains(';'))
        {
            foreach (var part in command.Split(';'))
            {
                if (int.TryParse(part, out int index)) AddJobByIndex(jobsToExecute, index);
            }
        }

        return jobsToExecute;
    }

    private void AddJobByIndex(List<SaveJobViewModel> list, int index)
    {
        int realIndex = index - 1;
        if (realIndex >= 0 && realIndex < Jobs.Count && !list.Contains(Jobs[realIndex]))
        {
            list.Add(Jobs[realIndex]);
        }
    }

    public void LoadJobs()
    {
        var savedJobs = _configservice.LoadJobs();
        Jobs.Clear();

        foreach (var job in savedJobs)
        {
            Jobs.Add(new SaveJobViewModel(_saveExecutor, _languageService)
            {
                Name = job.Name,
                SourceFolder = job.SourceFolder,
                TargetFolder = job.TargetFolder,
                Type = job.Type
            });
        }
    }

    public void SaveJobs()
    {
        var jobsToSave = Jobs.Select(vm => vm.CreateJob()).ToList();
        _configservice.SaveJobs(jobsToSave);
    }
}