using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class QuestionEvent : UnityEvent<Question> {}

public class ConversationController : MonoBehaviour
{
    public Conversation conversation;
    public Conversation defaultConversation;
    public QuestionEvent questionEvent;

    public GameObject speakerLeft;
    public GameObject speakerRight;

    private SpeakerUIController speakerUILeft;
    private SpeakerUIController speakerUIRight;

    //TEMP audio stuff
    public AudioSource audioSrc;
    public AudioClip[] talkSounds;
    public AudioClip progressConvSnd;

    public bool isTalking = false;
    public bool dialogueSkip = false;

    //public SpriteRenderer background;
    public GameObject background;

    private int activeLineIndex;
    private bool conversationStarted = false;

    public void ChangeConversation(Conversation nextConversation) {
        conversationStarted = false;
        conversation = nextConversation;
        AdvanceLine();
    }

    private void Start()
    {
        speakerUILeft  = speakerLeft.GetComponent<SpeakerUIController>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUIController>();
        audioSrc = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Select"))
        {
            //the logic i want to go for is:
            //if the text == the full line then advanceline()
            //if else fill out the text with the full line
            if(isTalking == true)
                dialogueSkip = true;
            else
                AdvanceLine();
        }
        else if (Input.GetKeyDown("x"))
            EndConversation();
    }

    private void EndConversation() {
        conversation = defaultConversation;
        conversationStarted = false;
        //speakerUILeft.Hide();
        //speakerUIRight.Hide();
        speakerUILeft.Deactivate();
        speakerUIRight.Deactivate();

        //lightening background
        //background.color = new Color(1, 1, 1, 1);
        background.SetActive(false);
    }

    private void Initialize() {
        conversationStarted = true;
        activeLineIndex = 0;
        speakerUILeft.Speaker = conversation.speakerLeft;
        speakerUIRight.Speaker = conversation.speakerRight;

        //darkening background for some reason the RGB is measured in 0 to 1
        //background.color = new Color(0.5f, 0.5f, 0.5f, 1);
        background.SetActive(true);

    }

    public void AdvanceLine() {
        if (conversation == null) return;
        if (!conversationStarted) Initialize();

        audioSrc.PlayOneShot(progressConvSnd);

        //if there's more lines in the convo display the next line
        if (activeLineIndex < conversation.lines.Length)
            DisplayLine();
        else
            AdvanceConversation();            
    }

    private void DisplayLine() {
        Line line = conversation.lines[activeLineIndex];
        Character character = line.character;

        if (speakerUILeft.SpeakerIs(character))
        {
            SetDialog(speakerUILeft, speakerUIRight, line);
        }
        else {
            SetDialog(speakerUIRight, speakerUILeft, line);
        }

        activeLineIndex += 1;
    }

    //if there's a question or conversation qeued up, then it'll switch to that convo
    private void AdvanceConversation() {
        // These are really three types of dialog tree node
        // and should be three different objects with a standard interface
        if (conversation.question != null)
            questionEvent.Invoke(conversation.question);
        else if (conversation.nextConversation != null)
            ChangeConversation(conversation.nextConversation);
        //TEMP creates a trigger and ends the conversation, if a trigger is present
        else if (conversation.trigger != null)
        {
            Instantiate(conversation.trigger);
            EndConversation();
        }
        else
            EndConversation();
    }

    //typing out the dialogue
    void SetDialog(
        SpeakerUIController activeSpeakerUI,
        SpeakerUIController inactiveSpeakerUI,
        Line line
    ) {
        activeSpeakerUI.Show();
        inactiveSpeakerUI.Hide();

        activeSpeakerUI.Dialog = "";
        activeSpeakerUI.Mood = line.mood;

        StopAllCoroutines();
        StartCoroutine(EffectTypewriter(line.text, activeSpeakerUI, PlayerPrefs.GetFloat("textSpeed")));
    }

    IEnumerator EffectTypewriter(string text, SpeakerUIController controller, float talkspeed) {
        isTalking = true;
        foreach(char character in text.ToCharArray()) {
            //if the player chooses to skip the dialogue, instantly type it out and end the loop!
            if (dialogueSkip == true)
            {
                controller.dialog.text = text;
                dialogueSkip = false;
                break;
            }
            controller.Dialog += character;
            //plays a random talk sound from the list of talk sounds if it's not a space
            if (character != ' ')
                audioSrc.PlayOneShot(talkSounds[Random.Range(0, talkSounds.Length)]);
            //could make this a talk speed option for the menu!
            yield return new  WaitForSeconds(talkspeed);
            // yield return null;
        }
        isTalking = false;
    }
}
