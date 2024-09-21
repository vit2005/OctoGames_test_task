using Naninovel;
using UnityEngine.SceneManagement;
using Naninovel.Commands;

[CommandAlias("MiniGame")]
public class MiniGame : Command
{
    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;

        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        scriptPlayer.Stop();

        var hidePrinter = new HidePrinter();
        hidePrinter.ExecuteAsync(asyncToken).Forget();

        SceneManager.LoadScene("Demo");
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = false;

        var stateManage = Engine.GetService<IUIManager>();
        stateManage.ResetService();

        return UniTask.CompletedTask;
    }
}