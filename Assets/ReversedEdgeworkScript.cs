using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;
using rnd = UnityEngine.Random;

public class ReversedEdgeworkScript : MonoBehaviour {

    public ReversedEdgeworkQuestion[] questions;
    public ReversedEdgeworkQuestion mainQuestion;

    public KMAudio ModuleAudio;
    public KMNeedyModule Module;
    public KMBombInfo BombInfo;

    private int _moduleId;
    public KMSelectable[] Buttons;
    public TextMesh[] texts;
    public ReversedEdgeworkQuestion[] assignedQuestions = new ReversedEdgeworkQuestion[4];

    public TextMesh screenText;

    private int startmin;
    private int startsec;
	// Use this for initialization
	void Start () {
        _moduleId++;

        startmin = int.Parse(BombInfo.GetFormattedTime().Split(':').First());
        startsec = int.Parse(BombInfo.GetFormattedTime().Split(':').Last());

        LogTheFile("The bomb's starting time was " + startmin + " minutes and " + startsec + " seconds.");

        questions = new ReversedEdgeworkQuestion[] {
            new ReversedEdgeworkQuestion{question = "# of INDs", answer = BombInfo.GetIndicators().Count() },
            new ReversedEdgeworkQuestion{question = "# of lit INDs", answer = BombInfo.GetOnIndicators().Count()},
            new ReversedEdgeworkQuestion{question = "# of off INDs", answer = BombInfo.GetOffIndicators().Count()},
            new ReversedEdgeworkQuestion{question = "# of ports", answer = BombInfo.GetPortCount()},
            new ReversedEdgeworkQuestion{question = "# of port p.s", answer = BombInfo.GetPortPlateCount()},
            new ReversedEdgeworkQuestion{question = "# of batts", answer = BombInfo.GetBatteryCount()},
            new ReversedEdgeworkQuestion{question = "# of D batts", answer = BombInfo.GetBatteryCount(Battery.D)},
            new ReversedEdgeworkQuestion{question = "# of AA batts", answer = BombInfo.GetBatteryCount(Battery.AA)},
            new ReversedEdgeworkQuestion{question = "# of batt hds", answer = BombInfo.GetBatteryHolderCount()},
            new ReversedEdgeworkQuestion{question = "First # of SN", answer = BombInfo.GetSerialNumberNumbers().First()},
            new ReversedEdgeworkQuestion{question = "Last # of SN", answer = BombInfo.GetSerialNumberNumbers().Last()},
            new ReversedEdgeworkQuestion{question = "# of mod", answer = BombInfo.GetModuleNames().Count()},
            new ReversedEdgeworkQuestion{question = "# of r. mod", answer = BombInfo.GetSolvableModuleNames().Count()},
            new ReversedEdgeworkQuestion{question = "# of n. mod", answer = BombInfo.GetModuleNames().Count() - BombInfo.GetSolvableModuleNames().Count()},
            new ReversedEdgeworkQuestion{question = "# of s. mod", answer = BombInfo.GetSolvedModuleNames().Count()},
            new ReversedEdgeworkQuestion{question = "# of u. mod", answer = BombInfo.GetModuleNames().Count() - BombInfo.GetSolvedModuleNames().Count()},
            new ReversedEdgeworkQuestion{question = "b. st. min", answer = startmin},
            new ReversedEdgeworkQuestion{question = "b. st. sec", answer = startsec}
        };
        for (int i = 0; i < 3; ++i) {
            Buttons[i].OnInteract += delegate {
                Buttons[i].AddInteractionPunch();
                ModuleAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[i].transform);
                if (assignedQuestions[i].answer != mainQuestion.answer) {
                    LogTheFile("You pressed " + assignedQuestions[i].question + ", which was wrong. Handling Strike and going to sleep.");
                    Module.HandleStrike();
                    Module.HandlePass();
                } else {
                    Module.HandlePass();
                    LogTheFile("Correct question pressed, going to sleep.");
                }
                return false;
                };
            Buttons[i].OnInteractEnded += delegate {
                ModuleAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, Buttons[i].transform);
                };
            }
        Module.OnNeedyActivation += delegate {
            GenerateQuestion();
            };
        Module.OnNeedyDeactivation += delegate {
            foreach (TextMesh text in texts) {
                text.text = "";
            }
            screenText.text = "";
        };
        Module.OnTimerExpired += delegate {
            LogTheFile("Timer expired, handling Strike");
            Module.HandleStrike();
        };
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateQuestion() {
        int mainQuestionn = rnd.Range(0, questions.Length );
        int question2n = rnd.Range(0, questions.Length);
        int question3n = rnd.Range(0, questions.Length);
        int question4n = rnd.Range(0, questions.Length);

        question2n = question2n == mainQuestionn ? question2n + 1 : question2n;
        question3n = question3n == question2n ? question3n + 1 : question3n;
        question4n = question4n == question3n ? question4n + 1 : question4n;

        mainQuestion = questions[mainQuestionn];
        ReversedEdgeworkQuestion question2 = questions[question2n];
        ReversedEdgeworkQuestion question3 = questions[question3n];
        ReversedEdgeworkQuestion question4 = questions[question4n];

        LogTheFile("I've chosen the following questions: " + mainQuestion.question + ", " + question2.question + ", " + question3.question + ", " + question4.question + ".");

        List<ReversedEdgeworkQuestion> ersiofjaoitjgfiowj = new List<ReversedEdgeworkQuestion>() { mainQuestion, question2, question3, question4 };

        screenText.text = mainQuestion.answer.ToString();

        for (int i = 0; i < 4; ++i) {
            assignedQuestions[i] = ersiofjaoitjgfiowj.PickRandom();
            ersiofjaoitjgfiowj.Remove(assignedQuestions[i]);
            texts[i].text = assignedQuestions[i].question;
        }
        LogTheFile(assignedQuestions[1].question + assignedQuestions[0].question + assignedQuestions[2].question + assignedQuestions[3].question);
    }

    void LogTheFile(string logMessage) {
        Debug.LogFormat("[Reversed Edgework #{0}] {1}", _moduleId, logMessage);
    }

    void HandlePress(int pressedButton) {
    }
}

public class ReversedEdgeworkQuestion{
    public string question;
    public int answer;
}
