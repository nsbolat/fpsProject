using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class DialogueManager : MonoBehaviour
{
    public GameObject player;
    public GameObject interactCamera;
    public Transform targetPosition, currentPosition;
    public Image actorImage;
    public TextMeshProUGUI actorName;
    public TextMeshProUGUI messageText;
    public RectTransform backgroundBox;
    public AudioSource typingSound; // Assign an AudioSource for the typing sound in the inspector

    private Message[] currentMessages;
    private Actor[] currentActors;
    private int activeMessage = 0;
    public static bool isActive = false;

    private bool isTyping = false;
    private bool skipTyping = false;

    public float typingSpeed = 0.05f; // Time between each letter
    public float moveSpeed = 2f; // Speed of camera movement

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        isActive = true;
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;

        Debug.Log("Messages loaded: " + messages.Length);
        DisplayMessage();

        // Kamera parent'ından çıkartılıyor
        interactCamera.transform.SetParent(null);
        player.SetActive(false);

        // Kamera hedef pozisyonuna kayıyor
        StartCoroutine(MoveCamera(interactCamera.transform, targetPosition.position));
        
        backgroundBox.transform.localScale = Vector3.one;
    }

    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        actorName.text = currentActors[messageToDisplay.actorId].name;
        actorImage.sprite = currentActors[messageToDisplay.actorId].sprite;

        StartCoroutine(TypeMessage(messageToDisplay.message));
    }

    IEnumerator TypeMessage(string message)
    {
        isTyping = true;
        skipTyping = false;
        messageText.text = ""; // Clear the text

        foreach (char letter in message.ToCharArray())
        {
            if (skipTyping)
            {
                messageText.text = message; // Skip to full message if skip is true
                break;
            }

            messageText.text += letter;
            typingSound.Play(); // Play typing sound for each letter
            yield return new WaitForSeconds(typingSpeed); // Delay between each letter
        }

        isTyping = false;
    }

    public void NextMessage()
    {
        if (isTyping)
        {
            skipTyping = true; // Skip the typing animation
        }
        else
        {
            activeMessage++;
            if (activeMessage < currentMessages.Length)
            {
                DisplayMessage();
            }
            else
            {
                Debug.Log("Dialogue finished!");
                isActive = false;
                backgroundBox.transform.localScale = Vector3.zero;
                
                player.SetActive(true);
                interactCamera.transform.SetParent(GameObject.Find("Main Camera").gameObject.transform);
                // Kamera current position'a kayıyor ve ardından parent olarak ana kameraya ekleniyor
                StartCoroutine(MoveCamera(interactCamera.transform, currentPosition.position, () =>
                {interactCamera.transform.localPosition = Vector3.zero; // Kameranın localPosition'u sıfırlanıyor
                }));

                
            }
        }
    }

    private void Start()
    {
        isActive = false;
        backgroundBox.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isActive)
        {
            NextMessage();
        }
    }

    IEnumerator MoveCamera(Transform cameraTransform, Vector3 targetPos, System.Action onComplete = null)
    {
        while (Vector3.Distance(cameraTransform.position, targetPos) > 0.01f)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null; // Bir sonraki frame'e geç
        }
        
        // Son pozisyonu hedef pozisyonla eşitliyoruz
        cameraTransform.position = targetPos;
        
        // Eğer işlem tamamlandıktan sonra yapılacak ekstra bir şey varsa
        onComplete?.Invoke();
    }
}
