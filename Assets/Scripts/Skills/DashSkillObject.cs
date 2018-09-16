using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashSkill", menuName = "Dash skill")]
public class DashSkillObject : SkillObject
{
    [SerializeField]
    private float dashSpeed;

    [SerializeField]
    private float dashDuration;

    [SerializeField]
    private float dashDamping;

    override
    public void Cast(Player player)
    {
        player.dashDamping = dashDamping;
        player.SetStatus(Player.Status.Dashing | Player.Status.Invunerable, dashDuration);

        Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (dashDirection.Equals(Vector2.zero))
            dashDirection = player.transform.right;

        player.GetComponent<Rigidbody2D>().velocity = dashDirection * dashSpeed;

    }
}
