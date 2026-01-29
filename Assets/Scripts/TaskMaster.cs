using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Taskmaster : MonoBehaviour
{
    [System.Serializable]
    public class Objective
    {
        public string text;
        public List<Key> requiredKeys;           // Keys that must be pressed
        [HideInInspector] public HashSet<Key> pressedKeys = new HashSet<Key>();
        [HideInInspector] public bool completed = false;
    }

    [SerializeField] private TMP_Text uiText;
    [SerializeField] private float typeSpeed = 0.04f;
    [SerializeField] private float completedDelay = 0.5f;
    [SerializeField] private List<Objective> objectives = new List<Objective>();

    private int index = 0;

    void Start()
    {
        StartCoroutine(RunTutorial());
    }

    void Update()
    {
        if (index >= objectives.Count) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Objective obj = objectives[index];
        bool changed = false;

        foreach (Key key in obj.requiredKeys)
        {
            if (keyboard[key].wasPressedThisFrame)
            {
                if (obj.pressedKeys.Add(key)) // only add if not already pressed
                    changed = true;
            }
        }

        // Mark objective as completed if all required keys pressed
        if (obj.pressedKeys.Count == obj.requiredKeys.Count)
            obj.completed = true;

        // Update TMP text only if keys pressed changed
        if (changed)
            uiText.text = GenerateDisplayText(obj);
    }

    IEnumerator RunTutorial()
    {
        while (index < objectives.Count)
        {
            Objective obj = objectives[index];
            obj.pressedKeys.Clear();
            obj.completed = false;

            // Typewriter effect for the objective text
            uiText.text = "";
            foreach (char c in obj.text)
            {
                uiText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            // Add initial key display after typewriter finishes
            uiText.text = GenerateDisplayText(obj);

            // Wait until objective is completed
            yield return new WaitUntil(() => obj.completed);

            // Strike-through when completed
            uiText.text = $"<s>{obj.text}</s>";
            yield return new WaitForSeconds(completedDelay);

            index++;
        }
    }

    // Generates the display text with pressed/unpressed keys using your palette
    private string GenerateDisplayText(Objective obj)
    {
        var keysDisplay = obj.requiredKeys
            .Select(k => obj.pressedKeys.Contains(k)
                ? $"<color=#FB8B24>{KeyToDisplay(k)}</color>"   // pressed = orange
                : $"<color=#FFE8D6>{KeyToDisplay(k)}</color>"); // unpressed = cream

        return $"{obj.text}\nKeys: {string.Join(" ", keysDisplay)}";
    }

    // Maps keys to display-friendly names
    private string KeyToDisplay(Key key)
    {
        switch (key)
        {
            case Key.W: return "W";
            case Key.A: return "A";
            case Key.S: return "S";
            case Key.D: return "D";
            case Key.R: return "R";
            case Key.Space: return "Space";
            case Key.LeftShift: return "Left Shift";

            default: return key.ToString(); // fallback
        }
    }
}
