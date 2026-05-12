using System;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using System.Threading;
using System.IO;

namespace EasySave.ViewModel;

/// <summary>
/// ViewModel représentant un seul job de sauvegarde.
/// Fait le lien entre l'interface utilisateur et le cœur métier.
/// </summary>
public class SaveJobViewModel : ViewModelBase
{
    // Champs privés (références au Core)
    private SaveJob _job;
    private readonly SaveExecutor _saveExecutor;
    private readonly LanguageService _languageService;

    // Backing fields
    private string _name = string.Empty;
    private string _sourceFolder = string.Empty;
    private string _targetFolder = string.Empty;
    private string _status = string.Empty;
    private string _resultMessage = string.Empty;
    private SaveType _type;
    private bool _isRunning;
    private int _progressValue;

    public SaveType Type
    {
        get => _type;
        set => Set(ref _type, value);
    }

    // Propriétés publiques (liées à l'interface)

    /// <summary>
    /// Nom du job de sauvegarde.
    /// </summary>
    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    /// <summary>
    /// Dossier source à sauvegarder.
    /// </summary>
    public string SourceFolder
    {
        get => _sourceFolder;
        set => Set(ref _sourceFolder, value);
    }

    /// <summary>
    /// Destination folder.
    /// </summary>
    public string TargetFolder
    {
        get => _targetFolder;
        set => Set(ref _targetFolder, value);
    }

    /// <summary>
    /// État actuel du job (Prêt, En cours, Terminé, Erreur).
    /// </summary>
    public string Status
    {
        get => _status;
        set => Set(ref _status, value);
    }

    /// <summary>
    /// Message de résultat après exécution.
    /// </summary>
    public string ResultMessage
    {
        get => _resultMessage;
        set => Set(ref _resultMessage, value);
    }

    /// <summary>Progression de la sauvegarde en cours (0-100).</summary>
    public int ProgressValue
    {
        get => _progressValue;
        set => Set(ref _progressValue, value);
    }

    /// <summary>True pendant l'exécution du job.</summary>
    public bool IsRunning
    {
        get => _isRunning;
        private set => Set(ref _isRunning, value);
    }

    /// <summary>Commande WPF pour lancer ce job depuis l'interface.</summary>
    public RelayCommand ExecuteCommand { get; }

    /// <summary>
    /// Constructeur du ViewModel.
    /// </summary>
    /// <param name="saveExecutor">Service d'exécution des sauvegardes</param>
    /// <param name="languageService">Service de localisation</param>
    public SaveJobViewModel(SaveExecutor saveExecutor, LanguageService languageService)
    {
        _saveExecutor = saveExecutor;
        _languageService = languageService;
        _job = new SaveJob();
        Status = _languageService.GetText("status.ready");
        ExecuteCommand = new RelayCommand(async _ => await Execute());
    }

    /// <summary>
    /// Returns true if all required fields are set and the source folder exists.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(SourceFolder) &&
               !string.IsNullOrWhiteSpace(TargetFolder) &&
               Directory.Exists(SourceFolder);
    }

    /// <summary>
    /// Crée l'entité métier SaveJob à partir des données du ViewModel.
    /// </summary>
    /// <returns>L'entité SaveJob configurée</returns>
    public SaveJob CreateJob()
    {
        _job.Name = Name;
        _job.SourceFolder = SourceFolder;
        _job.TargetFolder = TargetFolder;
        _job.Type = Type;

        return _job;
    }

    /// <summary>
    /// Executes the backup asynchronously.
    /// </summary>
    public async Task Execute()
    {
        if (!IsValid())
        {
            ResultMessage = _languageService.GetText("job.invalid");
            return;
        }

        IsRunning = true;
        ProgressValue = 0;
        Status = _languageService.GetText("status.running");
        ResultMessage = string.Empty;

        try
        {
            if (!Directory.Exists(TargetFolder))
                Directory.CreateDirectory(TargetFolder);

            var job = CreateJob();
            var progress = new Progress<SaveState>(state =>
            {
                ProgressValue = state.ProgressPercent;
            });

            await _saveExecutor.ExecuteAsync(job, progress, CancellationToken.None);

            Status = _languageService.GetText("status.done");
            ResultMessage = _languageService.GetText("job.success");
            ProgressValue = 100;
        }
        catch (Exception ex)
        {
            Status = _languageService.GetText("status.error");
            ResultMessage = _languageService.GetText("job.error") + ex.Message;
        }
        finally
        {
            IsRunning = false;
        }
    }
}