using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    protected static bool useKeyDown = true;
    protected static float jumpCooldown = 0.25f;
    protected static float forceMod = 200f;
    private static float groundedCheck = 0.5f;
    private static float chargeTime = 0.25f;
    private static float drag = 0.6f;
    private static float succRotationSpeed = 2f;
    private static float succSpeed = 0.4f;
    private static float succLerpSpeed = 0.1f;
    private static float succDeathHeight = 8*5;

    private float lastY;

    public Animator animator;
    public int id;  // only used in prefabs
    public string landSound;
    public string jumpSound;
    public string musicSound;
    public string deathSound;
    public string joinSound;
    public Vector2 jumpForce;
    public float speedMod = 1f;
    public float charge;
    public Rigidbody2D rb;
    public Collider2D collider;
    public LayerMask groundLayer;
    public bool grounded;
    public GameObject actionKeyPrefab;
    public ActionKey actionKey;
    public GameObject actionKeyGO;

    private Vector2 lastVel;
    public bool button;
    public bool buttonDown;
    public bool joined;
    public bool gone;
    public bool succed;
    public float succHeight;
    public bool unjoinedBefore;
    public float jumpTimer;
    private float spawnY;

    void OnTriggerEnter2D()
    {
        // suppose it's UFO
        succed = true;
        rb.isKinematic = true;
        succHeight = transform.position.y;
        GM.getGM().ufo.abduct();
        unjoin();
    }
    private void checkForActivation()
    {
        if (joined) return;  // only runs for unjoined
        Companion c = GM.getRun().getCompanionInActivationRange(transform.position);
        if (c == null) return;
        if (!c.joined) return;  // some edge case
        Chain chain = new Chain(c, this);
        GM.getAudioManager().play(joinSound);
        join();
    }
    public void join()
    {
        rb.isKinematic = false;
        
        GM.getAudioManager().unmute(musicSound);
        joined = true;
        createTooltip();
    }
    public void unjoin()
    {
        GM.getAudioManager().mute(musicSound);
        GM.getRun().removeAllChainsAttachedTo(this);
        if (joined)
        {
            Destroy(actionKeyGO);
        }
        joined = false;
        if (!unjoinedBefore)
        {
            GM.getAudioManager().play(deathSound);
            GM.getRun().loseHP();
            GM.getRun().checkPrematureRestart();
        }
        unjoinedBefore = true;
    }
    public void manualStart(Vector2Int loc2)
    {
        rb.freezeRotation = true;
        rb.isKinematic = true;
        charge = chargeTime;
        joined = false;
        Vector2 pos = loc2 * 8;
        spawnY = pos.y;
        
        transform.localPosition = pos;
        moveToFront();

        actionKey = actionKeyPrefab.GetComponent<ActionKey>();
        if (actionKey == null) Debug.LogWarning("sdsad");
        manualExtraStart();
    }
    public virtual void manualExtraStart()
    {

    }
    private void createTooltip()
    {
        actionKeyGO = Instantiate(actionKeyPrefab, transform);
        float tooltipRadius = actionKeyGO.GetComponent<SpriteRenderer>().bounds.extents.y;
        float thisRadius = collider.bounds.extents.y/transform.localScale.y;
        float dist = tooltipRadius + thisRadius;
        actionKeyGO.transform.localPosition = new Vector2(0f, dist);
    }
    public void manualUpdate()
    {
        if (!joined) return;
        if (Input.GetKey(actionKey.key)) button = true;
        if (Input.GetKeyDown(actionKey.key)) buttonDown = true;
    }

    public void manualFixedUpdate()
    {
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("VelX", rb.velocity.x);
        animator.SetFloat("VelY", rb.velocity.y);

        jumpTimer -= Time.fixedDeltaTime;
        if (succed)
        {
            Vector2 beamPos = new Vector2(GM.getGM().ufo.transform.position.x, succHeight);
            Vector2 pos = Vector2.Lerp(transform.position, beamPos, succLerpSpeed);
            transform.position = pos;
            moveToFront();
            Quaternion rot = transform.rotation;
            rot *= Quaternion.Euler(0f, 0f, succRotationSpeed);
            transform.rotation = rot;
            succHeight += succSpeed;
            if (succHeight > succDeathHeight) die();
            return;
        }




        // All companions
        if (!joined)
        {
            Vector2 pos = transform.position;
            pos.y = spawnY - 0.5f + Mathf.Sin(Time.time*4f);
            transform.position = pos;
            moveToFront();
            checkForActivation();
        }
        
        if (transform.position.y < GM.getGM().deathPit) die();


        // only joined companions
        if (!joined) return;

        // checks if grounded
        bool wasGrounded = grounded;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, collider.bounds.extents.y+groundedCheck, groundLayer);
        grounded = hit.collider != null;
        float weight = rb.mass;
        if (grounded && !wasGrounded) GM.getAudioManager().play(landSound, false, 1, 1 / weight);

        if (button)
        {
            button = false;
            charge -= Time.deltaTime;
            hold(charge <= 0f);
        } else
        {
            unhold(charge <= 0f);
            charge = chargeTime;
        }
        if (buttonDown)
        {
            buttonDown = false;
            if (useKeyDown) tap();
        }
    }
    private void moveToFront()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
    }
    public void die()
    {
        //GM.getAudioManager().play(instrumentCrashSound);
        unjoin();
        begone();
    }
    protected void jump(float volume = 1f, float pitch = 1f)
    {
        if (!canJump()) return;
        applyForce(jumpForce);
        justJumped(volume, pitch);
    }
    protected void justJumped(float volume = 1f, float pitch = 1f)
    {
        GM.getAudioManager().play(jumpSound, false, volume, pitch);
        grounded = false;
        jumpTimer = jumpCooldown;
    }
    public bool canJump()
    {
        if (!grounded) return false;
        if (jumpTimer > 0f) return false;
        return true;
    }
    public void begone()
    {
        GM.getRun().companions.Remove(this);
        Destroy(gameObject);
    }
    public virtual void hold(bool charged)
    {
        walk();
    }
    public virtual void unhold(bool charged)
    {
        walk();
    }
    public virtual void tap()
    {
        if (!canJump()) return;
        jump();
    }
    public void walk()
    {
        float allowedVel = Run.speed * speedMod;
        if (rb.velocity.x < allowedVel) rb.AddForce(new Vector2(100f, 0f));
        else rb.AddForce(new Vector2(-drag * rb.velocity.x, 0f));
    }
    public void applyForce(Vector2 f)
    {
        rb.AddForce(f*forceMod);
    }
}
