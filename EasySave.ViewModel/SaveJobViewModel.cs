using System;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using System.Security.AccessControl;
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

    // Backing fields pour les propriétés
    private string _name = string.Empty;
    private string _sourceFolder = string.Empty;
    private string _targetFolder = string.Empty;
    private string _status = string.Empty;
    private string _resultMessage = string.Empty;
    private string _strategyName = "Full";
    public SaveType Type { get; set; }

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
    /// Dossier de destination.
    /// </,summary>
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

    /// <summary>
    /// Type de stratégie (Full ou Differential).
    /// </summary>
    public string StrategyName
    {
        get => _strategyName;
        set => Set(ref _strategyName, value);
    }

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
        Status = "Prêt";
    }

    /// <summary>
    /// Vérifie que le job est correctement configuré.
    /// </summary>
    /// <returns>Vrai si le job est valide</returns>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(SourceFolder) ||
            string.IsNullOrWhiteSpace(TargetFolder))
        {
            return false;
        }

        // Vérifier que la source existe
        if (!Directory.Exists(SourceFolder))
        {
            return false;
        }

        // Créer la cible si elle n'existe pas
        if (!Directory.Exists(TargetFolder))
        {
            try
            {
                Directory.CreateDirectory(TargetFolder);
            }
            catch
            {
                return false;
            }
        }

        return true;
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
    /// Exécute la sauvegarde de manière asynchrone.
    /// </summary>
    public async Task Execute()
    {
        if (!IsValid())
        {
            ResultMessage = "Job invalide - Vérifiez les champs obligatoires";
            return;
        }

        Status = "En cours...";
        ResultMessage = string.Empty;

        try
        {
            var job = CreateJob();

            await _saveExecutor.ExecuteAsync(job, null, CancellationToken.None);

            Status = "Terminé";
            ResultMessage = "Sauvegarde réussie avec succès";
        }
        catch (Exception ex)
        {
            Status = "Erreur";
            ResultMessage = $"Erreur lors de la sauvegarde: {ex.Message}";
        }
    }
}