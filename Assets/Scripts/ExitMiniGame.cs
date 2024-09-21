using Naninovel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMiniGame : MonoBehaviour
{
    public void Exit()
    {
        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;

        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        scriptPlayer.PreloadAndPlayAsync("Loc2", label: "AfterMiniGame").Forget();

        var hidePrinter = new Naninovel.Commands.HidePrinter();
        hidePrinter.ExecuteAsync().Forget();

        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = true;

        SceneManager.LoadScene("Main");
    }
}
