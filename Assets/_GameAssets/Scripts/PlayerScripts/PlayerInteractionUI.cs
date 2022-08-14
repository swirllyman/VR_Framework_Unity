using UnityEngine;
using UnityEngine.UI;
public class PlayerInteractionUI : MonoBehaviour
{
    public LayerMask hitMask;
    public Transform[] handUIPointerTransforms;
    public PointerBeam[] pointerBeams;
    [SerializeField] float updateTime = .1f;
    [SerializeField] float holdTime = .25f;
    ButtonHelper[] vrButtons = new ButtonHelper[2];
    bool[] heldDown = new bool[2];
    float[] currentUpdateTimers = new float[2];
    float[] currentHoldTimers = new float[2];

    private void Awake()
    {
        SimpleInput.SimpleOpenXRInput.onTriggerClicked += MyInput_onTriggerInput;
        for (int i = 0; i < pointerBeams.Length; i++)
        {
            pointerBeams[i].Stop();
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < handUIPointerTransforms.Length; i++)
        {
            if (Physics.Raycast(new Ray(handUIPointerTransforms[i].position, handUIPointerTransforms[i].forward), out RaycastHit hit, 5.0f, hitMask))
            {
                if (hit.collider.CompareTag("UI"))
                {
                    if (vrButtons[i] == null)
                    {
                        ButtonHelper hitButton = hit.collider.GetComponent<ButtonHelper>();
                        if (hitButton.button.interactable)
                        {
                            vrButtons[i] = hitButton;
                            pointerBeams[i].Display(handUIPointerTransforms[i].position, hit.collider.transform.position);
                            vrButtons[i].button = hitButton.button;
                            vrButtons[i].button.image.color = vrButtons[i].button.colors.highlightedColor;
                            GameManager.PlayAudio(vrButtons[i].buttonHoverAudioClip);
                        }
                        else
                        {
                            pointerBeams[i].Display(handUIPointerTransforms[i].position, hit.point);
                        }
                    }
                    else
                    {
                        pointerBeams[i].Display(handUIPointerTransforms[i].position, vrButtons[i].transform.position);
                    }
                }
                else if (vrButtons[i] != null)
                {
                    pointerBeams[i].Display(handUIPointerTransforms[i].position, hit.point);
                    RemoveButton(i);
                }
                else
                {
                    pointerBeams[i].Display(handUIPointerTransforms[i].position, hit.point);
                }
            }
            else if (vrButtons[i] != null)
            {
                pointerBeams[i].Display(handUIPointerTransforms[i].position, handUIPointerTransforms[i].position + handUIPointerTransforms[i].forward * 5.0f, false);
                RemoveButton(i);
            }
            else
            {
                pointerBeams[i].Display(handUIPointerTransforms[i].position, handUIPointerTransforms[i].position + handUIPointerTransforms[i].forward * 5.0f, false);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < pointerBeams.Length; i++)
        {
            if (heldDown[i])
            {
                if (currentHoldTimers[i] < holdTime)
                {
                    currentHoldTimers[i] += Time.deltaTime;
                }
                else
                {
                    if(currentUpdateTimers[i] < updateTime)
                    {
                        currentUpdateTimers[i] += Time.deltaTime;
                    }
                    else
                    {
                        if(vrButtons[i] != null)
                        {
                            currentUpdateTimers[i] = 0;
                            GameManager.PlayAudio(vrButtons[i].buttonClickAudioClip);
                            vrButtons[i].button.onClick.Invoke();
                        }
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < pointerBeams.Length; i++)
        {
            pointerBeams[i].Stop();
            vrButtons[i] = null;
        }
    }

    private void MyInput_onTriggerInput(int handID, bool down)
    {
        if (!down) 
        {
            heldDown[handID] = down;
            currentHoldTimers[handID] = 0;
            currentUpdateTimers[handID] = 0;
        }

        if (vrButtons[handID] != null && enabled && down)
        {
            heldDown[handID] = down;
            GameManager.PlayAudio(vrButtons[handID].buttonClickAudioClip);
            vrButtons[handID].button.image.color = vrButtons[handID].button.colors.normalColor;
            vrButtons[handID].button.onClick.Invoke();
        }
    }

    

    void RemoveButton(int i)
    {
        vrButtons[i].button.image.color = vrButtons[i].button.colors.normalColor;
        vrButtons[i] = null;
        currentHoldTimers[i] = 0;
        currentUpdateTimers[i] = 0;
        heldDown[i] = false;
    }
}
