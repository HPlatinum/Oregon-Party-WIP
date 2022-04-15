using UnityEngine;

public class BeaverMotor : MonoBehaviour
{
    #region Inspector Variables

    #endregion

    #region Components
    internal Animator animator;
    internal GameObject beaver;
    internal Rigidbody rigidBody;
    #endregion

    #region Internal Variables
    internal bool isDead {
        get
        {
            return _isDead;
        }
        set
        {
            _isDead = value;
        }
    }
    internal bool isAttacking {
        get
        {
            return _isAttacking;
        }
        set
        {
            _isAttacking = value;
        }
    }
    internal bool isWalking {
        get
        {
            return _IsWalking;
        }
        set
        {
            _IsWalking = value;
        }
    }
    internal bool _isDead;
    internal bool _isAttacking;
    internal bool _IsWalking;
    internal float speed;
    #endregion


}
