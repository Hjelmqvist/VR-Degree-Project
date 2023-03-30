using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CombinationLock : MonoBehaviour
{
    [SerializeField] string combination;
    [SerializeField] char fillSymbol;
    [SerializeField] TextMeshPro text;
    [SerializeField] UnityEvent OnCorrectGuess;
    [SerializeField] UnityEvent OnWrongGuess;

    string guess = "";
    bool wasCorrect = false;

    private void Awake()
    {
        text.text = GetOutput();
    }

    public void AddInput(char value)
    {
        if (wasCorrect)
        {
            return;
        }

        guess += value;
        text.text = GetOutput();

        if (guess.Length == combination.Length)
        {
            ValidateGuess();
        }
    }

    private string GetOutput()
    {
        string output = guess;
        int missingLength = combination.Length - guess.Length;
        for (int i = 0; i < missingLength; i++)
        {
            output += fillSymbol;
        }
        return output;
    }

    private void ValidateGuess()
    {
        if (guess.Equals(combination))
        {
            wasCorrect = true;
            OnCorrectGuess.Invoke();
        }
        else
        {
            guess = "";
            text.text = GetOutput();
            OnWrongGuess.Invoke();
        }
    }
}
