﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkOwner))]
[RequireComponent(typeof(Stats))]

public class Deplacement : MonoBehaviour
{
    public Direction sens;

    //Physic
    Rigidbody rigid_body;
    public float gravity;
    public float speed;
    public float jump_speed;
    private bool is_walking;

    Vector2 movement;

    //Jump
    public bool on_ground;
    public float jump_time;
    public int multiple_jump;

    //Dash
    private bool on_dash;
    private float actual_dash_time;
    public float dash_time;
    public float dash_speed;
    private Vector2 dash_direction;

    private Stats stats;

    //Rotation
    private bool is_rotate;
    private Vector3 last_rotation;
    private Vector3 rotation_axis;
    private float start_rotation;
    public float rotation_time;

    Animation_Info anim_info;

    public enum Direction
    {
        X = 0x01F,
        mX = 0x02F,
        Y = 0x04F,
        mY = 0x08F,
        Z = 0x10F,
        mZ = 0x20F
    };

    public enum Flip
    {
        Front,
        Back,
        Left,
        Right
    }

    void Start()
    {
        enabled = GetComponent<NetworkOwner>().IsMine();

        rigid_body = GetComponent<Rigidbody>();
        stats = GetComponent<Stats>();
        on_ground = true;
        sens = Direction.Y;

        anim_info = GetComponent<Animation_Info>();
    }

    void FixedUpdate()
    {
        if (!stats.CanMovePlayer)
        {
            rigid_body.velocity = new Vector3(0, 0, 0);
            return;
        }

        float delta_time = Time.deltaTime;
        if (is_rotate)
            UpdateRotation(delta_time);
        else if (!stats.IsDead())
            MoveCharacter(delta_time);
    }

    #region movement

    void MoveCharacter(float delta_time)
    {
        if (stats.team != Stats.Team.None)
        {
            Vector3 direction = transform.up;
            is_walking = false;

            movement = new Vector2(0, 0);
            float movement_speed = speed;

            if (!on_dash)
                movement = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            else
            {
                Vector2 actual_direction = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
                if (Vector2.Dot(dash_direction, actual_direction) < -0.25)
                    on_dash = false;

                movement = dash_direction;
                movement_speed = dash_speed;

                actual_dash_time += delta_time;
                if (actual_dash_time > dash_time)
                    on_dash = false;
            }

            if (!on_ground)
            {
                jump_time += delta_time;
                movement *= 0.5f;
            }

            bool is_moving = (movement.x != 0.0f || movement.y != 0.0f);
            is_walking = is_moving && on_ground;
            if (is_moving)
            {
                //Clear the velocity of the deplacement axis
                rigid_body.velocity = new Vector3(rigid_body.velocity.x * Mathf.Abs(direction.x),
                                                  rigid_body.velocity.y * Mathf.Abs(direction.y),
                                                  rigid_body.velocity.z * Mathf.Abs(direction.z));
            }

            rigid_body.velocity += (transform.forward * movement.x + transform.right * movement.y) * movement_speed;


            Vector3 final_gravity = gravity * direction * delta_time;
            if (on_dash && on_ground)
                final_gravity /= 20;

            rigid_body.velocity += final_gravity;

            if (Input.GetKey(KeyCode.LeftShift) && on_ground && !on_dash && stats.CanDash())
            {
                on_dash = true;
                if (stats.can_dash_in_multiple_direction && (movement.x != 0 || movement.y != 0))
                    dash_direction = movement;
                else
                    dash_direction = new Vector2(1.0f, 0);

                actual_dash_time = 0.0f;
                stats.Dash();
            }

            if (anim_info)
            {
                anim_info.on_dash = on_dash;
                if (anim_info.double_jump)
                    anim_info.double_jump = false;
            }

            if (Input.GetAxis("Jump") != 0.0f && (on_ground || (multiple_jump < 2 && jump_time > 0.30)))
                Jump();

            if (Input.GetKey(KeyCode.Keypad1))
                SetDirection(Direction.X);
            else if (Input.GetKey(KeyCode.Keypad2))
                SetDirection(Direction.mX);
            else if (Input.GetKey(KeyCode.Keypad3))
                SetDirection(Direction.Y);
            else if (Input.GetKey(KeyCode.Keypad4))
                SetDirection(Direction.mY);
            else if (Input.GetKey(KeyCode.Keypad5))
                SetDirection(Direction.Z);
            else if (Input.GetKey(KeyCode.Keypad6))
                SetDirection(Direction.mZ);
        }
        else
        {
            movement = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            transform.position += (transform.forward * movement.x + transform.right * movement.y +
                transform.up * Input.GetAxis("Jump") - transform.up * (float)System.Convert.ToDouble(Input.GetKey(KeyCode.LeftShift))) * speed * Time.deltaTime;

        }
    }

    void Jump()
    {
        on_ground = false;

        if (anim_info)
            anim_info.jumping = true;

        jump_time = 0.0f;
        if (on_ground)
        {
            multiple_jump = 1;

        }
        else
        {
            multiple_jump++;

            if (anim_info)
            {
                anim_info.double_jump = true;

            }
        }

        Vector3 v = rigid_body.velocity;

        if (sens == Direction.X || sens == Direction.mX)
            v.x = transform.up.x * jump_speed;
        else if (sens == Direction.Y || sens == Direction.mY)
            v.y = transform.up.y * jump_speed;
        else if (sens == Direction.Z || sens == Direction.mZ)
            v.z = transform.up.z * jump_speed;

        rigid_body.velocity = v;
    }

    void OnCollisionEnter(Collision col)
    {
        foreach (ContactPoint contact in col.contacts)
        {
            if (Vector3.Dot(contact.normal, transform.up) > 0.5f)
            {
                setPlayerOnGround();
            }
        }
    }

    void OnCollisionStay(Collision col)
    {
        foreach (ContactPoint contact in col.contacts)
        {
            if (Vector3.Dot(contact.normal, transform.up) > 0.5f)
            {
                setPlayerOnGround();
            }
        }
    }

    void setPlayerOnGround()
    {
        on_ground = true;
        multiple_jump = 0;
        jump_time = 0.0f;

        if (anim_info)
            anim_info.jumping = false;
    }

    public Vector2 getMovement()
    {
        if (on_dash)
            return dash_direction;
        else
            return movement;
    }

    public bool onDash()
    {
        return (on_dash);
    }

    #endregion

    #region rotation
    public void CorrectRotation()
    {
        transform.localEulerAngles = AverageRotation(transform.localEulerAngles, transform.up);
    }

    void UpdateRotation(float delta_time)
    {
        start_rotation += delta_time;
        if (start_rotation >= rotation_time)
        {
            start_rotation = rotation_time;
            is_rotate = false;
        }

        transform.localEulerAngles = last_rotation;
        transform.RotateAround(transform.position, rotation_axis, 90 * start_rotation / rotation_time);

        if (!is_rotate)
        {
            sens = findGlobalAxis(AverageDirection(transform.up));
            CorrectRotation();
        }
    }

    public void DoFlip(Flip flip)
    {
        if (is_rotate)
            return;

        //Clear velocity
        rigid_body.velocity = new Vector3(0, 0, 0);
        is_rotate = true;
        start_rotation = 0.0f;

        if (flip == Flip.Front)
            rotation_axis = AverageDirection(transform.right);
        else if (flip == Flip.Back)
            rotation_axis = -AverageDirection(transform.right);
        else if (flip == Flip.Right)
            rotation_axis = AverageDirection(transform.forward);
        else if (flip == Flip.Left)
            rotation_axis = -AverageDirection(transform.forward);

        last_rotation = transform.localEulerAngles;
    }
    #endregion

    #region utils

    public void SetOnGround(bool b)
    {
        on_ground = b;
    }

    public bool IsWalking()
    {
        return (is_walking);
    }

    public bool canMoveCamera()
    {
        bool can_move = true;
        if (stats)
            can_move = stats.CanMovePlayer;

        return (can_move && !on_dash && !is_rotate);
    }

    public void SetDirection(Direction dir)
    {
        if (sens == dir || on_dash)
            return;

        on_ground = false;
        multiple_jump = 2;
        sens = dir;

        if (dir == Direction.X)
            transform.localEulerAngles = new Vector3(0, 0, -90);
        else if (dir == Direction.mX)
            transform.localEulerAngles = new Vector3(0, 0, 90);
        else if (dir == Direction.Y)
            transform.localEulerAngles = new Vector3(0, 0, 0);
        else if (dir == Direction.mY)
            transform.localEulerAngles = new Vector3(0, 0, -180);
        else if (dir == Direction.Z)
            transform.localEulerAngles = new Vector3(90, 0, 0);
        else if (dir == Direction.mZ)
            transform.localEulerAngles = new Vector3(-90, 0, 0);
    }

    Vector3 AverageDirection(Vector3 direction)
    {
        Vector3 result = new Vector3(0, 0, 0);
        float abs_x = Mathf.Abs(direction.x);
        float abs_y = Mathf.Abs(direction.y);
        float abs_z = Mathf.Abs(direction.z);
        if (abs_x > abs_y)
        {
            if (abs_x > abs_z)
            {//abs_x superior
                if (direction.x < 0.0f)
                    result.x = -1;
                else
                    result.x = 1;
            }
            else
            {//abs_z superior
                if (direction.z < 0.0f)
                    result.z = -1;
                else
                    result.z = 1;
            }
        }
        else
        {//abs_x < abs_y
            if (abs_y > abs_z)
            {//abs_y superior
                if (direction.y < 0.0f)
                    result.y = -1;
                else
                    result.y = 1;
            }
            else
            {//abs_z superior
                if (direction.z < 0.0f)
                    result.z = -1;
                else
                    result.z = 1;
            }
        }
        return result;
    }

    Vector3 AverageRotation(Vector3 rot, Vector3 up)
    {
        up = AverageDirection(up);
        if (up.x == 0.0f)
            rot.x = ((int)((rot.x + 45) / 90)) * 90;
        if (up.y == 0.0f)
            rot.y = ((int)((rot.y + 45) / 90)) * 90;
        if (up.z == 0.0f)
            rot.z = ((int)((rot.z + 45) / 90)) * 90;
        return (rot);
    }

    public Direction findGlobalAxis(Vector3 v)
    {
        if (v.x != 0.0f)
        {
            if (v.x < 0.0f)
                return (Direction.mX);
            else
                return (Direction.X);
        }
        else if (v.y != 0.0f)
        {
            if (v.y < 0.0f)
                return (Direction.mY);
            else
                return (Direction.Y);
        }
        else if (v.z != 0.0f)
        {
            if (v.z < 0.0f)
                return (Direction.mZ);
            else
                return (Direction.Z);
        }
        Debug.Log("findGlobalAxis failed");
        return (Direction.Y);
    }
    #endregion
}
