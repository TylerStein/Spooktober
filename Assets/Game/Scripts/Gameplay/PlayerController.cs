using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovementController2D))]
public class PlayerController : MonoBehaviour
{
    public P2DAudioSource p2dAudioSource;
    public P2DSubject p2dSubject;
    public float soundRadius = 2.5f;
    public float minSoundFallDistance = 0.5f;

    public Transform playerLight;

    public CharacterMovementController2D controller;
    public SpriteRenderer sprite;
    public UIManager uiManager;

    public bool wasGrounded = true;
    public bool isHidden = false;

    public Interactive interactive = null;
    public string interactiveTag = "Interactive";

    public List<Item> heldItems = new List<Item>(); 

    public float visibility => p2dSubject.visibility;

    // Start is called before the first frame update
    void Start() {
        if (!p2dAudioSource) p2dAudioSource = GetComponent<P2DAudioSource>();
        if (!p2dSubject) p2dSubject = GetComponent<P2DSubject>();

        playerLight.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        UpdateInput();
        UpdateSprite();

        if (wasGrounded == false && controller.isGrounded == true) {
            float diffY = controller.fallYStart - transform.position.y;
            if (diffY > minSoundFallDistance) {
                p2dAudioSource.Emit(10f);
            }
        }

        wasGrounded = controller.isGrounded;

        if (playerLight.gameObject.activeSelf) {
            Vector2 toMouseWorld = (GetMouseWorld() - (Vector2)transform.position).normalized;
            playerLight.transform.up = toMouseWorld;
            playerLight.transform.position = transform.position + (Vector3)(toMouseWorld * 0.5f);
        }

        if (Input.GetButtonDown("Fire1") && !isHidden) {
            playerLight.gameObject.SetActive(!playerLight.gameObject.activeSelf);
        }

        //if (controller.isOnStairs && controller.moveInput.x != 0f) {
        //    float distanceToStairEnd = controller.GetStairsDistanceToEnd(Mathf.Sign(controller.moveInput.x));
        //    if (distanceToStairEnd == 0f) {
        //        controller.UnMountStairs();
        //    }
        //}
    }

    void UpdateInput() {
        controller.Jump(Input.GetButton("Jump"));

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        controller.Move(new Vector2(h, v));

        if (Input.GetButtonDown("Fire2")) {
            if (interactive != null) {
                interactive.Interact(this);
            }
        }
    }

    void UpdateSprite() {
        sprite.flipX = controller.faceDirection == 1 ? false : true;
    }

    public bool HasItemWithId(string id) {
        foreach (Item item in heldItems) {
            if (item.itemId == id) return true;
        }
        return false;
    }

    public void DisposeItemWithId(string id) {
        foreach (Item item in heldItems) {
            if (item.itemId == id) {
                heldItems.Remove(item);
                break;
            }
        }
        uiManager.UpdateItemPanel(heldItems);
    }

    public void GiveItem(Item item) {
        heldItems.Add(item);
        uiManager.UpdateItemPanel(heldItems);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (interactive == null && collision.tag == interactiveTag) {
            interactive = collision.GetComponent<Interactive>();
            if (interactive.ShowPrompt(this)) {
                uiManager.SetPromptText(interactive.GetPrompt(this));
            } else {
                uiManager.ClearUIPrompt();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (interactive != null && collision.tag == interactiveTag) {
            if (collision.gameObject == interactive.gameObject) {
                interactive = null;
                uiManager.ClearUIPrompt();
            }
        }
    }

    Vector2 GetMouseWorld() {
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMouse.z = transform.position.z;
        return worldMouse;
    }
}
