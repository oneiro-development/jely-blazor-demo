namespace JelyUI;

using Microsoft.AspNetCore.Components.Forms;
using Refit;

record AppState(
    List<StreamPart> Files,
    int CurrentStep = 0
);

interface IAppStateService {
    AppState State { get; }
    event Action OnChange;
    void AddFile(StreamPart file);
    void NextStep();
    void Reset();
}

class AppStateService : IAppStateService {

    private AppState _state = new AppState(Enumerable.Empty<StreamPart>().ToList());

    public AppState State => _state;
    public event Action OnChange = () => {};

    public void AddFile(StreamPart file) {
        _state = _state with { Files = _state.Files.Append(file).ToList() };
        NotifyStateChanged();
    }

    public void NextStep() {
        _state = _state with { CurrentStep = _state.CurrentStep + 1 };
        NotifyStateChanged();
    }

    public void Reset() {
        _state = new AppState(Enumerable.Empty<StreamPart>().ToList()) with { CurrentStep = 0 };
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}