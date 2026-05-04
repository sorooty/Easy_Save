using System.Collections.ObjectModel;
using EasySave.Core.Model.Service;

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
    private readonly LanguageService _languageService;

    /// <summary>
    /// Collection observable des jobs (liée à l'interface).
    /// </summary>
    public ObservableCollection<SaveJobViewModel> Jobs { get; }

    /// <summary>
    /// Constructeur du ViewModel de liste.
    /// </summary>
    /// <param name="saveExecutor">Service d'exécution</param>
    /// <param name="languageService">Service de localisation</param>
    public SaveJobListViewModel(SaveExecutor saveExecutor, LanguageService languageService)
    {
        _saveExecutor = saveExecutor;
        _languageService = languageService;
        _jobs = new List<SaveJobViewModel>();
        Jobs = new ObservableCollection<SaveJobViewModel>();
    }

    /// <summary>
    /// Ajoute un nouveau job à la liste.
    /// </summary>
    public void AddJob()
    {
        var newJob = new SaveJobViewModel(_saveExecutor, _languageService);
        _jobs.Add(newJob);
        Jobs.Add(newJob);
    }

    /// <summary>
    /// Supprime un job de la liste.
    /// </summary>
    /// <param name="job">Le job à supprimer</param>
    public void RemoveJob(SaveJobViewModel job)
    {
        if (job == null)
        {
            return;
        }

        _jobs.Remove(job);
        Jobs.Remove(job);
    }

    /// <summary>
    /// Exécute tous les jobs valides de la liste.
    /// </summary>
    public void ExecuteAll()
    {
        // ToList() évite les exceptions si la collection est modifiée pendant l'exécution
        var jobsToExecute = Jobs.ToList();

        foreach (var job in jobsToExecute)
        {
            if (job.IsValid())
            {
                job.Execute();
            }
        }
    }

    /// <summary>
    /// Charge les jobs existants depuis la configuration.
    /// TODO: À implémenter avec ConfigService
    /// </summary>
    public void LoadJobs()
    {
        // Exemple d'implémentation future :
        // var configService = new ConfigService();
        // var savedJobs = configService.LoadJobs();
        // 
        // foreach (var savedJob in savedJobs)
        // {
        //     var jobVm = new SaveJobViewModel(_saveExecutor, _languageService)
        //     {
        //         Name = savedJob.Name,
        //         SourceFolder = savedJob.SourceFolder,
        //         TargetFolder = savedJob.TargetFolder,
        //         StrategyName = savedJob.Type == SaveType.Full ? "Full" : "Differential"
        //     };
        //     
        //     _jobs.Add(jobVm);
        //     Jobs.Add(jobVm);
        // }
    }
}