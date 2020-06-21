using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int _levelIndex;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    protected void Start()
    {
        _button.onClick.AddListener( () => GameManager.LoadLevel(_levelIndex));
    }
}
