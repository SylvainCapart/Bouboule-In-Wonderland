using UnityEngine;




[System.Serializable]
public class Scene
{
    [Header("Optional : ")]
    public int sceneIndex; //"SceneIndex is n - x, with n number of sentences, x number of clicks
    [Header("Optional : ")]
    public AnimationClip sceneClip;
}

[System.Serializable]
public class Dialogue {

    public enum DialogueID { INTRO };

    public string name;
    public DialogueID dialogueID;

    [TextArea(3,10)] public string[] sentences;
    public Scene[] scenes;

    [Header("Optional : ")] public SceneMgt sceneManager;

    [HideInInspector] public bool triggered;

}
