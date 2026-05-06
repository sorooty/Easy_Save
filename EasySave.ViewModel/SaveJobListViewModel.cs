using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using System.Collections.ObjectModel;

namespace EasySave.ViewModel;

/// <summary>
/// ViewModel gérant la collection de jobs de sauvegarde.
/// Contient et orchestre les SaveJobViewModel.
/// </summary>
public class SaveJobListViewModel : ViewModelBase
{
    // Champs privés
    private readonly List<SaveJobViewModel> _jobs;
    private readonly SaveExecutor _saveExecutor;
    private readonly ConfigService _configservice;
    private readonly LanguageService _languageService;
    private const int MaxJobs = 5;

    /// <summary>
    /// Collection observable des jobs (liée à l'interface).
    /// </summary>
    public ObservableCollection<SaveJobViewModel> Jobs { get; }

    /// <summary>
    /// Constructeur du ViewModel de liste.
    /// </summary>
    /// <param name="saveExecutor">Service d'exécution</param>
    /// <param name="languageService">Service de localisation</param>
    public SaveJobListViewModel(ConfigService configservice, LanguageService languageService , SaveExecutor saveExecutor)
    {
        _saveExecutor = saveExecutor;
        _configservice = configservice;
        _languageService = languageService;
        _jobs = new List<SaveJobViewModel>();
        Jobs = new ObservableCollection<SaveJobViewModel>();
    }

    /// <summary>
    /// Ajoute un nouveau job à la liste.
    /// </summary>
    public bool AddJob(string name, string sourceFolder, string targetFolder, string typeInput)
    {
        if (HasReachedMaxJobs())
        {
            return false;
        }

        var newJob = new SaveJobViewModel(_saveExecutor, _languageService);

        newJob.Name = name;
        newJob.SourceFolder = sourceFolder;
        newJob.TargetFolder = targetFolder;
        newJob.Type = typeInput == "1" ? SaveType.Full : SaveType.Differential;

        if (!newJob.IsValid())
        {
            return false;
        }

        bool alreadyExists = Jobs.Any(job =>
            job.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            return false;
        }

        _jobs.Add(newJob);
        Jobs.Add(newJob);

        SaveJobs();

        return true;
    }

    public bool HasReachedMaxJobs()
    {
        return Jobs.Count >= MaxJobs;
    }

    /// <summary>
    /// Supprime un job de la liste.
    /// </summary>
    /// <param name="job">Le job à supprimer</param>
    public bool RemoveJobByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        SaveJobViewModel? jobToRemove = Jobs
            .FirstOrDefault(job => job.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (jobToRemove == null)
            return false;

        Jobs.Remove(jobToRemove);
        _jobs.Remove(jobToRemove);

        SaveJobs();

        return true;
    }

    /// <summary>
    /// Exécute tous les jobs valides de la liste.
    /// </summary>
    public async Task ExecuteAll()
    {
        foreach (var job in Jobs.ToList())
        {
            if (job.IsValid())
            {
                await job.Execute();
            }
        }
    }

    public async Task<bool> ExecuteJobs(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return false;

        command = command.Trim();

        List<SaveJobViewModel> jobsToExecute = GetJobsFromCommand(command);

        if (jobsToExecute.Count == 0)
            return false;

        foreach (var job in jobsToExecute)
        {
            await job.Execute();
        }

        return true;
    }

    private List<SaveJobViewModel> GetJobsFromCommand(string command)
    {
        var jobsToExecute = new List<SaveJobViewModel>();

        if (command.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return Jobs.ToList();
        }

        // Exemple : 1
        if (int.TryParse(command, out int singleIndex))
        {
            AddJobByIndex(jobsToExecute, singleIndex);
            return jobsToExecute;
        }

        // Exemple : 1-3
        if (command.Contains('-'))
        {
            string[] parts = command.Split('-');

            if (parts.Length != 2)
                return jobsToExecute;

            if (!int.TryParse(parts[0], out int start))
                return jobsToExecute;

            if (!int.TryParse(parts[1], out int end))
                return jobsToExecute;

            if (start > end)
                return jobsToExecute;

            for (int i = start; i <= end; i++)
            {
                AddJobByIndex(jobsToExecute, i);
            }

            return jobsToExecute;
        }

        // Exemple : 1;3
        if (command.Contains(';'))
        {
            string[] parts = command.Split(';');

            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int index))
                    return new List<SaveJobViewModel>();

                AddJobByIndex(jobsToExecute, index);
            }

            return jobsToExecute;
        }

        return jobsToExecute;
    }

    private void AddJobByIndex(List<SaveJobViewModel> jobsToExecute, int index)
    {
        int realIndex = index - 1;

        if (realIndex < 0 || realIndex >= Jobs.Count)
            return;

        SaveJobViewModel job = Jobs[realIndex];

        if (!jobsToExecute.Contains(job))
        {
            jobsToExecute.Add(job);
        }
    }

    /// <summary>
    /// Charge les jobs existants depuis la configuration.
    /// TODO: À implémenter avec ConfigService
    /// </summary>
    public void LoadJobs()
    {
        var savedJobs = _configservice.LoadJobs();

        _jobs.Clear();
        Jobs.Clear();

        foreach (var savedJob in savedJobs)
        {
            var jobVm = new SaveJobViewModel(_saveExecutor, _languageService)
            {
                Name = savedJob.Name,
                SourceFolder = savedJob.SourceFolder,
                TargetFolder = savedJob.TargetFolder,
                Type = savedJob.Type
            };

            _jobs.Add(jobVm);
            Jobs.Add(jobVm);
        }
    }

    public void ChangeLanguage(string languageCode)
    {
        _languageService.SetLanguage(languageCode);
    }

    public string GetText(string key)
    {
        return _languageService.GetText(key);
    }

    public void SaveJobs()
    {
        var jobsToSave = Jobs
            .Select(jobVm => jobVm.CreateJob())
            .ToList();

        _configservice.SaveJobs(jobsToSave);
    }
}

