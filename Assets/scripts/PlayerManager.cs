using Edgar.Unity.Examples;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// Very simple implementation of a player that can interact with objects.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private IInteractable interactableInFocus;
    public float MoveSpeed = 5f;

    //general player stuff
    private Animator animator;
    private Vector2 movement;
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    //health stuff
    [SerializeField]
    private float hp; 
    [SerializeField]
    private float maxHp;

    //weapons
    [SerializeField]
    private Weapon leftWeapon;
    private SpriteRenderer leftWeaponRenderer;
    private BoxCollider2D leftWeaponHitbox;
    [SerializeField]
    private Weapon rightWeapon;
    private SpriteRenderer rightWeaponRenderer;
    [SerializeField]
    private Weapon spaceWeapon;

    //defense
    [SerializeField]
    private bool isParrying = false;

    [SerializeField]
    private ChestManager gameManager;

    //healthbar
    [SerializeField]
    private RectTransform healthBar;
    public float width, height;

    //melee stamina bar
    [SerializeField]
    private RectTransform BarMelee;
    [SerializeField]
    private RectTransform CooldownBarMelee;
    public float widthMelee, heightMelee;

    //ranged stamina bar
    [SerializeField]
    private RectTransform BarRanged;
    [SerializeField]
    private RectTransform CooldownBarRanged;
    public float widthRanged, heightRanged;

    //space stamina bar
    [SerializeField]
    private RectTransform BarSpace;
    [SerializeField]
    private RectTransform CooldownBarSpace;
    public float widthSpace, heightSpace;

    public int chestIndex = 0;

    private TextMeshProUGUI pickup;

    public void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        hp = maxHp;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ChestManager>();

        healthBar = gameManager.healthBar;
        BarMelee = gameManager.BarMelee;
        CooldownBarMelee = gameManager.CooldownBarMelee;
        BarRanged = gameManager.BarRanged;
        CooldownBarRanged = gameManager.CooldownBarRanged;
        BarSpace = gameManager.BarSpace;
        CooldownBarSpace = gameManager.CooldownBarSpace;

        pickup = gameManager.pickup;
    }

    /// <summary>
    /// If an interactable object is in focus and is allowed to interact, call its Interact() method.
    /// </summary>
    public void Update()
    {
        movement.x = InputHelper.GetHorizontalAxis();
        movement.y = InputHelper.GetVerticalAxis();

#if UNITY_6000_0_OR_NEWER
        animator.SetBool("running", rigidbody.linearVelocity.magnitude > float.Epsilon);
#else
        animator.SetBool("running", rigidbody.velocity.magnitude > float.Epsilon);
#endif

        // Flip sprite if needed
        var flipSprite = spriteRenderer.flipX ? movement.x > 0.01f : movement.x < -0.01f;
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            if (leftWeapon != null) {
                leftWeaponRenderer.flipX = !leftWeaponRenderer.flipX;
                leftWeaponHitbox.offset = -leftWeaponHitbox.offset;
            }
        }

        if (interactableInFocus != null)
        {
            if (interactableInFocus.IsInteractionAllowed())
            {
                interactableInFocus.Interact();
            }
            else
            {
                interactableInFocus.EndInteract();
                interactableInFocus = null;
            }
        }

        //weapon cooldown bar handling
        if (leftWeapon == null)
        {
            BarMelee.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            BarMelee.localScale = new Vector3(1, 1, 1);
            float newWidth = (leftWeapon.GetCooldown() / leftWeapon.GetTheoreticalCooldown()) * widthMelee;
            if(newWidth > widthMelee) { newWidth = widthMelee; }
            CooldownBarMelee.sizeDelta = new Vector2(newWidth, heightMelee);
        }

        //weapon cooldown bar handling
        if (rightWeapon == null)
        {
            BarRanged.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            BarRanged.localScale = new Vector3(1, 1, 1);
            float newWidth = (rightWeapon.GetCooldown() / rightWeapon.GetTheoreticalCooldown()) * widthRanged;
            if (newWidth > widthRanged) { newWidth = widthRanged; }
            CooldownBarRanged.sizeDelta = new Vector2(newWidth, heightRanged);
        }

        //weapon cooldown bar handling
        if (spaceWeapon == null)
        {
            BarSpace.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            BarSpace.localScale = new Vector3(1, 1, 1);
            float newWidth = (spaceWeapon.GetCooldown() / spaceWeapon.GetTheoreticalCooldown()) * widthSpace;
            if (newWidth > widthSpace) { newWidth = widthSpace; }
            CooldownBarSpace.sizeDelta = new Vector2(newWidth, heightSpace);
        }

        if (InputHelper.GetKeyDown(KeyCode.Mouse0) && !isParrying)
        {
            LeftAttack();
        }
        if (InputHelper.GetKeyDown(KeyCode.Mouse1) && !isParrying)
        {
            RightAttack();
        }
        if (InputHelper.GetKeyDown(KeyCode.Space) && !isParrying)
        {
            SpaceAttack();
        }
        if (hp <= 0)
        {
            PlayerDeath();
        }
    }
    public void FixedUpdate()
    {
        this.Move(rigidbody.position + movement.normalized * MoveSpeed * Time.fixedDeltaTime);
    }

    public void SetMoveSpeed(float val)
    {
        MoveSpeed = val;
    }

    /// <summary>
    /// If the collision is with an interactable object that is allowed to interact,
    /// make this object the current focus of the player.
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
    {
        var interactable = collider.GetComponent<IInteractable>();

        if (interactable == null || !interactable.IsInteractionAllowed())
        {
            return;
        }

        interactableInFocus?.EndInteract();
        interactableInFocus = interactable;
        interactableInFocus.BeginInteract();
    }

    /// <summary>
    /// If the collision is with the interactable object that is currently the focus
    /// of the player, make the focus null.
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerExit2D(Collider2D collider)
    {
        var interactable = collider.GetComponent<IInteractable>();

        if (interactable == interactableInFocus)
        {
            interactableInFocus?.EndInteract();
            interactableInFocus = null;
        }
    }

    public void Move(Vector2 v)
    {
        rigidbody.MovePosition(v);
    }

    public Rigidbody2D GetRigidbody()
    {
        return rigidbody;
    }

    public SpriteRenderer GetRenderer()
    {
        return spriteRenderer;
    }

    public Vector2 GetMovement()
    {
        return movement;
    }

    public void SetMovement(Vector2 m)
    {
        this.movement = m;
    }

    public float GetHorizontalMovement()
    {
        return InputHelper.GetHorizontalAxis();
    }

    public float GetVerticalMovement()
    {
        return InputHelper.GetVerticalAxis();
    }

    public void LeftAttack()
    {
        if (leftWeapon != null)
        {
            leftWeapon.gameObject.GetComponent<Weapon>().Attack();
        }
    }

    public void RightAttack()
    {
        if (rightWeapon != null)
        {
            rightWeapon.gameObject.GetComponent<Weapon>().Attack();
        }
    }

    public void SpaceAttack()
    {
        if(spaceWeapon != null)
        {
            spaceWeapon.gameObject.GetComponent<Weapon>().Attack();
        }
    }

    public Weapon GetLeftWeapon()
    {
        return this.leftWeapon;
    }

    public Weapon GetRightWeapon() 
    { 
        return this.rightWeapon; 
    }
    public Weapon GetSpaceWeapon()
    {
        return this.spaceWeapon;
    }

    public void SetLeftWeapon(Weapon weapon)
    {
        this.leftWeapon = weapon;
        leftWeaponRenderer = weapon.GetComponent<SpriteRenderer>();
        //leftWeaponRenderer.flipX = spriteRenderer.flipX;
        leftWeaponHitbox = weapon.GetComponent<BoxCollider2D>();
        if (spriteRenderer.flipX)
        {
            this.leftWeapon.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            leftWeaponRenderer.flipY = true;
        }
        StartCoroutine(ShowLabel(weapon.name));
    }

    public void SetRightWeapon(Weapon weapon)
    {
        this.rightWeapon = weapon;
        rightWeaponRenderer = weapon.GetComponentInChildren<SpriteRenderer>();
        //rightWeaponRenderer.flipX = spriteRenderer.flipX;
        StartCoroutine(ShowLabel(weapon.name));
    }

    public void SetSpaceWeapon(Weapon weapon)
    {
        this.spaceWeapon = weapon;
        StartCoroutine(ShowLabel(weapon.name));
    }

    public float GetHP()
    {
        return hp;
    }

    public float GetMaxHP()
    {
        return maxHp;
    }

    public void SetHP(float hp)
    {
        this.hp = hp;
        if(hp > maxHp)
        {
            hp = maxHp;
        }
        float newWidth = (hp/maxHp)*width;
        healthBar.sizeDelta = new Vector2(newWidth, height);
    }

    public void ScaleHP()
    {
        this.hp += 5;
        this.maxHp += 5;

        float newWidth = (hp / maxHp) * width;
        healthBar.sizeDelta = new Vector2(newWidth, height);
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    public void SetParrying(bool val)
    {
        this.isParrying = val;
    }

    public void PlayerDeath()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene("DeathMenu");
    }

    public IEnumerator ShowLabel(string label)
    {
        int index = label.IndexOf("(");
        if(index >= 0)label = label.Substring(0, index);
        pickup.enabled = true;
        pickup.text = "Picked up: " + label;
        yield return new WaitForSeconds(2);
        pickup.enabled = false;
    }
}