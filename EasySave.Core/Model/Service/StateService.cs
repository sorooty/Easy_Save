using System.Text.Json;
using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Maintient state.json à jour avec l'état de chaque travail de sauvegarde.
    /// Chaque appel à UpdateState reécrit le fichier entier (liste de tous les travaux).
    /// </summary>
    public class StateService : IStateService
    {
        private readonly IPathService _paths;

        // Verrou pour éviter les écritures concurrentes sur state.json
        private readonly object _lock = new();

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public StateService(IPathService paths)
        {
            _paths = paths;
        }

        public void UpdateState(SaveState state)
        {
            lock (_lock)
            {
                // Lecture de la liste existante pour ne pas écraser les autres travaux
                List<SaveState> states = ReadCurrentStates();

                int index = states.FindIndex(s => s.Name == state.Name);
                if (index >= 0)
                    states[index] = state;
                else
                    states.Add(state);

                File.WriteAllText(_paths.StateFile, JsonSerializer.Serialize(states, JsonOptions));
            }
        }

        private List<SaveState> ReadCurrentStates()
        {
            if (!File.Exists(_paths.StateFile))
                return new List<SaveState>();

            try
            {
                var json = File.ReadAllText(_paths.StateFile);
                return JsonSerializer.Deserialize<List<SaveState>>(json) ?? new List<SaveState>();
            }
            catch
            {
                // Fichier corrompu : on repart d'une liste vide plutôt que de crasher
                return new List<SaveState>();
            }
        }
    }
}
