using UnityEngine;

public class Bow : RangedWeapon
{
    [SerializeField]
    private float offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        RotateTowards(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void RotateTowards(Vector3 target)
    {
        Vector3 difference = target - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
    }

    public override bool Attack()
    {
        return base.Attack();
    }
}
