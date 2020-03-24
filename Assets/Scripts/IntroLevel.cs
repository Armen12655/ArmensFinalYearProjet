using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Defines the <see cref="IntroLevel" />
/// </summary>
public class IntroLevel : MonoBehaviour
{
    /// <summary>
    /// The StraightLevelClick
    /// </summary>
    public void StraightLevelClick()
    {
        SceneManager.LoadScene("straightPathsLevel"); // loading scene.
    }
}
