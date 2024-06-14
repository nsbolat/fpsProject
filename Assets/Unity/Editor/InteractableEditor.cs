using UnityEditor;

[CustomEditor(typeof(Interactable),true)]
public class InteractableEditor : Editor
{ 
    public override void OnInspectorGUI()
    {
        Interactable interactable = (Interactable)target;
        if (target.GetType()==typeof(EventOnlyInteractable))
        {
            interactable.promptMessage = EditorGUILayout.TextField("Propt Message", interactable.promptMessage);
            EditorGUILayout.HelpBox("EventOnlyInteractable sadece UnityEventler kullanılır ",MessageType.Info);
            if (interactable.GetComponent<InteractEvent>()==null)
            {
                interactable.useEvents = true;
                interactable.gameObject.AddComponent<InteractEvent>();
            }
        }
        else
        {
            base.OnInspectorGUI();

            // Check if we should use events
            if (interactable.useEvents)
            {
                // If the InteractEvent component is not present, add it
                if (interactable.GetComponent<InteractEvent>() == null)
                {
                    interactable.gameObject.AddComponent<InteractEvent>();
                }
            }
            else
            {
                // If the InteractEvent component is present, remove it
                InteractEvent interactEvent = interactable.GetComponent<InteractEvent>();
                if (interactEvent != null)
                {
                    DestroyImmediate(interactEvent);
                }
            }
        }
        
    }
    }

